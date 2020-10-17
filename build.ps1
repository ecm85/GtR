param (
	[Parameter(Mandatory=$true)]
	[string]$releaseVersion,

	[Parameter(Mandatory=$true)]
	[string]$githubApiToken
)

$ErrorActionPreference = 'Stop'

$windowsVersion = 'win-x64'
$osxVersion = 'osx.10.14-x64'
$dateStamp = [DateTime]::Now.ToString("yyyyMMdd-HHmmss")
$windowsReleaseDirectory = "Releases\v$releaseVersion-windows_$dateStamp"
$osxReleaseDirectory = "Releases\v$releaseVersion-osx_$dateStamp"
$windowsReleaseArchiveName = "v$releaseVersion-windows.zip"
$osxReleaseArchiveName = "v$releaseVersion-osx.zip"
$windowsReleaseArchive = "Releases\$windowsReleaseArchiveName"
$osxReleaseArchive = "Releases\$osxReleaseArchiveName"

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

$user = 'ecm85'
$pass = $githubApiToken

$pair = "$($user):$($pass)"

$encodedCreds = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes($pair))

$basicAuthValue = "Basic $encodedCreds"

$Headers = @{
	Authorization = $basicAuthValue
}

$params = @{
	tag_name = "$releaseVersion"
	name = "$releaseVersion"
}

$response = Invoke-RestMethod -Uri "https://api.github.com/repos/ecm85/GtR/releases" -Headers $Headers -Method Post -Body ($params|ConvertTo-Json) -ContentType "application/json"
$responseId = $response.Id

Invoke-RestMethod -Uri "https://uploads.github.com/repos/ecm85/GtR/releases/$responseId/assets?name=windowsReleaseArchiveName"  -Headers $Headers -Method Post -ContentType "application/zip" -Infile $windowsReleaseArchive
Invoke-RestMethod -Uri "https://uploads.github.com/repos/ecm85/GtR/releases/$responseId/assets?name=osxReleaseArchiveName"  -Headers $Headers -Method Post -ContentType "application/zip" -Infile $osxReleaseArchive