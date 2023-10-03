using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class FilePage : ContentPage
{
    public FilePage()
    {
        InitializeComponent();
    }

    string userI = "N/A";
    
    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private string getUserInput(object sender, EventArgs e)
    {
        return userI;
    }
    
    private void OnHomeBtnClicked(object sender, EventArgs e)
    {
        // Navigation.PushAsync(new MainPage());
        Shell.Current.GoToAsync(nameof(MainPage));
    }
    
    private void OnUsrChanged(object sender, TextChangedEventArgs e)
    {
        string oldUsr = e.OldTextValue;
        string newUsr = e.NewTextValue;  //Events for assigning 
    }
    
    void OnUsrEnter(object sender, EventArgs e)
    {
        userI = ((Entry)sender).Text;
        OnLoginBtnClicked(sender, e);
    }
    
    void OnLoginBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        userI = UserInput.Text;
        userIO.Text = $"{userI}";
        //this only changes the variable stored when the button is pressed
    }
    
    
}
