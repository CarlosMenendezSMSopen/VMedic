using Mopups.Pages;
using Mopups.Services;
using System.Threading.Tasks;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Visitas;

public partial class PromocionalesView : PopupPage
{
    private RestService servicio = new RestService();
    private TablaVisitasPendientes? Visitas { get; set; }

    public PromocionalesView(TablaVisitasPendientes visitas, string? nivelPrecio)
	{
		InitializeComponent();
        SincronizacionDataBase.ObtenerMateriales();
        Visitas = visitas;
        MostrarDoctor();
		MostrarMateriales();
	}

    private void MostrarDoctor()
    {
        doctor_nombre.Text = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
    }

    private async void MostrarMateriales()
    {
        await Task.Delay(1000);
        SelectMateriales.ItemsSource = App.materiales?.GetItems()?.Select(M => M.NOMBRE_MATERIAL).ToList();
        SelectMateriales.SelectedItem = App.materiales?.GetItems()?.Select(M => M.NOMBRE_MATERIAL).FirstOrDefault();
    }

    private void Materiales_Tapped(object sender, TappedEventArgs e)
    {
        SelectMateriales.Unfocus();
        SelectMateriales.Focus();
    }

    private async void Enviar_material_Clicked(object sender, EventArgs e)
    {
        var idMaterial = App.materiales?.GetItems()?.Where(M => M.NOMBRE_MATERIAL == SelectMateriales.SelectedItem.ToString()).FirstOrDefault()?.CODIGO_MATERIAL;
        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
        {
            OperacionID = $"VMedicA043",
            Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{Visitas?.CodCliente}','{idMaterial}','','{Visitas?.IDTipoVisita}','{Visitas?.Comentarios}','{Visitas?.Latitud}','{Visitas?.Longitud}','{Visitas?.FechaGPS}'",
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
                        ToastMaker.Make("Entrega de Materiales realizada con éxito", App.Current?.MainPage);
                        var DoctorSeleciconado = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).FirstOrDefault();
                        if (DoctorSeleciconado is not null)
                        {
                            DoctorSeleciconado.Visitas = 1;
                            App.doctores?.UpdateITEM(DoctorSeleciconado);
                        }
                        break;
                    case "2":
                        ToastMaker.Make("Médico no existente, selecicone otro", App.Current?.MainPage);
                        break;
                    case "3":
                        ToastMaker.Make("No tiene permisos para el registro de materiales", App.Current?.MainPage);
                        break;
                    default:
                        ToastMaker.Make("Lo sentimos, ha ocurrido un error inesperado", App.Current?.MainPage);
                        break;
                }

                await MopupService.Instance.PopAllAsync();
            }
        }
        else
        {
            ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para enviar la entrega de materiales automáticamente", App.Current?.MainPage);
            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
        }
    }

    private async void Close_Clicked(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAllAsync();
    }
}