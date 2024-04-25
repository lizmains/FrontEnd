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

public partial class BallArsenal : ContentPage
{
    public BallArsenal()
    {
        InitializeComponent();
    }
    
    async private void OnAddBallBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NewBall());
    }

    
    
}