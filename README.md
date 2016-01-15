# RideSafely

This is a demo of device-to-cloud telemetry and cloud-to-device command & control by using Windows 10 IoT Core and Microsoft Azure IoT Hub.

# Table of Contents
1. [Scenario Description](#Task1)
2. [What you will need](#Task2)
3. [Cloud Setup](#Task3)
4. [Device Setup](#Task4) (Optional)
5. [Compiling and deploying the code](#Task5)
6. [Running the solution](#Task6)
7. [Summary](#Summary)

<a name="Task1"></a>
## 1. Scenario Description

Motorcycle riders are a tight-knit group. They usually like to share the fun by going out in groups, sometimes very large ones.
One aspect that's often overlooked in such outings - especially if some people in the group are relatively new to riding - is keeping the group together. We can't have people wander off and leave the group, and we also don't want newbies to lose contact with the rest of the group, as getting large groups back together is very time-consuming. 

Group management is usually the concern of the group leader, who always rides up front. We'd like to make their job easier by automatically notifying him or her whenever one or more riders loses contact with the group.

For the purposes of this demo, we are using an [Raspberry Pi](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)-based appliance that's attached to the motorcycle, which is connected to an ultrasonic ranger sensor to measure distance to the rider ahead. 
Whenever the distance to the rider ahead is greater than a specific threshold, we communicate this fact to the leader.
> **Note:** If you don't have a Raspberry Pi, the solution also runs on your PC (the device and the ranger sensor are simulated in this case).

For device messaging we use [Microsoft Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/), a cloud-based service for the IoT space which allows device telemetry ingestion, device management and security, and also has command-and-control features.

Whenever a rider loses touch with the rest of the group, a message is sent from the device to the IoT Hub. Each message is then processed and sent to the group leader, and displayed on their dashboard.

<a name="Task2"></a>
## 2. What you will need

For this project you will need the following:

1. A Microsoft Azure Subscription. [Click here](https://azure.microsoft.com/en-us/pricing/free-trial/) to get a free trial subscription.
2. A PC or laptop with Windows 10 installed.
3. Visual Studio 2015. [Click here](https://www.visualstudio.com/en-us/visual-studio-homepage-vs.aspx) to download the free Community Edition.
4. Windows Powershell Tools for Azure. [Click here](http://aka.ms/webpi-azps) to download.
5. (Optional) A [Raspberry Pi 2](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/)
6. (Optional) A [GrovePi+ Starter Kit](http://www.dexterindustries.com/shop/GrovePi-starter-kit-2/) containing the Ultrasonic Ranger Sensor.
7. (Optional) Windows 10 IoT Core for Raspberry Pi 2. [Click here](http://ms-iot.github.io/content/en-US/Downloads.htm) to download it for free.

> **Note:** The device side of this demo uses a Raspberry Pi 2 as the bike-connected appliance, and the GrovePi+ board for connecting to sensors.
You are welcome to use another setup if you like, and modify the code to suit your own hardware.

> **Note:** If you do not have the Raspberry Pi 2 and GrovePi+ boards, you can simulate them by deploying the solution onto an x64 machine (your PC or laptop).

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

These two devices will represent our leader and one of our followers in the ride group.

For each device, make a note of the *ConnectionString* and *PrimaryKey* values, as you will need these later to configure your solution.

<a name="Task4"></a>
## 4. Device Setup (Optional)

If you do have a Raspberry Pi 2 and a GrovePi+ together with sensors, here is the setup you need:

1. [Install Windows 10 IoT Core on the Raspberry Pi 2](http://ms-iot.github.io/content/en-US/win10/RPI.htm).
2. Connect the GrovePi+ to the Raspberry Pi by using the GPIO pins (see the GrovePi manual).
3. Connect the **Ultrasonic Ranger Sensor** to the GrovePi **digital pin 4**.
4. Connect any **LED** (for example Green LED) to the GrovePi **digital pin 8**.
5. Connect the **LCD Display** to **any I2C port** on the GrovePi. 
6. Connect the Raspberry Pi to the same network as your PC or laptop, for example by using an Ethernet cable to your router or switch.
7. Connect the Raspberry Pi to a monitor by using an HDMI cable.

If you've done it right, this is what your setup should look like:

Now you can power up your Raspberry Pi. 

Once it boots up, you should be looking at a screen similar to the one below. Make a note of the device's IP address, as you will need it later when you deploy the solution.

<a name="Task5"></a>
## 5. Compiling and deploying the code

Open up the **RideSafely** solution in Visual Studio.

### Device

The **RideSafely.DeviceApp** is the app that runs on the device. It is a Universal Windows Platform app (UWP) and will run on both an x86 machine as well as an ARM machine (Raspberry Pi 2 with Windows 10 IoT Core installed).

#### Raspberry Pi and GrovePi

In the ARM configuration, it references and uses the **RideSafely.GrovePi** project to connect to the sensor and the LCD display and LED.

```cs
   public class GroveManager : IDeviceManager
    {
        // GrovePi Pin Configuration - configure these to match the sensors you plugged into GrovePi+
        // you can connect the LCD display to any I2C port.
        private static Pin LedPin = Pin.DigitalPin8;
        private static Pin UltraSonicSensorPin = Pin.DigitalPin4;

        private IRgbLcdDisplay LCD { get; set; }
        private IUltrasonicRangerSensor UltrasonicRangerSensor { get; set; }
        private ILed Led { get; set; }

        public GroveManager()
        {
            this.LCD = DeviceFactory.Build.RgbLcdDisplay();
            this.Led = DeviceFactory.Build.Led(LedPin);
            this.UltrasonicRangerSensor = DeviceFactory.Build.UltraSonicSensor(UltraSonicSensorPin);
        }

        public void ChangeAlarmState(bool alarmOn)
        {
            this.Led.ChangeState(alarmOn ? SensorStatus.On : SensorStatus.Off);
        }

        public void DisplayMessage(string message)
        {
            this.LCD.SetText(message);
        }

        public int GetDistanceFromLeader()
        {
            return UltrasonicRangerSensor.MeasureInCentimeters();
        }
    }
```

In this setup, you select the **ARM** platform architecture and **Remote Machine** as the deployment target.

In the **RideSafely.DeviceApp** project properties, in the **Debug** tab, enter the IP address of the Raspberry Pi in the **Remote Machine** field.

> **NOTE**: Your Raspberry Pi's IP address should be displayed on the monitor you connected it to.

#### Simulated

In the **x86** configuration, it simulates the ranger sensor, LCD and LED via the **SimulatedDeviceManager** class in the **DeviceApp** project. Essentially it receives random values for the distance, and does not display anything.

```cs
    public class SimulatedDeviceManager : IDeviceManager
    {
        private Random random;
        public SimulatedDeviceManager()
        {
            this.random = new Random();
        }
        public void ChangeAlarmState(bool alarmOn)
        {            
        }

        public void DisplayMessage(string message)
        {           
        }

        public int GetDistanceFromLeader()
        {
            return random.Next(1000);
        }
    }
```

In this setup, you select the **x86** or **AnyCPU** platform architecture and **Local Machine** as the deployment target.

### Build

Whether you selected ARM or x86, compile your solution in **Debug** mode. 

Then deploy it by clicking **Start** (or hitting F5) in Visual Studio.
