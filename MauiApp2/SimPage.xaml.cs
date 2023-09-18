using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class SimPage : ContentPage
{
    public SimPage()
    {
        InitializeComponent();
    }
    string ballwt = "N/A";
    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void OnWtChanged(object sender, TextChangedEventArgs e)
    {
        string oldWt = e.OldTextValue;
        string newWt = e.NewTextValue; //Entry for ball weight to be used in sim physics
        ballwt = weight.Text;
        WtDisplay.Text = $"Weight: {ballwt} lbs";
    }

    void OnWtEnter(object sender, EventArgs e)
    {
        ballwt = ((Entry)sender).Text;
        WtDisplay.Text = $"Weight: {ballwt} lbs";
    }

    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        Navigation.PushAsync(new NextPage()); //navigate to stat page
    }
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}

//CounterBtn.Text = $"Clicked {count} time";