using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp2;

public partial class BowlingFrame : INotifyPropertyChanged
{
    // public int frameNum { get; set; }
    public int firstRoll { get; set; }
    public int secondRoll { get; set; }
    public int frameScore { get; set; }

    public bool isFirstShot = true;
    // public int? firstRollPinsLeft { get; set; }
    // public int? secondRollPinsLeft { get; set; }
    //
    // private int firstRollScore;
    // private int secondRollScore;
    // private int frameScore;
    
    // public int? FirstRoll
    // {
    //     get => firstRoll;
    //     set => SetField<>(ref firstRoll, value);
    // }
    //
    // public int? SecondRoll
    // {
    //     get => secondRoll;
    //     set => SetField<>(ref secondRoll, value);
    // }
    //
    // public int FrameScore
    // {
    //     get => frameScore;
    //     set => SetField(ref frameScore, value);
    // }

    // public void calcluateScore()
    // {
    //     // if (firstRollPinsLeft.HasValue)
    //     // {
    //     //     firstRoll = 10 - firstRollPinsLeft.Value;
    //     // }
    //     //
    //     // if (secondRollPinsLeft.HasValue)
    //     // {
    //     //     secondRoll = firstRoll.HasValue
    //     //         ? 10 - secondRollPinsLeft.Value
    //     //         : 10 - (firstRollPinsLeft.Value + secondRollPinsLeft.Value);
    //     // }
    //     //
    //     // frameScore = (firstRoll ?? 0) + (secondRoll ?? 0);
    //
    //     if (firstRoll != 0)
    //     {
    //         firstRoll = 10 - firstRoll;
    //         
    //     }
    //
    //     if (secondRoll != 0)
    //     {
    //         secondRoll = 10 - (firstRoll + secondRoll);
    //     }
    // }
    
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
        frameScore = firstRoll + secondRoll;
        Console.WriteLine($"firstRollPinsLeft.Value {firstRoll}");
        Console.WriteLine($"secondRollPinsLeft.Value {secondRoll}");
        Console.WriteLine($"frameScore {frameScore}");
        OnPropertyChanged(nameof(frameScore));
    }
    


    // public int FirstRollScore
    // {
    //     get => firstRoll;
    //     set
    //     {
    //         if (firstRoll != value)
    //         {
    //             firstRoll = value;
    //             OnPropertyChanged(nameof(firstRoll));
    //             UpdateFrameScore();
    //         }
    //     }
    // }
    //
    // public int SecondRollScore
    // {
    //     get => secondRoll;
    //     set
    //     {
    //         if (secondRoll != value)
    //         {
    //             secondRoll = value;
    //             OnPropertyChanged(nameof(secondRoll));
    //             UpdateFrameScore();
    //         }
    //     }
    // }

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
        frameScore = firstRoll + secondRoll;
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