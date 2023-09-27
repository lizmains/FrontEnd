using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shiny;

// using Shiny.BluetoothLE;


namespace MauiApp2;
public partial class btPage : ContentPage
{
    // private IBleManager bleManager;
    private ILogger<btPage> logger;
    private string[] stuff;
    private ConnectivityChangedEventArgs c;
    
    public btPage()
    {
        InitializeComponent();
    }

    private void ScanClicked(object sender, EventArgs e)
    {
        // var scanner = this.bleManager.Scan().Subscribe(result =>
        // {
        //     stuff.Append(result.Peripheral.ToString());
        //     stuff.Append(result.AdvertisementData.ToString());
        //     
        // });
        // Device.Text = "result.Peripheral.ToString()";
        // scanner.Dispose();
        
    }

    private void Connectivity_ConnectivityChanged(object sender, EventArgs e)
    {
        if (c.NetworkAccess == NetworkAccess.ConstrainedInternet)
            Console.WriteLine("Internet access is available but is limited.");

        else if (c.NetworkAccess != NetworkAccess.Internet)
            Console.WriteLine("Internet access has been lost.");

        // Log each active connection
        Console.Write("Connections active: ");

        foreach (var item in c.ConnectionProfiles)
        {
            switch (item)
            {
                case ConnectionProfile.Bluetooth:
                    Console.Write("Bluetooth");
                    break;
                case ConnectionProfile.Cellular:
                    Console.Write("Cell");
                    break;
                case ConnectionProfile.Ethernet:
                    Console.Write("Ethernet");
                    break;
                case ConnectionProfile.WiFi:
                    Console.Write("WiFi");
                    break;
                default:
                    break;
            }
        }

        Console.WriteLine();
    }
}