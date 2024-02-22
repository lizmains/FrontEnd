using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2;

public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();
        
        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        for (int i = 4; i > 0; i++)
        {
            var horizontalStackLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center
            };

            for (int j = 0; j <= i; j++)
            {
                var circularButton = new Button()
                {
                    Text = $"Pin i",
                    WidthRequest = 50,
                    HeightRequest = 50,
                    CornerRadius = 25,
                };
                
                horizontalStackLayout.Children.Add(circularButton);
            }

            stackLayout.Children.Add(horizontalStackLayout);
        }

        Content = stackLayout;
    }
    
    
    private async void OnNewGameBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        await Navigation.PushAsync(new NewGame());
    }
}