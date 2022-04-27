$basePath = "..\Cmf.Custom.Help"
						  
$packagesPath = "$basePath\src\packages"

$packageDir = get-childitem -path $packagesPath -directory -filter "cmf.docs.area.*"
Write-Host ('packageDir ' + $packageDir)
$projectName = "amsosram"
Write-Host ('projectName ' + $projectName)
$assetsPath = "$packagesPath\cmf.docs.area.$projectName\assets"

function Get-MetadataFromFolder {
	param (
		[string]$folder,
		[string]$parentFolder
)	
	$metadata = $null
	if ( $parentFolder )
	{
		Write-Host "Searching folder: $folder"
		$files = Get-ChildItem -Path "$folder" -Filter "*.md" -File
	
		foreach ( $file in $files )
		{
			$fileName = $file.BaseName
			Write-Host "File: $fileName"

			if ( $metadata )
			{
				$metadata += "," + [Environment]::NewLine
			}
			
			$fileContent = Get-Content -Path $file.FullName | Out-String
			
			$indexOfNewLine = $fileContent.indexof([Environment]::NewLine)
			if ( $indexOfNewLine -gt 0 ) {
				$fileContent = $fileContent.substring(0,$indexOfNewLine)
			}
			
			$fileContent = $fileContent -replace "#",""
			
			$fileContent = $fileContent.trim()
			
			$title = $fileContent
			
			# Write-Host $title
			
			$fileNameLower = $fileName.ToLower()
			$parentFolderLower = $parentFolder.ToLower()

			$metadata += "{`n"
			$metadata += "   ""id"": ""$fileNameLower""," + [Environment]::NewLine
			$metadata += "   ""menuGroupId"": ""$parentFolderLower""," + [Environment]::NewLine
			$metadata += "   ""title"": ""$title""," + [Environment]::NewLine
			$metadata += "   ""actionId"": """"" + [Environment]::NewLine
			$metadata += "}"
		}
	}
	
	$folders = Get-ChildItem -Path "$folder" -Directory -Exclude "images"
	foreach ( $folder in $folders )
	{

		#Write-Host $folder
		$folderName = $folder.substring($folder.LastIndexOf("\") + 1)
		Write-Host "getting metadata from folder: $folder - $folderName"
		$metadataFromSubFolder = Get-MetadataFromFolder $folder $folderName
		
		if ( $metadataFromSubFolder -and $metadata )
		{
			$metadata += "," + [Environment]::NewLine
		}
		$metadata += $metadataFromSubFolder

	}
	return $metadata
}



$mainFolder = $assetsPath
$metadata = Get-MetadataFromFolder $mainFolder

#metadata as array
$metadata = "[" + [Environment]::NewLine + "$metadata" + [Environment]::NewLine + "]"

$metadata | Set-Content "$assetsPath\__generatedMenuItems.json"
Write-Host ('File ''' + $assetsPath + '\__generatedMenuItems.json'' Updated')
#(Get-Content -Path "$assetsPath\$template" | Out-String ) -replace "@TableData@", $output `
#		| Set-Content "$assetsPath\$outputFile"
