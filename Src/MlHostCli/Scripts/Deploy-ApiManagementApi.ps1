<#
.DESCRIPTION
	Execute Azure API Management PS scripts to create if required "Product" and import API from swagger

.PARAMETER Environment
	Name of the environment, "dev", "acpt", "prod"

.PARAMETER SwaggerPath
	Path to the swagger json file.  Will read "title" and "x-apiPath" from this file.

.PARAMETER ProductId
	Azure API Management product name
#>

Param(
    [Parameter(Mandatory)]
    [ValidateSet("dev", "acpt", "prod")]
    [string] $Environment,

	[Parameter(Mandatory)]
	[string] $SwaggerPath,

    [string] $ProductId = "VirtualAssistantML",

    [string] $ProductName = "Virtual Assistant ML",

    [string] $ProductDescription = "Virtual Assistant Machine Learning Model API"
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";

$environments = @{
    "dev" = @{
        "Subscription" = "preprod-sub-01"
        "ResourceGroupName" = "dev-psp-shared-w2-01"
        "ApimServiceName" = "devapmtdsg01w2"
    }
    "acpt" = @{
        "Subscription" = "prod-sub-01"
        "ResourceGroupName" = "acpt-psp-shared-w2-01"
        "ApimServiceName" = "acptapmtdsg01w2"
    }
    "prod" = @{
        "Subscription" = "prod-sub-01"
        "ResourceGroupName" = "prod-psp-shared-w2-01"
        "ApimServiceName" = "prodapmtdsg01w2"
    }
}

$envContext = $environments[ $Environment ];

function SetSubscription
{
    #Switch to the correct subscription if required
    $currentSubscription = Get-AzContext
    if( $currentSubscription.Subscription.Name -ne $envContext.Subscription )
    {
        Write-Host "Current subscription is $($currentSubscription.Subscription.Name), setting to $($envContext.Subscription)";
        Set-AzContext -Subscription $envContext.Subscription;
    }
}

function VerifyApiManagement
{
    # Lookup API Management, must exist
    $apiManagement = Get-AzApiManagement -ResourceGroupName $envContext.ResourceGroupName -Name $envContext.ApimServiceName -ErrorAction SilentlyContinue;
    if( !$apiManagement )
    {
        Write-Error "Api management $($envContext.ApimServiceName) does not exist in resource group $($envContext.ResourceGroupName)";
        Exit(1);
    }
}

function ReadSwagger
{
    if( !( Test-Path $SwaggerPath) )
    {
        Write-Error $"Swagger $SwaggerPath does not exist";
        Exit(1);
    }

    Write-Host "Reading $SwaggerPath for title and api path";
    $json = Get-Content $SwaggerPath | ConvertFrom-Json;

    $values = @{
        "Title" = $json.Info.title
        "ApiPath" = $json.Info."x-apiPath";
    }

    return $values;
}

function SetProduct
{
    param(
        [Parameter(Mandatory)]
        [PSObject] $Context
    )

    $product = Get-AzApiManagementProduct -Context $Context -ProductId $ProductId -ErrorAction SilentlyContinue;
    if( !$product )
    {
        Write-Host "Creating Product $ProductId";
        $product = New-AzApiManagementProduct `
            -Context $Context `
            -ProductId $ProductId `
            -Title $ProductName `
            -Description $ProductDescription `
            -State Published `
            -SubscriptionRequired $true;
    }

    return $product;
}

SetSubscription;
VerifyApiManagement;

# Create the API Management context
$context = New-AzApiManagementContext -ResourceGroupName $envContext.ResourceGroupName -ServiceName $envContext.ApimServiceName

$product = SetProduct -Context $context;

$apiSwagger = ReadSwagger

$currentApi = Get-AzApiManagementApi -Context $context -Name $apiSwagger.Title;
if( $currentApi )
{
    # Update API
    $api = Import-AzApiManagementApi `
        -Context $context `
        -SpecificationPath  $SwaggerPath `
        -SpecificationFormat Swagger `
        -Path $apiSwagger.ApiPath `
        -ApiId $currentApi.ApiId;
}
else
{
    # Create new
    $api = Import-AzApiManagementApi -Context $context -SpecificationPath  $SwaggerPath -SpecificationFormat Swagger -Path $apiSwagger.ApiPath;
}

# import api from Url
if( !$api )
{
    Write-Error "Import API from $SwaggerPath failed";
    Exit(1);
}

# Add the petstore api to the published Product, so that it can be called in developer portal console
Add-AzApiManagementApiToProduct -Context $context -ProductId $product.ProductId -ApiId $api.ApiId

Write-Host "Completed";
