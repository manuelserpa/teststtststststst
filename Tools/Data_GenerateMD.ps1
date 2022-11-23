## Generates MastarData zip for a given package with all the DEEs 
##	Sample usage:
##		.\Data_GenerateMD.ps1 -version 1.0.0
##		.\Data_GenerateMD.ps1 (and then the user inserts the version)

param (
	[Parameter(Mandatory=$true)]
	[Alias("version")] 
	[string]$packageId
)

$root = Split-Path $MyInvocation.MyCommand.Path -Parent

while ((Split-Path -Path $root) -and !(Test-Path -Path (Join-Path -Path $root -ChildPath ".project-config.json"))) {
	$root = Split-Path -Path $root
}

if (!(Split-Path -Path $root)) {
	throw "Project not found!"
}

$deeFolderRelative = "Cmf.Custom.Data\DEEs\*"
$deeFolder = Join-Path -Path $root -ChildPath $deeFolderRelative 

if (!(Test-Path -Path $deeFolder)) {
   throw "${deeFolderRelative} does not exist."
}

$mdFolderRelative = "Cmf.Custom.Data\MasterData\${packageId}\*"
$mdFolder = Join-Path -Path $root -ChildPath $mdFolderRelative

if (!(Test-Path -Path $mdFolder)) {
   throw "${mdFolderRelative} does not exist."
}

$fileName = "MD-${packageId}"
$countFileName = Get-ChildItem -filter "$fileName*" -path $root | Measure-Object | Select -ExpandProperty Count

$scriptName = Join-Path -Path $root -ChildPath "MD-${packageId}-${countFileName}.zip"

$compress = @{
  Path = $deeFolder, $mdFolder
  CompressionLevel = "Fastest"
  DestinationPath = $scriptName
}

Compress-Archive @compress
