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
using CommunityToolkit.Maui.Views;
using NativeMedia;

// using NativeMedia;

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
        //IN THE WORKS
        var video = await MediaPicker.CapturePhotoAsync();
        // var video = await MediaGallery.CapturePhotoAsync();
        await MediaGallery.SaveAsync(MediaFileType.Image, await video.OpenReadAsync(), "myMedia.png");
        if (video == null)
        {
            await DisplayAlert("Null Video", "Video could not be saved", "OK");
        }

    }
    
    public async void PickImage()
    {
        
        // Not using MediaGallery, but the built-in MediaPicker
        var results = await MediaPicker.PickVideoAsync();
        // var results = await MediaGallery.PickAsync(1, MediaFileType.Image)
        
        //For some reason none of this is being hit at all...
        await DisplayAlert("You are here 1", "here 1", "OK");
        
        if (results == null)
        {
            await DisplayAlert("Null Photos", "The photos you have selected are null", "OK");
            return;
        }
        
        var fileName = Path.GetFileNameWithoutExtension(results.FullPath);
        var extension = Path.GetExtension(results.FullPath);
        var contentType = results.ContentType;
        var readIt = await results.OpenReadAsync();
        
        await DisplayAlert(fileName, $"Extension: {extension}, Full Path: {results.FullPath}, File Name: {results.FileName}", "OK");
        
        // myVideo.Source = ImageSource.FromStream(() => readIt);

        // mediaElement.Source = MediaSource.FromFile(results.FileName);
        mediaElement.Source = FileMediaSource.FromFile(results.FullPath);
        
        await DisplayAlert("You are here", "Here", "OK");
    }
    //public System.Threading.Tasks.Task<Microsoft.Maui.Storage.FileResult> PickVideoAsync (Microsoft.Maui.Media.MediaPickerOptions? options = default);
    
}