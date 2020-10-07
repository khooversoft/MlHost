<#
.DESCRIPTION
	Generate ML Test model to be used with intergration tests, uses fake model

.PARAMETER CliBinPath
	Path where ML Host CLI exceute is

.PARAMETER SpecFile
	Path where ML Package Specification file is store

.PARAMETER PackageFile
	Where to write ML Package

.PARAMETER ModelExePath
	Path of execute for fake model server
#>

Param(
	[Parameter(Mandatory)]
    [string] $SpecPath,
    
    [switch] $UseSecretId,

	[string] $Configuration = "debug"
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

if( !(Test-Path $SpecPath) )
{
	Write-Error "Cannot find $SpecPath";
	return;
}

$CliBinPath = [System.IO.Path]::Combine($currentLocation, "..\bin\$Configuration\netcoreapp3.1\MlHostCli.exe");

if( !(Test-Path $CliBinPath) )
{
	Write-Error "Cannot find MlHostCli.exe at $CliBinPath";
	return;
}

if( $UseSecretId )
{
    $SecretCmd = "SecretId=MlHostCli.exe";
}

& $CliBinPath Build Delete Upload $SecretCmd SpecFile=$SpecPath;

