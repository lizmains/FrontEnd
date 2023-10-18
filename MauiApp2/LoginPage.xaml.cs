using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace MauiApp2;

public partial class LoginPage : ContentPage
{
    public string usrnm;
    public LoginPage()
    {
        InitializeComponent();
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

    void OnLoginBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        if (usrnm != null && pass != null)//placeholder until user db set up
        {
            MakeFile("test.txt");
            WriteFile("test.txt", usrnm);
            BluetoothState state = ble.State;
            Navigation.PushAsync(new MainPage(usrnm));
        }
        else DisplayAlert("Alert", "Please Enter Username and Password", "OK");

    }
}


