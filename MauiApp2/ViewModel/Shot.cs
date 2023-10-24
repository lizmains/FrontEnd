using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;

namespace MauiApp2.ViewModel;

public class Shot
{
    public int shot_id { get; set; }
    
    public long frame_id { get; set; }
    public long ball_id{ get; set; }
    public long video_id{ get; set; }
    public bool[,] pins_remaning { get; set; }
    //somehow make this binary data
    public string curTime { get; set; }
    //date time can be stored in a string variable if formatted correctly
    public int[,] laneNum { get; set; }
    //somehow make this binary data
    //convert this to a 2D Binary array
    //does int work for representing binary data?
    public float[,] ddx { get; set; }
    //make this float(8)[100]
    public float[,] ddy { get; set; }
    //make this float(8)[100]
    public float[,] ddz { get; set; }
    //make this float(8)[100]
    public float[,] x_pos { get; set; }
    //make this float(8)[100]
    public float[,] y_pos { get; set; }
    //make this float(8)[100]
    public float[,] z_pos { get; set; }
    //make this float(8)[100]
    


    public Shot(int identifier, long ballId)
    {
        shot_id = identifier;
        ball_id = ballId;
        
    }

    public void setShotID(int ID)
    {
        shot_id = ID;
    }

    public int getShotID()
    {
        return shot_id;
    }

    public void setFrameID(long ID)
    {
        frame_id = ID;
    }

    public long getFrameID()
    {
        return frame_id;
    }
    
    public void setBallID(long ID)
    {
        ball_id = ID;
    }

    public long getBallID()
    {
        return ball_id;
    }

    public void setVidID(long ID)
    {
        video_id = ID;
    }

    public long getVidID()
    {
        return video_id;
    }

    public void setCurrentTime(String time)
    {
        curTime = time;
    }

    public String getCurrentTime()
    {
        return curTime;
    }
    
    
}