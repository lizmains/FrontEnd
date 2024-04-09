using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp2;

public partial class BowlingFrame : INotifyPropertyChanged
{
    public int frameNum { get; set; }
    private int? firstRoll;
    private int? secondRoll;
    public int? firstRollPinsLeft { get; set; }
    public int? secondRollPinsLeft { get; set; }
    public int frameScore;
    private int firstRollScore;
    private int secondRollScore;
    // private int frameScore;
    
    public int? FirstRoll
    {
        get => firstRoll;
        set => SetField(ref firstRoll, value);
    }
    
    public int? SecondRoll
    {
        get => secondRoll;
        set => SetField(ref secondRoll, value);
    }
    
    public int FrameScore
    {
        get => frameScore;
        set => SetField(ref frameScore, value);
    }

    public void calcluateScore()
    {
        // if (firstRollPinsLeft.HasValue)
        // {
        //     firstRoll = 10 - firstRollPinsLeft.Value;
        // }
        //
        // if (secondRollPinsLeft.HasValue)
        // {
        //     secondRoll = firstRoll.HasValue
        //         ? 10 - secondRollPinsLeft.Value
        //         : 10 - (firstRollPinsLeft.Value + secondRollPinsLeft.Value);
        // }
        //
        // frameScore = (firstRoll ?? 0) + (secondRoll ?? 0);

        if (firstRollPinsLeft.Value != 0)
        {
            firstRoll = 10 - firstRollPinsLeft.Value;
            Console.WriteLine($"firstRollPinsLeft.Value {firstRollPinsLeft.Value}");
        }

        if (secondRollPinsLeft.Value != 0)
        {
            secondRoll = 10 - (firstRollPinsLeft.Value + secondRollPinsLeft.Value);
        }
    }
    
    public void addRoll(int pins)
    {
        if (firstRoll + secondRoll == 10)
        {
            firstRoll = pins;
        }
        else
        {
            secondRoll = pins;
        }
    }
    
    public void CalculateScore()
    {
        // frameScore = (int)(firstRoll + (secondRoll ?? 0));
        frameScore = firstRollScore + secondRollScore;
        OnPropertyChanged(nameof(FrameScore));
    }
    


    public int FirstRollScore
    {
        get => firstRollScore;
        set
        {
            if (firstRollScore != value)
            {
                firstRollScore = value;
                OnPropertyChanged(nameof(firstRollScore));
                UpdateFrameScore();
            }
        }
    }

    public int SecondRollScore
    {
        get => secondRollScore;
        set
        {
            if (secondRollScore != value)
            {
                secondRollScore = value;
                OnPropertyChanged(nameof(secondRollScore));
                UpdateFrameScore();
            }
        }
    }

    // public int FrameScore
    // {
    //     get => frameScore;
    //     private set
    //     {
    //         if (frameScore != value)
    //         {
    //             frameScore = value;
    //             OnPropertyChanged(nameof(frameScore));
    //         }
    //     }
    // }

    private void UpdateFrameScore()
    {
        // Logic to update the frame score based on first and second roll scores
        FrameScore = FirstRollScore + SecondRollScore;
        // Include any additional logic for strikes, spares, and bonuses if necessary
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}