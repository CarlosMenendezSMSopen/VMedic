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
    private ObservableRangeCollection<string> Repetir { get; set; } = ["Una Sola Visita", "Varios Días", "Una Vez a la Semana", "Una Vez al Mes"];
    private readonly RestService servicio = new();
    private PlanificacionView? PlanificacionContext { get; set; }
    private string? ClassFecha { get; set; }
    private List<TablaSemanasDias>? ListavisitasMensuales { get; set; }
    public NuevaVisitaPlanificacionView(PlanificacionView bindingContext)
    {
        InitializeComponent();
        MostrarMedicos();
        MostrarPeriodoRepeticion();

        time_fechaInicial.Time = DateTime.Now.TimeOfDay;
        time_fechaFinal.Time = DateTime.Now.AddHours(1).TimeOfDay;

        PressedPreferences.EndPressed();
        PlanificacionContext = bindingContext;
    }

    private void MostrarPeriodoRepeticion()
    {
        searchbox_repetir.ItemsSource = Repetir;
        searchbox_repetir.SelectedItem = Repetir.FirstOrDefault();
    }

    public async void MostrarMedicos()
    {
        var listaMedicos = await SincronizacionDataBase.ObtenerDoctores();

        await Task.Delay(1000);

        if (listaMedicos is not null)
        {
            if (listaMedicos.Count > 0)
            {
                Medicos = new ObservableRangeCollection<dynamic>(listaMedicos.Select(m => new
                {
                    Medico = m.CODIGO_DE_CLIENTE + " - " + m.NOMBRE_COMERCIAL,
                    CodigoMedico = m.CODIGO_DE_CLIENTE
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

    private void Btn_Cancelar_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAllAsync();
    }

    private void Swicth_repetir_Toggled(object sender, ToggledEventArgs e)
    {
        searchbox_repetir.IsEnabled = e.Value;
    }

    private async void Btn_AgregarVisita_Clicked(object sender, EventArgs e)
    {
        try
        {
            var FechaInicial = date_FechaInicial.Date.ToString("yyyyMMdd");
            var FechaFinal = date_FechaFinal.Date.ToString("yyyyMMdd");

            var HoraInicial = time_fechaInicial.Time.ToString(@"hh\:mm\:ss");
            var HoraFinal = time_fechaFinal.Time.ToString(@"hh\:mm\:ss");

            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
            {
                IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA054").ToList().Count,
                OperacionID = "VMedicA054",
                Parametros = $"'{App.Usuario?.GetItem().UsuarioName}',{(searchbox_medicos.SelectedItem as dynamic)?.CodigoMedico},'{FechaInicial} {HoraInicial}','{FechaFinal} {HoraFinal}',{searchbox_repetir.SelectedIndex + 1}",
                ClavesVacias = 0,
                TipoRestService = 1,
                ModuloSolicitud = 3
            };

            var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
            if (datos is not null)
            {
                switch (datos.MSG)
                {
                    case "1":
                        await MopupService.Instance.PopAllAsync();

                        PlanificacionContext?.Btn_actualizar_Clicked(null, null);
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
            else if (DatosCompartidos.ErrorResponseValue is not null)
            {
                DatosCompartidos.CantidadIntentos++;

                if (DatosCompartidos.CantidadIntentos == 4)
                {
                    await NuevaVisitaPlanificacion.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");
                    App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);

                    await MopupService.Instance.PopAllAsync();

                    DatosCompartidos.CantidadIntentos = 0;
                }
                else
                {
                    ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, App.Current?.Windows[0].Page);
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error Agregar Visita: " + ex);
        }
    }

    private async void Searchbox_medicos_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        try
        {
            if (searchbox_medicos.Text != "")
            {
                await Task.Delay(250);
                CerrarTeclado.Close();
                searchbox_medicos.Unfocus();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void Searchbox_repetir_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if (searchbox_repetir.SelectedIndex == 0)
        {
            date_FechaFinal.Date = date_FechaInicial.Date;
        }
        else
        {
            date_FechaFinal.Date = date_FechaInicial.Date.AddDays(1);
        }

        await Task.Delay(1000);
        searchbox_repetir.Unfocus();
    }

    private void Date_FechaInicial_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate >= DateTime.Today)
        {
            if (searchbox_repetir.SelectedIndex == 0)
            {
                date_FechaFinal.Date = e.NewDate;
            }
            else
            {
                date_FechaFinal.Date = e.NewDate.AddDays(1);
            }
        }
        else
        {
            date_FechaInicial.Date = DateTime.Today;
            ToastMaker.Make("No es posibble planificar en fechas pasadas", NuevaVisitaPlanificacion);
        }
    }

    private void Date_FechaFinal_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate < DateTime.Today && e.NewDate < date_FechaInicial.Date)
        {
            date_FechaFinal.Date = date_FechaInicial.Date.AddDays(1);
            ToastMaker.Make("No es posible planificar en fechas previas a la inicial", NuevaVisitaPlanificacion);
        }

        if (searchbox_repetir.SelectedIndex == 2)
        {
            var FechaMasSemaanaa = date_FechaInicial.Date.AddDays(7);

            if (e.NewDate >= FechaMasSemaanaa)
            {
                date_FechaFinal.Date = FechaMasSemaanaa.AddDays(-1);
                ToastMaker.Make("No se debe planificar un período mayor a 7 días para repetir semanalmente", NuevaVisitaPlanificacion);
            }
        }

        if (searchbox_repetir.SelectedIndex == 3)
        {
            var FechaMasMes = date_FechaInicial.Date.AddMonths(1);

            if (e.NewDate >= FechaMasMes)
            {
                date_FechaFinal.Date = FechaMasMes.AddDays(-1);
                ToastMaker.Make("No se debe planificar un período mayor a un mes para repetir mensualmente", NuevaVisitaPlanificacion);
            }
        }
    }

    private void Time_fechaInicial_TimeSelected(object sender, TimeChangedEventArgs e)
    {
        time_fechaFinal.Time = e.NewTime.Add(TimeSpan.FromHours(1));
    }

    private void Rime_fechaFinal_TimeSelected(object sender, TimeChangedEventArgs e)
    {
        if (e.NewTime.Hours - time_fechaInicial.Time.Hours < 1)
        {
            time_fechaFinal.Time = e.NewTime.Add(TimeSpan.FromHours(1));
            ToastMaker.Make("No es posible planificar una visita menor a una hora", NuevaVisitaPlanificacion);
        }
    }
}