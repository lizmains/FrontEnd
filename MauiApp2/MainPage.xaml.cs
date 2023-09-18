namespace MauiApp2;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
    }
    Boolean bt = false;
    private void OnCounterClicked(object sender, EventArgs e) //bluetooth connection button
    {
        bTBtn.Text = $"Pairing...";
        //steps for actual pairing will go here
        bt = true;

        SemanticScreenReader.Announce(bTBtn.Text);
    }
    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        if (bt) //check to see if bt connection is established
        {
            Navigation.PushAsync(new NextPage()); //if yes, navigate to stat page
        }
        else //else tell user to connect to bt
        {
            NextBtn.Text = "Connect To SmartDot";
            SemanticScreenReader.Announce(NextBtn.Text);
        }

    }
    private void OnSimBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new SimPage());
    }
}