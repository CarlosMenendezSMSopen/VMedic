using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Planificacion
{
    [AddINotifyPropertyChangedInterface]
    public partial class PlanificacionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private int _rowSpan;

        [ObservableProperty]
        private int _semanasVisibles;

        [ObservableProperty]
        private bool _visibilidadAdenda;

        private static readonly RestService servicio = new();
        private TablaUsuario? Usuario { get; set; } = App.Usuario?.GetItem();
        private ObservableRangeCollection<SchedulerAppointment> Tareas { get; set; } = [];
        private ObservableRangeCollection<SchedulerAppointment> TareasDía { get; set; } = [];
        private List<string> Dias { get; set; } = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
        //colores para mostrar la zona de calor desde el rojo hasta el violeta
        private List<Color> ColoresOrden { get; set; } =
            [
                Color.FromArgb("#442F11"),
                Color.FromArgb("#5F9EDB"),
                Color.FromArgb("#C38348"),
                Color.FromArgb("#0D5AA0"),
                Color.FromArgb("#C44A56"),
                Color.FromArgb("#B0D4B8"),
                Color.FromArgb("#EAE7D6"),
                Color.FromArgb("#A4C3A2"),
                Color.FromArgb("#5D7B6F"),
                Color.FromArgb("#F5D491"),
                Color.FromArgb("#F9B9B7"),
                Color.FromArgb("#F06C9B"),
                Color.FromArgb("#96C9DC"),
                Color.FromArgb("#61A0AF"),
                Color.FromArgb("#C0DBAA"),
                Color.FromArgb("#F0C0BF"),
                Color.FromArgb("#F1E6B8"),
                Color.FromArgb("#A1CAE1"),
            ];
        public PlanificacionViewModel()
        {
            ShowAgendaDia(false);
            ValidarSolicitudesPendientes();
        }

        public void ValidarSolicitudesPendientes()
        {
            if (DatosCompartidos.ContenedorCuentaPlanificacion is not null && DatosCompartidos.LabelContarPendientesPlanificacion is not null)
            {
                DatosCompartidos.ContenedorCuentaPlanificacion.IsVisible = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList()?.Count > 0;
                DatosCompartidos.LabelContarPendientesPlanificacion.Text = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList().Count.ToString();
            }
        }

        public void ShowAgendaDia(bool show)
        {
            RowSpan = show ? 1 : 2;
            SemanasVisibles = show ? 3 : 6;
            VisibilidadAdenda = show;
        }

        public async Task ObtenerPlanificaciones(SfScheduler calendario, ActivityIndicator status)
        {
            try
            {
                if (DatosCompartidos.CalendarioPlanificacion is not null)
                {
                    var data = await servicio.ResultadoGET<TablaAgenda>($"VMedicA047/'{Usuario?.UsuarioName}',{new DateTimeOffset(DatosCompartidos.CalendarioPlanificacion.DisplayDate.ToUniversalTime()).ToUnixTimeMilliseconds()}", null);
                    if (data is not null)
                    {
                        if (App.Agenda is not null)
                        {
                            App.Agenda.DeleteItems();
                            App.Agenda?.InsertItems(data);

                            App.Current?.Dispatcher.Dispatch(() =>
                            {
                                Tareas.Clear();
                                var ListaAgenda = App.Agenda?.GetItems();
                                if (ListaAgenda is not null)
                                    foreach (var tarea in ListaAgenda)
                                    {
                                        Tareas.Add(new SchedulerAppointment
                                        {
                                            StartTime = DateTime.ParseExact(tarea.FECHA_INICIAL + "", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                                            EndTime = DateTime.ParseExact(tarea.FECHA_FINAL + "", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                                            Subject = "Diario",
                                            Background = ColorOrden(tarea.FECHA_INICIAL?.Split(' ')[0], tarea.TableID),
                                        });
                                    }

                                calendario.AppointmentsSource = Tareas;
                                calendario.ResumeAppointmentViewUpdate();
                                status.IsRunning = false;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                App.Current?.Dispatcher.Dispatch(() =>
                {
                    calendario.ResumeAppointmentViewUpdate();
                    calendario.ShowBusyIndicator = false;
                });
            }
            finally
            {

            }
        }

        private Color ColorOrden(string? Fecha, int TableID)
        {
            if (App.Agenda is not null && Fecha is not null)
                if (!App.Agenda.IsEmpty())
                {
                    var TareasFecha = App.Agenda.GetItems()?.Where(A => A.FECHA_INICIAL is not null && A.FECHA_INICIAL.Contains(Fecha)).ToList();
                    var Agenda = App.Agenda.GetItems()?.FirstOrDefault(A => A.TableID == TableID);
                    if (Agenda is not null)
                    {
                        var i = TareasFecha?.FindIndex(X => X.TableID == Agenda.TableID);
                        if (i is not null)
                        {
                            return ColoresOrden[(int)i];
                        }
                    }
                }

            return Colors.Black;
        }

        public void VisualizarTareas(SfScheduler calendario, SfScheduler agendaTareas, DateTime newValue, ActivityIndicator status)
        {
            ShowAgendaDia(true);

            var FechaSeleccionada = newValue.Date.ToString("yyyyMMdd");
            var TareasFecha = App.Agenda?.GetItems()?.Where(A => A.FECHA_INICIAL is not null && A.FECHA_INICIAL.Contains(FechaSeleccionada)).ToList();
            agendaTareas.SuspendAppointmentViewUpdate();

            TareasDía.Clear();
            if (TareasFecha is not null)
                foreach (var tarea in TareasFecha)
                {
                    var DoctorFecha = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == tarea.CODIGO_DE_CLIENTE.ToString());

                    TareasDía.Add(new SchedulerAppointment
                    {
                        ClassId = tarea.CODIGO_CONTROL_VISITAS + "",
                        StartTime = DateTime.ParseExact(tarea.FECHA_INICIAL + "", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                        EndTime = DateTime.ParseExact(tarea.FECHA_FINAL + "", "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
                        Subject = $"{DoctorFecha?.CODIGO_DE_CLIENTE}\n{DoctorFecha?.NOMBRE_COMERCIAL}",
                        Background = ColorOrden(tarea.FECHA_INICIAL?.Split(' ')[0], tarea.TableID),
                        AutomationId = tarea.CODIGO_DE_PROGRAMACION + "",
                        StyleId = tarea.TIPO_DE_PROGRAMACION + "",
                    });
                }

            agendaTareas.AppointmentsSource = TareasDía;

            var fechaMasCercana = TareasDía.OrderBy(TD => Math.Abs((TD.StartTime - newValue).TotalSeconds)).FirstOrDefault()?.StartTime;
            if (fechaMasCercana is not null)
            {
                agendaTareas.DisplayDate = (DateTime)fechaMasCercana;
                agendaTareas.ResumeAppointmentViewUpdate();
                status.IsRunning = false;
            }
        }

        public async void CerrarVistaAgenda(SfScheduler calendario, SfScheduler agendaTareas, ActivityIndicator status)
        {
            ShowAgendaDia(false);
            calendario.SelectedDate = null;

            await Task.Delay(500);

            PressedPreferences.EndPressed();
        }

        public async void EliminarVisitaAgenda(SchedulerAppointment? SelectedAppointment)
        {
            if (SelectedAppointment is not null)
            {
                int NumberEliminar = 0;
                bool MensajeAlerta = false;
                var MainPage = App.Current?.Windows[0].Page;
                var ListaRepeticiones = new List<TablaAgenda>();

                if (MainPage is not null)
                {
                    var VisitaSeleccionada = App.Agenda?.GetItems()?.FirstOrDefault(A => A.CODIGO_CONTROL_VISITAS == int.Parse(SelectedAppointment.ClassId));

                    if (SelectedAppointment.AutomationId != "" && !SelectedAppointment.AutomationId.Contains("null"))
                    {
                        ListaRepeticiones = App.Agenda?.GetItems()?.Where(A => A.CODIGO_DE_PROGRAMACION is not null && A.CODIGO_DE_PROGRAMACION == int.Parse(SelectedAppointment.AutomationId) && A.CODIGO_DE_CLIENTE == int.Parse(SelectedAppointment.Subject.Split("\n")[0])).ToList();
                    }
                    else
                    {
                        ListaRepeticiones = App.Agenda?.GetItems()?.Where(A => A.FECHA_INICIAL is not null && A.FECHA_INICIAL.Contains(SelectedAppointment.StartTime.ToString("HH:mm:ss")) && A.FECHA_FINAL is not null && A.FECHA_FINAL.Contains(SelectedAppointment.EndTime.ToString("HH:mm:ss")) && A.CODIGO_DE_CLIENTE == int.Parse(SelectedAppointment.Subject.Split("\n")[0])).ToList();
                    }

                    //Repeticion diaria
                    if (ListaRepeticiones?.Count > 7 && ListaRepeticiones.Count <= 31)
                    {
                        var Opciones = await MainPage.DisplayActionSheet
                            (
                                "Esta instancia se programó con visitas diarias",
                                "CANCELAR",
                                null,
                                "1. Eliminar esta Instancia",
                                "2. Eliminar la Semana de esta Instancia",
                                $"3. Eliminar las Instancias de los {ObtenerPlanificaciones(VisitaSeleccionada?.DIA)}",
                                "4. Eliminar Todas las Instancias"
                            );

                        switch (Opciones.Split('.')[0])
                        {
                            case "1":
                                NumberEliminar = 1;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar la agenda de visita seleccionada?", "ELIMINAR", "CANCELAR");
                                break;
                            case "2":
                                NumberEliminar = 2;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar la semana agendada de la visita seleccionada?", "ELIMINAR", "CANCELAR");
                                break;
                            case "3":
                                NumberEliminar = 3;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", $"¿Está seguro/a de Eliminar de las visitas agendadas para los {ObtenerPlanificaciones(VisitaSeleccionada?.DIA)}?", "ELIMINAR", "CANCELAR");
                                break;
                            case "4":
                                NumberEliminar = 4;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", $"¿Está seguro/a de Eliminar toda la programación de visita agendada?", "ELIMINAR", "CANCELAR");
                                break;
                            default:
                                break;
                        }
                    }
                    //Repeticion semanal
                    else if (ListaRepeticiones?.Count <= 7 && ListaRepeticiones?.Count > 3)
                    {
                        var Opciones = await MainPage.DisplayActionSheet
                            (
                                "Esta instancia se programó con visitas semanales",
                                "CANCELAR",
                                null,
                                "1. Eliminar esta Instancia",
                                $"2. Eliminar las Instancias de los {ObtenerPlanificaciones(VisitaSeleccionada?.DIA)}",
                                "3. Eliminar Todas las Instancias"
                            );

                        switch (Opciones.Split('.')[0])
                        {
                            case "1":
                                NumberEliminar = 1;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar la agenda de Visita seleccionada?", "ELIMINAR", "CANCELAR");
                                break;
                            case "2":
                                NumberEliminar = 3;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", $"¿Está seguro/a de Eliminar las visitas agendadas para los {ObtenerPlanificaciones(VisitaSeleccionada?.DIA)}?", "ELIMINAR", "CANCELAR");
                                break;
                            case "3":
                                NumberEliminar = 4;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar toda la programación de visita agendada?", "ELIMINAR", "CANCELAR");
                                break;
                            default:
                                break;
                        }
                    }
                    //Repeticion Mensual
                    else if (ListaRepeticiones?.Count <= 3)
                    {
                        var Opciones = await MainPage.DisplayActionSheet
                            (
                                "Esta instancia se programó con visitas mensuales",
                                "CANCELAR",
                                null,
                                "1. Eliminar esta Instancia",
                                "2. Eliminar Todas las Instancias"
                            );

                        switch (Opciones.Split('.')[0])
                        {
                            case "1":
                                NumberEliminar = 1;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar la agenda de visita seleccionada?", "ELIMINAR", "CANCELAR");
                                break;
                            case "2":
                                NumberEliminar = 4;
                                MensajeAlerta = await MainPage.DisplayAlert("Eliminar Visita", "¿Está seguro/a de Eliminar toda la programación de visita agendada?", "ELIMINAR", "CANCELAR");
                                break;
                            default:
                                break;
                        }
                    }

                    var parametros = $"{SelectedAppointment.Subject.Split("\n")[0]},'{App.Usuario?.GetItem().UsuarioName}',{VisitaSeleccionada?.SEMANA},{VisitaSeleccionada?.DIA},{NumberEliminar},{SelectedAppointment.AutomationId},{SelectedAppointment.StyleId}";

                    if (MensajeAlerta)
                    {
                        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                        {
                            OperacionID = "VMedicA049",
                            Parametros = $"{SelectedAppointment.Subject.Split("\n")[0]},'{App.Usuario?.GetItem().UsuarioName}',{VisitaSeleccionada?.SEMANA},{VisitaSeleccionada?.DIA},{NumberEliminar}",
                            ClavesVacias = 0,
                            TipoRestService = 1,
                        };

                        if (IsInternet.Avilable())
                        {
                            var datos = (await servicio.ResultadoGET<Resultado>(SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                            if (datos is not null)
                            {
                                switch (datos.MSG)
                                {
                                    case "1":
                                        ShowAgendaDia(false);
                                        if (DatosCompartidos.CalendarioPlanificacion is not null)
                                            DatosCompartidos.CalendarioPlanificacion.SelectedDate = null;
                                        ToastMaker.Make("Visita Eliminada Correctamente", App.Current?.Windows[0].Page);
                                        break;
                                    case "2":
                                        ToastMaker.Make("Error: Medico no existente", App.Current?.Windows[0].Page);
                                        break;
                                    case "3":
                                        ToastMaker.Make("No es posible eliminar la agenda seleciconada porque es la configuración predeterminada", App.Current?.Windows[0].Page);
                                        break;
                                    case "4":
                                        ToastMaker.Make("Ha ocurrido un error inesperado", App.Current?.Windows[0].Page);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para sincronizar la eliminación de la visita", App.Current?.Windows[0].Page);
                            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                            await Task.Delay(1000);
                            ValidarSolicitudesPendientes();
                        }
                    }
                }

                PressedPreferences.EndPressed();
            }
        }

        private string ObtenerPlanificaciones(int? diaSeleccionado)
        {
            if (diaSeleccionado is not null)
            {
                return Dias[(int)diaSeleccionado - 1];
            }
            return "";
        }

        public async void EnviarSolicitudesPendientes()
        {
            var EstadoMensaje = 0;
            if (IsInternet.Avilable())
            {
                var SolicitudesEnviar = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList();
                if (SolicitudesEnviar is not null)
                    foreach (var SolicitudEnviar in SolicitudesEnviar)
                    {
                        var datos = (await servicio.ResultadoGET<Resultado>(SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                        if (datos is not null)
                        {
                            switch (datos.MSG)
                            {
                                case "1":
                                    App.SolicitudesPendientes?.DeleteItem(SolicitudEnviar);
                                    EstadoMensaje += 1;
                                    break;
                                case "2":
                                    EstadoMensaje += 0;
                                    break;
                                case "3":
                                    EstadoMensaje += 0;
                                    break;
                                case "4":
                                    EstadoMensaje += 0;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                if (EstadoMensaje == SolicitudesEnviar?.Count)
                {
                    ValidarSolicitudesPendientes();
                    ToastMaker.Make("Sincronización realizada Correctamente", App.Current?.Windows[0].Page);
                }
                else
                {
                    ToastMaker.Make("Ha ocurrido un error inesperado, vuelve a intentarlo más tarde", App.Current?.Windows[0].Page);
                }

            }
            else
            {
                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para sincronizar la visita agregada nuevamente", App.Current?.Windows[0].Page);
            }
        }
    }
}