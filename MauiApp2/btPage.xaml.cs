using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Input;
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
    private /*List*/ObservableCollection<Ball> savedDots;
    private object selectBall;
    private Ball theBall;
    private Ball newBall;
    private Ball addedBall;
    private bool refsh;//for refreshview, not used currently
    private ObservableCollection<Ball> displayList;

    public btPage()
    {
        InitializeComponent();
        adapter.ScanMode = ScanMode.LowLatency;
        ble = CrossBluetoothLE.Current;
        //adapter = CrossBluetoothLE.Current.Adapter; //this being here is causing btpage not to open?????
        deviceList = new List<IDevice>();
        deviceList.Clear();
        savedDots = new /*List*/ObservableCollection<Ball>(); //dont keep this when db made
        displayList = new ObservableCollection<Ball>();
        savedDots.Add(new Ball(null));
        newBall = new Ball(null);
        addedBall = new Ball(null);
        DotsList.ItemsSource = displayList;//savedDots;
        RefView.Command = new Command(async () => await RefreshItems());
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

    private void LoadData() //updates collectionView to reflect savedDots list
    {                       //For UI to update properly, collection bound to view is separate from working list
        displayList.Clear();
        foreach (Ball ball in savedDots)
        {
            displayList.Add(ball);
        }
        DotsList.ItemsSource = displayList;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (newBall.dev != null)
        {
            addedBall.dev = newBall.dev;
            savedDots.Add(new Ball(addedBall.dev));
            newBall.dev = null;
        }
        LoadData();
    }

    private void OnConnBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
    }

    async Task RefreshItems()
    {
        RefView.IsRefreshing = true;
        await Task.Delay(TimeSpan.FromSeconds(2));
        RefView.IsRefreshing = false;
    }

    async private void OnBallSelect(object sender, EventArgs e)
    {
        selectBall = DotsList.SelectedItem;
        theBall = (Ball) selectBall;
        if (selectBall != null)
        {
            if (theBall.dev != null)
            {
                await DisplayAlert("Ball Specs", theBall.name + "\n" + 
                                                      "Weight: " + theBall.weight + "lbs.\n" + 
                                                      "Color: " + theBall.color + "\n" + 
                                                      "Core: " + theBall.core + "\n" + 
                                                      "CoverStock: " + theBall.cover + "\n" + 
                                                      "ID: " + theBall.dev.Id + "\n", 
                                               "Done");
            }
            else
            {
                await DisplayAlert("Ball Specs", theBall.name + "\n" + 
                                           "Weight: " + theBall.weight + "lbs.\n" + 
                                           "Color: " + theBall.color + "\n" + 
                                           "Core: " + theBall.core + "\n" + 
                                           "CoverStock: " + theBall.cover + "\n" + 
                                           "ID: No Device" + "\n", 
                    "Done");
            }
           
        } else Console.WriteLine("Selection failed");
    }

    async void OnBallEdit(object sender, EventArgs e)
    {
        selectBall = DotsList.SelectedItem;
        theBall = (Ball) selectBall; 
        await Navigation.PushModalAsync(new EditBall(theBall));
        LoadData();
    }

    async void OnBallConnect(object sender, EventArgs e)
    {
        selectBall = DotsList.SelectedItem;
        theBall = (Ball) selectBall;
        try
        {
            Console.WriteLine("Trying to Connect"); //tries connecting to the device with ID devId
            device = await adapter.ConnectToKnownDeviceAsync(theBall.dev.Id);
            //await adapter.ConnectToDeviceAsync(deviceList[0]);
        }
        catch (DeviceConnectionException erm)
        {
            // ... could not connect to device
            await DisplayAlert("Alert", "Unable to connect to device", "OK");
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
    }

    async void OnDisconnect(object sender, EventArgs e)
    {
        if (device != null)
        {
           await adapter.DisconnectDeviceAsync(device); 
           Console.WriteLine("Disconnected from " + device.Name); 
        } else Console.WriteLine("No device connected");
        
    }
    
    async private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        await Shell.Current.GoToAsync("..");
    }
    
    async void OnScanPage(object sender, EventArgs e)
    {
        //newBall = new Ball(null);
        await Navigation.PushModalAsync(new ScanPage(newBall));
        //savedDots.Add(newBall);
        //LoadData();
    }
    
}