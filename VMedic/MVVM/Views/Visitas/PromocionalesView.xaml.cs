using Mopups.Pages;
using Mopups.Services;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class PromocionalesView : PopupPage
{
    private readonly RestService servicio = new();
    private TablaVisitasPendientes? Visitas { get; set; }
    private TablaDoctores? Medico { get; set; }
    private int ActualizarUbicacion { get; set; }
    public PromocionalesView(TablaVisitasPendientes visitas, int actualizarUbicacion)
    {
        InitializeComponent();
        Visitas = visitas;
        ActualizarUbicacion = actualizarUbicacion;
        Medico = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente);
        MostrarDoctor();
        MostrarMateriales();
    }

    private void MostrarDoctor()
    {
        doctor_nombre.Text = Medico?.CODIGO_DE_CLIENTE + " - " + Medico?.NOMBRE_COMERCIAL;
    }

    private async void MostrarMateriales()
    {
        var listMateriales = await SincronizacionDataBase.ObtenerMateriales();
        await Task.Delay(250);
        if (listMateriales is not null)
        {
            searchbox_Materiales.ItemsSource = listMateriales.Select(M => M.NOMBRE_MATERIAL).ToList();
            searchbox_Materiales.SelectedItem = listMateriales?.Select(M => M.NOMBRE_MATERIAL).FirstOrDefault();
        }
    }

    private async void Enviar_material_Clicked(object sender, EventArgs e)
    {
        var idMaterial = App.Materiales?.GetItems()?.Where(M => M.NOMBRE_MATERIAL == searchbox_Materiales.SelectedItem?.ToString()).FirstOrDefault()?.CODIGO_MATERIAL;
        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
        {
            IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA043").ToList().Count,
            OperacionID = $"VMedicA043",
            Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{Visitas?.CodCliente}','{idMaterial}','','{Visitas?.IDTipoVisita}','{Visitas?.Comentarios}','{Visitas?.Latitud}','{Visitas?.Longitud}','{Visitas?.FechaGPS}','{Medico?.LATITUD}','{Medico?.LONGITUD}',{ActualizarUbicacion}",
            ClavesVacias = 0,
            TipoRestService = 1,
            CodigoCliente = Visitas?.CodCliente,
            ModuloSolicitud = 1
        };

        var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"{SolicitudEnviar.OperacionID}/{SolicitudEnviar.Parametros}", null))?.FirstOrDefault();
        if (datos is not null)
        {
            switch (datos.MSG)
            {
                case "1":
                    ToastMaker.Make("Entrega de Materiales realizada con éxito", App.Current?.Windows[0].Page);
                    await MopupService.Instance.PopAllAsync();
                    break;
                case "2":
                    ToastMaker.Make("Médico no existente, selecicone otro", App.Current?.Windows[0].Page);
                    break;
                case "3":
                    ToastMaker.Make("No tiene permisos para el registro de materiales", App.Current?.Windows[0].Page);
                    break;
                default:
                    ToastMaker.Make("Lo sentimos, ha ocurrido un error inesperado", App.Current?.Windows[0].Page);
                    break;
            }
        }
        else if (DatosCompartidos.ErrorResponseValue is not null)
        {
            DatosCompartidos.CantidadIntentos++;

            if (DatosCompartidos.CantidadIntentos == 4)
            {
                Promocionales?.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");
                await MopupService.Instance.PopAllAsync();
                App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                DatosCompartidos.CantidadIntentos = 0;
            }
            else
            {
                ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, Promocionales);
            }
        }
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAllAsync();
    }
}