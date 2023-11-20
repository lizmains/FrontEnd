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
using System.Windows.Input;
using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Impl;
using MbientLab.MetaWear.Impl.Platform;
using MbientLab.MetaWear.Peripheral;
using MbientLab.MetaWear.Peripheral.Led;
using MbientLab.Warble;
using IAccelerometer = MbientLab.MetaWear.Sensor.IAccelerometer;
using netstandard = MbientLab.MetaWear.NetStandard;

//using Color = System.Drawing.Color;

namespace MauiApp2;

public partial class ScanPage : ContentPage
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
    private /*List*/ObservableCollection<Ball> savedDots;
    private object selectDev;
    private ObservableCollection<IDevice> devDisplay;
    private int dataRange;
    private Ball tempNew;
    private IService mmsBat;
    private IService mmsServ2;
    private IBluetoothLeGatt gatt;
    private ILibraryIO io;
    private IMetaWearBoard metawear;
    
    //sim vars
    private IService simServ;
    private ICharacteristic simWrite;
    
    public ScanPage(Ball newBall)
    {
        InitializeComponent();
        adapter.ScanMode = ScanMode.LowLatency;
        ble = CrossBluetoothLE.Current;
        //adapter = CrossBluetoothLE.Current.Adapter; //this being here is causing btpage not to open?????
        deviceList = new List<IDevice>();
        deviceList.Clear();
        savedDots = new /*List*/ObservableCollection<Ball>(); //dont keep this when db made
        devDisplay = new ObservableCollection<IDevice>();
        savedDots.Add(new Ball(null));
        DevsList.ItemsSource = deviceList; //displayList;
        //adapter.ScanTimeout = 60000; //timeout for bluetooth scanning 60 seconds(?)
        dataRange = 0;
        tempNew = newBall;
    }
    
    private void OnConnBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
    
    private void LoadData(object sender, EventArgs e) //updates collectionView to reflect deviceList
    {                       //For UI to update properly, collection bound to view is separate from working list
        devDisplay.Clear(); //called directly by button
        for (int i=0; i<10; i++)
        {
            devDisplay.Add(deviceList[i]);
        }
        DevsList.ItemsSource = devDisplay;
    }

    private void MoreData(object sender, EventArgs e)
    {
        dataRange += 10;
        LoadData(dataRange);
    }
    private void LoadData(int range) //updates collectionView to reflect deviceList
    {                       //For UI to update properly, collection bound to view is separate from working list
        devDisplay.Clear(); //called indirectly by xaml to chnage range
        for (int i=range; i<10+range; i++)
        {
            devDisplay.Add(deviceList[i]);
        }
        DevsList.ItemsSource = devDisplay;
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
                    //stop scan
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
                        services = await device.GetServicesAsync();
                        //charstics = await services[0].GetCharacteristicsAsync();
                        savedDots.Add(new Ball(device)); //adds device to list of saved balls
                        tempNew.dev = device;
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
    
    /// <summary>
    /// Bond with connected device
    /// This is essentially pairing,but with a saved secure key
    /// This is separate for testing, eventually it will be a
    /// part of the connection feature
    /// Meaning every connection will require pairing
    /// </summary>
    async void OnBondEnter(object sender, EventArgs e)
    {
        if (device != null)
        {
            try
            {
                await adapter.BondAsync(device);
            }
            catch (DeviceConnectionException erm)
            {
                DisplayAlert("Alert", "Unable to Bond", "OK");
            }
            finally
            {
                if (device.BondState == DeviceBondState.Bonded)
                {
                    Console.WriteLine("Bonded");
                    BondedState.Text = "Paired";//not working
                } 
                else if (device.BondState == DeviceBondState.NotSupported)
                {
                    Console.WriteLine("Bonding Not Supported");
                }else Console.WriteLine("Bond Failed");
            }
        }
    }


    async void getServices(object sender, EventArgs e)
    {
        if (device != null)//displays all device info from service down to characteristic
        {
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
               Console.WriteLine("RSSI: "+ device.Rssi);
            } else Console.WriteLine("Can't Do it Boss");
            
        } else DisplayAlert("Alert", "Connect to a device", "OK");
    }

    async void WriteToSim(object sender, EventArgs e)
    {
        try
        {
            simServ = await device.GetServiceAsync(new Guid("19536e67-3682-4588-9f3a-5340b6712150"));
            simWrite = await simServ.GetCharacteristicAsync(new Guid("72563044-db33-4692-a45d-c5212eebabfa"));
            string toSend = "Data from Michael!";
            byte[] writeBytes = Encoding.ASCII.GetBytes(toSend);//new byte[2] {5, 5};
            await simWrite.WriteAsync(writeBytes);
            Console.WriteLine("Writing to Sim");
        }
        catch (DeviceConnectionException erm)
        {
            Console.WriteLine("Write failed");
        }
    }

    async void getMMSInfo(object sender, EventArgs e) //service 0 id 0000180f-0000-1000-8000-00805f9b34fb
    {//testing getting data from specific MMS module
        if (device.Id == new Guid("ec5ddd38-6362-c5e0-6cd0-ae865b8d483c"))
        {
            mmsBat = await device.GetServiceAsync(new Guid("0000180f-0000-1000-8000-00805f9b34fb"));
            charstics = await mmsBat.GetCharacteristicsAsync();
            var bytes = await charstics[0].ReadAsync();
            Console.WriteLine("MMS battery: " + bytes.data[0] + "%");
            DevInfo.Text += $"MMS Battery: {bytes.data[0]}%";
            mmsServ2 = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
            charstics = await mmsServ2.GetCharacteristicsAsync();
            for (int j = 0; j < charstics.Count; j++)
            {
                var mmsBytes = await charstics[j].ReadAsync();
                Console.WriteLine("MMS Bytes from Service 2: " + mmsBytes.data[0]);
                
                for (int i = 0; i < mmsBytes.data.Length; i++)
                {
                    Console.WriteLine("Byte " +i+ ": " + mmsBytes.data[i]);
                }
            }
            
            //Console.WriteLine(mmsBytes.resultCode);
            Console.WriteLine("RSSI: "+ device.Rssi);
            //ILed led = (ILed) null;
            //led.Play();

        } else await DisplayAlert("Alert", "Connect to the MMS", "OK");
    }
    
    async void OnDisconnect(object sender, EventArgs e)
    {
        if (device != null)
        {
            await adapter.DisconnectDeviceAsync(device); 
            Console.WriteLine("Disconnected from " + device.Name);
            device = null;
        } else Console.WriteLine("No device connected");
        
    }

    async void OnDevSelect(object Sender, EventArgs e)
    {
        selectDev = DevsList.SelectedItem;
        device = (IDevice) selectDev;
        try
        {
            await adapter.ConnectToDeviceAsync(device);
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
                tempNew.dev = device;
                savedDots.Add(tempNew); //adds device to list of saved balls
            } else Console.WriteLine("Failed to Connect");
        }
    }
    
    //----------THE METAWEAR ZONE-----------

    async void WriteMMS(object sender, EventArgs e)
    {
        //3 for accelerometer, 3 for accelerometer data config, stuff from example, acc_range
        //byte[] data = { 3, 3, (8 << 4)+ (2<<1)+1, 3<<5};               //mms read/write service
        IService writeServ = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
        ICharacteristic writeChar =//mms write characteristic, presumably
            await writeServ.GetCharacteristicAsync(new Guid("326a9001-85cb-9195-d9dd-464cfbbae75a"));
        //byte[] data = { 25, 2, 1, (3 << 4)+1}; //write the mode --> fusion
        //byte[] data = { 3, 2, 1, (3 << 4) }; //just accel uc
        byte[] data = { 3, 4, 1 }; //just accel --> simple way BMi160 accel to be specific
        await writeChar.WriteAsync(data);
        
        //data = new byte[]{ 3, 3, (8 << 4)+(2<<1), 3<<5}; // configure accel write uc
        data = new byte[] { 3, 1, 1 }; //simple accel 
        await writeChar.WriteAsync(data);//send command
        
        /*data = new byte[]{ 19, 3, (8 << 4)+ (2<<2)+1, 14}; //configure gyro write
        await writeChar.WriteAsync(data);
        
        data = new byte[]{ 21, 4, 4, 14}; //configure magneto write - data repetitions
        await writeChar.WriteAsync(data);
        
        data = new byte[]{ 21, 3, 6}; //further configure magneto write - data rate
        await writeChar.WriteAsync(data);*/
        //start the stuff
        data = new byte[]{ 3, 2, 1, 0}; //accel enable write
        await writeChar.WriteAsync(data);
        
        /*data = new byte[]{ 19, 2, 1, 0}; //gyro enable write
        await writeChar.WriteAsync(data);
        
        data = new byte[]{ 21, 2, 1, 0}; //magneto enable write
        await writeChar.WriteAsync(data);*/
        
        /*data = new byte[]{ 3, 1, 1}; //start accel write uc
        await writeChar.WriteAsync(data);*/ 
        
        /*data = new byte[]{ 19, 1, 1}; //start gyro write
        await writeChar.WriteAsync(data);
        
        data = new byte[]{ 21, 1, 1}; //start magneto write
        await writeChar.WriteAsync(data);*/
        
        //enable fusion
        //byte mask = 1 << 8;
        /*data = new byte[] {25, 3, 1<<4, 0 }; //configure fusion for euler angles
        await writeChar.WriteAsync(data);
        
        data = new byte[] {25, 1, 1 }; //enable
        await writeChar.WriteAsync(data);
        
        data = new byte[] {25, 8, 1 }; //start
        await writeChar.WriteAsync(data);*/

        await Task.Delay(15000); //wait 15 seconds

        data = new byte[] { 3, 2, 0, 1 }; //disable accel simple
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 4, 0}; //disable accel simple
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 1, 0 }; //disable accel simple
        await writeChar.WriteAsync(data);
        
        Console.WriteLine("Accelerometer Recording Complete");
    }
    
    async void MetaMotionStuff(object sender, EventArgs e)
    {
        try
        {
            metawear = netstandard.Application.GetMetaWearBoard("C4:0E:7F:6E:59:3A"); //hard coded for an mms
            await metawear.InitializeAsync();
            Console.WriteLine("Board Initialized;");
        }
        catch(Exception erm)
        {
            Console.WriteLine("Error: " + erm.Message);
        }
        
    }
    
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        Shell.Current.GoToAsync(nameof(MainPage));
    }
    
    async void OnBack(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

}