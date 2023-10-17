using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.ViewModel;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;
//using Plugin.BLE.iOS;
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
    private IReadOnlyList<ICharacteristic> charstics;
    private IReadOnlyList<IDescriptor> descs;
    string deviceName;
    private IReadOnlyList<ICharacteristic> batteryChars;
    private IReadOnlyList<IDescriptor> batCharDescs;
    private List<Ball> savedDots;
    private object selectBall;
    private Ball theBall;


    public btPage()
    {
        InitializeComponent();
        adapter.ScanMode = ScanMode.LowLatency;
        ble = CrossBluetoothLE.Current;
        //adapter = CrossBluetoothLE.Current.Adapter; //this being here is causing btpage not to open?????
        deviceList = new List<IDevice>();
        deviceList.Clear();
        savedDots = new List<Ball>();//dont keep this
        savedDots.Add(new Ball(null));
        DotsList.ItemsSource = savedDots;
        //adapter.ScanTimeout = 60000; //timeout for bluetooth scanning 60 seconds(?)
        
        Ball ball1 = new Ball(null); //test balls for display purposes
        ball1.name="ball one";
        Ball ball2 = new Ball(null);
        ball2.name="ball two";
        Ball ball3 = new Ball(null);
        ball3.name="ball three";
        savedDots.Add(ball1);
        savedDots.Add(ball2);
        savedDots.Add(ball3);
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
            if (!ble.Adapter.IsScanning)
            {
                Console.WriteLine("Scanning...");
                await adapter.StartScanningForDevicesAsync();
            }
            //scans for devices and adds discovered devices to device list
        }
        catch (Exception ex)
        {
            DisplayAlert("Alert", "Scan Failed", "OK");
        }
        
    }
    
    void Checkcheck(object sender, EventArgs e)//temporary check to see if scanner is working
    {
        if (deviceList[0] == null)
        {
            DisplayAlert("Alert", "Scan failed", "OK");
        }
        else
        {
            Devices.Text = $"Name: {deviceList[0].Name}, ID: {deviceList[0].Id}\n";
            for (int i = 1; i < 10; i++)
            {
                Devices.Text += $"Name: {deviceList[i].Name}, ID: {deviceList[i].Id}\n";
            }
        }
    }

    async void BtEntered(object sender, EventArgs e)
    {
        Guid devId = new Guid("62bee4be-e214-6607-7526-91db3e197832"); 
        //this uses the bluetooth id of michael's headphones for testing 
        //if (deviceList[0].IsConnectable)
        //{
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
                if (device != null) //if a device is connected, write to console, get device info
                {
                    Console.WriteLine("Connected to Device");
                    services = await device.GetServicesAsync();
                    //charstics = await services[0].GetCharacteristicsAsync();
                    ConDev.Text = "Connected: " + device.Name;
                } else Console.WriteLine("Failed to Connect");
            }
        //} else DisplayAlert("Alert", "Unable to connect to device", "OK");
        
    }
    
    private void OnDevChanged(object sender, TextChangedEventArgs e)
    {
        string oldName = e.OldTextValue;
        string newName = e.NewTextValue;
        deviceName = devName.Text;
    }

    async void OnDevEnter(object sender, EventArgs e)
    {
        Console.WriteLine("Searching List...");
        deviceName = devName.Text;
        for (int i = 0; i < deviceList.Count(); i++)
        {
            if (deviceList[i].Name == deviceName)
            {
                try
                {
                   await adapter.ConnectToDeviceAsync(deviceList[i]);
                   device = deviceList[i];
                }
                catch (DeviceConnectionException erm)
                {
                    DisplayAlert("Alert", "Unable to connect to device", "OK");
                }
                finally
                {
                    if (device != null) //if a device is connected, write to console, get device info
                    {
                        Console.WriteLine("Connected to Device");
                        services = await device.GetServicesAsync();
                        //charstics = await services[0].GetCharacteristicsAsync();
                        ConDev.Text = "Connected: " + device.Name;
                        savedDots.Add(new Ball(device)); //adds device to list of saved balls
                        if (savedDots[0] != null)
                        {
                            Dots.Text += savedDots[0].name;
                            //Dots.Text += savedDots[0].getDev().Id;
                        }
                        else Console.WriteLine("ball class no work");
                    } else Console.WriteLine("Failed to Connect");
                }
                break;
            }
        }

        if (device == null)
        {
            DisplayAlert("Alert", "Unknown Device", "OK");
        }
    }


    async void getServices(object sender, EventArgs e)
    {
        if (device != null)//displays all device info from service down to characteristic
        {
            for (int j = 0; j < services.Count(); j++)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine("Service "+j+": " + services[j].Name);
                DevInfo.Text += $"Service: {services[j].Name}\n";
                charstics = await services[j].GetCharacteristicsAsync();
                for (int i = 0; i < charstics.Count(); i++)
                {
                    Console.WriteLine(".....Characteristic "+j+"-" + i + ": " + charstics[i].Name);
                    if (charstics[i].CanRead)
                    {
                        var charBytes = await charstics[i].ReadAsync();
                        Console.WriteLine("     Read Bytes: " + charstics[i].StringValue);
                    }
                    DevInfo.Text += $"      Characteristic: {charstics[i].Name}\n";
                    
                    descs = await charstics[i].GetDescriptorsAsync();
                    for (int c = 0; c < descs.Count(); c++)
                    {
                        Console.WriteLine("..........Descriptor "+j+"-" + i + "-" + c+ ": " + descs[0].Name);
                        DevInfo.Text += $"                Descriptor: {descs[i].Name}\n";
                    }
                }
            }
            Console.WriteLine("------------------------------------");
            //read bytes from 1st characteristic of 3rd service for iPhone battery
            batteryChars = await services[3].GetCharacteristicsAsync();
            if (batteryChars[0].CanRead)
            {
               var bytes = await batteryChars[0].ReadAsync();
               //var stringBytes = batteryChars[0].StringValue;// same as bytes.data;
               Console.WriteLine("Battery: " + bytes.data[0] + "%");
               DevInfo.Text += $"Battery: {bytes.data[0]}%";
            } else Console.WriteLine("Can't Do it Boss");
            
        } else DisplayAlert("Alert", "Connect to a device", "OK");
    }

    private void OnBallSelect(object sender, EventArgs e)
    {
        selectBall = DotsList.SelectedItem;
        theBall = (Ball) selectBall;
        if (selectBall != null)
        {
           DisplayAlert("Ball Specs", theBall.name + "\n" + 
                                      "Weight: " + theBall.weight + "lbs.\n" + 
                                      "Color: " + theBall.color + "\n" + 
                                      "ID: " + /*savedDots[2].dev.Id*/"Device Placeholder" + "\n", 
                               "Done"); 
        } else Console.WriteLine("Selection failed");
    }
    
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        Shell.Current.GoToAsync(nameof(MainPage));
    }
}