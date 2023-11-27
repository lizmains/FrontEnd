using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Client;
using MauiApp2.ViewModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace MauiApp2;

public partial class LoginPage : ContentPage
{
    private string usrnm;
    private FeaturedAPI api;
    private string pass;

    private string checkUsrnm;
    private string checkPass;
    public LoginPage()
    {
        InitializeComponent();
        //api = new FeaturedAPI("https://revmetrixapi.robertwood.dev/api/");
        api = new FeaturedAPI("https://api.revmetrix.io/api/");
    }

    public LoginPage(FeaturedAPI apiIn)
    {
        api = apiIn;
    }
    
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

    void OnUsrEnter(object sender, EventArgs e)
    {
        usrnm = ((Entry)sender).Text;
        OnLoginBtnClicked(sender, e);
    }
    void OnPassEnter(object sender, EventArgs e)
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
            Console.WriteLine("Inserted Username===" + usrnm);
            Console.WriteLine("Inserted Password===" + pass);
            //MakeFile("test.txt");
            //WriteFile("test.txt", usrnm);
            BluetoothState state = ble.State;
            MainViewModel mvm = new MainViewModel(usrnm, pass);
            //ApiLogin();
            await api.Login(usrnm, pass); //commented out until db server active
            //Console.WriteLine("logged in maybe");
            Console.WriteLine("API CONNECTION==");
            //Console.WriteLine((await api.Get("Test/TestAuthorize")).StatusCode);
            HttpStatusCode authentication = (await api.Get("Test/TestAuthorize")).StatusCode;
            if (authentication == HttpStatusCode.OK)
            {
                Console.WriteLine(authentication);
                await Navigation.PushAsync(new MainPage(usrnm, mvm));
            }
            else
            {
                Console.WriteLine(authentication);
                await DisplayAlert("Alert", "Incorrect Username or Password", "OK");
            }
        }
        else await DisplayAlert("Alert", "Please Enter Username and Password", "OK");
    }
    
    async void OnCreateAccBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateAccountPage(api));
    }
    
}