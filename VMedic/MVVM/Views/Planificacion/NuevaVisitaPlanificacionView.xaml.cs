using Mopups.Pages;
using Mopups.Services;
using MvvmHelpers;
using System.Diagnostics;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.ViewModels.Planificacion;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Planificacion;

public partial class NuevaVisitaPlanificacionView : PopupPage
{
    private ObservableRangeCollection<dynamic>? Medicos { get; set; }
    private ObservableRangeCollection<string> Repetir { get; set; } = ["Diariamente", "Semanalmente", "Mensualmente"];

    private readonly RestService servicio = new();
    private PlanificacionView? PlanificacionContext { get; set; }
    public NuevaVisitaPlanificacionView(PlanificacionView bindingContext)
    {
        InitializeComponent();
        MostrarMedicos();
        MostrarPeriodoRepeticion();
        PressedPreferences.EndPressed();
        PlanificacionContext = bindingContext;
    }

    private void MostrarPeriodoRepeticion()
    {
        searchbox_repetir.ItemsSource = Repetir;
        searchbox_repetir.SelectedItem = Repetir.LastOrDefault();
    }

    public async void MostrarMedicos()
    {
        SincronizacionDataBase.ObtenerDoctores();

        var listaMedicos = App.Doctores?.GetItems()?.ToList();

        if (listaMedicos is not null)
        {
            if (listaMedicos.Count > 0)
            {
                Medicos = new ObservableRangeCollection<dynamic>(listaMedicos.Select(m => new
                {
                    Medico = m.CODIGO_DE_CLIENTE + " - " + m.NOMBRE_COMERCIAL,
                    CodigoMedico = m.CODIGO_DE_CLIENTE,
                    Negocio = m.GIRO_DE_NEGOCIO,
                    NivelPrecio = m.NIVEL_PRECIO,
                    IVA = m.PRECIOS_CON_IVA,
                    Visita = m.Visitas,
                }));

                searchbox_medicos.ItemsSource = Medicos;

                await Task.Delay(1000);

                searchbox_medicos.SelectedItem = Medicos.FirstOrDefault();
            }
            else
            {
                searchbox_medicos.Text = "No hay medicos disponibles";
            }
        }
    }

    private void btn_Cancelar_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAllAsync();
    }

    private void switch_TodoDia_Tapped(object sender, TappedEventArgs e)
    {
        switch_TodoDia.IsToggled = !switch_TodoDia.IsToggled;

        if (switch_TodoDia.IsToggled)
        {
            timer_FechaInicial.Time = new TimeSpan(8, 0, 0);
            timer_FechaFinal.Time = new TimeSpan(18, 0, 0);
        }
        else
        {
            timer_FechaInicial.Time = new TimeSpan(8, 0, 0);
            timer_FechaFinal.Time = new TimeSpan(9, 0, 0);
        }
    }

    private void swicth_repetir_Toggled(object sender, ToggledEventArgs e)
    {
        searchbox_repetir.IsEnabled = e.Value;
    }

    private void timer_FechaInicial_TimeSelected(object sender, TimeChangedEventArgs e)
    {
        switch_TodoDia.IsToggled = (timer_FechaFinal.Time.Hours - timer_FechaInicial.Time.Hours) >= 8;
        if (timer_FechaFinal.Time.Hours - timer_FechaInicial.Time.Hours <= 1)
        {
            timer_FechaFinal.Time = new TimeSpan(e.NewTime.Hours + 1, e.NewTime.Minutes, e.NewTime.Seconds);
        }
    }

    private void timer_FechaFinal_TimeSelected(object sender, TimeChangedEventArgs e)
    {
        switch_TodoDia.IsToggled = (timer_FechaFinal.Time.Hours - timer_FechaInicial.Time.Hours) >= 8;
    }

    private async void btn_AgregarVisita_Clicked(object sender, EventArgs e)
    {
        try
        {
            var FechaInicial = DateTime.Today.Add(timer_FechaInicial.Time).ToString("yyyyMMdd HH:mm:ss");
            var FechaFinal = DateTime.Today.Add(timer_FechaFinal.Time).ToString("yyyyMMdd HH:mm:ss");

            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
            {
                OperacionID = "VMedicA048",
                Parametros = $"'{App.Usuario?.GetItem().UsuarioName}',{(searchbox_medicos.SelectedItem as dynamic)?.CodigoMedico},'{FechaInicial}','{FechaFinal}',10,{(swicth_repetir.IsToggled ? searchbox_repetir.SelectedIndex + 1 : 0)}",
                ClavesVacias = 0,
                TipoRestService = 1,
            };

            await MopupService.Instance.PopAllAsync();

            if (IsInternet.Avilable())
            {
                var datos = (await servicio.ResultadoGET<Resultado>(SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                if (datos is not null)
                {
                    switch (datos.MSG)
                    {
                        case "1":
                            PlanificacionContext?.btn_actualizar_Clicked(null, null);
                            ToastMaker.Make("Visita registrada con éxito", App.Current?.Windows[0].Page);
                            break;
                        case "2":
                            ToastMaker.Make("Error: Cliente no existente, inténtelo de nuevo", App.Current?.Windows[0].Page);
                            break;
                        case "3":
                            ToastMaker.Make("Error: no tiene permisos para agregar visitas", App.Current?.Windows[0].Page);
                            break;
                        case "4":
                            ToastMaker.Make("Ha ocurrido un error inesperado!", App.Current?.Windows[0].Page);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                await MopupService.Instance.PopAllAsync();
                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para sincronizar la visita agregada", App.Current?.Windows[0].Page);
                App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);

                await Task.Delay(1000);

                if (DatosCompartidos.ContenedorCuentaPlanificacion is not null && DatosCompartidos.LabelContarPendientesPlanificacion is not null)
                {
                    DatosCompartidos.ContenedorCuentaPlanificacion.IsVisible = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList()?.Count > 0;
                    DatosCompartidos.LabelContarPendientesPlanificacion.Text = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList().Count.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error Agregar Visita: " + ex);
        }
        finally
        {
            
        }
    }
}