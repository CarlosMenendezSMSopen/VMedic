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
    private string? NiveldePrecio { get; set; }

    public PromocionalesView(TablaVisitasPendientes visitas, string NivelPrecio)
	{
		InitializeComponent();
        SincronizacionDataBase.ObtenerMateriales();
        Visitas = visitas;
        NiveldePrecio = NivelPrecio;
        MostrarDoctor();
		MostrarMateriales();
	}

    private void MostrarDoctor()
    {
        doctor_nombre.Text = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
    }

    private async void MostrarMateriales()
    {
        await Task.Delay(1000);
        SelectMateriales.ItemsSource = App.Materiales?.GetItems()?.Select(M => M.NOMBRE_MATERIAL).ToList();
        SelectMateriales.SelectedItem = App.Materiales?.GetItems()?.Select(M => M.NOMBRE_MATERIAL).FirstOrDefault();
    }

    private void Materiales_Tapped(object sender, TappedEventArgs e)
    {
        SelectMateriales.Unfocus();
        SelectMateriales.Focus();
    }

    private async void Enviar_material_Clicked(object sender, EventArgs e)
    {
        var idMaterial = App.Materiales?.GetItems()?.Where(M => M.NOMBRE_MATERIAL == SelectMateriales.SelectedItem.ToString()).FirstOrDefault()?.CODIGO_MATERIAL;
        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
        {
            OperacionID = $"VMedicA043",
            Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{Visitas?.CodCliente}','{idMaterial}','','{Visitas?.IDTipoVisita}','{Visitas?.Comentarios}','{Visitas?.Latitud}','{Visitas?.Longitud}','{Visitas?.FechaGPS}'",
            ClavesVacias = 0,
            TipoRestService = 1,
            CodigoCliente = Visitas?.CodCliente,
        };
        
        if (IsInternet.Avilable())
        {
            var datos = (await servicio.ResultadoGET<Resultado>($"{SolicitudEnviar.OperacionID}/{SolicitudEnviar.Parametros}", null))?.FirstOrDefault();
            if (datos is not null)
            {
                switch (datos.MSG)
                {
                    case "1":
                        ToastMaker.Make("Entrega de Materiales realizada con éxito", App.Current?.Windows[0].Page);
                        var DoctorSeleciconado = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).FirstOrDefault();
                        if (DoctorSeleciconado is not null)
                        {
                            DoctorSeleciconado.Visitas = 1;
                            App.Doctores?.UpdateITEM(DoctorSeleciconado);
                        }
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

                await MopupService.Instance.PopAllAsync();
            }
        }
        else
        {
            App.Current?.Dispatcher.Dispatch(delegate
            {
                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para enviar la entrega de materiales pendiente", App.Current?.Windows[0].Page);
            });

            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
            if (DatosCompartidos.Lbl_CatntidadPendientes_Visitas is not null)
            {
                DatosCompartidos.Lbl_CatntidadPendientes_Visitas.Text = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDVisitas.Contains(SP.OperacionID)).ToList().Count.ToString();
            }
        }
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAllAsync();
    }
}