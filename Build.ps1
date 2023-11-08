
param(
    [Parameter(Mandatory=$true)]
    [string] $OutputPath = "D:\Writable Folder\Band BoomBox\",
    [Parameter(Mandatory=$false)]
    [string] $GameVersion = "",
    [Parameter(Mandatory=$false)]
    [string] $UnityPath = "C:\Program Files\Unity\2021.2.12f1\Editor\Unity.exe"
)

function Clean-TargetFolder($target)
{
    if (Test-Path($folder))
    {
        Remove-Item($folder)
    }
}

function Build-Project()
{
    param(
        [Parameter(Mandatory=$true)]
        [string] $ProjectPath,
        [Parameter(Mandatory=$true)]    
        [string] $BuildPath,
        [Parameter(Mandatory=$true)]
        [string] $LogFilePath,
        [Parameter(Mandatory=$true)]
        [string] $TargetPlatform,
        [Parameter(Mandatory=$true)]
        [string] $GameName,
        [Parameter(Mandatory=$false)]
        [string] $GameVersion = ""      
    )

    [string] $folder = [System.IO.Path]::GetDirectoryName($BuildPath)
    [string] $targetSwitch = "-build" + $TargetPlatform + "Player"
    [string] $debugArg = $debugBuild ? "-debugCodeOptimization" : ""
    #Clean the target folder. Powershell will ask for confirmation if it is not empty.
    Clean-TargetFolder $folder
    Write-Host "Building $TargetPlatform to $BuildPath..."
    . $unityPath -quit -batchmode -projectpath $ProjectPath -$targetSwitch $BuildPath $debugArg -logFile $windowsLogPath | Out-Default
    Get-ChildItem -Path $folder *_DoNotShip -Recurse | Remove-Item -Recurse


    [string] $archiveName =  "$folder\$GameName $GameVersion ($TargetPlatform).zip"
    Write-Host "Creating Archive at $archiveName"
    Compress-Archive -Path "$folder\*" -DestinationPath "$archiveName"
}

if (-not($outputPath.EndsWith("\")))
{
    $outputPath+= "\"
}

[string] $projectPath  = $PSScriptRoot
[string] $gameName = "Band BoomBox"

[string] $windowsBuildPath   = $outputPath + "Windows\$($gameName).exe" 
[string] $linuxBuildPath     = $outputPath + "Linux\$($gameName).x86_64"
[string] $windowsLogPath     = $outputPath + "Logs\log_Windows.txt"
[string] $linuxLogPath       = $outputPath + "Logs\log_Linux.txt"

$ErrorActionPreference = "Stop"

[int] $solutionFiles = (Get-ChildItem -Path $projectPath *.sln).Count

if ($solutionFiles -eq 0)
{
   Write-Error("No .sln files were found at $projectPath.") 
}
if (-not (Test-Path($unityPath)))
{
    Write-Error("Unity.exe was not found at $unityPath.")
}

#Build For Windows
Build-Project -ProjectPath $ProjectPath -buildPath $windowsBuildPath -logFilePath $windowsLogPath -targetPlatform "Windows64" -GameName $GameName -gameVersion $GameVersion

# Build For Linux
Build-Project -ProjectPath $ProjectPath -buildPath $linuxBuildPath -logFilePath $linuxLogPath -targetPlatform "Linux64" -GameName $GameName -gameVersion $GameVersion

Write-Host "All builds completed successfully!"