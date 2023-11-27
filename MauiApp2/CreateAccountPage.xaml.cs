using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Client;
using Common.POCOs;
using MauiApp2.ViewModel;

namespace MauiApp2;

public partial class CreateAccountPage : ContentPage
{
    private string createUsr;
    private string createPass;
    private FeaturedAPI api;
    //API will be used for authentication with created username and password
    public CreateAccountPage(FeaturedAPI apiIn)
    {
        InitializeComponent();
        api = apiIn;
    }

    async void OnBackBtn(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(api));
    }

    private void OnUsrNameChanged(object sender, TextChangedEventArgs e)
    {
        string oldUsr = e.OldTextValue;
        string newUsr = e.NewTextValue;
        createUsr = newUsr;
    }

    private void OnUsrPassChanged(object sender, TextChangedEventArgs e)
    {
        string oldPass = e.OldTextValue;
        string newPass = e.NewTextValue;
        createPass = newPass;
    }

    async void OnSubmitClicked(object sender, EventArgs e)
    {
        if (createUsr != null && createPass != null)
        {
            //Handle API Creation of an account here
            UserIdentification UserID = new UserIdentification(createUsr, createUsr,
  createUsr, createPass, createUsr, createUsr);
            // We want to execute a POST to User/Authorize
            // We will provide a UserIdentification (for now just username and password)
            // We expect back a DualToken POCO
            APIResponse<DualToken> response = await api.Post<DualToken, UserIdentification>("User/Register", UserID);
            if (response != null)
            {
                if (response.WasSuccessful)
                {
                    if (response.Result != null)
                    {
                        // Let's use that DualToken and store the retrieved authorization and refresh tokens in the API object for future use
                        DualToken tokens = response.Result;
                        api.SetAuthJWT(tokens.TokenA);
                        api.RefreshToken = tokens.TokenB;
                    }
        
                }
            }
            Console.WriteLine("New Username===" + createUsr);
            Console.WriteLine("New Password===" + createPass);
            
            
            Ball ball1 = new Ball(null)
            {
                color = "green", comments = "n/a",core = "",cover = "", name = "ball1-1",weight = 10,serial = Guid.NewGuid()
            };
        
            Ball ball2 = new Ball(null)
            {
                color = "red", comments = "", core = "", cover = "", name = "ball 2-1", weight = 123, serial = Guid.NewGuid()
            };
        
            var bowlingBallList = new List<Ball> { ball1, ball2 };
            string bowlingBallListString = JsonSerializer.Serialize(bowlingBallList);
            App.UserRepository.Add(new User
            {
                LastLogin = DateTime.Now,
                Password = createPass,
                UserName = createUsr,
                BallList = bowlingBallListString
                });
            await Navigation.PushAsync(new LoginPage());
        }
        else
        {
            await DisplayAlert("Alert", "Please Enter Username and Password", "OK");
        }
    }
}