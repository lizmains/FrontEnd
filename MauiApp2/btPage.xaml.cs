using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;
public partial class btPage : ContentPage
{
    //AppShell shelly = new AppShell();
    public btPage()
    {
        InitializeComponent();
    }
    private void OnConnBtnClicked(object sender, EventArgs e)
    {
        //set bt to true on home page
        //shelly.SetBluetooth(true);
        Navigation.PushAsync(new MainPage());
    }
}