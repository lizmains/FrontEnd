using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class LoginPage : ContentPage
{

    public LoginPage()
    {
        InitializeComponent();
    }
    string usrnm;
    string pass;

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
    }
    void OnPassEnter(object sender, EventArgs e)
    {
        pass = ((Entry)sender).Text;
    }

    private void OnLoginBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        if (usrnm != null && pass != null)//placeholder until user db set up
        {
            Navigation.PushAsync(new MainPage());
        }
        else DisplayAlert("Alert", "Please Enter Username and Password", "OK");

    }
}


