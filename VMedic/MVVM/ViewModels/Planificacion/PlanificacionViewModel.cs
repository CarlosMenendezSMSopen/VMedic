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
        private int _semanasVisibles = 6;

        [ObservableProperty]
        private bool _visibilidadAdenda;

        [ObservableProperty]
        private bool _indicador;

        private static readonly RestService servicio = new();
        private TablaUsuario? Usuario { get; set; } = App.Usuario?.GetItem();
        private ObservableRangeCollection<SchedulerAppointment> Tareas { get; set; } = [];
        private ObservableRangeCollection<SchedulerAppointment> TareasDía { get; set; } = [];
        private List<string> Dias { get; set; } = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];
        //colores para mostrar la zona de calor desde el rojo hasta el violeta
        private List<Color> ColoresOrden { get; set; } =
            [
                Color.FromArgb("#5F9EDB"),
                Color.FromArgb("#5D7B6F"),
                Color.FromArgb("#C44A56"),
                Color.FromArgb("#C38348"),
            ];
        public PlanificacionViewModel()
        {
            ShowAgendaDia(false);
        }

        //metodo que cambia la visibilidad de la vista de agenda por día
        public void ShowAgendaDia(bool show)
        {
            RowSpan = show ? 1 : 2;
            //SemanasVisibles = show ? 6 : 6;
            VisibilidadAdenda = show;
        }

        //funcion tarea para obtener los registros de las fechas de visita de los médicos y asignarlos en los recursos de la agenda principal
        public void ObtenerPlanificaciones(SfScheduler calendario, SfScheduler agendaTareas)
        {
            Task.Run(async () =>
            {
                try
                {
                    var data = await servicio.ResultadoGET<TablaAgenda>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA047/'{Usuario?.UsuarioName}',{new DateTimeOffset(calendario.DisplayDate.ToUniversalTime()).ToUnixTimeMilliseconds()},{calendario.DisplayDate.Month}", null);
                    if (data is not null)
                    {
                        if (App.Agenda is not null)
                        {
                            App.Agenda.DeleteItems();
                            App.Agenda?.InsertItems(data);

                            var ListaTareas = new List<SchedulerAppointment>();

                            foreach (var tarea in data.ToList())
                            {
                                ListaTareas.Add(new SchedulerAppointment
                                {
                                    ClassId = tarea.CodigoControlVisita + "",
                                    StartTime = DateTime.ParseExact(tarea.Fecha?.Replace("T", " ") + "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                    EndTime = DateTime.ParseExact(tarea.Fecha?.Replace("T", " ") + "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                    Subject = $"{tarea.CodigoCliente}\n{tarea.Cliente}",
                                    Background = tarea.HoraLlegada is not null ? Color.FromArgb("#4CAF50") : tarea.FechaFinal is not null && DateTime.Parse(tarea.FechaFinal) < DateTime.Today ? Color.FromArgb("#F44336") : Color.FromArgb("#2196F3"),
                                    AutomationId = tarea.CodigoControlVisita + "",
                                    StyleId = tarea.TipoControl + "",
                                    TextColor = tarea.HoraLlegada is not null ? Color.FromArgb("#4CAF50") : tarea.FechaFinal is not null && DateTime.Parse(tarea.FechaFinal) < DateTime.Today ? Color.FromArgb("#F44336") : Color.FromArgb("#2196F3"),
                                });
                            }

                            App.Current?.Dispatcher.Dispatch(() =>
                            {
                                Indicador = true;
                                Tareas = new ObservableRangeCollection<SchedulerAppointment>(ListaTareas);

                                calendario.AppointmentsSource = Tareas;
                                calendario.ResumeAppointmentViewUpdate();

                                VisualizarTareas(calendario, agendaTareas, new DateTimeOffset(calendario.DisplayDate.ToUniversalTime()).ToUnixTimeMilliseconds());
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    App.Current?.Dispatcher.Dispatch(() =>
                    {
                        ExceptionMessageMaker.Make("Calendario error", ex.ToString(), ex.Message, App.Current.Windows[0].Page);
                        calendario.ResumeAppointmentViewUpdate();
                        Indicador = false;
                    });
                }
                finally
                {

                }
            });
        }

        //metodo que asigna las visitas en la agenda de vista de día
        public async void VisualizarTareas(SfScheduler calendario, SfScheduler agendaTareas, long v)
        {
            try
            {
                var data = await servicio.ResultadoGET<TablaAgenda>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA047/'{Usuario?.UsuarioName}',{v}", null);
                agendaTareas.SuspendAppointmentViewUpdate();

                var ListaTareas = new List<SchedulerAppointment>();
                TareasDía.Clear();

                if (data is not null)
                {
                    foreach (var tarea in data)
                    {
                        if (tarea.Fecha is not null && tarea.FechaFinal is not null)
                        {
                            var colorVerde = Color.FromArgb("#994CAF50");
                            var colorRojo = Color.FromArgb("#99F44336");
                            var colorAzul = Color.FromArgb("#992196F3");

                            ListaTareas.Add(new SchedulerAppointment
                            {
                                ClassId = tarea.CodigoControlVisita + "",
                                StartTime = DateTime.ParseExact(tarea.Fecha?.Replace("T", " ") + "", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                EndTime = DateTime.ParseExact(tarea.Fecha?.Split("T")[0] + " " + tarea.FechaFinal.Split("T")[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                Subject = $"{tarea.CodigoCliente}\n{tarea.Cliente}",
                                Background = tarea.HoraLlegada is not null ? colorVerde : tarea.FechaFinal is not null && DateTime.Parse(tarea.FechaFinal) < DateTime.Today ? colorRojo : colorAzul,
                                AutomationId = tarea.CodigoControlVisita + "",
                                StyleId = tarea.TipoControl + "",
                                TextColor = tarea.HoraLlegada is not null ? colorVerde : tarea.FechaFinal is not null && DateTime.Parse(tarea.FechaFinal) < DateTime.Today ? colorRojo : colorAzul,
                            });
                        }
                    }

                    TareasDía = new ObservableRangeCollection<SchedulerAppointment>(ListaTareas);

                    agendaTareas.AppointmentsSource = TareasDía;
                    agendaTareas.ResumeAppointmentViewUpdate();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                App.Current?.Dispatcher.Dispatch(() =>
                {
                    ExceptionMessageMaker.Make("Habilitar Día Calendario error", ex.ToString(), ex.Message, App.Current.Windows[0].Page);
                    calendario.ResumeAppointmentViewUpdate();
                });
            }
            finally
            {
                Indicador = false;
            }
        }

        //metodo que elimina una programnación de visita, solicitando a la vez cual es la que se desea eliminar
        public async void EliminarVisitaAgenda(SchedulerAppointment? SelectedAppointment, Views.Planificacion.PlanificacionView planificacionView)
        {
            if (SelectedAppointment is not null)
            {
                var MainPage = App.Current?.Windows[0].Page;

                if (MainPage is not null)
                {
                    var VisitaSeleccionada = App.Agenda?.GetItems()?.FirstOrDefault(A => A.CodigoControlVisita == int.Parse(SelectedAppointment.ClassId));

                    if (VisitaSeleccionada is not null)
                    {
                        if (VisitaSeleccionada.HoraLlegada is null)
                        {
                            var Opciones = await MainPage.DisplayAlert
                                (
                                    $"{VisitaSeleccionada.TipoControl switch { 1 => "PROGRAMACIÓN DE UNA SOLA VISITA", 2 => "PROGRAMACIÓN DE DÍAS HÁBILES", 3 => "PROGRAMACIÓN DE UNA VEZ A LA SEMANA", 4 => "PROGRAMACIÓN DE UNA VEZ AL MES", _ => "" }}",
                                    $"La instancia de\n{SelectedAppointment.Subject.Replace('\n', ' ')}\nse deshabilitará de la programación de visitas",
                                    "ELIMINAR",
                                    "CANCELAR"
                                );

                            if (Opciones)
                            {
                                var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                                {
                                    IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA049").ToList().Count,
                                    OperacionID = "VMedicA049",
                                    Parametros = $"{SelectedAppointment.Subject.Split("\n")[0]},'{App.Usuario?.GetItem().UsuarioName}',{VisitaSeleccionada?.CodigoControlVisita}",
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
                                            ToastMaker.Make("Visita Eliminada Correctamente", App.Current?.Windows[0].Page);

                                            planificacionView.Btn_actualizar_Clicked(null, null);
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
                                else if (DatosCompartidos.ErrorResponseValue is not null)
                                {
                                    DatosCompartidos.CantidadIntentos++;

                                    if (DatosCompartidos.CantidadIntentos == 4)
                                    {
                                        var MyMainPage = App.Current?.Windows[0].Page;

                                        if (MyMainPage is not null)
                                            await MyMainPage.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");
                                        
                                        App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                                        DatosCompartidos.CantidadIntentos = 0;
                                    }
                                    else
                                    {
                                        ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, App.Current?.Windows[0].Page);
                                    }
                                }
                            }
                        }
                        else
                        {
                            await MainPage.DisplayAlert
                                (
                                    "Visita Completada", $"La Visita seleccionada se efectuó el día {VisitaSeleccionada.HoraLlegada.Split("T")[0]} a las {VisitaSeleccionada.HoraLlegada.Split("T")[1]} horas, por lo que no se puede inhabilitar de la programación de visitas", "CANCELAR"
                                );
                        }
                    }
                }

                PressedPreferences.EndPressed();
            }
        }

        //funcion string que obtiene el nombre del día en base al numero de dia seleciconado de la agenda
        private string ObtenerPlanificaciones(int? diaSeleccionado)
        {
            if (diaSeleccionado is not null)
            {
                return Dias[(int)diaSeleccionado - 1];
            }
            return "";
        }

        //metodo que envía las solicitudes pendientes cuando se hace clic en el botón de actualizar
        public async void EnviarSolicitudesPendientes()
        {
            var EstadoMensaje = 0;
            if (IsInternet.Avilable())
            {
                var SolicitudesEnviar = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList();
                if (SolicitudesEnviar is not null)
                    foreach (var SolicitudEnviar in SolicitudesEnviar)
                    {
                        var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
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