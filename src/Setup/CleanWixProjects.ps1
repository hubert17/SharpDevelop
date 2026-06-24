$PSScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$filesPath = Join-Path $PSScriptRoot "Files.wxs"
$setupPath = Join-Path $PSScriptRoot "Setup.wxs"

# Backup original files if backups don't exist
if (-not (Test-Path "$filesPath.bak")) {
    Copy-Item $filesPath "$filesPath.bak"
    Write-Host "Created backup of Files.wxs"
}
if (-not (Test-Path "$setupPath.bak")) {
    Copy-Item $setupPath "$setupPath.bak"
    Write-Host "Created backup of Setup.wxs"
}

[xml]$filesXml = Get-Content $filesPath
[xml]$setupXml = Get-Content $setupPath

$ns = New-Object System.Xml.XmlNamespaceManager($filesXml.NameTable)
$ns.AddNamespace("wix", "http://schemas.microsoft.com/wix/2006/wi")

# Find all components in Files.wxs
$components = $filesXml.SelectNodes("//wix:Component", $ns)
$toDelete = New-Object System.Collections.Generic.List[string]

Write-Host "Checking components for missing files..."
foreach ($comp in $components) {
    $compId = $comp.Id
    $files = $comp.SelectNodes("wix:File", $ns)
    $shouldDelete = $false
    
    foreach ($file in $files) {
        $source = $file.Source
        if ($source) {
            $absPath = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, $source))
            if (-not (Test-Path $absPath)) {
                $shouldDelete = $true
                Write-Host "File not found: $absPath (Component: $compId)"
                break
            }
        }
    }
    
    if ($shouldDelete) {
        $toDelete.Add($compId)
        $comp.ParentNode.RemoveChild($comp) | Out-Null
    }
}

Write-Host "Total components marked for deletion: $($toDelete.Count)"

# Remove ComponentRefs from Setup.wxs
$compRefs = $setupXml.SelectNodes("//wix:ComponentRef", $ns)
$removedRefsCount = 0
foreach ($ref in $compRefs) {
    if ($toDelete.Contains($ref.Id)) {
        $ref.ParentNode.RemoveChild($ref) | Out-Null
        $removedRefsCount++
    }
}
Write-Host "Removed $removedRefsCount ComponentRefs from Setup.wxs"

# Also check for ComponentRefs in Files.wxs itself (in case there are nested features/groups)
$compRefsFiles = $filesXml.SelectNodes("//wix:ComponentRef", $ns)
$removedRefsFilesCount = 0
foreach ($ref in $compRefsFiles) {
    if ($toDelete.Contains($ref.Id)) {
        $ref.ParentNode.RemoveChild($ref) | Out-Null
        $removedRefsFilesCount++
    }
}
Write-Host "Removed $removedRefsFilesCount ComponentRefs from Files.wxs"

# Save updated XML files
$filesXml.Save($filesPath)
$setupXml.Save($setupPath)
Write-Host "Wxs files cleaned and saved successfully."
