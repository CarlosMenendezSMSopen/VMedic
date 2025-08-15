#if IOS
using UIKit;
using Foundation;

#endif
#if ANDROID
using Android.Content.PM;
using Android.OS;
#endif

using VMedic.Utilities;
using VMedic.MVVM.ViewModels;

namespace VMedic
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            lblUserName.Text = App.usuario?.GetItem().UsuarioName;
#if IOS
            var versionName = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];

            lbl_version.Text = versionName.ToString();
#endif

#if ANDROID
            PackageManager? packagemanager = Android.App.Application.Context.PackageManager;
            string? packagename = Android.App.Application.Context.PackageName;
            PackageInfo? packageinfo = null;
            try
            {
                if (packagename is not null)
                    packageinfo = packagemanager?.GetPackageInfo(packagename, 0);
            }
            catch (Exception ex)
            {
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error PackageInfo", ex.ToString(), ex.Message, App.Current?.MainPage);
            }

            lbl_version.Text = packageinfo?.VersionName;
#endif
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (PressedPreferences.ValidatePressing())
            {
                PressedPreferences.Pressing(sender);

                var validation = await AppShell.Current.DisplayAlert("Aviso", "¿Desea cerrar esta sesión?", "SI", "NO");
                if (validation)
                {
                    var vm = (AppShellViewModel)BindingContext;
                    vm.SignOut();
                }
                else
                {
                    PressedPreferences.EndPressed();
                }
            }
        }
    }
}
