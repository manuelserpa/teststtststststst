## Generates Templates and MenuItems on the documentation using the CMF CLI
##	Sample usage:
##		.\Help_GenerateAll.ps1 

if (!(Get-Command cmf -errorAction SilentlyContinue))
{
    throw "CMF command is missing. Please install it first before use this script."
}

$root = Split-Path $MyInvocation.MyCommand.Path -Parent

while ((Split-Path -Path $root) -and !(Test-Path -Path (Join-Path -Path $root -ChildPath ".project-config.json"))) {
	$root = Split-Path -Path $root
}

if (!(Split-Path -Path $root)) {
	throw "Project not found!"
}

$helpFolder = Join-Path -Path $root -ChildPath "Cmf.Custom.Help"
$currentLocation = Get-Location

try {
	Set-Location $helpFolder
 
	Write-Host "`nRunning generateBasedOnTemplates...`n" -ForegroundColor blue
	cmf build help generateBasedOnTemplates

	Write-Host "`nRunning generateMenuItems...`n" -ForegroundColor blue
	cmf build help generateMenuItems

	Write-Host "`nBuilding the solution...`n" -ForegroundColor blue
	gulp build
}
catch {
  	Write-Host "`nGulp is not installed or the build is failing. Please, make sure the Help solution builds with success." -ForegroundColor red
	Write-Host "See more info about the error below.`n" -ForegroundColor red
  	$error = $_
 	Write-Error $error
}
finally {	
	Set-Location $currentLocation
}
