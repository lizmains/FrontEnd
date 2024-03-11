using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using UIKit;

namespace MauiApp2;

public partial class GamePage : ContentPage
{
    public List<List<string>> ButtonItems { get; set; }
    public GamePage()
    {
        InitializeComponent();

        ButtonItems = new List<List<string>>
        {
            new List<string> { "Pin 7", "Pin 8", "Pin 9", "Pin 10" },
            new List<string> { "Pin 4", "Pin 5", "Pin 6" },
            new List<string> { "Pin 2", "Pin 3" },
            new List<string> { "Pin 1" }
        };
        // DataContext = this;
    }
    
    private async void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        await Navigation.PushAsync(new NewGame());
    }

    private async void OnPinBtnClicked(object sender, EventArgs e)
    {
        //thing happens here
    }
    
    private async void OnGutterballBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next shot in frame OR next frame
        //no pins fall
        //score += 0
    }
    
    private async void OnFoulBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next shot in frame OR next frame
        //no pins fall
        //score += 0
        //idk what fouls mean in bowling
    }
    
    private async void OnStrikeBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next frame
        //all pins fall
        //score += 10
    }
    
    private async void OnSpareBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next frame
        //all pins fall
        //score += whatevers left over from first shot
    }
    
    private async void OnPrevFrameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next frame
        //all pins fall
        //score += whatevers left over from first shot
    }
    
    private async void OnNextFrameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // go to next frame
        //all pins fall
        //score += whatevers left over from first shot
    }
}