$scriptPath = "..\Cmf.Custom.Help"

# Generate Based on Templates
Write-Host "Generating based on templates"
& ".\GenerateBasedOnTemplates.ps1"

# Generate Menu Items
Write-Host "Generating Menu Item Json"
& ".\GenerateMenuItemJson.ps1"

Push-Location $scriptPath

# Build in Production
Write-Host "GULP BUILD --PRODUCTION"
gulp build --production

Pop-Location
