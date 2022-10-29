## Generates Templates and Menu Items on the documentation using the CMF CLI
##	Sample usage:
##		.\Data_GenerateMD.ps1 
param (
	[Parameter(Mandatory=$true)]
	[string]$packageId
)

$scriptLocation = Split-Path $MyInvocation.MyCommand.Path -Parent

$dataSolutionRelativePath = "..\Cmf.Custom.Data"
$dataSolutionAbsolutePath = Join-Path -Path $scriptLocation -ChildPath $dataSolutionRelativePath

$deeRelativePath = $dataSolutionRelativePath + ".\DEEs\*";
$deeFolder = Join-Path -Path $dataSolutionAbsolutePath -ChildPath $deeRelativePath

$mdRelativePath = $dataSolutionRelativePath + ".\MasterData\" + $packageId + "\*";
$mdFolder = Join-Path -Path $dataSolutionAbsolutePath -ChildPath $mdRelativePath

$scriptName = Join-Path -Path $scriptLocation -ChildPath "MD.zip"

$compress = @{
  Path = $deeFolder, $mdFolder
  CompressionLevel = "Fastest"
  DestinationPath = $scriptName
}

try {
	Compress-Archive @compress
}
catch {
 	Write-Error $error
}
