using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp2.ViewModel;

public partial class MainViewModel: ObservableObject
{
    //Variable to hold Username in this model
    string username;
    
    //This is an Observable Property, meaning it's a variable that interacts with the xaml. 
    //When I tell a variable that it is Observable Property, Maui creates code for that variable to be edited/adjusted
    //by Xaml or through model here. You reference this variable in xaml through 'WelcomePhrase'
    [ObservableProperty] 
    string welcomePhrase;
    
    [ObservableProperty]
    string password;
    
    
    public MainViewModel(string username, string password)
    {
        this.welcomePhrase = "Welcome to RevMetrix Bowling Assistant " + username + "!";
        this.username = username;
        this.password = password;
    }
    
    // [RelayCommand] //**future change
    // void Settings()
    // {
    //     string navigation_str = "";
    //     navigation_str += "$" + nameof(SettingsPage) + $"?username={username}&password={password}";
    //     Console.WriteLine(navigation_str);
    //     Shell.Current.GoToAsync($"{nameof(SettingsPage)}?username={username}&password={password}");
    // }
}