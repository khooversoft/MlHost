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
    [string] $PackageFile,
    
	[Parameter(Mandatory)]
    [int] $Port,
    
	[Parameter(Mandatory)]
	[int] $ModelPort,
	
	[string] $Configuration = "debug"
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

if( !(Test-Path $PackageFile) )
{
	Write-Error "Cannot find $PackageFile";
	return;
}

$modelHostPath = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\bin\$Configuration\netcoreapp3.1\MlHost.exe");

if( !(Test-Path $modelHostPath) )
{
	Write-Error "Cannot find $modelHostPath";
	return;
}

& $modelHostPath PackageFile=$PackageFile Port=$Port ModelPort=$ModelPort;

