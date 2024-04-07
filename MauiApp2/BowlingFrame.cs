namespace MauiApp2;

public class BowlingFrame
{
    public int frameNum { get; set; }
    public int? firstRoll { get; set; }
    public int? secondRoll { get; set; }
    public int? firstRollPinsLeft { get; set; }
    public int? secondRollPinsLeft { get; set; }
    public int frameScore { get; set; }
    //add other shit in here later for things like 10th frame
    //do i add the pin flags here or somewhere else

    public void calcluateScore()
    {
        if (firstRollPinsLeft.HasValue)
        {
            firstRoll = 10 - firstRollPinsLeft.Value;
        }

        if (secondRollPinsLeft.HasValue)
        {
            secondRoll = firstRoll.HasValue
                ? 10 - secondRollPinsLeft.Value
                : 10 - (firstRollPinsLeft.Value + secondRollPinsLeft.Value);
        }

        frameScore = (firstRoll ?? 0) + (secondRoll ?? 0);
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
        throw new NotImplementedException();
    }
}