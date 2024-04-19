using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp2;

public class BowlingGameViewModel : INotifyPropertyChanged
{
    public BowlingGame game { get; }
    private int currentFrameIndex { get; set; }
    public ICommand pinButtonCommand { get; private set; }
    public ICommand nextShotCommand { get; set; }
    public BowlingFrame CurrentFrame => Frames[currentFrameIndex];
    public ICommand addRollCommand { get; private set; }
    // private ObservableCollection<BowlingFrame> frames;
    public ObservableCollection<BowlingFrame> Frames { get; }
    
    // public ObservableCollection<BowlingFrame> Frames
    // {
    //     get => frames;0
    //     set
    //     {
    //         if (frames != value)
    //         {
    //             frames = value;
    //             OnPropertyChanged(nameof(Frames));
    //         }
    //     }
    // }
    
    // public int CurrentFrameIndex
    // {
    //     get => currentFrameIndex;
    //     set
    //     {
    //         if (currentFrameIndex != value)
    //         {
    //             currentFrameIndex = value;
    //             OnPropertyChanged(nameof(CurrentFrameIndex));
    //             OnPropertyChanged(nameof(CurrentFrame));
    //         }
    //     }
    // }
    
    
    public BowlingGameViewModel()
    {
        game = new BowlingGame();
        Frames = new ObservableCollection<BowlingFrame>();
        for (int i = 0; i < 10; i++)
        {
            Frames.Add(new BowlingFrame());
        }
        currentFrameIndex = 0;
        addRollCommand = new Command<int>(addRoll);
        pinButtonCommand = new Command<int>(HandlePinButtonPress);
        nextShotCommand = new Command(MoveToNextShot);
    }
    
    private void HandlePinButtonPress(int pinsLeft)
    {
        // BowlingFrame currentFrame = GetCurrentFrame();
    
        if (CurrentFrame.isFirstShot)
        {
            //CurrentFrame is a bowlingframe from the MODELS -> checking to see if firstRoll has a score associated with it, 
            //should there be a flag here? bc otherwise each individual pin is going to trigger this and it wont work...
            // If this is the first roll, calculate the score accordingly.
            CurrentFrame.firstRoll = 10 - pinsLeft;
            Console.WriteLine($"CurrentFrame.FirstRoll {CurrentFrame.firstRoll}");
            
        }
        else if (!CurrentFrame.isFirstShot)
        {
            // If this is the second roll, calculate the score accordingly.
            CurrentFrame.secondRoll = 10 - pinsLeft;
        }
        else
        {
            //throw error here
        }

        CurrentFrame.CalculateScore();
    
        OnPropertyChanged(nameof(Frames));
        
    }
    
    private void MoveToNextFrame()
    {
        
        if (currentFrameIndex < game.Frames.Count - 1)
        {
            currentFrameIndex++;
            Console.WriteLine($"CurrentFrameIndex: {currentFrameIndex}");
            Console.WriteLine($"FramecCFI].frameScore {Frames[currentFrameIndex].frameScore}");
            CurrentFrame.isFirstShot = !CurrentFrame.isFirstShot;
        }
        
        else
        {
            //end of game
        }
        OnPropertyChanged(nameof(CurrentFrame));
        OnPropertyChanged(nameof(game));
    }

    private void MoveToNextShot()
    {
        

        if (CurrentFrame.isFirstShot)
        {
            CurrentFrame.isFirstShot = !CurrentFrame.isFirstShot;
            //switches bool
        }
        else
        {
            if (currentFrameIndex < game.Frames.Count - 1)
            {
                currentFrameIndex++;
                Console.WriteLine($"CurrentFrameIndex: {currentFrameIndex}");
                Console.WriteLine($"FramecCFI].frameScore {Frames[currentFrameIndex].frameScore}");
                CurrentFrame.isFirstShot = !CurrentFrame.isFirstShot;
            }
        
            else
            {
                //end of game
            }
            OnPropertyChanged(nameof(CurrentFrame));
        }
        OnPropertyChanged(nameof(game));
    }
    
    // private BowlingFrame GetCurrentFrame()
    // {
    //     //logic here idk
    //     BowlingFrame currentFrame = Frames[currentFrameIndex];
    //     
    //     return currentFrame;
    // }
    
    private void addRoll(int pins)
    {
        var currentFrame = findCurrentFrame();
        currentFrame.addRoll(pins);
        
        OnPropertyChanged(nameof(Frames));
    }
    
    private BowlingFrame findCurrentFrame()
    {
        //logic goes here
        // return game.Frames.First(); 
        return Frames[currentFrameIndex];
    }
    
    // private void updateFrameScore(string pinNum)
    // {
    //     int frameScore = 10;
    //
    //     Frames[1].frameScore = frameScore;
    // }
    
    
    // public void AddRollToFrame(int frameIndex, int rollIndex, int pinsDown)
    // {
    //     var frame = Frames[frameIndex];
    //
    //     if (rollIndex == 1)
    //     {
    //         frame.FirstRollScore = pinsDown;
    //     }
    //     else if (rollIndex == 2)
    //     {
    //         frame.SecondRollScore = pinsDown;
    //     }
    //
    // }
    
    //this was auto generated by the IDE bc it was mad about the INotifyPropertyChanged thing
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
    //end previous notes

    
    
}

