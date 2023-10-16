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

public class Ball
{
    public IDevice dev { get; set; }
    public string name { get; set; }
    public int weight{ get; set; }
    public string color{ get; set; }

    public Ball(IDevice newDev)
    {
        dev = newDev;
        name = "Unnamed Ball";
        weight = 1;
        color = "Unknown";
    }

    /*public void setName(string newName)
    {
        name = newName;
    }
    public string getName()
    {
        return name;
    }

    public void setColor(string newCol)
    {
        color = newCol;
    }

    public string getColor()
    {
        return color;
    }

    public void setWeight(int newWt)
    {
        weight = newWt;
    }

    public int getWeight()
    {
        return weight;
    }
    public IDevice getDev()
    {
        return dev;
    }*/
}