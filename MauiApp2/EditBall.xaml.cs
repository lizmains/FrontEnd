using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.ViewModel;

namespace MauiApp2;

public partial class EditBall : ContentPage
{
    private Ball tempBall;
    private Ball prevBall;
    private string cName;
    private int cWt;
    private string cCol;
    private string cCor;
    private string cCov;
    public EditBall(Ball toEdit)
    {
        InitializeComponent();
        tempBall = toEdit;
        //prevBall = toEdit;
        BallName.Text = tempBall.name + "\n";
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
        cWt = 15;//ballWeight.Text; need to figure out how to get an int
    }
    void OnWtEnter(object sender, EventArgs e)
    {
        cWt = 15;//((Entry)sender).Text;
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

    async void OnSave(object sender, EventArgs e)
    {
        tempBall.name = cName;
        tempBall.weight = cWt;
        tempBall.color = cCol;
        tempBall.core = cCor;
        tempBall.cover = cCov;
        await Navigation.PopModalAsync();
    }
    
    async void OnCancel(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}