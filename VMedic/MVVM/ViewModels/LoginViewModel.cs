using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.Interfaces;
using VMedic.MVVM.Models.WebServices;
using VMedic.Utilidades;
using VMedic.Servicios;

using VMedic.Behaviors;
using VMedic.MVVM.Views;
using VMedic.MVVM.Views.Visitas;

#if IOS
using Foundation;
#endif
#if ANDROID
using Android.Content.PM;
#endif

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

        private readonly RestService servicio = new();
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
                ExceptionMessageMaker.Make("Error PackageInfo", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
            }

            _version = packageinfo?.VersionName;
#endif
            if (App.Usuario is not null)
                if (App.Usuario.IsEmpty())
                {
                    MopupService.Instance.PushAsync(new ConfiguracionView());
                }
                else if (App.Usuario.GetItem().CodVendedor > -1 && App.Usuario.GetItem().Remember is 1)
                {
                    _userName = App.Usuario.GetItem().UsuarioName;
                    _password = App.Usuario.GetItem().UsuarioName;
                    _guardar = App.Usuario.GetItem().Remember is 1;
                }
        }

        public async void Login()
        {
            if (PressedPreferences.ValidatePressing())
            {
                PressedPreferences.Pressing(null);

                try
                {
                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    {
                        if (App.Usuario is not null)
                            if (!App.Usuario.IsEmpty())
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
                                            ToastMaker.Make("Usuario o contraseña incorrectas", App.Current?.Windows[0].Page);
                                            Indicador = false;
                                        }
                                        else
                                        {
                                            var tablaUsuario = App.Usuario.GetItem();

                                            tablaUsuario.CodVendedor = Sesion.CODIGO_VENDEDOR;
                                            tablaUsuario.UsuarioName = UserName;
                                            tablaUsuario.Contraseña = Password;
                                            tablaUsuario.Remember = Guardar ? 1 : 0;
                                            tablaUsuario.UbicacionRequerida = Sesion.UBICACION_REQUERIDA;
                                            tablaUsuario.CodPortafolio = Sesion.CODIGO_PORTAFOLIO;

                                            if (DatosCompartidos.Lbl_UsuarioNombre is not null)
                                            {
                                                DatosCompartidos.Lbl_UsuarioNombre.Text = UserName;
                                            }

                                            App.Usuario.UpdateITEM(tablaUsuario);

                                            SincronizacionDataBase.SincronizarTodo();
                                            await Shell.Current.GoToAsync(new ShellNavigationState($"//{nameof(VisitasView)}"));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Indicador = false;
                                ToastMaker.Make("No se ha configurado su activación de licencia", App.Current?.Windows[0].Page);
                            }
                    }
                    else
                    {
                        Indicador = false;
                        ToastMaker.Make("Los campos de usuario y contraseña no deben de estar vacíos", App.Current?.Windows[0].Page);
                    }
                }
                catch (Exception ex)
                {
                    Indicador = false;
                    PressedPreferences.EndPressed();
                    ExceptionMessageMaker.Make("Error boton de Inicio de Sesión", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                }
                finally
                {
                    //Indicador = false;
                    PressedPreferences.EndPressed();
                }
            }
        }
    }
}
