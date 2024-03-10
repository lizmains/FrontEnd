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

public partial class SmartDot : ContentPage
{
    public SmartDot()
    {
        InitializeComponent();
    }
    
    async private void OnAddDotBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NewBall());
    }
    
    private void OnAddSmartDotBtnClicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new AddSmartDotPage()); //navigate to main page
    }
}