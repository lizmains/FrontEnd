using MauiApp2.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace MauiApp2;

public partial class MainPage : ContentPage
{
    public MainPage(string usrname, MainViewModel mvm)
    {
        InitializeComponent();
        BindingContext = mvm;
        IBluetoothLE ble = CrossBluetoothLE.Current;
        BluetoothState blue = ble.State;
        BlueStat.Text = $"Bluetooth: {blue}";
        ble.StateChanged += (s, e) =>
        {
            BluetoothState blue = ble.State;
            BlueStat.Text = $"Bluetooth: {blue}";
        };
    }
    Boolean bt;
    
    private void OnBTClicked(object sender, EventArgs e) //bluetooth connection button
    {
        bTBtn.Text = $"Pairing...";
        //steps for actual pairing will go here
        // Navigation.PushAsync(new btPage());
        Shell.Current.GoToAsync(nameof(btPage));
        bt = true;

        SemanticScreenReader.Announce(bTBtn.Text);
    }
    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        if (bt/*shell.GetBluetooth()*/) //check to see if bt connection is established
        {
            Navigation.PushAsync(new NextPage()); //if yes, navigate to stat page
        }
        else //else tell user to connect to bt
        {
            DisplayAlert("Alert", "Please Connect to SmartDot", "OK");
        }

    }
    private void OnSimBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new SimPage());
        //Shell.Current.GoToAsync("//SimPage");
    }

    private void OnFileBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new FilePage());
    }

    private void OnVidBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // Navigation.PushAsync(new VideoPage());
        Shell.Current.GoToAsync(nameof(VideoPage));
    }
}