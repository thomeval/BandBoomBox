
param(
    [Parameter(Mandatory=$true)]
    [string] $OutputPath = "D:\Writable Folder\Band BoomBox\",
    [Parameter(Mandatory=$false)]
    [string] $UnityPath = "C:\Program Files\Unity\2022.3.20f1\Editor\Unity.exe"
)

function Clean-TargetFolder($target)
{
    if ((Test-Path($target)) -and ((Get-ChildItem $target).Count -gt 0))
    {
        Write-Warning "The destination build folder ($target) exists and is not empty. If you continue, everything in this folder will be deleted. Are you sure?"
        $response = Read-Host -Prompt "Continue (y/n)"

        if (($response -ne "y") -and ($response -ne "Y"))
        {
            return $false
        }
        Remove-Item $target -Recurse
        Write-Host "Deleting $target ..."
        return $true
    }

    return $true
}

function Get-ProjectVersion()
{
    param(
        [Parameter(Mandatory=$true)]
        [string] $ProjectPath
    )

    [string] $projectJsonPath = [System.IO.Path]::Combine($ProjectPath, "ProjectSettings", "ProjectSettings.asset")

    Write-Host "Reading version number from $projectJsonPath"
    if (-not(Test-Path($projectJsonPath)))
    {
        Write-Warning "Project settings file was not found at : $projectJsonPath"
        return ""
    }

    
    $versionLine = get-content $projectJsonPath | select-string -Pattern "^\W*bundleVersion:"
    $versionLine = $versionLine.Line.Trim().Replace("bundleVersion: ","")

    Write-Host "Found Version: $versionLine"
    return $versionLine
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
        [string] $ReleasesPath,
        [Parameter(Mandatory=$true)]
        [string] $GameName,
        [Parameter(Mandatory=$false)]
        [string] $GameVersion = ""      
    )

    [string] $folder = [System.IO.Path]::GetDirectoryName($BuildPath)
    [string] $targetSwitch = "-build" + $TargetPlatform + "Player"

    Write-Host "Building $TargetPlatform to $BuildPath..."
    . $unityPath -quit -batchmode -projectpath $ProjectPath -$targetSwitch $BuildPath $debugArg -logFile $LogFilePath | Out-Default
    Get-ChildItem -Path $folder *_DoNotShip -Recurse | Remove-Item -Recurse

    [string] $archiveName =  "$ReleasesPath\$GameName $GameVersion ($TargetPlatform).zip"
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
[string] $macBuildPath       = $outputPath + "MacOSX\$($gameName).app"
[string] $windowsLogPath     = $outputPath + "Logs\log_Windows.txt"
[string] $linuxLogPath       = $outputPath + "Logs\log_Linux.txt"
[string] $macLogPath         = $outputPath + "Logs\log_Mac.txt"
[string] $releasesPath       = $outputPath + "Releases"

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

#Clean the target folder. Powershell will ask for confirmation if it is not empty.
$continue = Clean-TargetFolder $outputPath
if (-not($continue))
{
    Write-Host "Aborting..."
    return
}

$gameVersion = Get-ProjectVersion $projectPath

# Create Releases Subfolder
if (-not(Test-Path $releasesPath))
{
    New-Item -ItemType Directory $releasesPath | Out-Null
}

#Build For Windows
$targetPlatform = "Windows64"
Build-Project -ProjectPath $ProjectPath -buildPath $windowsBuildPath -logFilePath $windowsLogPath -targetPlatform $targetPlatform -GameName $GameName -gameVersion $gameVersion -ReleasesPath $releasesPath

# Build For Linux
$targetPlatform = "Linux64"
Build-Project -ProjectPath $ProjectPath -buildPath $linuxBuildPath -logFilePath $linuxLogPath -targetPlatform $targetPlatform -GameName $GameName -gameVersion $gameVersion -ReleasesPath $releasesPath

# Build For Mac OSX
$targetPlatform = "OSXUniversal"
Build-Project -ProjectPath $ProjectPath -buildPath $macBuildPath -logFilePath $macLogPath -targetPlatform $targetPlatform -GameName $GameName -gameVersion $gameVersion -ReleasesPath $releasesPath

Write-Host "All builds completed successfully!"