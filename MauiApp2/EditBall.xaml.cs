using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiApp2.ViewModel;

namespace MauiApp2;

public partial class EditBall : ContentPage
{
    private Ball tempBall;
    private string cName;
    private int cWt;
    private string cCol;
    private string cCor;
    private string cCov;
    private string cCom;
    public EditBall(Ball toEdit)
    {   
        InitializeComponent();
        tempBall = toEdit;
        BallName.Text = tempBall.name + "\n";
        ballName.Text = tempBall.name;
        ballColor.Text = tempBall.color;
        ballCover.Text = tempBall.cover;
        ballWeight.Text = tempBall.weight.ToString();
        ballComm.Text = tempBall.comments;
        ballCore.Text = tempBall.core;
        ballSerialID.Text = tempBall.serial.ToString();
        cWt = -1;
    }
    void OnNameChanged(object sender, TextChangedEventArgs e)
    {   
        string oldName = e.OldTextValue;
        string newName = e.NewTextValue;  //Events for assigning 
        cName = ballName.Text;
    }
    void OnNameEnter(object sender, EventArgs e)
    {
        cName = ((Entry)sender).Text;
    }
    
    void OnWtChanged(object sender, TextChangedEventArgs e)
    {
        string oldWt = e.OldTextValue;
        string newWT = e.NewTextValue;  //Events for assigning 
        cWt = Convert.ToInt32(ballWeight.Text);
    }
    void OnWtEnter(object sender, EventArgs e)
    {
        cWt = Convert.ToInt32(((Entry)sender).Text);//((Entry)sender).Text;
    }
    
    void OnColorChanged(object sender, TextChangedEventArgs e)
    {
        string oldCol = e.OldTextValue;
        string newCol = e.NewTextValue;  //Events for assigning 
        cCol = ballColor.Text;
    }
    void OnColorEnter(object sender, EventArgs e)
    {
        cCol = ((Entry)sender).Text;
    }
    
    void OnCoreChanged(object sender, TextChangedEventArgs e)
    {
        string oldCor = e.OldTextValue;
        string newCor = e.NewTextValue;  //Events for assigning 
        cCor = ballCore.Text;
    }
    void OnCoreEnter(object sender, EventArgs e)
    {
        cCor = ((Entry)sender).Text;
    }
    
    void OnCovChanged(object sender, TextChangedEventArgs e)
    {
        string oldCov = e.OldTextValue;
        string newCov = e.NewTextValue;  //Events for assigning 
        cCov = ballCover.Text;
    }
    void OnCovEnter(object sender, EventArgs e)
    {
        cCov = ((Entry)sender).Text;
    }
    
    void OnComChanged(object sender, TextChangedEventArgs e)
    {
        string oldCom = e.OldTextValue;
        string newCom = e.NewTextValue;  //Events for assigning 
        cCom = ballComm.Text;
    }
    void OnComEnter(object sender, EventArgs e)
    {
        cCom = ((Entry)sender).Text;
    }

    async void OnSave(object sender, EventArgs e)
    {
        if (cName != null)
        {
           tempBall.name = cName; 
        }
        if (cWt != -1)
        {
            tempBall.weight = cWt;
        }
        if (cCol != null)
        {
            tempBall.color = cCol;
        }
        if (cCor != null)
        {
          tempBall.core = cCor;  
        }
        if (cCov != null)
        {
            tempBall.cover = cCov;
        }
        if (cCom != null)
        {
           tempBall.comments = cCom; 
        }
        
        //This is the same code that is in btPage, I call the 
        string bowlingBallStringList = App.UserRepository.GetAllUsers()[0].BallList;
        List<Ball> ballList = JsonSerializer.Deserialize<List<Ball>>(bowlingBallStringList);
        Ball ballToEdit;
        for (int i = 0; i < ballList.Count; i++)
        {
            if (ballList[i].serial.ToString() == tempBall.serial.ToString())
            {
                // Console.WriteLine("Name: " + ballList[i].name);
                ballList[i] = tempBall;
                
                
            }
        }
        string bowlingBallListString = JsonSerializer.Serialize(ballList);
        Console.WriteLine("string : " + bowlingBallListString);
        // App.UserRepository.GetAllUsers()[0].BallList = bowlingBallListString;
        App.UserRepository.EditBallList(App.UserRepository.GetAllUsers()[0],bowlingBallListString);
        
        Console.WriteLine("After saving editing changes: " + App.UserRepository.GetAllUsers()[0].BallList);
        await Navigation.PopModalAsync();
    }
    
    async void OnCancel(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}