using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.ViewModel;
//using MauiApp2.MetaWearInterfaces;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;
using TextChangedEventArgs = Microsoft.Maui.Controls.TextChangedEventArgs;

namespace MauiApp2;

public partial class SimPage : ContentPage
{
    IBluetoothLE ble;// = CrossBluetoothLE.Current;
    IAdapter  adapter = CrossBluetoothLE.Current.Adapter;
    List<IDevice> deviceList;
    //ObservableCollection<IDevice> deviceList;
    IDevice device;
    string deviceName;
    IReadOnlyList<IService> services;
    private IReadOnlyList<ICharacteristic> charstics;
    private IReadOnlyList<IDescriptor> descs;
    private string displayRead;
    private string writeText;
    
    
    //sim vars
    private IService simServ;
    private ICharacteristic simWrite;
    
    public SimPage()
    {
        InitializeComponent();
        adapter.ScanMode = ScanMode.LowLatency;
        ble = CrossBluetoothLE.Current;
        //adapter = CrossBluetoothLE.Current.Adapter; //this being here is causing btpage not to open?????
        deviceList = new List<IDevice>();
        deviceList.Clear();
    }
    string ballwt = "N/A";
    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    /*private void OnDevChanged(object sender, TextChangedEventArgs e)
    {
        string oldName = e.OldTextValue;
        string newName = e.NewTextValue;
        deviceName = DevEntry.Text;
    }*/

    async void OnDevEnter(object sender, EventArgs e)
    {
        Console.WriteLine("Searching List...");
        deviceName = "LUKESCOOLLAPTOP";//hardcoded for lukes machine running sim
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
                        ConDev.Text = "Connected: " + device.Name;
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
    
    async void ScanClicked(object sender, EventArgs e)
    {
        try
        {
            //deviceList.Clear();
            ScanBtn.Text = "Scanning...";
            adapter.DeviceDiscovered += (s,a) => deviceList.Add(a.Device);
            //LoadData();
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
    
    async void getServices(object sender, EventArgs e)
    {
        if (device != null)//displays all device info from service down to characteristic
        {
            services = await device.GetServicesAsync();
            for (int j = 0; j < services.Count(); j++)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine("Service "+j+": " + services[j].Name + " - ID: " + services[j].Id);
                DevInfo.Text += $"Service: {services[j].Name}\n";
                charstics = await services[j].GetCharacteristicsAsync();
                for (int i = 0; i < charstics.Count(); i++)
                {
                    Console.WriteLine(".....Characteristic "+j+"-" + i + ": " + charstics[i].Name + " - ID: " + charstics[i].Id);
                    if (charstics[i].CanRead)
                    {
                        var charBytes = await charstics[i].ReadAsync();
                        Console.WriteLine("     Read Bytes: " + charstics[i].StringValue);
                        if (i == 1 && j == 1)
                        {
                            displayRead = charstics[i].StringValue;
                            ReadInfo.Text = "Read data: " + displayRead;
                        }
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
            
        } else DisplayAlert("Alert", "Connect to a device", "OK");
    }
    
    private void OnWriteChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        writeText = WriteData.Text;
    }

    void OnWriteEnter(object sender, EventArgs e)
    {
        writeText = ((Entry)sender).Text;
        WriteToSim(sender, e);
    }
    
    async void WriteToSim(object sender, EventArgs e)
    {
        if (device != null)
        {
           try
           {
               simServ = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
               simWrite = await simServ.GetCharacteristicAsync(new Guid("72563044-db33-4692-a45d-c5212eebabfa"));
               //string toSend = "Data from Michael!";
               byte[] writeBytes = Encoding.ASCII.GetBytes(writeText);//new byte[2] {5, 5};
               await simWrite.WriteAsync(writeBytes);
               Console.WriteLine("Writing to Sim");
           }
           catch (DeviceConnectionException erm)
           {
               Console.WriteLine("Write Failed");
           } 
        }
        else await DisplayAlert("Alert", "Connect to a device", "OK");
        
    }

    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        Navigation.PushAsync(new NextPage()); //navigate to stat page
    }
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("..");
    }
}

//CounterBtn.Text = $"Clicked {count} time";