namespace MauiApp2;

public partial class MainPage : ContentPage
{
    //AppShell shell = new AppShell();
    public MainPage()
    {
        InitializeComponent();
    }
    Boolean bt;
    private void OnBTClicked(object sender, EventArgs e) //bluetooth connection button
    {
        bTBtn.Text = $"Pairing...";
        //steps for actual pairing will go here
        Navigation.PushAsync(new btPage());
        bt = true;

        SemanticScreenReader.Announce(bTBtn.Text);
    }
    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        if (bt/*shell.GetBluetooth()*/) //check to see if bt connection is established
        {
            Navigation.PushAsync(new NextPage()); //if yes, navigate to stat page
        }
        else //else tell user to connect to bt
        {
            DisplayAlert("Alert", "Please Connect to SmartDot", "OK");
        }

    }
    private void OnSimBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new SimPage());
    }
    
    private void OnVidBtnClicked(object sender, EventArgs e) //navigate to simulator
    {
        Navigation.PushAsync(new VideoPage());
    }
}