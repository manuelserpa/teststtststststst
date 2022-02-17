## Generates an MD for a specific package version
##	Sample usage:
##		.\GeneratePackageReleaseNotes.ps1 "<package_id>" "<custom_version>" "<release_date>" "<mes_version>" "<depends_on_custom_version>" "<packagesIncluded>"
##		.\GeneratePackageReleaseNotes.ps1 "01-Sprint01" "1.1.0" "2019/01/01" "7.0.0" "*None*"
##		.\GeneratePackageReleaseNotes.ps1 "02-Sprint02" "1.2.0" "2019/01/01" "7.0.0" "1.1.0"
param (
	[Parameter(Mandatory=$true)]
	[string]$packageId,
	[Parameter(Mandatory=$true)]
	[string]$packageVersion,
	[Parameter(Mandatory=$true)]
	[string]$expectedReleaseDate,
	[Parameter(Mandatory=$true)]
	[string]$mesVersion,
	[Parameter(Mandatory=$true)]
	[string]$customDependsOnVersion,
	[Parameter(Mandatory=$true)]
	[string]$packagesIncluded
)

$basePath = ".\..\Cmf.Custom.Help"
$packagesPath = "$basePath\src\packages"

$TfsServerUrl = "https://tfs-projects.cmf.criticalmanufacturing.com/ImplementationProjects"
$TfsProject = "AMSOSRAM"
$TfsProjectTeam = "$TfsProject Team"
$TfsWiqlUrl = "$TfsServerUrl/$TfsProject/$TfsProjectTeam/_apis/wit/wiql"

$packageName = "cmf.docs.area.amsosram"
$assetsPath = ".\$basePath\src\packages\$packageName\assets"

$releaseNotesId = "releasenotes"

$releaseNotesFolderPath = (Resolve-Path -Path "$assetsPath\$releaseNotesId").Path

$packageTemplateFile = "$releaseNotesFolderPath\packagetemplate"

$releaseNotesUrl = "$TfsProject/$releaseNotesId>"

$releaseNotesTemplateFile = "$assetsPath\releasenotes_template"

# Execute WIQL
$headers = @{
	'Accept' = 'application/json;api-version=4.1;excludeUrls=true'
	'Accept-Encoding' = 'gzip, deflate, br'
	'Content-Type' = 'application/json'
}
$query = "SELECT [System.Id], [System.Title], [System.WorkItemType], [System.State], [Project.ReleaseNotes] FROM workitems WHERE [System.TeamProject] = @project AND [System.WorkItemType] IN ('User Story', 'Bug') AND ( [System.State] = 'Resolved' OR [System.State] = 'Closed' ) AND NOT [System.Tags] CONTAINS 'Internal' AND NOT [System.Tags] CONTAINS 'Product' AND [Project.DocumentationImpact] CONTAINS '$packageVersion' ORDER BY [System.WorkItemType] DESC"
$body = "{'query': ""$query""}"
$uri = $TfsWiqlUrl + '?timePrecision=true&$top=50'
$query
$wiqlResult = (Invoke-RestMethod -Method Post -Headers $headers -UseDefaultCredentials -uri $uri -Body $body)
#$wiqlResult
# build columns to be returned in query
$columns = "";
foreach ($column in $wiqlResult.columns) {
	if ( $columns )
	{
		$columns += ","
	}
	$columns +=  "$($column.referenceName)"
}

# build work item ids to be returned in query
$workItemIds = "";
foreach ($column in $wiqlResult.workItems) {
	if ( $workItemIds )
	{
		$workItemIds += ","
	}
	#Write-Host "$($column.id)"
	$workItemIds += "$($column.id)"
}

$releaseNotesContent = ""
if ( $workItemIds )
{
	$uri = "$TfsServerUrl/_apis/wit/workItems?ids=$workItemIds&fields=$columns"
	$WorkItems =  (Invoke-RestMethod -Method Get -UseDefaultCredentials -uri $uri).value | Select-Object -Property Id -ExpandProperty Fields

	# Header
	$releaseNotesContent += "## User Stories/Bugs`n`n"
	$releaseNotesContent += "| Title        | Notes            |`n"
	$releaseNotesContent += "| :----------- | :--------------- |`n"
	
	foreach ( $column in $wiqlResult.workItems )
	{
		$workItemId = "$($column.id)"
		foreach ($wi in $WorkItems)
		{
			if ($wi.'id' -eq $workItemId) {
				$item = $wi
			}
		}
		if ( -not $item )  { 
			throw "cabum" 
		}
		#Write-Host $item.'id';continue;

		$releasenotes = $($item.'Project.ReleaseNotes')
		if( -not $releasenotes ) {
			$releasenotes = '-'
		}
		# Remove HTML tags
		$releasenotes = $releasenotes -replace "<[^>]*?>|<[^>]*>", ""

		$idSection = "**$($item.'id')**"
		$isBug = -not ( $item.'System.WorkItemType' -eq 'User Story' )
		if ( $isBug ) {
			$idSection = "<span style='color:red'>$idSection</span>"
		}
		$releaseNotesContent += "| $idSection $($item.'System.Title') | $($releasenotes) |`n"
	}
	
	$releaseNotesContent += "`nBugs are marked in <span style='color:red'>red</span>`n"
}

$packageIncludedSplitBySemiColon = $packagesIncluded.Split(';')
$packagesIncluded = ""
foreach( $packageToInclude in $packageIncludedSplitBySemiColon ) {
	if ( $packagesIncluded ) {
		$packagesIncluded += ", "
	}
	
	if ( $packageToInclude -and (Test-Path "$releaseNotesFolderPath\$packageToInclude.md") ) {
		$packageToInclude = "[$($packageToInclude)]($releaseNotesUrl$($packageToInclude))"
	}

	$packagesIncluded += $packageToInclude
}

$sprint = $packageId.Split("-")[1]
(Get-Content "$packageTemplateFile" -Encoding UTF8)    `
	-replace '@PackageId@', $packageId	`
	-replace '@SprintNumber@', "$($sprint -replace 'Sprint', '')"	`
	-replace '@PackageVersion@', $packageVersion	`
	-replace '@ExpectedReleaseDate@', $expectedReleaseDate	`
	-replace '@MESVersion@', $mesVersion	`
	-replace '@CustomDependsOn@', $customDependsOnVersion	`
	-replace '@UserStories@', $releaseNotesContent	`
	-replace '@PackageDeliverablesIncluded@', $packagesIncluded	`
| Set-Content ($packageTemplateFile -replace 'packagetemplate', "$packageId.md") -Encoding UTF8
