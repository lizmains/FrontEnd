using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class NewGame : ContentPage
{
    private int count = 1;
    private int strike = 0;
    private int spare = 0;
    private int miss = 0;
    public NewGame()
    {
        InitializeComponent();
    }
    
    private void OnNextFrameClicked(object sender, EventArgs e)
    {
        if (count <= 10)
        {
            count++;
        }
        FrameNum.Text = $"Frame {count}";
        SemanticScreenReader.Announce(FrameNum.Text); //not too sure what this does
    }
    
    private void OnPrevFrameClicked(object sender, EventArgs e)
    {
        if (count > 1)
        {
            count --;
        }
        FrameNum.Text = $"Frame {count}";
        SemanticScreenReader.Announce(FrameNum.Text);
    }
    
    private void OnStrikeBtnClicked(object sender, EventArgs e)
    {
        strike++;
        StrikeBtn.Text = $"Strike {strike}";
        SemanticScreenReader.Announce(StrikeBtn.Text);
    }
    
    private void OnSpareBtnClicked(object sender, EventArgs e)
    {
        spare++;
        SpareBtn.Text = $"Spare {spare}";
        SemanticScreenReader.Announce(SpareBtn.Text);
    }
    
    private void OnMissBtnClicked(object sender, EventArgs e)
    {
        miss++;
        MissBtn.Text = $"Miss {miss}";
        SemanticScreenReader.Announce(MissBtn.Text);
    }
}