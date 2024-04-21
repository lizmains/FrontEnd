using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.ViewModel;
using MauiApp2.MetaWearInterfaces;
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
    private bool flag;
    private IService piServ;
    private ICharacteristic readChar;
    private (byte[] data, int resultCode) simData;
    
    
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
        flag = true;
    }
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
        deviceName = /*"ESAM";*/"raspberrypi";//hardcoded for lukes machine running sim
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

    async void OnPiConnect(object sender, EventArgs e)
    {
        try
        {
            await adapter.ConnectToKnownDeviceAsync(new Guid("2c1e6c52-e236-86f4-f3f3-760e9668a739"));
            device = adapter.ConnectedDevices[0];
        }
        catch (DeviceConnectionException erm)
        {
            await DisplayAlert("Alert", "Unable to connect to device", "OK");
        }
        finally
        {
            if (device != null) //if a device is connected, write to console, get device info
            {
                Console.WriteLine("Connected to Pi");
                ConDev.Text = "Connected: " + device.Name;
                services = await device.GetServicesAsync();
                //charstics = await services[0].GetCharacteristicsAsync();
                if (device.Name == "raspberrypi")
                { //Getting characteristics for pi interactions
                    piServ = await device.GetServiceAsync(new Guid("00000001-710e-4a5b-8d75-3e5b444bc3cf"));
                    readChar =//mms write characteristic, presumably
                        await piServ.GetCharacteristicAsync(new Guid("00000002-710e-4a5b-8d75-3e5b444bc3cf"));
                }
                        
                //await adapter.StopScanningForDevicesAsync();
            } else Console.WriteLine("Failed to Connect");
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
            Console.WriteLine("------------------------------------");
            Console.WriteLine("\n");
            /*IService serv0 = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
            Console.WriteLine("Got here");
            ICharacteristic char0 =
                await serv0.GetCharacteristicAsync(new Guid("72563044-DB33-4692-A45D-C5212EEBABFA"));
            Console.WriteLine("Can Read Read/Write: "+char0.CanRead);
            Console.WriteLine("Can Read Read/Write: "+char0.CanWrite);
            var bytes = await char0.ReadAsync();
            Console.WriteLine("Byte 0: "+bytes.data[0]);
            Console.WriteLine("Byte 0: "+bytes.data[1]);
            Console.WriteLine(bytes.data + " Got here too "+bytes.resultCode);
            for (int i = 0; i < bytes.data.Length; i++)
            {
                Console.WriteLine("Serv 0 Bytes: "+ bytes.data[i]);
            }*/

            services = await device.GetServicesAsync();
            for (int j = 0; j < services.Count; j++)
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
                            ReadInfo.Text += "Read data: " + displayRead;
                        }
                    }
                    else Console.WriteLine("\nCan't Read");
                    DevInfo.Text += $"      Characteristic: {charstics[i].Name}\n";
                    
                    descs = await charstics[i].GetDescriptorsAsync();
                    for (int c = 0; c < descs.Count(); c++)
                    {
                        Console.WriteLine("..........Descriptor "+j+"-" + i + "-" + c+ ": " + descs[0].Name);
                        DevInfo.Text += $"                Descriptor: {descs[i].Name}\n";
                    }
                }
            }

        } else DisplayAlert("Alert", "Connect to a device", "OK");
    }

    async void ReadSim(object sender, EventArgs e)
    {
        //for (int j = 0; j < 5; j++)
        //{
           await readChar.ReadAsync();
           Console.WriteLine("Reading sim data...");
           for (int i = 0; i < readChar.Value.Length; i++)
           {
               Console.WriteLine("Byte " + i + ": " + readChar.Value[i]);
           }
           
           ReadInfo.Text += "Sim Data: \n"+readChar.StringValue+"\n"; 
       // }
        
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
               
               /*IService servy0 = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
               Console.WriteLine("Got here");
               ICharacteristic chary0 =
                   await servy0.GetCharacteristicAsync(new Guid("72563044-DB33-4692-A45D-C5212EEBABFA"));
               byte[] writetest = Encoding.ASCII.GetBytes("writeBytes");
               await chary0.WriteAsync(writetest);*/
               
               simServ = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
               simWrite = await simServ.GetCharacteristicAsync(new Guid(/*"bc1926ea-6ffa-4d04-928b-76cccd068cea"*/"72563044-db33-4692-a45d-c5212eebabfa"));
               byte[] writeBytes = Encoding.ASCII.GetBytes(writeText);
               await simWrite.WriteAsync(writeBytes);
               Console.WriteLine("Writing to Sim...");
           }
           catch (DeviceConnectionException erm)
           {
               Console.WriteLine("Write Failed");
           } 
        }
        else await DisplayAlert("Alert", "Connect to a device", "OK");
        
    }

    async void listen(object sender, EventArgs e) //listener function for notify characteristics
    {
        IService theService = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
        ICharacteristic notify = await theService.GetCharacteristicAsync(new Guid("BC1926EA-6FFA-4D04-928B-76CCCD068CEA"));

        if (flag)
        {
            Console.WriteLine("Listening...");
            flag = false;
            await notify.StartUpdatesAsync();
        }

        if (flag == false)
        {
            await notify.StopUpdatesAsync();
            Console.WriteLine("Stopped Listening");
            flag = true;
        }
        
    }
    
    async void OnDisconnect(object sender, EventArgs e)
    {
        if (device != null)
        {
            await adapter.DisconnectDeviceAsync(device); 
            Console.WriteLine("Disconnected from " + device.Name);
            Console.WriteLine("\n");
            device = null;
        } else Console.WriteLine("No device connected");
        
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