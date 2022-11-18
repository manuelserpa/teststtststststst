## Generates MastarData zip for a given package with all the DEEs 
##	Sample usage:
##		.\Data_GenerateMD.ps1 -version 1.0.0
##		.\Data_GenerateMD.ps1 (and then the user inserts the version)

param (
	[Parameter(Mandatory=$true)]
	[Alias("version")] 
	[string]$packageId
)

$scriptLocation = Split-Path $MyInvocation.MyCommand.Path -Parent

$rootSolutionRelativePath = "..\"
$rootSolutionAbsolutePath = Join-Path -Path $scriptLocation -ChildPath $rootSolutionRelativePath

$deeFolderRelative = "Cmf.Custom.Data\DEEs\*"
$deeFolder = Join-Path -Path $rootSolutionAbsolutePath -ChildPath $deeFolderRelative 

if (!(Test-Path -Path $deeFolder)) {
   throw "${deeFolderRelative} does not exist."
}

$mdFolderRelative = "Cmf.Custom.Data\MasterData\${packageId}\*"
$mdFolder = Join-Path -Path $rootSolutionAbsolutePath -ChildPath $mdFolderRelative

if (!(Test-Path -Path $mdFolder)) {
   throw "${mdFolderRelative} does not exist."
}

$fileName = "MD-${packageId}"
$countFileName = Get-ChildItem -filter "$fileName*" -path $scriptLocation | Measure-Object | Select -ExpandProperty Count

Write-Output $countFileName
$scriptName = Join-Path -Path $scriptLocation -ChildPath "MD-${packageId}-${countFileName}.zip"

$compress = @{
  Path = $deeFolder, $mdFolder
  CompressionLevel = "Fastest"
  DestinationPath = $scriptName
}

Compress-Archive @compress
