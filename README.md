# RideSafely

This is a demo of device-to-cloud telemetry and cloud-to-device command & control by using Windows 10 IoT Core and Microsoft Azure IoT Hub.

# Table of Contents
1. [Scenario Description](#Task1)
2. [What you will need](#Task2)
3. [Cloud Setup](#Task3)
4. [Compiling and deploying the code](#Task4)
5. [Running the solution](#Task5)
6. [Summary](#Summary)

<a name="Task1"></a>
## 1. Scenario Description

Motorcycle riders are a tight-knit group. They usually like to share the fun by going out in groups, sometimes very large ones.
One aspect that's often overlooked in such outings - especially if some people in the group are relatively new to riding - is keeping the group together. We can't have people wander off and leave the group, and we also don't want newbies to lose contact with the rest of the group, as getting large groups back together is very time-consuming. 

Group management is usually the concern of the group leader, who always rides up front. We'd like to make their job easier by automatically notifying him or her whenever one or more riders loses contact with the group.

For the purposes of this demo, we are using an [Raspberry PI](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)-based appliance that's attached to the motorcycle, which is connected to an ultrasonic ranger sensor to measure distance to the rider ahead. 
Whenever the distance to the rider ahead is greater than a specific threshold, we communicate this fact to the leader.
> **Note:** If you don't have a Raspberry PI, the solution also runs on your PC (the device and the ranger sensor are simulated in this case).

For device messaging we use [Microsoft Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/), a cloud-based service for the IoT space which allows device telemetry ingestion, device management and security, and also has command-and-control features.

Whenever a rider loses touch with the rest of the group, a message is sent from the device to the IoT Hub. Each message is then processed and sent to the group leader, and displayed on their dashboard.

<a name="Task2"></a>
## 2. What you will need

For this project you will need the following:

1. A Microsoft Azure Subscription. [Click here](https://azure.microsoft.com/en-us/pricing/free-trial/) to get a free trial subscription.
2. A PC or laptop with Windows 10 installed.
3. Visual Studio 2015. [Click here](https://www.visualstudio.com/en-us/visual-studio-homepage-vs.aspx) to download the free Community Edition.
4. Windows Powershell Tools for Azure. [Click here](http://aka.ms/webpi-azps) to download.
5. (Optional) A [Raspberry PI 2](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)
6. (Optional) A [GrovePI+ Starter Kit](http://www.dexterindustries.com/shop/grovepi-starter-kit-2/) containing the Ultrasonic Ranger Sensor.
7. (Optional) Windows 10 IoT Core for Raspberry PI 2. [Click here](http://ms-iot.github.io/content/en-US/Downloads.htm) to download it for free.

> **Note:** The device side of this demo uses a Raspberry PI 2 as the bike-connected appliance, and the GrovePI+ board for connecting to sensors.
You are welcome to use another setup if you like, and modify the code to suit your own hardware.

> **Note:** If you do not have the Raspberry PI 2 and GrovePI+ boards, you can simulate them by deploying the solution onto an x64 machine (your PC or laptop).

<a name="Task3"></a>
## Cloud Setup

For the cloud (back-end) part of this solution, we need to deploy the following onto an Azure subscription:

1. A [Resource Group](https://azure.microsoft.com/en-us/documentation/articles/resource-group-portal/), which is a logical container for the rest of the cloud components.
2. An [IoT hub](https://azure.microsoft.com/en-us/services/iot-hub/) (Free tier) to receive events from devices, and relay commands to the leader.
3. An [App Service](https://azure.microsoft.com/en-us/services/app-service/) (and associated Free app service plan) to host the server-side code which will read events from the hub, and send back commands. This code will be deployed as an [Azure WebJob](https://azure.microsoft.com/en-us/documentation/articles/web-sites-create-web-jobs/).
4. A [Storage Account](https://azure.microsoft.com/en-us/documentation/services/storage/) to act as a backend for monitoring.

You can create the above services in one of two ways:

### Powershell setup

We've prepared a Powershell script to create all the above components for you automatically. You can find this script as **src/Azure/scripts/CreateAzureObjects.ps1**.

Open up the file in your editor of choice, and edit the first three lines (script parameters).
Make sure you edit the name of the hub, and this needs to be unique (it's part of the DNS name of the hub).

```powershell
# fill in Azure parameters 
$subscriptionId = 'xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx' # put in your subscription Id here; use the Get-AzureRmSubscription cmdlet if you need to find out the Id
$resourceGroupName = 'RideSafely' # you must change this since the IoT Hub Hostname needs to be unique
$location = 'North Europe'
```

Then run the script. At the end, you should be able to see the Resource Group containing the above resources in the [Azure Portal](http://portal.azure.com/).


### Portal setup

You can alternatively create each component from the [Azure Portal](http://portal.azure.com/).

### Cloud Device management

Once you've created the IoT Hub, you need to register your devices so they are recognized and allowed to communicate with the hub. 

The IoT Hub allows this [through an open source SDK](https://github.com/Azure/azure-iot-sdks). The demo also uses this SDK to connect to the IoT Hub and send messages; it's referenced [as a NuGet package](https://www.nuget.org/packages/Microsoft.Azure.Devices.Client/).

Fortunately, as part of the SDK the good folks at Microsoft have also included a GUI device management tool called [Device Explorer](https://github.com/Azure/azure-iot-sdks/blob/master/tools/DeviceExplorer/doc/how_to_use_device_explorer.md). Get this tool and run it.

Use the instructions provided in the link above to connect to your IoT Hub. Then register two devices with the following Ids:

1. **ridesafely-leader**
2. **ridesafely-follower**

These two devices will represent our leader and one of our followers.

For each device, make a note of the *ConnectionString* and *PrimaryKey* values, as you will need these later to configure your solution.

<a name="Task4"></a>
## 4. Compiling and deploying the code

Open up the RideSafely solution in Visual Studio.



