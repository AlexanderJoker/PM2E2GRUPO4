using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.CurrentActivity;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Android;

namespace PM2E2GRUPO4.Droid
{
    [Activity(Label = "PM2E2GRUPO4", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string[] Permission = {
         Android.Manifest.Permission.WriteExternalStorage,
         Android.Manifest.Permission.ReadExternalStorage,
         Manifest.Permission.RecordAudio,
         Manifest.Permission.AccessFineLocation
        };
        const int RequestId = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            RequestPermissions(Permission, RequestId);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
       
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}