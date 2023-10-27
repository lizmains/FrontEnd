using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using Android.Media;
// using Android.Media;
// using Android.Hardware.Camera2;
// using Android.Media;
using Stream = System.IO.Stream;

namespace MauiApp2;

public partial class VideoPage : ContentPage
{
    public VideoPage()
    {
        InitializeComponent();
    }
    private void OnPrevBtnClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    
    private void OnNextBtnClicked(object sender, EventArgs e) //Button to view smartdot stats
    {
        Navigation.PushAsync(new NextPage()); //navigate to stat page
    }
    private void OnTakeVidBtnClicked(object sender, EventArgs e)
    {
        //TakePhoto(); //Take photo
        TakeVideo(); //Take video
    }
    private void OnPicVidBtnClicked(object sender, EventArgs e)
    {
        //Navigation.PushAsync(new MainPage()); //navigate to main page
        //PickVideo(); //Pick video from camera roll
        PickImage();
    }
    
    public async void TakePhoto()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into camera roll
                // string cameraRollPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), photo.FileName);
                string cameraRollPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream cameraRollFileStream = File.OpenWrite(cameraRollPath);

                await sourceStream.CopyToAsync(cameraRollFileStream);
            }
        }
    }

    
    // found this code at: https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/device-media/picker?tabs=macios#take-a-photo
    public async void TakeVideo()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult video = await MediaPicker.Default.CaptureVideoAsync();

            if (video != null)
            {
                // save the file into local storage
                // string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), video.FileName); //not sure if this is the right directory
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, video.FileName); //not sure if this is the right directory

                using Stream sourceStream = await video.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                await Shell.Current.DisplayAlert("OOPS", localFileStream.Name, "Ok");
            }
        }
    }
    
    public async void PickVideo()
    {
        // if (MediaPicker.Default.IsCaptureSupported)
        // {
        //     FileResult video = await MediaPicker.Default.PickVideoAsync(); //opens camera roll, nothing happens after select video as of right now
        // }

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick video",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
        {
            return;
        }

        var stream = await result.OpenReadAsync();
        myVideo.Source = ImageSource.FromStream(() => stream); //I think this has something to do with why it wont come up
    }
    
    public async void PickImage()
    {
        // if (MediaPicker.Default.IsCaptureSupported)
        // {
        //     FileResult video = await MediaPicker.Default.PickVideoAsync(); //opens camera roll, nothing happens after select video as of right now
        // }

        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick video",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null)
        {
            return;
        }

        var stream = await result.OpenReadAsync();
        myVideo.Source = ImageSource.FromStream(() => stream);
    }
    //public System.Threading.Tasks.Task<Microsoft.Maui.Storage.FileResult> PickVideoAsync (Microsoft.Maui.Media.MediaPickerOptions? options = default);
    
}