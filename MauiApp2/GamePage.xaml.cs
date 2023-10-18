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
    
    private void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new NewGame());
    }
}