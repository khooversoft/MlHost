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

.PARAMETER Force
	Overwrite ML package in ADLS.
#>

Param(
	[Parameter(Mandatory)]
	[string] $ModelName,

	[Parameter(Mandatory)]
	[string] $VersionId,

	[Parameter(Mandatory)]
	[string] $PackagePath,

	[string] $CliBinPath,

	[string] $ConfigFile,

	[switch] $Force
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

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
	$currentLocation = Get-Location;
	$ConfigFile = [System.IO.Path]::Combine($currentLocation, "..\Config\PublishProdConfig-Activate.json");
}

if( !(Test-Path $ConfigFile) )
{
	Write-Error "Cannot find configuration file at $ConfigFile";
	return;
}

if( !(Test-Path $PackagePath) )
{
	Write-Error "Cannot find ML Package at $PackagePath";
	return;
}

if( $Force )
{
	$forceCmd = "Force=true"
}

& $CliBinPath Upload ModelName=$ModelName VersionId=$VersionId PackageFile=$PackagePath ConfigFile=$ConfigFile $forceCmd;

