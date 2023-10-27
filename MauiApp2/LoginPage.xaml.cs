using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using Common.POCOs;
using MauiApp2.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace MauiApp2;

public partial class LoginPage : ContentPage
{
    public string usrnm;
    public FeaturedAPI api;
    public LoginPage()
    {
        InitializeComponent();
        api = new FeaturedAPI("https://revmetrixapi.robertwood.dev/api/");
        
    }
    string pass;
    IBluetoothLE ble = CrossBluetoothLE.Current;

    private void OnUsrChanged(object sender, TextChangedEventArgs e)
    {
        string oldUsr = e.OldTextValue;
        string newUsr = e.NewTextValue;  //Events for assigning 
        usrnm = username.Text;
    }
    private void OnPassChanged(object sender, TextChangedEventArgs e)
    {
        string oldPass = e.OldTextValue;
        string newPass = e.NewTextValue;
        pass = password.Text;
    }

    async void OnUsrEnter(object sender, EventArgs e)
    {
        usrnm = ((Entry)sender).Text;
        OnLoginBtnClicked(sender, e);
    }
    async void OnPassEnter(object sender, EventArgs e)
    {
        pass = ((Entry)sender).Text;
        OnLoginBtnClicked(sender, e);
    }

    void MakeFile(string fileName)
    {
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        if (File.Exists(targetFile))
        {
            Console.Write("\nNo need to make file.\n");
        }
        else
        {
            Console.Write("\nCreating new file\n");
            FileStream fs = File.Create(targetFile);
        }
    }

    void WriteFile(string fileName, string text)
    {
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        if (File.Exists(targetFile))
        {
            using FileStream outStream = System.IO.File.OpenWrite(targetFile);
            using StreamWriter streamWriter = new StreamWriter(outStream);
            streamWriter.Write(text);
            Console.WriteLine("\n Wrote to File successfully \n");
        }
        else
        {
            Console.Write("Not Possible Can't do it!\n");
        }
    }

    async void OnLoginBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        if (usrnm != null && pass != null)//placeholder until user db set up
        {
            MakeFile("test.txt");
            WriteFile("test.txt", usrnm);
            BluetoothState state = ble.State;
            MainViewModel mvm = new MainViewModel(usrnm, pass);
            //ApiLogin();
            Console.WriteLine(await api.Login(usrnm, pass)); //commented out until db server active
            //Console.WriteLine("logged in maybe");
            Console.WriteLine((await api.Get<DateTimePoco>("Test/TestTime")).Result.DateTime);
            await Navigation.PushAsync(new MainPage(usrnm, mvm));
        }
        else await DisplayAlert("Alert", "Please Enter Username and Password", "OK");

    }
    
}


