using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;
using Shiny;
//using Device = Plugin.BLE.iOS.Device;

// using Shiny.BluetoothLE;

namespace MauiApp2;
public partial class btPage : ContentPage
{
    IBluetoothLE ble;// = CrossBluetoothLE.Current;
    IAdapter  adapter = CrossBluetoothLE.Current.Adapter;
    List<IDevice> deviceList;
    //ObservableCollection<IDevice> deviceList;
    IDevice device;
    IReadOnlyList<IService> services;

    public btPage()
    {
        InitializeComponent();
        adapter.ScanMode = ScanMode.LowLatency;
        ble = CrossBluetoothLE.Current;
        //adapter = CrossBluetoothLE.Current.Adapter; //this being here is causing btpage not to open?????
        deviceList = new List<IDevice>();
        deviceList.Clear();
        //adapter.ScanTimeout = 60000; //timeout for bluetooth scanning 60 seconds(?)
    }
    private void OnConnBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
    
    async void ScanClicked(object sender, EventArgs e)
    {
        try
        {
            //deviceList.Clear();
            ScanBtn.Text = "Scanning...";
            adapter.DeviceDiscovered += (s,a) => deviceList.Add(a.Device);
            //DeviceO.Text = $"Name: {deviceList[0].Name}, ID: {deviceList[0].Id}\n";
            if (!ble.Adapter.IsScanning)
            {
                //DisplayAlert("Alert", "Nice", "OK");
                Console.WriteLine("Scanning...");
                await adapter.StartScanningForDevicesAsync();
            }
            //scans for devices and adds discovered devices to device list
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", "Scan didn't work", "OK");
        }
        
    }
    
    void Checkcheck(object sender, EventArgs e)//temporary check to see if scanner is working
    {
        if (deviceList[0] == null)
        {
            DisplayAlert("Alert", "fuck", "OK");
        }
        else
        {
            DeviceO.Text = $"Name: {deviceList[0].Name}, ID: {deviceList[0].Id}\n";
            for (int i = 1; i < 10; i++)
            {
                DeviceO.Text += $"Name: {deviceList[i].Name}, ID: {deviceList[i].Id}\n";
            }
        }
    }

    async void BtEntered(object sender, EventArgs e)
    {
        Guid devId = new Guid("62bee4be-e214-6607-7526-91db3e197832"); 
        //this uses the bluetooth id of michael's headphones for testing 
        //if (deviceList[0].IsConnectable)
        //{
            //Console.WriteLine("Gate 1 passed");
            try
            {
                Console.WriteLine("Trying to Connect"); //tries connecting to the device with ID devId
                device = await adapter.ConnectToKnownDeviceAsync(devId);
                //await adapter.ConnectToDeviceAsync(deviceList[0]);
            }
            catch (DeviceConnectionException erm)
            {
                // ... could not connect to device
                DisplayAlert("Alert", "Unable to connect to device", "OK");
            }
            finally
            {
                if (device != null) //if a device is connected, write to console, get its services, and display the name
                {
                    Console.WriteLine("Connected to Device");
                    services = await device.GetServicesAsync();
                    ConDev.Text = device.Name;
                } else Console.WriteLine("Failed to Connect");
            }
        //} else DisplayAlert("Alert", "Unable to connect to device", "OK");
        
    }

    async void getServices(object sender, EventArgs e)
    {
        if (device != null)//displays the first service of the device in the console
        {
            Console.WriteLine("Service 1: " + services[0].Name);
        } else DisplayAlert("Alert", "Connect to a device", "OK");
    }

    private void Connectivity_ConnectivityChanged(object sender, EventArgs e)
    {
        /*if (c.NetworkAccess == NetworkAccess.ConstrainedInternet)
            Console.WriteLine("Internet access is available but is limited.");

        else if (c.NetworkAccess != NetworkAccess.Internet)
            Console.WriteLine("Internet access has been lost.");

        // Log each active connection
        Console.Write("Connections active: ");

        foreach (var item in c.ConnectionProfiles)
        {
            switch (item)
            {
                case ConnectionProfile.Bluetooth:
                    Console.Write("Bluetooth");
                    break;
                case ConnectionProfile.Cellular:
                    Console.Write("Cell");
                    break;
                case ConnectionProfile.Ethernet:
                    Console.Write("Ethernet");
                    break;
                case ConnectionProfile.WiFi:
                    Console.Write("WiFi");
                    break;
                default:
                    break;
            }
        }

        Console.WriteLine();*/
        
    }
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        Shell.Current.GoToAsync(nameof(MainPage));
    }
}