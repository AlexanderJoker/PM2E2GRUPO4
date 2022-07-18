using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO4
{
    public partial class App : Application
    {
        PermissionStatus estado;
        public App()
        {
            InitializeComponent();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RequestCameraAccessAsync();
                if (estado==PermissionStatus.Granted)
                {
                    MainPage = new NavigationPage(new MainPage());
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }
            });
            
        }
        private async void RequestCameraAccessAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                // Prompt the user to turn on in settings
                // On iOS once a permission has been denied it may not be requested again from the application
                return;
            }

            status = Permissions.RequestAsync<Permissions.LocationWhenInUse>().Result;
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
