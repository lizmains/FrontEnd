using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private ICharacteristic readChar;
    private (byte[] data, int resultCode) sensorData;
    private int sens; //temporary for differentiating sensor used, delete once sensor fusion is figured out
    private int light_flag;
    private IService writeServ;
    private ICharacteristic writeChar;
    
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
        IService writeServ = null;
        ICharacteristic writeChar = null;
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
        if (true/*device.Id == new Guid("ec5ddd38-6362-c5e0-6cd0-ae865b8d483c")*/)
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
               SensorInfo.Text += "Accelerometer Data: X: " + x + ", Y: " + y + ", Z: " + z + "\n"; 
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
                SensorInfo.Text += "Gyroscope Data: X: " + x + ", Y: " + y + ", Z: " + z + "\n"; 
            }

            //Console.WriteLine(mmsBytes.resultCode);
            //Console.WriteLine("RSSI: "+ device.Rssi);

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
        
        //start the stuff
        data = new byte[] { 3, 1, 1 }; //accel starting
        await writeChar.WriteAsync(data);//send command
        
        data = new byte[]{ 3, 2, 1, 0}; //accel enable sampling
        await writeChar.WriteAsync(data); //necessary?

        await Task.Delay(15000); //wait 15 seconds

        data = new byte[] { 3, 2, 0, 1 }; //disable accel simple
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 4, 0}; //disable accel simple
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 3, 1, 0 }; //disable accel simple
        await writeChar.WriteAsync(data);

        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);
        
        Console.WriteLine("Accelerometer Recording Complete");
    }

    async void lightSensor(object sender, EventArgs e)
    {
        sens = 2;
        /*if (light_flag == 0)
        {
            light_flag = 1;
        }
        else light_flag = 0;*/
        light_flag = light_flag == 0 ? 1 : 0;

        SensorInfo.Text = ""; //clear display sensor data in app
        ICharacteristic readChar =
            await writeServ.GetCharacteristicAsync(new Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
        byte[] led;
        byte[] data;

        //if (light_flag == 1)
        //{
            Color col = Color.Green; //green for light sensor to differentiate from accel for now
            led = new byte[]//set led pattern
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

            Console.WriteLine("Starting Ambient Light Sensor..."); //light sensor code = 20
            Gain gain = Gain._1x; 
            //int gainMask = (gain == Gain._48x || gain == Gain._96x ? (int) (gain + 2) : (int) gain) << 2;
            IntegrationTime time = IntegrationTime._100ms;
            MeasurementRate mRate = MeasurementRate._500ms;

            //configure
            /*data = new byte[]
            {
                20,
                2,
                (byte)((gain == Gain._48x || gain == Gain._96x ? (int)(gain + 2) : (int)gain) << 2),
                (byte)((MeasurementRate)((int)time << 3) | mRate)
            };*/
            //{ 20, 2, (byte) gainMask, (byte) ((MeasurementRate) ((int) time << 3) | mRate) };
            data = new byte[] { 14, 2, 0x0c, 0x28 }; //gain=8x, Integrationtime=250ms, MeasurementRate=50ms
            await writeChar.WriteAsync(data);

            //start
            //data = (20, 3, dataattribute(byte{4}, 1, 0, false) not the correct byte array but potentially useful info from metawear
            data = new byte[] { 14, 1, 1 }; // Sensor #, 1, 1 starts the accelerometer and led so maybe this too?
            await writeChar.WriteAsync(data);
            
            await Task.Delay(15000); //wait 15 seconds

            data = new byte[] { 14, 1, 0 };
            await writeChar.WriteAsync(data);
            
            led = new byte[] { 2, 2, 0 }; //stop led
            await writeChar.WriteAsync(led);
            Console.WriteLine("Light Sensor Recording Complete");
            /*readChar.ValueUpdated += (s, a) =>
            {
                Console.WriteLine("DATA RECEIVED!");
                var bytes = a.Characteristic.Value;
                for (int i = 2; i < bytes.Length; i++)
                {
                    Console.WriteLine("Light Bytes: " + bytes[i]);//need to correctly parse bytes into data
                }
            };
            await readChar.StartUpdatesAsync();*/
            //}

            //await Task.Delay(15000); //wait 15 seconds
            /*else if (light_flag == 0)
            {
                sensorData = await readChar.ReadAsync();
                for (int i = 0; i < sensorData.data.Length; i++)
                {//get rid of this later
                    Console.WriteLine("data? "+sensorData.data[i]);
                }

                await readChar.StopUpdatesAsync();
                data = new byte[] { 20, 1, 0 }; //1=enable register?, 0 = stops the sensor
                await writeChar.WriteAsync(data);

                led = new byte[] { 2, 2, 0 }; //stop led
                await writeChar.WriteAsync(led);
                Console.WriteLine("Light Sensor Recording Complete");
            }*/

    }

    async void gyroSensor(object sender, EventArgs e)
    {
        sens = 3;
        SensorInfo.Text = ""; //clear display sensor data in app
        
        //gyro module # = 19
        
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
        /*byte[] gyroConfig = { 34, 0 };
        gyroConfig[1] &= 248;
        gyroConfig[1] |= (byte) range;
        gyroConfig[0] = (byte)(odr + 6 | (ODR)((int)filter << 4));
        byte[] data = { 19, 3, gyroConfig[0], gyroConfig[1] };*/
        /*byte[] data =
        {
            19,                 //nonfunctional configuration
            3,
            (byte)(((int) odr << 4) + (2 << 2)),
            (byte) ((int)range << 5)
        };
        await writeChar.WriteAsync(data);
        
        //start gyro
        Console.WriteLine("Starting Gyroscope");
        
        data = new byte[] { 19, 1, 1}; 
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 19, 2, 1, 0 }; 
        await writeChar.WriteAsync(data);*/
        
        byte[] data = { 19, 4, 1 }; //just accel --> simple way BMi160 accel to be specific
        await writeChar.WriteAsync(data);

        data = new byte[] { 19, 1, 1 }; //simple accel 
        await writeChar.WriteAsync(data);//send command
        
        //start the stuff
        data = new byte[]{ 19, 2, 1, 0}; //accel enable write
        await writeChar.WriteAsync(data);

        await Task.Delay(15000);
        
        //stop gyro
        data = new byte[] { 19, 2, 0, 1 };
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 19, 4, 0, };
        await writeChar.WriteAsync(data);
        
        data = new byte[] { 19, 1, 0 }; 
        await writeChar.WriteAsync(data);
        Console.WriteLine("Stopping Gyroscope");
        
        led = new byte[] { 2, 2, 0 }; //stop led
        await writeChar.WriteAsync(led);
        Console.WriteLine("Gyroscope Recording Complete");
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