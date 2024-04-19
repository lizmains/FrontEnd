using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp2;

public partial class GamePage : ContentPage
{
    //I dont think any of the onBtnClicked events are needed, they'd all connect to a
    //Command within the viewModel instead of the logic being done here...
    //Also dont think you should have these private variables here, not what this files for...
    // private List<Boolean> pins = new List<Boolean>(10);
    // public List<int> frames = new List<int>(10);
    // public List<int> shots = new List<int>(21);
    // private int frameNum = 0;
    // private int shotNum = 0;
    // public int gameNum = 1;
    // public int score = 0;
    // private int scoreNum = 10;
    
    public GamePage()
    {
        InitializeComponent();

        // for (int i = 0; i < 10; i++)
        // {
        //     pins.Add(false);
        //     //KNOCKED DOWN = FALSE
        // }

        BindingContext = new BowlingGameViewModel(); //this is the thing causing the upset, im not sure why

    }
    
    private async void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        await Navigation.PushAsync(new NewGame());
    }

    // private void OnPinBtnClicked(object sender, EventArgs e)
    // {
    //     var button = (Button)sender;
    //     int pin = int.Parse(button.Text);
    //     
    //     int frameScore = 10;
    //     pins[pin] = !pins[pin];
    //
    //     for (int i = 0; i < 10; i++)
    //     {
    //         if (pins[i])
    //         {
    //             frameScore--;
    //             
    //         }
    //     }
    //
    //     button.BackgroundColor = button.BackgroundColor.Equals(Colors.DarkOrchid) ? Colors.Teal : Colors.DarkOrchid;
    //
    //     scoreNum = frameScore;
    //     score += scoreNum;
    //     UpdateScoreDisplay();
    //     
    // }

    // void UpdateScoreDisplay()
    // {
    //     // FrameP1.Text = $"{scoreNum}";
    //     // F1.Text = $"{scoreNum}";
    //     //update score display
    //     
    // }
    
    
    // private async void OnGutterballBtnClicked(object sender, EventArgs e) //navigate to simulator
    // {
    //     frameNum++;
    //     FrameP2.Text = $"{frameNum + 1}";
    //     shotNum++;
    // }
    //
    // private async void OnFoulBtnClicked(object sender, EventArgs e) //navigate to simulator
    // {
    //     //score += 0
    //     //still have to calculate how many pins were knocked down, just doesn;'t contribute to score
    //     //can spare off a foul
    //     //idk what fouls mean in bowling
    //     frameNum++;
    //     FrameP2.Text = $"{frameNum + 1}";
    // }
    //
    // private void OnStrikeBtnClicked(object sender, EventArgs e) //navigate to simulator
    // {
    //     // go to next frame
    //     //all pins fall
    //     //score += 10
    //     frameNum++;
    //     FrameP2.Text = $"{frameNum + 1}";
    // }
    //
    // private async void OnSpareBtnClicked(object sender, EventArgs e) //navigate to simulator
    // {
    //     // go to next frame
    //     //all pins fall
    //     //score += whatevers left over from first shot
    //     frameNum++;
    //     FrameP2.Text = $"{frameNum + 1}";
    // }

    private async void OnPrevFrameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // frameNum--;
        // FrameP2.Text = $"{frameNum+1}";
        //
        // if (frameNum < 0)
        // {
        //     frameNum++;
        //     FrameP2.Text = $"{frameNum+1}";
        //     await DisplayAlert("No", "You are on the first frame", "OK");
        // }
    }
    
    private async void OnNextFrameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        // frameNum++;
        // FrameP2.Text = $"{frameNum + 1}"; //this should get handled in the viewmodel i think
        // if (frameNum > 9)
        // {
        //     frameNum--;
        //     FrameP2.Text = $"{frameNum + 1}";
        //     await DisplayAlert("No", "You are on the last frame", "OK");
        // }
    }
}