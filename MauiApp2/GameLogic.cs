namespace MauiApp2;

public class GameLogic
{
    private List<Boolean> pins = new List<Boolean>(10);
    private List<int> frames = new List<int>(10); //holds score per frame
    private List<int> shots = new List<int>(21); //holds score per shot
    private int frameNum;
    private int shotNum;
    public int score;

    public GameLogic()
    {
        for (int i = 0; i < 10; i++)
        {
            pins.Add(false);
            //KNOCKED DOWN = FALSE
        }
        frameNum = 0;
        shotNum = 0;
        score = 0;
    }

    public int CurrentFrame => frameNum++;
    public int CurrentShot => shotNum++;
    
    
}