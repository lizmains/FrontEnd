using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp2.ViewModel;

namespace MauiApp2;

public partial class EditBall : ContentPage
{
    private Ball tempBall = new Ball(null);
    public EditBall()
    {
        InitializeComponent();
    }
    void OnNameChanged(object sender, TextChangedEventArgs e)
    {
        string oldName = e.OldTextValue;
        string newName = e.NewTextValue;  //Events for assigning 
        tempBall.name = ballName.Text;
    }
    void OnNameEnter(object sender, EventArgs e)
    {
        tempBall.name = ((Entry)sender).Text;
    }
    
    void OnWtChanged(object sender, TextChangedEventArgs e)
    {
        string oldWt = e.OldTextValue;
        string newWT = e.NewTextValue;  //Events for assigning 
        tempBall.weight = 15;//ballWeight.Text; need to figure out how to get an int
    }
    void OnWtEnter(object sender, EventArgs e)
    {
        tempBall.weight = 15;//((Entry)sender).Text;
    }
    
    void OnColorChanged(object sender, TextChangedEventArgs e)
    {
        string oldCol = e.OldTextValue;
        string newCol = e.NewTextValue;  //Events for assigning 
        tempBall.color = ballColor.Text;
    }
    void OnColorEnter(object sender, EventArgs e)
    {
        tempBall.color = ((Entry)sender).Text;
    }
    
    void OnCoreChanged(object sender, TextChangedEventArgs e)
    {
        string oldCor = e.OldTextValue;
        string newCor = e.NewTextValue;  //Events for assigning 
        tempBall.core = ballCore.Text;
    }
    void OnCoreEnter(object sender, EventArgs e)
    {
        tempBall.core = ((Entry)sender).Text;
    }
    
    void OnCovChanged(object sender, TextChangedEventArgs e)
    {
        string oldCov = e.OldTextValue;
        string newCov = e.NewTextValue;  //Events for assigning 
        tempBall.cover = ballCover.Text;
    }
    void OnCovEnter(object sender, EventArgs e)
    {
        tempBall.cover = ((Entry)sender).Text;
    }

    async void OnSave(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
    
    async void OnCancel(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}