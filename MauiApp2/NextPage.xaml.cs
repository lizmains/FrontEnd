using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class NextPage : ContentPage
{
    public NextPage()
    {
        InitializeComponent();
    }

    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private void OnSimBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new SimPage());
    }
    private void OnVidBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new VideoPage());
    }
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
    }
}