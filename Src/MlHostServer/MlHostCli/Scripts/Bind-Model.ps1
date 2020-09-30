<#
.DESCRIPTION
	Execute MlHostCli to download a ML Model from ADLS

.PARAMETER ModelName
	Name of the model (lower alpha numeric, '-').  Example: "sentiment"

.PARAMETER VersionId
	Name of the version (lower alpha numeric, '-').  Example: "sentiment-demo"

.PARAMETER PackagePath
	Path to ML Package

.PARAMETER CliBinPath
	Path to the MlHostCli.EXE utility (uploads the ML Model)

.PARAMETER ConfigFile
	Path to the configuration file, stores Key Vault and Store configurations.
#>

Param(
	[Parameter(Mandatory)]
	[string] $ModelName,

	[Parameter(Mandatory)]
	[string] $VersionId,

	[string] $VsProject,

	[string] $CliBinPath,

	[string] $ConfigFile,

	[string] $AccountKey
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

Write-Host "Current location= $currentLocation";

if( !$CliBinPath )
{
	$CliBinPath = [System.IO.Path]::Combine($currentLocation, "..\bin\Release\netcoreapp3.1\MlHostCli.exe");
}

if( !(Test-Path $CliBinPath) )
{
	Write-Error "Cannot find MlHostCli.exe at $CliBinPath";
	return;
}

if( !$ConfigFile )
{
	$ConfigFile = [System.IO.Path]::Combine($currentLocation, "..\Config\PublishProdConfig-Activate.json");
}

if( !(Test-Path $ConfigFile) )
{
	Write-Error "Cannot find configuration file at $ConfigFile";
	return;
}

if( !$VsProject )
{
	$VsProject = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\MlHost.csproj");
}

if( !(Test-Path $VsProject) )
{
	Write-Error "Cannot find VS Project $VsProject";
	return;
}

if( $AccountKey )
{
	$AccountKeyCmd = "Store:AccountKey=$AccountKey";
}

# Bind downloaded model to ML Host (make embedded)
& $CliBinPath Bind ModelName=$ModelName VersionId=$VersionId ConfigFile=$ConfigFile VsProject=$VsProject $AccountKeyCmd;

if($LASTEXITCODE -ne 0)
{
    Write-Error "Bind command failed!";
}

