$basePath = "..\Cmf.Custom.Help"
$path = "$basePath\src\packages\cmf.docs.area.amsosram\assets"

$templateSuffix = "_template"

function Get-Title {
	param (
		$content
	)
	$title = $content
	
	$indexOfNewLine = $title.indexof([Environment]::NewLine) #"`n")
	if ( $indexOfNewLine -gt 0 ) {
		$title = $title.substring(0,$indexOfNewLine)
	}

	$title = $title -replace "#",""		
	$title = $title.trim()

	return $title
}

function Get-Description {
	param (
		$content
	)
	$description = "-"
	
	$overviewHeader = "## Overview"

	$indexOfOverview = $content.IndexOf($overviewHeader)
	if ( $indexOfOverview -gt 0 )
	{
		$description = $content
		
		$indexOfOverview += $overviewHeader.length
		$description = $description.substring($indexOfOverview, $description.length - $indexOfOverview)
		
		$indexOfNextSection = $description.IndexOf("##")
		if ( $indexOfNextSection -gt 0)
		{
			$description = $description.substring(0, $indexOfNextSection)
		}
		
		$description = $description.trim()
	}
	
	return $description
}


$templateFiles = Get-ChildItem -File -Recurse -Path $path -Filter "*$templateSuffix"
foreach ( $templateFile in $templateFiles ) {
	if ( "$($templateFile.BaseName)" -eq "$templateSuffix" ) {
		continue
	}
	
	$template = $templateFile.BaseName
	$name = $template -replace $templateSuffix, ""
		
	# Add .md to file
	$outputFile = "$name.md"
		
	Write-Host "template: $template"
	Write-Host "outputFile: $outputFile"
	
	$baseFolder = $templateFile.FullName -replace $templateFile.BaseName, ""
	Write-Host "BaseFolder: $baseFolder"
		
	$filesPath = (Resolve-Path "$baseFolder\$name").Path
	$files = Get-ChildItem "$filesPath" -filter "*.md"
	Write-Host "Number of files: $($files.length)"
	
	$output = ""
	
	$templateContent = (Get-Content -Path "$($templateFile.FullName)" -Encoding UTF8	| Out-String )
	
	$isTableMode = $templateContent.contains("@TableData@")
	$isIndexMode = $templateContent.contains("@IndexData@")
	
	if ( $isTableMode ) {
		$toReplaceToken = "@TableData@"
	} else {
		$toReplaceToken = "@IndexData@"
	}
	
	foreach( $file in $files ) 
	{
		$fileContent = Get-Content $file.FullName -Encoding UTF8 | Out-String
		
		
		$fileName = "$($file.BaseName)"
		
		# Title
		$title = Get-Title $fileContent
		
		#write-host $file.FullName
		$folderPath = $file.FullName.substring(0, $file.FullName.lastindexof([IO.Path]::DirectorySeparatorChar))
		$folderPath = $folderPath.substring($folderPath.indexof("assets" + [IO.Path]::DirectorySeparatorChar) + ("assets" + [IO.Path]::DirectorySeparatorChar).length)
		$separator = [IO.Path]::DirectorySeparatorChar.ToString()+[IO.Path]::DirectorySeparatorChar.ToString();
		$folderPath = $folderPath -replace $separator, ">"
		#Write-Host "$folderPath"
		#exit
		$link = "/AMSOsram/$folderPath>$fileName"
		
		if ( $isIndexMode ) {
			# Index Mode
			$output += "* [$title]($link)" + [Environment]::NewLine
		} else {
			if ( $isTableMode ) {
				# Table Mode
			
				$description = Get-Description $fileContent
				
				$output += "| [$title]($link) | $description | " + [Environment]::NewLine
			}
		}
	}
	
	$templateContent -replace $toReplaceToken, $output `
		| Set-Content "$baseFolder\$outputFile" -Encoding UTF8
}
