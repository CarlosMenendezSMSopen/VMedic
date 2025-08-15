using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMedic.Behaviors;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.Views.Visitas;
using VMedic.Services;
using VMedic.Utilities;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;
using Timer = System.Timers.Timer;

namespace VMedic.MVVM.ViewModels.Visitas
{
    [AddINotifyPropertyChangedInterface]
    public partial class VisitasViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableRangeCollection<string>? _semanas;

        [ObservableProperty]
        private string? _numeroSemana = "";

        [ObservableProperty]
        private ObservableRangeCollection<string>? _diasSemana;

        [ObservableProperty]
        private string? _nombreDia = "";

        [ObservableProperty]
        private ObservableRangeCollection<string?>? _tiposVisitas;

        [ObservableProperty]
        private string? _tipoVisita = "";

        [ObservableProperty]
        private ObservableRangeCollection<dynamic>? _medicos;

        [ObservableProperty]
        private string? _ubicacionimg = "";

        [ObservableProperty]
        private ObservableRangeCollection<string>? _motivos;

        [ObservableProperty]
        private string? _motivo = "";

        [ObservableProperty]
        private ObservableRangeCollection<string?>? _lugaresVenta;

        [ObservableProperty]
        private string? _lugarVenta = "";

        [ObservableProperty]
        private string? _comentarios = "";

        [ObservableProperty]
        private string? _medicoSeleciconado = "";
        
        [ObservableProperty]
        private string? _textoBoton = "";

        [ObservableProperty]
        private bool _visibilidadNumeroSemana;

        [ObservableProperty]
        private bool _visibilidadDiaSemana;

        [ObservableProperty]
        private bool _visibilidadLugarVenta;

        [ObservableProperty]
        private bool _visibilidadOpciones;

        [ObservableProperty]
        private bool _visibilidadMedicos;

        [ObservableProperty]
        private bool _visibilidadMotivo;

        [ObservableProperty]
        private bool _visibilidadComentarios;

        [ObservableProperty]
        private bool _entradaOp;

        [ObservableProperty]
        private bool _salidaOp;

        private RestService servicio = new RestService();
        private Timer? locationTimer { get; set; }
        private string? IDTiposVisitas { get; set; }
        private string? IDLugaresEventos { get; set; }
        private int? PositionDia { get; set; }
        private int? PositionSemana { get; set; }
        public dynamic? Medico { get; set; }
        private bool IsLocationImportant { get; set; }
        private Location? LocalizacionUsuario { get; set; }
        private string? FechaGPS { get; set; }
        public VisitasViewModel()
        {
            _textoBoton = "Enviar";
            _ubicacionimg = "gps_off.png";
            SincronizacionDataBase.ObtenerVisitasMensuales();
            SincronizacionDataBase.ObtenerDoctores();
            SincronizacionDataBase.ObtenerTiposVisitas();
            ConsultarSemanadeMes();
            ConsultarDiaSemana();
            MostrarTiposVisitas();
            GeolocationsPermissions();
            MostrarMotivos();
        }

        private async void MostrarLugaresdeVenta()
        {
            var ListalugaresdeVenta = App.lugaresventas?.GetItems()?.Select(lv => lv.DESCRIPCION).ToList();
            if (ListalugaresdeVenta is not null)
            {
                LugaresVenta = new ObservableRangeCollection<string?>(ListalugaresdeVenta);
                await Task.Delay(1000);
                LugarVenta = LugaresVenta.FirstOrDefault();
            }
        }

        public void SeleccionarLugarID()
        {
            IDLugaresEventos = App.lugaresventas?.GetItems()?.Where(LVE => LVE.DESCRIPCION == LugarVenta).FirstOrDefault()?.CODIGO_LUGAR;
        }

        private async void MostrarMotivos()
        {
            Motivos = new ObservableRangeCollection<string>
            {
                "Establecimiento Cerrado", "No se dió atención", "Doctor no disponible", "Otro"
            };

            await Task.Delay(1000);

            Motivo = Motivos.FirstOrDefault();
        }

        public async void MostrarMedicos()
        {
            if (NumeroSemana is not null)
                PositionSemana = Semanas?.IndexOf(NumeroSemana) + 1;
            if (NombreDia is not null)
                PositionDia = DiasSemana?.IndexOf(NombreDia) + 1;

            if (PositionSemana is not null && PositionDia is not null)
            {
                var listavisitasMensuales = App.visitasmensuales?.GetItems()?.Where(lvm => lvm.SEMANA == PositionSemana && lvm.DIA == PositionDia).ToList();
                var listaMedicos = App.doctores?.GetItems()?.Where(D => listavisitasMensuales is not null ? listavisitasMensuales.Any(LVM => LVM.CODIGO_DE_CLIENTE == (D.CODIGO_DE_CLIENTE is not null ? int.Parse(D.CODIGO_DE_CLIENTE) : 0)) : D is not null).ToList();

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

                        await Task.Delay(1000);

                        Medico = Medicos.FirstOrDefault();
                    }
                    else
                    {
                        MedicoSeleciconado = "No hay medicos disponibles";
                    }
                }
            }
        }

        private async void GeolocationsPermissions()
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                App.Current?.MainPage?.DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación.", "OK");
                return;
            }
            else
            {
                locationTimer = new Timer(5000); // cada 5 segundos
                locationTimer.Elapsed += async (s, e) =>
                {
                    LocalizacionUsuario = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));

                    if (LocalizacionUsuario != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            Ubicacionimg = "gps_off.png";
                            await Task.Delay(100);
                            Ubicacionimg = "gps_on.png";
                        });

                        FechaGPS = LocalizacionUsuario.Timestamp.LocalDateTime.ToString("yyyyMMdd HH:mm:ss");
                    }
                };
                locationTimer.AutoReset = true;
                locationTimer.Enabled = true;
            }
        }

        private async void MostrarTiposVisitas()
        {
            await Task.Delay(1000);
            var ListaTiposVisitas = App.tiposvisitas?.GetItems()?.OrderBy(tv => tv.CODIGO_TIPO_VISITA).Select(tv => tv.DESCRIPCION).ToList();
            if (ListaTiposVisitas is not null)
            {
                TiposVisitas = new ObservableRangeCollection<string?>(ListaTiposVisitas);
                await Task.Delay(1000);
                TipoVisita = TiposVisitas.FirstOrDefault();
            }
        }

        private async void ConsultarDiaSemana()
        {
            DiasSemana = new ObservableRangeCollection<string>();

            var dias = new string[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo", "TODOS" };

            foreach (var dia in dias)
            {
                DiasSemana.Add(dia);
            }

            var diaHoy = (int)DateTime.Today.DayOfWeek;
            int indexSeleccionado;

            if (diaHoy > 0)
                indexSeleccionado = diaHoy - 1;
            else
                indexSeleccionado = 6;

            await Task.Delay(1000);

            NombreDia = DiasSemana?[indexSeleccionado];
        }

        private async void ConsultarSemanadeMes()
        {
            Semanas = new ObservableRangeCollection<string>();

            if (Semanas is not null)
            {
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var today = DateTime.Today;

                var primerDiaMes = new DateTime(today.Year, today.Month, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
                var semanaActualMes = ((today.Day + (int)primerDiaMes.DayOfWeek - 1) / 7) + 1;

                var regla = CalendarWeekRule.FirstFourDayWeek;
                var primerDiaSemana = DayOfWeek.Monday;

                var semanaInicio = calendar.GetWeekOfYear(primerDiaMes, regla, primerDiaSemana);
                var semanaFin = calendar.GetWeekOfYear(ultimoDiaMes, regla, primerDiaSemana);

                int Cantidadsemanas = semanaFin - semanaInicio + 1;

                if (semanaFin < semanaInicio)
                {
                    int semanasEnAño = calendar.GetWeekOfYear(
                        new DateTime(today.Year, 12, 31), regla, primerDiaSemana);
                    Cantidadsemanas = (semanasEnAño - semanaInicio + 1) + semanaFin;
                }

                for (var i = 1; i <= Cantidadsemanas; i++)
                {
                    Semanas?.Add($"Semana {i}");
                }
                Semanas?.Add("TODOS");

                await Task.Delay(1000);

                NumeroSemana = Semanas?[semanaActualMes - 1];
            }
        }

        public void ChangeTipoVisitas()
        {
            IDTiposVisitas = App.tiposvisitas?.GetItems()?.Where(tv => tv.DESCRIPCION == TipoVisita).FirstOrDefault()?.CODIGO_TIPO_VISITA;
            switch (IDTiposVisitas)
            {
                case "1":
                    break;
                case "2":
                    VisibilidadNumeroSemana = true;
                    VisibilidadDiaSemana = true;
                    VisibilidadLugarVenta = false;
                    VisibilidadOpciones = false;
                    VisibilidadMedicos = true;
                    VisibilidadMotivo = true;
                    VisibilidadComentarios = true;
                    TextoBoton = "Enviar";
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "5":
                    VisibilidadNumeroSemana = true;
                    VisibilidadDiaSemana = true;
                    VisibilidadLugarVenta = false;
                    VisibilidadOpciones = false;
                    VisibilidadMedicos = true;
                    VisibilidadMotivo = false;
                    VisibilidadComentarios = true;
                    TextoBoton = "Siguiente";
                    break;
                case "6":
                    break;
                case "7":
                    SincronizacionDataBase.ObtenerLugaresdeVentas();
                    VisibilidadNumeroSemana = false;
                    VisibilidadDiaSemana = false;
                    VisibilidadLugarVenta = true;
                    VisibilidadOpciones = true;
                    VisibilidadMedicos = false;
                    VisibilidadMotivo = false;
                    VisibilidadComentarios = true;
                    TextoBoton = "Enviar";
                    MostrarLugaresdeVenta();
                    EntradaOp = true;
                    break;
                case "8":
                    VisibilidadNumeroSemana = true;
                    VisibilidadDiaSemana = true;
                    VisibilidadLugarVenta = false;
                    VisibilidadOpciones = false;
                    VisibilidadMedicos = true;
                    VisibilidadMotivo = false;
                    VisibilidadComentarios = true;
                    TextoBoton = "Siguiente";
                    break;
                case "9":
                    VisibilidadNumeroSemana = true;
                    VisibilidadDiaSemana = true;
                    VisibilidadLugarVenta = false;
                    VisibilidadOpciones = false;
                    VisibilidadMedicos = true;
                    VisibilidadMotivo = false;
                    VisibilidadComentarios = true;
                    TextoBoton = "Enviar";
                    break;
                default:
                    break;
            }
        }

        public async void EnviarVisitas()
        {
            try
            {
                if (MedicoSeleciconado != "No hay medicos disponibles" || IDTiposVisitas == "7")
                {
                    var visitas = new TablaVisitasPendientes
                    {
                        CodCliente = Medico?.CodigoMedico,
                        CodLugar = IDLugaresEventos,
                        IDTipoVisita = IDTiposVisitas,
                        Comentarios = IDTiposVisitas == "8" || IDTiposVisitas == "5" ? Comentarios : Motivo + " " + Comentarios,
                        CodVendedor = App.usuario?.GetItem().UsuarioName,
                        FechaGPS = FechaGPS,
                        Latitud = LocalizacionUsuario?.Latitude,
                        Longitud = LocalizacionUsuario?.Longitude
                    };

                    var NivelPrecio = Medico?.NivelPrecio;

                    if (Preferences.Default.ContainsKey("ModeTipoVisitas"))
                    {
                        Preferences.Default.Remove("ModeTipoVisitas");
                    }
                    Preferences.Default.Set("ModeTipoVisitas", 1);

                    var count = 0;
                    if (App.usuario?.GetItem().UbicacionRequerida == 1)
                    {
                        if (count < 3)
                        {
                            IsLocationImportant = true;
                            count++;
                        }
                        else if (count > 2)
                        {
                            IsLocationImportant = false;
                            count = 0;
                        }
                    }
                    else
                    {
                        IsLocationImportant = false;
                    }

                    if (IsLocationImportant)
                    {
                        if (LocalizacionUsuario?.Longitude.ToString().Replace(",", ".") == "0"
                            || LocalizacionUsuario?.Latitude.ToString().Replace(",", ".") == "0"
                            || LocalizacionUsuario?.Longitude.ToString().Replace(",", ".") == ""
                            || LocalizacionUsuario?.Latitude.ToString().Replace(",", ".") == ""
                            )
                        {
                            ToastMaker.Make("Espere mientras el GPS obtiene su ubicación", App.Current?.MainPage);
                            return;
                        }
                        else
                        {
                            if (IDTiposVisitas == "7")
                            {
                                if (EntradaOp)
                                {
                                    visitas.Comentarios = "ENTRADA. " + Comentarios;
                                }

                                if (SalidaOp)
                                {
                                    visitas.Comentarios = "SALIDA. " + Comentarios;
                                }

                                if (Preferences.Default.ContainsKey("ModeTipoVisitas"))
                                {
                                    Preferences.Default.Remove("ModeTipoVisitas");
                                }
                                Preferences.Default.Set("ModeTipoVisitas", 2);
                            }
                            else if (IDTiposVisitas == "8")
                            {
                                await Shell.Current.Navigation.PushAsync(new EvaluacionesView(visitas, NivelPrecio));
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (IDTiposVisitas == "2" && Motivo == "Otro" && Comentarios == "")
                        {
                            ToastMaker.Make("Digite un comentario por favor", App.Current?.MainPage);
                            return;
                        }
                        else if (IDTiposVisitas == "5" && IsInternet.Avilable())
                        {
                            await MopupService.Instance.PushAsync(new PromocionalesView(visitas, NivelPrecio));
                            return;
                        }
                        else if (IDTiposVisitas == "7")
                        {
                            if (EntradaOp)
                            {
                                visitas.Comentarios = "ENTRADA. " + Comentarios;
                            }

                            if (SalidaOp)
                            {
                                visitas.Comentarios = "SALIDA. " + Comentarios;
                            }

                            if (Preferences.Default.ContainsKey("ModeTipoVisitas"))
                            {
                                Preferences.Default.Remove("ModeTipoVisitas");
                            }
                            Preferences.Default.Set("ModeTipoVisitas", 2);
                        }
                        else if (IDTiposVisitas == "8")
                        {
                            await Shell.Current.Navigation.PushAsync(new EvaluacionesView(visitas, NivelPrecio));
                            return;
                        }
                    }

                    //1 = visitas ------- 2 = visitas ENTRADA Y SALIDA
                    enviarDatos(visitas);
                }
                else
                {
                    ToastMaker.Make("Debe seleccionar una semana y un día en el que hayan médicos disponibles", App.Current?.MainPage);
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageMaker.Make("Error al enviar las visitas", ex.ToString(), ex.Message, App.Current?.MainPage);
            }
        }

        private async void enviarDatos(TablaVisitasPendientes visitas)
        {
            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
            {
                OperacionID = Preferences.Default.Get("ModeTipoVisitas", -1) == 1 ?
                                    $"VMedicA017" //insertar visitas
                                    : $"VMedicA038",//insertar visitas ENTRADA SALIDA
                Parametros = Preferences.Default.Get("ModeTipoVisitas", -1) == 1 ? 
                                    $"'{visitas.CodVendedor}','{visitas.CodCliente}','{IDTiposVisitas}','{visitas.Comentarios}','{visitas.FechaGPS}','{visitas.Longitud}','{visitas.Latitud}'" 
                                    : $"'{visitas.CodVendedor}','{visitas.CodLugar}','{IDTiposVisitas}','{visitas.Comentarios}','{visitas.FechaGPS}','{visitas.Longitud}','{visitas.Latitud}'",
                ClavesVacias = 0,
                TipoRestService = 1,
                CodigoCliente = visitas.CodCliente,
            };

            if (IsInternet.Avilable())
            {
                var datos = (await servicio.ResultadoGET<Resultado>($"{SolicitudEnviar.OperacionID}/{SolicitudEnviar.Parametros}", null))?.FirstOrDefault();
                if (datos is not null)
                {
                    switch (datos.MSG)
                    {
                        case "1":
                            ToastMaker.Make("Datos enviados con éxito", App.Current?.MainPage);
                            var DoctorSeleciconado = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == visitas.CodCliente).FirstOrDefault();
                            if (DoctorSeleciconado is not null)
                            {
                                DoctorSeleciconado.Visitas = 1;
                                App.doctores?.UpdateITEM(DoctorSeleciconado);
                            }
                            break;
                        case "2":
                            ToastMaker.Make("Médico no existente, enviar datos de médico primero", App.Current?.MainPage);
                            break;
                        case "3":
                            ToastMaker.Make("No tiene permisos para el registro de visitas", App.Current?.MainPage);
                            break;
                        default:
                            ToastMaker.Make("Lo sentimos, ha ocurrido un error inesperado", App.Current?.MainPage);
                            break;
                    }
                }
            }
            else
            {
                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para enviar la visita automáticamente", App.Current?.MainPage);
                App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
            }
        }
    }
}
