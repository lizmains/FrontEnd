using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp2;

public partial class GamePage : ContentPage
{
    public List<Boolean> pins = new List<Boolean>(10);
    public List<int> frames = new List<int>(10);
    public List<int> shots = new List<int>(21);
    private int frameNum = 0;
    private int shotNum = 0;
    public int gameNum = 1;
    public int score = 0;
    public int scoreNum = 10;
    
    public GamePage()
    {
        InitializeComponent();

        for (int i = 0; i < 10; i++)
        {
            pins.Add(false);
            //KNOCKED DOWN = FALSE
        }
        
    }
    
    private async void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        await Navigation.PushAsync(new NewGame());
    }

    private async void OnPinBtnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int pin = int.Parse(button.Text);
        
        int frameScore = 10;
        pins[pin] = !pins[pin];

        for (int i = 0; i < 10; i++)
        {
            if (pins[i])
            {
                frameScore--;
                
            }
        }

        if (button.BackgroundColor == Colors.DarkOrchid)
        {
            button.BackgroundColor = Colors.Teal;
        }
        else
        {
            button.BackgroundColor = Colors.DarkOrchid;
        }

        scoreNum = frameScore;
        score += scoreNum;
        UpdateScoreDisplay();

    }

    void UpdateScoreDisplay()
    {
        FrameP1.Text = $"{scoreNum}";
    }
    
    
    private async void OnGutterballBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        frameNum++;
        shotNum++;
    }
    
    private async void OnFoulBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        //score += 0
        //still have to calculate how many pins were knocked down, just doesn;'t contribute to score
        //can spare off a foul
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
        frameNum--;
    }
    
    private async void OnNextFrameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        frameNum++;
    }
}