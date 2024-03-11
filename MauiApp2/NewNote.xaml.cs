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

public partial class NewNote : ContentPage
{
    public NewNote()
    {
        InitializeComponent();
    }
    
    private void OnTitleAdded(object sender, TextChangedEventArgs e)
    {
        string oldUsr = e.OldTextValue;
        string newUsr = e.NewTextValue;
    }
    
    private void OnNoteBodyAdded(object sender, TextChangedEventArgs e)
    {
        string oldUsr = e.OldTextValue;
        string newUsr = e.NewTextValue;
    }
    
    private void OnSaveNoteBtnClicked(object sender, EventArgs e)
    {
        //something happens
        Navigation.PushAsync(new NotePage());
    }
    
    
}