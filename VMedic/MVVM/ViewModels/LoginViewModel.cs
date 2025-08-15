using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using PropertyChanged;
using System.Threading.Tasks;
using VMedic.MVVM.Views;

#if IOS
using Foundation;
#endif
#if ANDROID
using Android.Content.PM;
#endif

using Microsoft.Toolkit.Mvvm.Input;
using VMedic.MVVM.Models.WebserviceModels;
using VMedic.Services;
using VMedic.MVVM.Views.Visitas;
using VMedic.Behaviors;
using VMedic.Utilities;

namespace VMedic.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? _version = "";

        [ObservableProperty]
        private string? _userName = "";

        [ObservableProperty]
        private string? _password = "";

        [ObservableProperty]
        private bool _guardar;

        [ObservableProperty]
        private bool _indicador;

        private RestService servicio = new RestService();
        public LoginViewModel()
        {
            _indicador = false;
#if IOS
            var versionName = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];

            _version = versionName.ToString();
#endif

#if ANDROID
            PackageManager? packagemanager = Android.App.Application.Context.PackageManager;
            string? packagename = Android.App.Application.Context.PackageName;
            PackageInfo? packageinfo = null;
            try
            {
                if (packagename != null)
                    packageinfo = packagemanager?.GetPackageInfo(packagename, 0);
            }
            catch (Exception ex)
            {
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error PackageInfo", ex.ToString(), ex.Message, App.Current?.MainPage);
            }

            _version = packageinfo?.VersionName;
#endif
            if (App.usuario is not null)
                if (App.usuario.IsEmpty())
                {
                    MopupService.Instance.PushAsync(new ConfiguracionView());
                }
                else if (App.usuario.GetItem().CodVendedor > -1 && App.usuario.GetItem().Remember is 1)
                {
                    _userName = App.usuario.GetItem().UsuarioName;
                    _password = App.usuario.GetItem().UsuarioName;
                    _guardar = App.usuario.GetItem().Remember is 1;
                }
        }

        [ICommand]
        public async void Login()
        {
            try
            {
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    if (App.usuario is not null)
                        if (!App.usuario.IsEmpty())
                        {
                            Indicador = true;
                            var consulta = $"{nameof(VMedicA001)}/'{UserName}','{Password}'";
                            var login = await servicio.ResultadoGET<VMedicA001>(consulta, null);
                            if (login is not null)
                            {
                                var Sesion = login.ToList().FirstOrDefault();
                                if (Sesion is not null)
                                {
                                    if (Sesion.CODIGO_VENDEDOR is -1)
                                    {
                                        ToastMaker.Make("Usuario o contraseña incorrectas", App.Current?.MainPage);
                                        Indicador = false;
                                    }
                                    else
                                    {
                                        var tablaUsuario = App.usuario.GetItem();

                                        tablaUsuario.CodVendedor = Sesion.CODIGO_VENDEDOR;
                                        tablaUsuario.UsuarioName = UserName;
                                        tablaUsuario.Contraseña = Password;
                                        tablaUsuario.Remember = Guardar ? 1 : 0;
                                        tablaUsuario.UbicacionRequerida = Sesion.UBICACION_REQUERIDA;
                                        tablaUsuario.CodPortafolio = Sesion.CODIGO_PORTAFOLIO;


                                        App.usuario.UpdateITEM(tablaUsuario);

                                        SincronizacionDataBase.SincronizarTodo();
                                        await Shell.Current.GoToAsync(new ShellNavigationState($"//{nameof(VisitasView)}"));
                                    }
                                }
                            }
                        }
                        else
                        {
                            Indicador = false;
                            ToastMaker.Make("No se ha configurado su activación de licencia", App.Current?.MainPage);
                        }
                }
                else
                {
                    Indicador = false;
                    ToastMaker.Make("Los campos de usuario y contraseña no deben de estar vacíos", App.Current?.MainPage);
                }
            }
            catch (Exception ex)
            {
                Indicador = false;
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error boton de Inicio de Sesión", ex.ToString(), ex.Message, App.Current?.MainPage);
            }
            finally
            {
                Indicador = false;
            }
        }
    }
}
