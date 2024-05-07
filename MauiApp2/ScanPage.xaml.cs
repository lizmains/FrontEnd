using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
//using Android;
//using Android.OS.Strictmode;
using CommunityToolkit.Maui.Converters;
using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Impl;
using MbientLab.MetaWear.Impl.Platform;
using MbientLab.MetaWear.Peripheral;
using MbientLab.MetaWear.Peripheral.Led;
using MbientLab.Warble;
using IAccelerometer = MbientLab.MetaWear.Sensor.IAccelerometer;
using netstandard = MbientLab.MetaWear.NetStandard;
using MbientLab.MetaWear.Peripheral.Led;
using MbientLab.MetaWear.Sensor;
using MbientLab.MetaWear.Sensor.AmbientLightLtr329;
using Color = MbientLab.MetaWear.Peripheral.Led.Color;
using Gain = MbientLab.MetaWear.Sensor.AmbientLightLtr329.Gain;
using ODR = MbientLab.MetaWear.Sensor.GyroBmi160.OutputDataRate;
using DRg = MbientLab.MetaWear.Sensor.GyroBmi160.DataRange;
using FM = MbientLab.MetaWear.Sensor.GyroBmi160.FilterMode;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.ApplicationModel;


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
    public string deviceName;
    private IReadOnlyList<ICharacteristic> batteryChars;
    private IReadOnlyList<IDescriptor> batCharDescs;
    private /*List*/ObservableCollection<Ball> savedDots;
    public object selectDev;
    public ObservableCollection<IDevice> devDisplay;
    private int dataRange;
    private Ball tempNew;
    private IService mmsBat;
    private IService mmsServ2;
    private IBluetoothLeGatt gatt;
    private ILibraryIO io;
    private IMetaWearBoard metawear;
    private ICharacteristic readChar;
    private (byte[] data, int resultCode) sensorData;
    private int sens; //temporary for differentiating sensor used, delete once sensor fusion is figured out
    private int light_flag;
    private int accFlag;
    private IService writeServ;
    private ICharacteristic writeChar;
    private int metamotion;
    private FileStream outStream;
    private StreamWriter streamWriter;
    private FileStream gyroStream;
    private StreamWriter gyroWriter;
    private FileStream lightStream;
    private StreamWriter lightWriter;
    private string targetFile;
    private string lightFile;
    private string gyroFile;
    private Stopwatch timer;
    
    //graphing vars
    private graphingVM sensorGraph;
    private List<float> csvData;
    private List<float> xcsv;
    private List<float> ycsv;//could be condensed into 1 list if you want to be fancy about it
    private List<float> zcsv;//and optimize i guess
    private FileStream inStream;
    private FileStream inAccStream;
    private StreamReader reader;
    private StreamReader accReader;
    private FileStream inGyroStream;
    private StreamReader gyroReader;

    
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
        sens = 0;
        light_flag = 0;
        accFlag = 0;
        IService writeServ = null;
        ICharacteristic writeChar = null;
        metamotion = 0;
        targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "accData.csv");//"/Users/michaelhensel/Desktop/sensorData.csv"; //temporary
        lightFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "lightData.csv");//"/Users/michaelhensel/Desktop/lightData.csv"; //temporary
        gyroFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "gyroData.csv");//"/Users/michaelhensel/Desktop/gyroData.csv"; //temporary
        timer = new Stopwatch();
        
        sensorGraph = new graphingVM();
        csvData = new List<float>();
        xcsv = new List<float>();
        ycsv = new List<float>();
        zcsv = new List<float>();
    }
    
    private void OnConnBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
    
    private void LoadData(object sender, EventArgs e) //updates collectionView to reflect deviceList
    {                       //For UI to update properly, collection bound to view is separate from working list
        devDisplay.Clear(); //called directly by button
        int j;
        j = deviceList.Count >= 10 ? 10 : deviceList.Count;
        for (int i=0; i<j; i++)
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
        #if ANDROID
                var enable = new Android.Content.Intent(Android.Bluetooth.BluetoothAdapter.ActionRequestEnable);
                enable.SetFlags(Android.Content.ActivityFlags.NewTask);

                var disable = new Android.Content.Intent(Android.Bluetooth.BluetoothAdapter.ActionRequestDiscoverable);
                disable.SetFlags(Android.Content.ActivityFlags.NewTask);

                var bluetoothManager = (Android.Bluetooth.BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);
                var bluetoothAdapter = bluetoothManager.Adapter;

                if (bluetoothAdapter.IsEnabled)
                {
                    Android.App.Application.Context.StartActivity(disable);
                    // Disable the Bluetooth;
                }
                else
                {
                    // Enable the Bluetooth
                    Android.App.Application.Context.StartActivity(enable);
                }

                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Granted)
                {
                    DisplayAlert("Alert", "Good", "OK"); 
                }
        
        #endif
        try
        {
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
                    await DisplayAlert("Alert", "Unable to connect to device", "OK");
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
                        if (device.Name == "MetaWear")
                        { //Getting characteristics for mms interactions
                            writeServ = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
                            writeChar =//mms write characteristic, presumably
                                await writeServ.GetCharacteristicAsync(new Guid("326a9001-85cb-9195-d9dd-464cfbbae75a"));
                        }
                        
                        await adapter.StopScanningForDevicesAsync();
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
    
    async void ConnectMMS(object sender, EventArgs e)
    {
        try
        {
            await adapter.ConnectToKnownDeviceAsync(new Guid("ec5ddd38-6362-c5e0-6cd0-ae865b8d483c"));
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
                Console.WriteLine("Connected to MMS");
                metamotion = 0;
                ConDev.Text = "Connected: " + device.Name;
                services = await device.GetServicesAsync();
                //charstics = await services[0].GetCharacteristicsAsync();
                savedDots.Add(new Ball(device)); //adds device to list of saved balls
                tempNew.dev = device;
                if (device.Name == "MetaWear")
                { //Getting characteristics for mms interactions
                    writeServ = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
                    writeChar =//mms write characteristic, presumably
                        await writeServ.GetCharacteristicAsync(new Guid("326a9001-85cb-9195-d9dd-464cfbbae75a"));
                }
                        
                //await adapter.StopScanningForDevicesAsync();
            } else Console.WriteLine("Failed to Connect");
        }
        if (device == null)
        {
            DisplayAlert("Alert", "Unknown Device", "OK");
        }
    }
    
    async void ConnectMMC(object sender, EventArgs e)
    {
        try
        {
            await adapter.ConnectToKnownDeviceAsync(new Guid("e504016e-4f0e-f64b-4a1c-39a7cae943a5"));
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
                Console.WriteLine("Connected to MMC");
                metamotion = 1;
                ConDev.Text = "Connected: " + device.Name;
                services = await device.GetServicesAsync();
                //charstics = await services[0].GetCharacteristicsAsync();
                savedDots.Add(new Ball(device)); //adds device to list of saved balls
                tempNew.dev = device;
                if (device.Name == "MetaWear")
                { //Getting characteristics for mms interactions
                    writeServ = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
                    writeChar =//mms write characteristic, presumably
                        await writeServ.GetCharacteristicAsync(new Guid("326a9001-85cb-9195-d9dd-464cfbbae75a"));
                }
                        
                //await adapter.StopScanningForDevicesAsync();
            } else Console.WriteLine("Failed to Connect");
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
                Console.WriteLine("RSSI: " + device.Rssi);
            }
            else Console.WriteLine("Can't Do it Boss");

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
        if (device.Name == "MetaWear"/*true device.Id == new Guid("ec5ddd38-6362-c5e0-6cd0-ae865b8d483c")*/)
        {
            mmsBat = await device.GetServiceAsync(new Guid("0000180f-0000-1000-8000-00805f9b34fb"));
            charstics = await mmsBat.GetCharacteristicsAsync();
            var bytes = await charstics[0].ReadAsync();
            Console.WriteLine("MMS battery: " + bytes.data[0] + "%");
            DevInfo.Text = $"MMS Battery: {bytes.data[0]}%";
            mmsServ2 = await device.GetServiceAsync(new Guid("326a9000-85cb-9195-d9dd-464cfbbae75a"));
            charstics = await mmsServ2.GetCharacteristicsAsync();
            for (int j = 0; j < charstics.Count; j++)
            {
                var mmsBytes = await charstics[j].ReadAsync();
                Console.WriteLine("MMS Bytes from Service 2: ");
                
                for (int i = 0; i < mmsBytes.data.Length; i++)
                {
                    Console.WriteLine("Byte " +i+ ": " + mmsBytes.data[i]);
                }
            }

            readChar = await mmsServ2.GetCharacteristicAsync(new Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
            sensorData = await readChar.ReadAsync();
            if (sens == 1)
            {
               float x = BitConverter.ToInt16(sensorData.data, 2);
               float y = BitConverter.ToInt16(sensorData.data, 4);
               float z = BitConverter.ToInt16(sensorData.data, 6);
               x = (x/32768.0f)*16.0f;
               y = (y/32768.0f)*16.0f;
               z = (z/32768.0f)*16.0f;
               
               Console.WriteLine("\nX: "+x+"  Y: "+y+"  Z: "+z);
               SensorInfo.Text /*+*/= "Accelerometer Data: X: " + x + ", Y: " + y + ", Z: " + z + "\n"; 
            }
            else if (sens == 2)
            {
                for (int i = 2; i < sensorData.data.Length; i++)
                {
                    Console.WriteLine(sensorData.data[i]);//need to correctly parse bytes into data
                }
                float lux = BitConverter.ToInt16(sensorData.data, 2);
                Console.WriteLine("Light Data: " + lux); 
                SensorInfo.Text += "Light Data: " + lux + " \n";
            }
            
            else if (sens == 3)
            {//gyro may convert just like the accelerometer 
                float x = BitConverter.ToInt16(sensorData.data, 2);
                float y = BitConverter.ToInt16(sensorData.data, 4);
                float z = BitConverter.ToInt16(sensorData.data, 6);
                x = (x/32768.0f)*2.0f;
                y = (y/32768.0f)*2.0f;
                z = (z/32768.0f)*2.0f;
               
                Console.WriteLine("\nX: "+x+"  Y: "+y+"  Z: "+z);
                SensorInfo.Text /*+*/= "Gyroscope Data: X: " + x + ", Y: " + y + ", Z: " + z + "\n"; 
            }

            //Console.WriteLine(mmsBytes.resultCode);
            //Console.WriteLine("RSSI: "+ device.Rssi);

        } 
        else if (device.Name == "raspberrypi")
        {
            mmsServ2 = await device.GetServiceAsync(new Guid("00000001-710e-4a5b-8d75-3e5b444bc3cf"));
            readChar = await mmsServ2.GetCharacteristicAsync(new Guid("00000002-710e-4a5b-8d75-3e5b444bc3cf"));

            sensorData = await readChar.ReadAsync();
            Console.WriteLine("reading Sim");
            for (int i = 0; i < sensorData.data.Length; i++)
            {
                Console.WriteLine("Sim Data: "+ sensorData.data[i]);
            }
            Console.WriteLine("Sim Data Value: "+readChar.StringValue);
            SensorInfo.Text += "Sim Data: "+readChar.StringValue;
        }
        else await DisplayAlert("Alert", "Connect to the MMS", "OK");
    }

    void ConvertMMSData(byte[] bytes, StreamWriter stream, long ms)
    {
        if (sens == 1)
        {
            float x = BitConverter.ToInt16(bytes, 2);
            float y = BitConverter.ToInt16(bytes, 4);
            float z = BitConverter.ToInt16(bytes, 6);
            x = (x/32768.0f)*16.0f;
            y = (y/32768.0f)*16.0f;
            z = (z/32768.0f)*16.0f;
               
            Console.WriteLine("\nX: "+x+"  Y: "+y+"  Z: "+z);
            SensorInfo.Text /*+*/= "Accelerometer Data: X: " + x.ToString("0.00") + ", Y: " + y.ToString("0.00") + ", Z: " + z.ToString("0.00") + "\n"; 
            //WriteFile(x + "," + y + "," + z);
            stream.WriteLine(ms + "," + x + "," + y + "," + z); 
            Console.WriteLine("\n Wrote to File successfully \n");
        }
        else if (sens == 2)
        {
            float lux;
            float ratio;
            float ch0 = (bytes[3] << 8) | bytes[2];
            float ch1 = (bytes[5] << 8) | bytes[4];
            if (ch1 != 0 || ch0 != 0)
            {
                ratio = ch1 / (ch1 + ch0);
            }
            else ratio = 0;
            Console.WriteLine("Ratio: " + ratio);
            if (ratio < 0.45f)
            {
                lux = (1.7743f * ch0 + 1.1059f * ch1) / 48 / 4; // gain/integration time
            }
            else if (ratio is < 0.64f and >= 0.45f)
            {
                lux = (4.2785f * ch0 - 1.9548f * ch1) / 48 / 4;
            }
            else if (ratio is < 0.85f and >= 0.64f)
            {
                lux = (0.5926f * ch0 + 0.1185f * ch1) / 48 / 4;
            }
            else lux = 0;

            LightInfo.Text = "Light Data " + lux.ToString("0.00") + " Lux";
            stream.WriteLine(ms + "," + lux);
            Console.WriteLine("\n Wrote to File successfully \n");
        }
            
        else if (sens == 3)
        { 
            float x = BitConverter.ToInt16(bytes, 2);
            float y = BitConverter.ToInt16(bytes, 4);
            float z = BitConverter.ToInt16(bytes, 6);
            x = (x/32768.0f)*2.0f;
            y = (y/32768.0f)*2.0f;
            z = (z/32768.0f)*2.0f;
               
            Console.WriteLine("\nX: "+x+"  Y: "+y+"  Z: "+z);
            stream.WriteLine(ms + "," + x + "," + y + "," + z);
            GyroInfo.Text /*+*/= "Gyroscope Data: X: " + x.ToString("0.00") + ", Y: " + y.ToString("0.00") + ", Z: " + z.ToString("0.00") + "\n"; 
            Console.WriteLine("\n Wrote to File successfully \n");
        }
    }
    
    async void OnDisconnect(object sender, EventArgs e)
    {
        if (device != null)
        {
            await adapter.DisconnectDeviceAsync(device); 
            Console.WriteLine("Disconnected from " + device.Name);
            device = null;
            ConDev.Text = "";
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
        sens = 1;
        SensorInfo.Text = ""; //clear display sensor data in app
        
        //3 for accelerometer, 3 for accelerometer data config, stuff from example, acc_range
        //byte[] data = { 3, 3, (8 << 4)+ (2<<1)+1, 3<<5};               //mms read/write service

        Color col = Color.Blue;
        byte[] led = //set led pattern --> solid
        {
            2, 3, (byte) col, 2, 1, 1,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            0, 0, 0xff
        };
        await writeChar.WriteAsync(led);

        led = new byte[] { 2, 1, 1 }; //play led
        await writeChar.WriteAsync(led);
        
        Console.WriteLine("Starting Accelerometer");
        
        byte[] data = { 3, 4, 1 }; //subscribes to accel reading
        await writeChar.WriteAsync(data); 

        data = new byte[] { 0x03, 0x03, 0x28, 0x03 }; //configures bmi160 to 100Hz
        await writeChar.WriteAsync(data);             //bmi270 version dont seem to work right
        
        data = new byte[] { 0x03, 0x03, 0x28, 0x0c }; //configures bmi160 to 16 range
        await writeChar.WriteAsync(data);             
        
        data = new byte[]{ 3, 2, 1, 0}; //accel enable sampling
        await writeChar.WriteAsync(data); //necessary?
        
        //start the stuff
        data = new byte[] { 3, 1, 1 }; //accel starting
        await writeChar.WriteAsync(data);//send command

        await Task.Delay(15000); //wait 15 seconds

        data = new byte[] { 3, 2, 0, 1 }; //disable accel simple
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 4, 0}; //unsubscribe
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 1, 0 }; //disable accel simple
        await writeChar.WriteAsync(data);
        
        /*data = new byte[] { 0x0b, 0x10, 0x01 }; //flush cache, not working
        await writeChar.WriteAsync(data);*/

        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);
        
        Console.WriteLine("Accelerometer Recording Complete");
    }

    async void lightSensor(object sender, EventArgs e)
    {
        if (device != null)
        {


            light_flag = light_flag == 0 ? 1 : 0;

            SensorInfo.Text = ""; //clear display sensor data in app
            ICharacteristic readChar =
                await writeServ.GetCharacteristicAsync(new Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
            byte[] led;
            byte[] data;

            switch (light_flag)
            {
                case 1:
                {
                    Color col = Color.Green; //green for light sensor to differentiate from accel for now
                    led = new byte[] //set led pattern
                    {
                        2, 3, (byte)col, 2, 1, 1,
                        1, 1 >> 8,
                        1, 1 >> 8,
                        1, 1 >> 8,
                        1, 1 >> 8,
                        0, 0, 0xff
                    };
                    await writeChar.WriteAsync(led);

                    led = new byte[] { 2, 1, 1 }; //play led
                    await writeChar.WriteAsync(led);

                    Console.WriteLine("Starting Everything..."); //light sensor code = 20 / 0x14

                    MakeFiles(); //create new csv to write sensor data into

                    Console.WriteLine("Stopwatch good");

                    data = new byte[] { 0x14, 0x02, 0x18, 0x03 };
                    await writeChar.WriteAsync(data); //gain 48x, lux values should be between 0.02 and 1.3k

                    data = new byte[] { 0x14, 0x02, 0x00, 0x1b }; //Integration time 400ms
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 0x14, 0x02, 0x00, 0x00 }; //Measurement 0 for 50ms, 3 for 500ms
                    await writeChar.WriteAsync(data);

                    //start
                    data = new byte[] { 0x14, 0x03, 0x01 }; // 3 for streaming?
                    await writeChar.WriteAsync(data); //subscribes to notifications?

                    data = new byte[] { 0x14, 0x01, 0x01 }; // start
                    await writeChar.WriteAsync(data);
                    //-------------------------
                    //---------------------accelerometer section--------
                    data = new byte[] { 3, 4, 1 }; //subscribes to accel reading
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 0x03, 0x03, 0x29, 0x03 }; //configures bmi160 to 200Hz
                    await writeChar.WriteAsync(data); //bmi270 version dont seem to work right

                    data = new byte[] { 0x03, 0x03, 0x28, 0x0c }; //configures bmi160 to 16 range
                    await writeChar.WriteAsync(data);
                    //-----------------------
                    //start the stuff
                    data = new byte[] { 3, 1, 1 }; //accel starting
                    await writeChar.WriteAsync(data); //send command

                    data = new byte[] { 3, 2, 1, 0 }; //accel enable sampling
                    await writeChar.WriteAsync(data);
                    //---------------------------gyro section-------
                    data = new byte[] { 0x13, 0x05, 0x01 }; //subscribe to gyro
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 0x13, 0x3, 0x29, 0x0 }; //odr 200Hz
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 0x13, 0x03, 0x28, 0x03 }; //fsr 250dps
                    await writeChar.WriteAsync(data); //send command

                    data = new byte[] { 0x13, 0x01, 0x01 }; //start gyro
                    await writeChar.WriteAsync(data); //send command

                    //start the stuff
                    data = new byte[] { 0x13, 0x02, 0x01, 0x00 }; //enable sampling
                    await writeChar.WriteAsync(data);

                    timer.Start();
                    //----------------------------
                    /*await Task.Delay(15000); //wait 15 seconds

                    data = new byte[] { 0x14, 0x03, 0x00 };
                    await writeChar.WriteAsync(data);

                    led = new byte[] { 2, 2, 0 }; //stop led
                    await writeChar.WriteAsync(led);
                    Console.WriteLine("Light Sensor Recording Complete");*/

                    if (File.Exists(targetFile) && File.Exists(lightFile) && File.Exists(gyroFile))
                    {
                        outStream = System.IO.File.OpenWrite(targetFile);
                        streamWriter = new StreamWriter(outStream);

                        lightStream = System.IO.File.OpenWrite(lightFile);
                        lightWriter = new StreamWriter(lightStream);

                        gyroStream = System.IO.File.OpenWrite(gyroFile);
                        gyroWriter = new StreamWriter(gyroStream);


                        streamWriter.WriteLine("Milliseconds,X,Y,Z");
                        lightWriter.WriteLine(
                            "Milliseconds,Lux"); //async overload warning generated, seems fine for now
                        gyroWriter.WriteLine("Milliseconds,X,Y,Z");

                        readChar.ValueUpdated += (s, a) =>
                        {
                            Console.WriteLine("DATA RECEIVED!");
                            var bytes = a.Characteristic.Value;
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                Console.WriteLine("Sensor Bytes: " + bytes[i]);
                            }

                            switch (bytes[0])
                            {
                                case 20:
                                    sens = 2;
                                    ConvertMMSData(bytes, lightWriter, timer.ElapsedMilliseconds);
                                    break;
                                case 3:
                                    sens = 1;
                                    ConvertMMSData(bytes, streamWriter, timer.ElapsedMilliseconds);
                                    break;
                                case 19:
                                    sens = 3;
                                    ConvertMMSData(bytes, gyroWriter, timer.ElapsedMilliseconds);
                                    break;
                                default:
                                    Console.WriteLine("Invalid Sensor Use!");
                                    break;
                            }
                        };
                        await readChar.StartUpdatesAsync();
                    }
                    else
                    {
                        Console.Write("File write failed\n");
                    }

                    break;
                }
                //await Task.Delay(15000); //wait 15 seconds
                case 0:
                    await readChar.StopUpdatesAsync();
                    timer.Stop();
                    timer.Reset();
                    data = new byte[] { 0x14, 0x03, 0x00 }; //stop als stream
                    await writeChar.WriteAsync(data);
                    data = new byte[] { 0x14, 0x01, 0x00 }; //stop
                    await writeChar.WriteAsync(data);
                    //------   
                    data = new byte[] { 3, 2, 0, 1 }; //disable accel simple
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 3, 4, 0 }; //unsubscribe
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 3, 1, 0 }; //disable accel simple
                    await writeChar.WriteAsync(data);
                    //------------   
                    data = new byte[] { 0x13, 0x02, 0x00, 0x01 }; //disable sampling
                    await writeChar.WriteAsync(data);

                    /*data = new byte[] { 19, 4, 0, };
                    await writeChar.WriteAsync(data);*/
                    data = new byte[] { 0x13, 0x05, 0x00 }; //unsubscribe gyro
                    await writeChar.WriteAsync(data);

                    data = new byte[] { 0x13, 0x01, 0x00 }; //stop gyro
                    await writeChar.WriteAsync(data);
                    Console.WriteLine("Stopping Everything");
                    //------------------

                    led = new byte[] { 2, 2, 0 }; //stop led
                    await writeChar.WriteAsync(led);
                    Console.WriteLine("Sensor Recording Complete");
                    streamWriter.Close();
                    lightWriter.Close();
                    gyroWriter.Close();
                    ToGraphs(sender, e);
                    break;
            }
        } else await DisplayAlert("Alert", "Connect to the Device", "OK");

    }

    async void gyroSensor(object sender, EventArgs e)
    {
        sens = 3;
        SensorInfo.Text = ""; //clear display sensor data in app
        
        //gyro module # = 19 / 0x13
        
        Color col = Color.Red;
        byte[] led = //set led pattern --> solid
        {
            2, 3, (byte) col, 2, 1, 1,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            0, 0, 0xff
        };
        await writeChar.WriteAsync(led);

        led = new byte[] { 2, 1, 1 }; //play led
        await writeChar.WriteAsync(led);
        
        //configure gyro
        Console.WriteLine("Configuring Gyroscope");
        ODR odr = ODR._100Hz;
        DRg range = DRg._2000dps;
        FM filter = FM.Normal;
        
        byte[] data = { 0x13, 0x05, 0x01}; //subscribe to gyro
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x3, 0x29, 0x0 }; //odr 200Hz
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x03, 0x28, 0x03 }; //fsr 250dps
        await writeChar.WriteAsync(data);//send command

        data = new byte[] { 0x13, 0x01, 0x01 }; //start gyro
        await writeChar.WriteAsync(data);//send command
        
        //start the stuff
        data = new byte[]{ 0x13, 0x02, 0x01, 0x00}; //enable sampling
        await writeChar.WriteAsync(data);

        await Task.Delay(15000);
        
        //stop gyro
        data = new byte[] { 0x13, 0x02, 0x00, 0x01 }; //disable sampling
        await writeChar.WriteAsync(data);
        
        /*data = new byte[] { 19, 4, 0, };
        await writeChar.WriteAsync(data);*/
        data = new byte[] { 0x13, 0x05, 0x00 }; //unsubscribe gyro
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x01, 0x00 }; //stop gyro
        await writeChar.WriteAsync(data);
        Console.WriteLine("Stopping Gyroscope");
        
        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);
        Console.WriteLine("Gyroscope Recording Complete");
    }

    async void StreamAcc(object sender, EventArgs e)
    {
        sens = 1;
        accFlag = accFlag == 0 ? 1 : 0;
        
        SensorInfo.Text = ""; //clear display sensor data in app
        ICharacteristic readChar =
            await writeServ.GetCharacteristicAsync(new Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
        byte[] led;
        byte[] data;
        
        Color col = Color.Blue;
        led = new byte[]//set led pattern --> solid
        {
            2, 3, (byte) col, 2, 1, 1,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            0, 0, 0xff
        };
        await writeChar.WriteAsync(led);
  
        led = new byte[] { 2, 1, 1 }; //play led
        await writeChar.WriteAsync(led);
          
        Console.WriteLine("Starting Accelerometer");
          
        data = new byte[]{ 3, 4, 1 }; //subscribes to accel 
        await writeChar.WriteAsync(data); 
  
        data = new byte[] { 0x03, 0x03, 0x28, 0x03 }; //configures bmi160 to 100Hz
        await writeChar.WriteAsync(data);             //bmi270 version dont seem to work right
          
        data = new byte[] { 0x03, 0x03, 0x28, 0x0c }; //configures bmi160 to 16 range
        await writeChar.WriteAsync(data);             
          
        data = new byte[]{ 3, 2, 1, 0}; //accel enable sampling
        await writeChar.WriteAsync(data); //necessary?
        //start the stuff
        data = new byte[] { 3, 1, 1 }; //accel starting stream
        await writeChar.WriteAsync(data);//send command

        Stopwatch s = new Stopwatch();
        s.Start();
        while (s.Elapsed < TimeSpan.FromSeconds(15))
        {
            getMMSInfo(sender, e); //auto-poll the MMS the lazy way
            await Task.Delay(100); //polling too much causes a crash
        }
        s.Stop();
        
        data = new byte[] { 3, 2, 0, 1 }; //disable accel simple
        await writeChar.WriteAsync(data);

        data = new byte[] { 3, 4, 0}; //unsubscribe
        await writeChar.WriteAsync(data);

        data = new byte[] { 3, 1, 0 }; //disable accel simple
        await writeChar.WriteAsync(data);

        /*data = new byte[] { 0x0b, 0x10, 0x01 }; //flush cache, not working
        await writeChar.WriteAsync(data);*/

        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);

        Console.WriteLine("Accelerometer Recording Complete");
    }

    async void StreamGyro(object sender, EventArgs e)
    {
        sens = 3;
        
        SensorInfo.Text = ""; //clear display sensor data in app
        ICharacteristic readChar =
            await writeServ.GetCharacteristicAsync(new Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
        byte[] led;
        byte[] data;
        
        Color col = Color.Red;
        led = new byte[]//set led pattern --> solid
        {
            2, 3, (byte) col, 2, 1, 1,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            1, 1 >> 8,
            0, 0, 0xff
        };
        await writeChar.WriteAsync(led);
  
        led = new byte[] { 2, 1, 1 }; //play led
        await writeChar.WriteAsync(led);
          
        Console.WriteLine("Starting Gyroscope");
          
        data = new byte[]{ 0x13, 0x05, 0x01}; //subscribe to gyro
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x3, 0x29, 0x0 }; //odr 200Hz
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x03, 0x28, 0x03 }; //fsr 250dps
        await writeChar.WriteAsync(data);//send command

        data = new byte[] { 0x13, 0x01, 0x01 }; //start gyro
        await writeChar.WriteAsync(data);//send command
        
        //start the stuff
        data = new byte[]{ 0x13, 0x02, 0x01, 0x00}; //enable sampling
        await writeChar.WriteAsync(data);

        Stopwatch s = new Stopwatch();
        s.Start();
        while (s.Elapsed < TimeSpan.FromSeconds(15))
        {
            getMMSInfo(sender, e); //auto-poll the MMS the lazy way
            await Task.Delay(100);
        }
        s.Stop();
        
        //stop gyro
        data = new byte[] { 0x13, 0x02, 0x00, 0x01 }; //disable sampling
        await writeChar.WriteAsync(data);
        
        /*data = new byte[] { 19, 4, 0, };
        await writeChar.WriteAsync(data);*/
        data = new byte[] { 0x13, 0x05, 0x00 }; //unsubscribe gyro
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 0x13, 0x01, 0x00 }; //stop gyro
        await writeChar.WriteAsync(data);
        Console.WriteLine("Stopping Gyroscope");
        
        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);
        Console.WriteLine("Gyroscope Recording Complete");
    }
    
    void MakeFiles()
    {
        //string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "sensorData.csv");
        targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "accData.csv");//"/Users/michaelhensel/Desktop/graphs/accData"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv";
        lightFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "lightData.csv");//"/Users/michaelhensel/Desktop/graphs/lightData"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv";
        gyroFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "gyroData.csv");//"/Users/michaelhensel/Desktop/graphs/gyroData"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+".csv";
        
        if (File.Exists(targetFile))
        {
            File.Delete(targetFile);
            //Console.Write("\nNo need to make acc file.\n");
            Console.WriteLine("Killing Old file"); //have to do this if creating file in app directory maybe
            Console.Write("\nCreating new file...\n");
            FileStream fs1 = File.Create(targetFile);
            Console.WriteLine("New acc file created");
            fs1.Close();
        }
        else
        {
            Console.Write("\nCreating new file...\n");
            FileStream fs1 = File.Create(targetFile);
            Console.WriteLine("New acc file created");
            fs1.Close(); //there's a better way to do this, but I don't get paid for this
        }
        
        if (File.Exists(lightFile))
        {
            //Console.Write("\nNo need to make light file.\n");
            Console.WriteLine("Killing Old light file");
            File.Delete(lightFile);
            Console.Write("\nCreating new file...\n");
            FileStream fs2 = File.Create(lightFile);
            Console.WriteLine("New light file created");
            fs2.Close();
        }
        else
        {
            Console.Write("\nCreating new file...\n");
            FileStream fs2 = File.Create(lightFile);
            Console.WriteLine("New light file created");
            fs2.Close();
        }
        
        if (File.Exists(gyroFile))
        {
            //Console.Write("\nNo need to make gyro file.\n");
            Console.WriteLine("Killing Old light file");
            File.Delete(gyroFile);
            Console.Write("\nCreating new file...\n");
            FileStream fs3 = File.Create(gyroFile);
            Console.WriteLine("New gyro file created");
            fs3.Close();
        }
        else
        {
            Console.Write("\nCreating new file...\n");
            FileStream fs3 = File.Create(gyroFile);
            Console.WriteLine("New gyro file created");
            fs3.Close();
        }
        
    }
    
    /*private void readLight(object sender, EventArgs e)
    {
        inStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "lightData.csv"));
        reader = new StreamReader(inStream);
        string line;
        csvData.Clear();//empties out the csvdata list once data has been uploaded to graph

        Console.WriteLine("Line: " + reader.ReadLine());
        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                csvData.Add(float.Parse(line.Split(',')[1]));
            } else Console.WriteLine("uh");
            
        }
        reader.Close();
        Console.WriteLine("Data Changed");
        DataGraph.Series = sensorGraph.PopulateSeries(csvData); //graphiel.OnNewDataDX(sender, e);
    }
    
    private void readAcc(object sender, EventArgs e)
    {
        inAccStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "accData.csv"));
        accReader = new StreamReader(inAccStream);
        string line;
        xcsv.Clear();//empties out the csvdata list once data has been uploaded to graph
        ycsv.Clear();
        zcsv.Clear();
        Console.WriteLine("Line: " + accReader.ReadLine());
        while (!accReader.EndOfStream)
        {
            line = accReader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                xcsv.Add(float.Parse(line.Split(',')[1]));
                ycsv.Add(float.Parse(line.Split(',')[2]));
                zcsv.Add(float.Parse(line.Split(',')[3]));
            } else Console.WriteLine("uh");
            
        }
        accReader.Close();
        Console.WriteLine("Data Changed");
        DataGraph.Series = sensorGraph.PopulateAccSeries(xcsv, ycsv, zcsv); //graphiel.OnNewDataDX(sender, e);
    }
    
    private void readGyro(object sender, EventArgs e)
    {
        inGyroStream = File.OpenRead(System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "gyroData.csv"));
        gyroReader = new StreamReader(inGyroStream);
        string line;
        xcsv.Clear();//empties out the csvdata list once data has been uploaded to graph
        ycsv.Clear();
        zcsv.Clear();
        Console.WriteLine("Line: " + gyroReader.ReadLine());
        while (!gyroReader.EndOfStream)
        {
            line = gyroReader.ReadLine();
            Console.WriteLine("Line: " + line);
            if (line != null)
            {
                xcsv.Add(float.Parse(line.Split(',')[1]));
                ycsv.Add(float.Parse(line.Split(',')[2]));
                zcsv.Add(float.Parse(line.Split(',')[3]));
            } else Console.WriteLine("uh");
            
        }
        gyroReader.Close();
        Console.WriteLine("Data Changed");
        DataGraph.Series = sensorGraph.PopulateAccSeries(xcsv, ycsv, zcsv); //graphiel.OnNewDataDX(sender, e);
    }*/

    async void ToGraphs(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new graphtest());
    }
    
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        
    }
    
    async void OnBack(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
    
    
    /*public ISeries[] PopulateSeries(List<float> list)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = list,
                Name = "Lux",
                Fill = null
            }
        };
    }
    
    public ISeries[] PopulateAccSeries(List<float> xlist, List<float> ylist, List<float> zlist)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = xlist,
                Name = "X",
                Fill = null
            },
            new LineSeries<float>
            {
                Values = ylist,
                Name = "Y",
                Fill = null
            },
            new LineSeries<float>
            {
                Values = zlist,
                Name = "Z",
                Fill = null
            }
            
        };
    }*/

}

public partial class graphingVM
{
    public ISeries[] Series { get; set; }
        = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = new float[] {1, 3, 2},
                Name = "X",
                Fill = null
            }
        };

    public ISeries[] PopulateSeries(List<float> list)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = list,
                Name = "Lux",
                Fill = null
            }
        };
    }
    
    public ISeries[] PopulateAccSeries(List<float> xlist, List<float> ylist, List<float> zlist)
    {
        Console.WriteLine("Data Loading");
        return Series = new ISeries[]
        {
            new LineSeries<float>
            {
                Values = xlist,
                Name = "X",
                Fill = null
            },
            new LineSeries<float>
            {
                Values = ylist,
                Name = "Y",
                Fill = null
            },
            new LineSeries<float>
            {
                Values = zlist,
                Name = "Z",
                Fill = null
            }
            
        };
    }
}