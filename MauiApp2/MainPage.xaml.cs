using System.Text.Json;
using Client;
using MauiApp2.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

namespace MauiApp2;

public partial class MainPage : ContentPage
{
    string user = "N/A";
    public MainPage()
    {
        //Client.APIConnection;
        InitializeComponent();
        
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
    public MainPage(String usernm, MainViewModel mvm)
    {
        InitializeComponent();
        IBluetoothLE ble = CrossBluetoothLE.Current;
        BluetoothState blue = ble.State;
        BlueStat.Text = $"Bluetooth: {blue}";
        ble.StateChanged += (s, e) =>
        {
            BluetoothState blue = ble.State;
            BlueStat.Text = $"Bluetooth: {blue}";
        };
        user = usernm;
        App.UserRepository.GetAllUsers();
        //We need an option where if user created account, it creates ballList, all new everything
        //Otherwise, we pull from api infrastructure
        
        /*
         * If User is creating a new account
         */
        
        //For the time being, just creating two new bowling balls for every account
        Ball ball1 = new Ball(null)
        {
            color = "green", comments = "n/a",core = "",cover = "", name = "ball1-1",weight = 10,serial = Guid.NewGuid()
        };
        
        Ball ball2 = new Ball(null)
        {
            color = "red", comments = "", core = "", cover = "", name = "ball 2-1", weight = 123, serial = Guid.NewGuid()
        };
        
        var bowlingBallList = new List<Ball> { ball1, ball2 };
        string bowlingBallListString = JsonSerializer.Serialize(bowlingBallList);
        
        App.UserRepository.Add(new User
        {
            LastLogin = DateTime.Now,
            Password = mvm.Password,
            UserName = mvm.Username,
            BallList = bowlingBallListString
        });
        
        for (int i = /*0*/App.UserRepository.GetAllUsers().Count-1; i < App.UserRepository.GetAllUsers().Count; i++)
        {
            Console.WriteLine(App.UserRepository.GetAllUsers()[i].UserName+" - "+ App.UserRepository.GetAllUsers()[i].LastLogin);
            // App.UserRepository.Delete(App.UserRepository.GetAllUsers()[i].UserId);
        }
        UsrDisplay.Text = $"Hello {user}!";
    }
    
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
    
    private void OnGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new GamePage());
    }
}