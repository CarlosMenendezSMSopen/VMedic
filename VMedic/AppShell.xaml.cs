using VMedic.Global;
using VMedic.MVVM.ViewModels;
using VMedic.Utilidades;
using VMedic.Servicios;

#if IOS
using UIKit;
using Foundation;
#endif
#if ANDROID
using Android.Content.PM;
using Android.OS;
#endif

namespace VMedic
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();
            DatosCompartidos.Lbl_UsuarioNombre = lblUserName;
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
                ExceptionMessageMaker.Make("Error PackageInfo", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
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
                    SincronizacionDataBase.EliminarTodo();

                    if (App.Current is not null)
                        App.Current.Windows[0].Page = new AppShell();
                }
                else
                {
                    PressedPreferences.EndPressed();
                }
            }
        }
    }
}
