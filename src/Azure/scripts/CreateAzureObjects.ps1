# fill in Azure parameters 
$subscriptionId = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx' # put in your subscription Id here; use the Get-AzureRmSubscription cmdlet if you need to find out the Id
$resourceGroupName = 'RideSafely' # you must change this since the IoT Hub Hostname needs to be unique
$location = 'North Europe'


# login to Azure and select subscription.
Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionId $subscriptionId

#if you need to delete a previously defined Resource Group and all its resources, uncomment the line below.
#Remove-AzureRmResourceGroup -Name $resourceGroupName

# create the resource group.
New-AzureRmResourceGroup -Name $resourceGroupName -Location $location

# create the IoT Hub.
New-AzureRmResource -Location $location -Properties @{"location"="$location"} -ResourceName "$resourceGroupName-Hub" -ResourceType "Microsoft.Devices/Iothubs" `
     -ResourceGroupName $resourceGroupName -SkuObject @{"name"="F1"; "tier"="Free"; "capacity"="1"} -Force

# create the App Service Plan
$plan = New-AzureRMAppServicePlan -Location $location -ResourceGroupName $resourceGroupName -Name "$resourceGroupName-AppPlan" -Tier Free 

# create the Azure App Service which will host your WebJob
New-AzureRmResource -Location $location -Properties @{"location"="$location"; "ServerFarmId"="$($plan.Id)"} -ResourceName "$resourceGroupName-App" -ResourceType "microsoft.web/sites" `
    -ResourceGroupName $resourceGroupName -Force

# create the storage account 
New-AzureRmStorageAccount -Location $location -Name "$($resourceGroupName.ToLower())store" -ResourceGroupName $resourceGroupName -Type Standard_LRS

