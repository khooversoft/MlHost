<#
.DESCRIPTION
	Clear, rebuild, publish and run n number of ML host.  Used for testing ML Frontend
#>

$debugpreference = "Continue";
$ErrorActionPreference = "Stop";

Write-Host "Starting ML services..."
Write-Host "***"
Write-Host "***"

Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\idcard-classifier-v1\idcard-classifier-v1a.mlPackage" -Port 5000 -ModelPort 5020';
Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\intent-classifier-v3\intent-classifier-v1a.mlPackage" -Port 5001 -ModelPort 5021';
Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\emotional-sentiment-v1\emotional-sentiment-v1a.mlPackage" -Port 5002 -ModelPort 5004';
#Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-MlFrontEnd.ps1';
