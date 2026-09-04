using Microsoft.Maui.Storage;
using Mopups.Pages;
using Mopups.Services;
using System.Threading.Tasks;
using VMedic.MVVM.Models.WebServices;
using VMedic.Servicios;
using VMedic.MVVM.Models.DataBase;
using VMedic.Utilidades;
using System.Diagnostics;

#if IOS
using UIKit;
using Foundation;
#endif
#if ANDROID
using Android.Content.PM;
using Android.OS;
#endif

namespace VMedic.MVVM.Views;

public partial class ConfiguracionView : PopupPage
{
    private readonly RestService servicio = new();
    private string? IMEI { get; set; }
    public ConfiguracionView()
    {
        InitializeComponent();
        if (App.Usuario is not null)
            if (!App.Usuario.IsEmpty())
            {
                txt_usuario.Text = App.Usuario.GetItems()?.FirstOrDefault()?.LicenciaName;
                txt_contraseña.Text = App.Usuario.GetItems()?.FirstOrDefault()?.LicenciaPass;
            }
        ObtenerIMEI();
    }

    private void ObtenerIMEI()
    {
#if IOS
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                IMEI = UIDevice.CurrentDevice.IdentifierForVendor?.ToString();
            }
            else
            {
                Debug.WriteLine("IMEI is not avilable for this device");
                IMEI = "";
            }
#endif

#if ANDROID
        IMEI = Guid.NewGuid().ToString();
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
            ExceptionMessageMaker.Make("Error package info", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
        }
#endif
    }

    private async void Btn_cancelarLicencia_Clicked(object sender, EventArgs e)
    {
        if (App.Usuario is not null)
            if (App.Usuario.IsEmpty())
            {
                if (await Configura.DisplayAlert("SALIR", "¿Desea salir de configuración sin activar la licencia?", "SI", "NO"))
                {
                    await MopupService.Instance.PopAllAsync();
                }
            }
            else
            {
                if (await Configura.DisplayAlert("SALIR", "¿Desea salir de configuración sin guardar cambios?", "SI", "NO"))
                {
                    await MopupService.Instance.PopAllAsync();
                }
            }
    }

    private async void Btn_aceptarLicencia_Clicked(object sender, EventArgs e)
    {
        if (txt_contraseña.Text is not "" && txt_usuario.Text is not "")
        {
            if (txt_contraseña.Text is "")
            {
                ToastMaker.Make("No se ha digitado la contraseña", Configura);
                return;
            }

            if (txt_usuario.Text is "")
            {
                ToastMaker.Make("No se ha digitado el nombre de usuario", Configura);
                return;
            }

            var consulta = $"{nameof(KontrolA030)}/'{txt_usuario.Text}','{EncriptarSHA1.GetSHA1(txt_contraseña.Text)}','{IMEI}'";
            var licencia = await servicio.ResultadoGET<KontrolA030>((await URL.GetDomain()), consulta, null);
            if (licencia is not null)
            {
                var usuarioLicencia = licencia.ToList().FirstOrDefault();
                if (usuarioLicencia?.licenciaid is "-1")
                {
                    ToastMaker.Make("La licencia para el uso de este software ha expirado, favor ponerse en contacto con su proveedor", Configura);
                }
                else if (usuarioLicencia?.licenciaid is "-2")
                {
                    ToastMaker.Make("Usuario o contraseña de licencia no válidos, favor verifíque los datos e intente nuevamente", Configura);
                }
                else
                {
                    if (App.Usuario is not null)
                        if (App.Usuario.IsEmpty())
                        {
                            var tablaUsuario = new TablaUsuario
                            {
                                CodVendedor = -1,
                                UsuarioName = "",
                                Contraseña = "",
                                Remember = -1,
                                UbicacionRequerida = -1,
                                CodPortafolio = -1,
                                IDLicencia = usuarioLicencia?.licenciaid,
                                DominioIP = "https://" + usuarioLicencia?.URL?.Split('/')?[2] + "/",
                                LicenciaName = txt_usuario.Text,
                                LicenciaPass = txt_contraseña.Text,
                                Logo = usuarioLicencia?.LOGO,
                                CompanyID = usuarioLicencia?.COMPANYID,
                            };

                            App.Usuario.InsertItem(tablaUsuario);
                            ToastMaker.Make("Configuracion realizada con éxito", App.Current?.Windows[0].Page);
                        }
                        else
                        {
                            var tablaUsuario = App.Usuario.GetItem();
                            var URLS = usuarioLicencia?.URL?.Split('/');

                            tablaUsuario.IDLicencia = usuarioLicencia?.licenciaid;
                            tablaUsuario.DominioIP = "https://" + URLS?[2] + "/";
                            tablaUsuario.LicenciaName = txt_usuario.Text;
                            tablaUsuario.LicenciaPass = txt_contraseña.Text;
                            tablaUsuario.Logo = usuarioLicencia?.LOGO;
                            tablaUsuario.CompanyID = usuarioLicencia?.COMPANYID;

                            App.Usuario.UpdateITEM(tablaUsuario);
                            ToastMaker.Make("Configuracion realizada con éxito", App.Current?.Windows[0].Page);
                        }

                    await MopupService.Instance.PopAllAsync();
                }
            }
        }
        else
        {
            ToastMaker.Make("No se ha digitado el usuario", Configura);
        }
    }

    private async void Configuracion_BackgroundClicked(object sender, EventArgs e)
    {
        if (App.Usuario is not null)
            if (App.Usuario.IsEmpty())
            {
                if (await Configura.DisplayAlert("SALIR", "¿Desea salir de configuración sin activar la licencia?", "SI", "NO"))
                {
                    await MopupService.Instance.PopAllAsync();
                }
            }
            else
            {
                if (await Configura.DisplayAlert("SALIR", "¿Desea salir de configuración sin guardar cambios?", "SI", "NO"))
                {
                    await MopupService.Instance.PopAllAsync();
                }
            }
    }
}