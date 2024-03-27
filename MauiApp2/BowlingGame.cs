namespace MauiApp2;

public class BowlingGame
{
    public List<BowlingFrame> Frames { get; set; }

    public BowlingGame()
    {
        Frames = new List<BowlingFrame>(10);
        InitializeFrames();
    }

    private void InitializeFrames()
    {
        for (int i = 0; i < 10; i++)
        {
            Frames.Add(new BowlingFrame());
        }
    }
    
    //calculate shit in here later also for scores and rolls

    private int calculateScore(Frame frame, int score)
    {
        
        return score;
    }
}