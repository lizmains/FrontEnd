using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();
    }
    
    private async void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        await Navigation.PushAsync(new NewGame());
    }
}