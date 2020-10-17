param (
	[Parameter(Mandatory=$true)]
	[string]$releaseVersion
)

$ErrorActionPreference = 'Stop'

$windowsVersion = 'win-x64'
$osxVersion = 'osx.10.14-x64'
$dateStamp = [DateTime]::Now.ToString("yyyyMMdd-HHmmss")
$windowsReleaseDirectory = "Releases\v$releaseVersion-windows_$dateStamp"
$osxReleaseDirectory = "Releases\v$releaseVersion-osx_$dateStamp"
$windowsReleaseArchive = "Releases\v$releaseVersion-windows.zip"
$osxReleaseArchive = "Releases\v$releaseVersion-osx.zip"

Push-Location GtR
& dotnet publish -r $windowsVersion -c Release /p:PublishSingleFile=true /p:CopyOutputSymbolsToPublishDirectory=false /p:PublishTrimmed=true
if ($lastExitCode -ne 0) {
    Pop-Location
    exit $lastExitCode
}
& dotnet publish -r $osxVersion -c Release /p:PublishSingleFile=true /p:CopyOutputSymbolsToPublishDirectory=false /p:PublishTrimmed=true
if ($lastExitCode -ne 0) {
    Pop-Location
    exit $lastExitCode
}
Pop-Location

New-Item -ItemType "directory" $windowsReleaseDirectory
New-Item -ItemType "directory" $osxReleaseDirectory

Copy-Item -Path "GtR\GtR\bin\Release\netcoreapp3.1\$windowsVersion\publish\*" -Destination $windowsReleaseDirectory -recurse
Copy-Item -Path "GtR\GtR\bin\Release\netcoreapp3.1\$osxVersion\publish\*" -Destination $osxReleaseDirectory -recurse
Copy-Item readme.md -Destination $windowsReleaseDirectory
Copy-Item readme.md -Destination $osxReleaseDirectory
Copy-Item NeuzeitGro-BolModified.ttf -Destination $windowsReleaseDirectory
Copy-Item NeuzeitGro-BolModified.ttf -Destination $osxReleaseDirectory
Copy-Item NeuzeitGro-RegModified.ttf -Destination $windowsReleaseDirectory
Copy-Item NeuzeitGro-RegModified.ttf -Destination $osxReleaseDirectory

Compress-Archive -Path "$windowsReleaseDirectory\*" -DestinationPath $windowsReleaseArchive -Force
Compress-Archive -Path "$osxReleaseDirectory\*" -DestinationPath $osxReleaseArchive -Force
