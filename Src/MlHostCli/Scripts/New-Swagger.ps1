<#
.DESCRIPTION
	Execute MlHostCli to generate swagger for ML-Host

.PARAMETER ModelName
	Name of the model (lower alpha numeric, '-').  Example: "sentiment"

.PARAMETER Environment
	Name of environment, valid names are "dev", "acpt", "prod"

.PARAMETER SwaggerFile
	File to write swagger
#>

Param(
	[Parameter(Mandatory)]
	[string] $ModelName,

    [Parameter(Mandatory)]
    [ValidateSet("dev", "acpt", "prod")]
	[string] $Environment,

	[Parameter(Mandatory)]
	[string] $SwaggerFile,

	[string] $CliBinPath
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

& $CliBinPath Swagger ModelName=$ModelName Environment=$Environment SwaggerFile=$SwaggerFile;

