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
        [string] $gameName
    )

    [string] $folder = [System.IO.Path]::GetDirectoryName($BuildPath)
    [string] $targetSwitch = "-build" + $TargetPlatform + "Player"
    #Clean the target folder. Powershell will ask for confirmation if it is not empty.
    Clean-TargetFolder $folder
    Write-Host "Building $TargetPlatform to $BuildPath..."
    . $unityPath -quit -batchmode -projectpath $ProjectPath -$targetSwitch $BuildPath -logFile $windowsLogPath | Out-Default
    Get-ChildItem -Path $folder *_DoNotShip -Recurse | Remove-Item -Recurse

    [string] $archiveName =  "$folder\$gameName ($TargetPlatform).zip"
    Write-Host "Creating Archive at  $archiveName"
    Compress-Archive -Path "$folder\*" -DestinationPath "$archiveName"
}

[string] $outputPath = "D:\Writable Folder\Band BoomBox\"
[string] $unityPath = "C:\Program Files\Unity\2021.2.12f1\Editor\Unity.exe" 

[string] $projectPath  = $PSScriptRoot
[string] $gameName = "Band BoomBox"
[string] $windowsBuildPath   = $outputPath + "Test\Windows\$($gameName).exe" 
[string] $linuxBuildPath     = $outputPath + "Test\Linux\$($gameName).x86_64"
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
Build-Project -ProjectPath $ProjectPath -buildPath $windowsBuildPath -logFilePath $windowsLogPath -targetPlatform "Windows64" -gameName $gameName

# Build For Linux
Build-Project -ProjectPath $ProjectPath -buildPath $linuxBuildPath -logFilePath $linuxLogPath -targetPlatform "Linux64" -gameName $gameName

Write-Host "All builds completed successfully!"