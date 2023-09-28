using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class VideoPage : ContentPage
{
    public VideoPage()
    {
        InitializeComponent();
    }
    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    
    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        Navigation.PushAsync(new NextPage()); //navigate to stat page
    }
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage()); //navigate to main page
    }
}