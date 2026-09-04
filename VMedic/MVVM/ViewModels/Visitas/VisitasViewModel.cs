using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.Interfaces;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.Views.Visitas;
using VMedic.Servicios;
using VMedic.Utilidades;
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
        private string? _avisoGPS = "";

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
        private bool _indicador;

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

        private readonly RestService servicio = new();
        private Timer? LocationTimer { get; set; }
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
            _indicador = true;
            _textoBoton = "Enviar";
            _ubicacionimg = "gps_off.png";
            _avisoGPS = "GPS No Disponible";
            _visibilidadNumeroSemana = true;
            _visibilidadDiaSemana = true;
            _visibilidadLugarVenta = false;
            _visibilidadOpciones = false;
            _visibilidadMedicos = true;
            _visibilidadMotivo = true;
            _visibilidadComentarios = true;

            GeolocationsPermissions();
            ConsultarSemanadeMes();
            ConsultarDiaSemana();
            MostrarTiposVisitas();
            MostrarLugaresdeVenta();
            MostrarMotivos();
            PressedPreferences.EndPressed();
        }

        //metodo que llena la lista desplegable con los lugares de venta en la visita de tipo Lugar
        private async void MostrarLugaresdeVenta()
        {
            var ListalugaresdeVenta = (await SincronizacionDataBase.ObtenerLugaresdeVentas())?.Select(lv => lv.DESCRIPCION).ToList();
            await Task.Delay(5);
            if (ListalugaresdeVenta is not null)
            {
                LugaresVenta = new ObservableRangeCollection<string?>(ListalugaresdeVenta);
                await Task.Delay(5);
                LugarVenta = LugaresVenta.FirstOrDefault();
            }
        }

        //metodo para obtener el id del lugar de evento seleccionado
        public void SeleccionarLugarID()
        {
            IDLugaresEventos = App.Lugaresventas?.GetItems()?.Where(LVE => LVE.DESCRIPCION == LugarVenta).FirstOrDefault()?.CODIGO_LUGAR;
        }

        //metodo que llena la lista desplegable con motivos por defecto cuando se selecciona el tipo de visita de lugar
        private async void MostrarMotivos()
        {
            Motivos =
            [
                "Establecimiento Cerrado", "No se dió atención", "Doctor no disponible", "Otro"
            ];

            await Task.Delay(5);

            Motivo = Motivos.FirstOrDefault();
        }

        //metodo para llenar la lista desplegable con los nombres y ids de los medicos
        public async void MostrarMedicos()
        {
            if (NumeroSemana is not null)
                PositionSemana = Semanas?.IndexOf(NumeroSemana) + 1;
            if (NombreDia is not null)
                PositionDia = DiasSemana?.IndexOf(NombreDia) + 1;

            if (PositionSemana is not null && PositionDia is not null)
            {
                var listavisitasMensuales = (await SincronizacionDataBase.ObtenerVisitasMensuales(null)).Where(VM =>
                {
                    if (PositionDia < 8 && PositionSemana < Semanas?.Count) //Numero día && Numero Semana
                    {
                        return VM.TIPO_CONTROL is not null && VM.FECHA is not null && VM.FECHAFINAL is not null && VM.ESTADO == 1 && VM.SEMANA == PositionSemana && VM.DIA == PositionDia;
                    }
                    else if (PositionDia == 8 && PositionSemana < Semanas?.Count)//Todos los días && Numero Semana 
                    {
                        return VM.TIPO_CONTROL is not null && VM.FECHA is not null && VM.FECHAFINAL is not null && VM.ESTADO == 1 && VM.SEMANA == PositionSemana;
                    }
                    else if (PositionDia < 8 && PositionSemana == Semanas?.Count)//Numero día && Todas las semanas
                    {
                        return VM.TIPO_CONTROL is not null && VM.FECHA is not null && VM.FECHAFINAL is not null && VM.ESTADO == 1 && VM.DIA == PositionDia;
                    }
                    else if (PositionDia == 8 && PositionSemana == Semanas?.Count)//Todos los días && Todas las semanas
                    {
                        return VM.TIPO_CONTROL is not null && VM.FECHA is not null && VM.FECHAFINAL is not null && VM.ESTADO == 1;
                    }

                    return true;
                }
                ).ToList();

                await Task.Delay(5);

                Medicos = new ObservableRangeCollection<dynamic>
                (
                    (from a in await SincronizacionDataBase.ObtenerDoctores()
                     join b in listavisitasMensuales on a.CODIGO_DE_CLIENTE equals (b.CODIGO_DE_CLIENTE + "")
                     select new
                     {
                         a.CODIGO_DE_CLIENTE,
                         a.NOMBRE_COMERCIAL,
                         a.LATITUD,
                         a.LONGITUD,
                         b.HORA_LLEGADA,
                         b.FECHA,
                         b.FECHAFINAL
                     }
                     ).Select(m => new
                     {
                         Medico = m.CODIGO_DE_CLIENTE + " - " + m.NOMBRE_COMERCIAL,
                         CodigoMedico = m.CODIGO_DE_CLIENTE,
                         ColorEstado = m.HORA_LLEGADA is not null ? Colors.Green : m.HORA_LLEGADA is null && DateTime.Parse(m.FECHAFINAL.Replace("T", " ")) < DateTime.Today ? Colors.Black : Colors.Red,
                         Latitud = m.LATITUD,
                         Longitud = m.LONGITUD,
                     }).DistinctBy(m => m.CodigoMedico).ToList()
                );

                await Task.Delay(5);

                if (Medicos is not null)
                {
                    if (Medicos.Count > 0)
                    {
                        Medico = Medicos.FirstOrDefault();
                    }
                    else
                    {
                        MedicoSeleciconado = "No hay medicos disponibles";
                    }
                }

                await Task.Delay(10);

                Indicador = false;
            }
        }

        //metodo paraa solicitar permisos de localización del usuario
        private async void GeolocationsPermissions()
        {
            await Task.Delay(1000);

            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                App.Current?.Windows[0].Page?.DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación.", "OK");
                return;
            }
            else
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));
                    LocalizacionUsuario = await Geolocation.Default.GetLocationAsync(request);

                    if (LocalizacionUsuario != null)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            Ubicacionimg = "gps_off.png";
                            AvisoGPS = "GPS No Disponible";
                            await Task.Delay(100);
                            Ubicacionimg = "gps_on.png";
                            AvisoGPS = "GPS Disponible";
                        });

                        FechaGPS = LocalizacionUsuario.Timestamp.LocalDateTime.ToString("yyyyMMdd HH:mm:ss");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("localizacionjdjhdhd" + ex);
                }
            }
        }

        //metodo que llena la lista desplegable de los tipos de visita
        private async void MostrarTiposVisitas()
        {
            var ListaTiposVisitas = (await SincronizacionDataBase.ObtenerTiposVisitas())?.OrderBy(tv => tv.CODIGO_TIPO_VISITA).Select(tv => tv.DESCRIPCION).ToList();
            await Task.Delay(5);
            if (ListaTiposVisitas is not null)
            {
                TiposVisitas = new ObservableRangeCollection<string?>(ListaTiposVisitas);
                await Task.Delay(5);
                TipoVisita = TiposVisitas.FirstOrDefault();
            }
        }

        //metodo que llena la lista desplegable del día de semana correspondiente a la fecha actual
        private async void ConsultarDiaSemana()
        {
            DiasSemana = [];

            var dias = new string[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "TODOS" };

            foreach (var dia in dias)
            {
                DiasSemana.Add(dia);
            }

            var diaHoy = (int)DateTime.Today.DayOfWeek + 1;
            int indexSeleccionado;

            if (diaHoy > 0)
                indexSeleccionado = diaHoy - 1;
            else
                indexSeleccionado = 6;

            await Task.Delay(50);

            NombreDia = DiasSemana?[indexSeleccionado];
        }

        //metodo que llena la lista desplegable de la semana correspondiente a la fecha actual
        private async void ConsultarSemanadeMes()
        {
            Semanas = [];

            if (Semanas is not null)
            {
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var today = DateTime.Today;

                var primerDiaMes = new DateTime(today.Year, today.Month, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
                var semanaActualMes = ((today.Day + (int)primerDiaMes.DayOfWeek - 1) / 7) + 1;

                var regla = CalendarWeekRule.FirstFourDayWeek;
                var primerDiaSemana = DayOfWeek.Sunday;

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

                await Task.Delay(50);

                NumeroSemana = Semanas?[semanaActualMes - 1];
            }
        }

        //metodo que cambia la visibilidad de los apartados del formulario de visitas segun el tipo de visita
        public void ChangeTipoVisitas()
        {
            IDTiposVisitas = App.Tiposvisitas?.GetItems()?.Where(tv => tv.DESCRIPCION == TipoVisita).FirstOrDefault()?.CODIGO_TIPO_VISITA;
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
                    TextoBoton = "Enviar Registro de Visita";
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
                    VisibilidadNumeroSemana = false;
                    VisibilidadDiaSemana = false;
                    VisibilidadLugarVenta = true;
                    VisibilidadOpciones = true;
                    VisibilidadMedicos = false;
                    VisibilidadMotivo = false;
                    VisibilidadComentarios = true;
                    TextoBoton = "Enviar Registro de Visita";
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
                    TextoBoton = "Enviar Registro de Visita";
                    break;
                default:
                    break;
            }
        }

        //metodo para enviar las visitas, y segun su tipo, enviara una solicitud de api rest o navegara hacia otra pantalla
        public async void EnviarVisitas()
        {
            try
            {
                if (FechaGPS is not null)
                {
                    if ((MedicoSeleciconado != "No hay medicos disponibles" || IDTiposVisitas == "7") && Medico is not null)
                    {
                        var Distancia = CalcularDistancia(Medico.Latitud, Medico.Longitud, LocalizacionUsuario?.Latitude, LocalizacionUsuario?.Longitude) * 1000;

                        int Actualizar_ubicacion = 0;

                        if (Distancia > 100 && IDTiposVisitas != "7")
                        {
                            var MainPage = App.Current?.Windows[0].Page;

                            if (MainPage is not null)
                            {
                                var Opciones = await MainPage.DisplayAlert("Advertencia", "La ubicación del médico no se encuentra en el área en que intenta registrar la visita, ¿Desea actualizar la ubicación del médico?", "Sí", "No");

                                if (Opciones)
                                {
                                    Actualizar_ubicacion = 1;
                                }
                                else
                                {
                                    Actualizar_ubicacion = -1;
                                }
                            }
                        }
                        else
                        {
                            Actualizar_ubicacion = 0;
                        }

                        if (Actualizar_ubicacion > -1)
                        {
                            var visitas = new TablaVisitasPendientes
                            {
                                CodCliente = Medico?.CodigoMedico,
                                CodLugar = IDLugaresEventos,
                                IDTipoVisita = IDTiposVisitas,
                                Comentarios = IDTiposVisitas == "2" ? Motivo + " " + Comentarios : Comentarios,
                                CodVendedor = App.Usuario?.GetItem().UsuarioName,
                                FechaGPS = FechaGPS,
                                Latitud = LocalizacionUsuario?.Latitude,
                                Longitud = LocalizacionUsuario?.Longitude
                            };

                            if (Preferences.Default.ContainsKey("ModeTipoVisitas"))
                            {
                                Preferences.Default.Remove("ModeTipoVisitas");
                            }
                            Preferences.Default.Set("ModeTipoVisitas", 1);

                            var count = 0;
                            if (App.Usuario?.GetItem().UbicacionRequerida == 1)
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
                                    ToastMaker.Make("Espere mientras el GPS obtiene su ubicación", App.Current?.Windows[0].Page);
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
                                        await Shell.Current.Navigation.PushAsync(new EvaluacionesView(visitas, Actualizar_ubicacion));
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (IDTiposVisitas == "2" && Motivo == "Otro" && Comentarios == "")
                                {
                                    ToastMaker.Make("Digite un comentario por favor", App.Current?.Windows[0].Page);
                                    return;
                                }
                                else if (IDTiposVisitas == "5")
                                {
                                    await MopupService.Instance.PushAsync(new PromocionalesView(visitas, Actualizar_ubicacion));
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
                                    await Shell.Current.Navigation.PushAsync(new EvaluacionesView(visitas, Actualizar_ubicacion));
                                    return;
                                }
                            }

                            //1 = visitas ------- 2 = visitas ENTRADA Y SALIDA
                            EnviarDatos(visitas, Actualizar_ubicacion);
                        }
                    }
                    else
                    {
                        ToastMaker.Make("Debe seleccionar una semana y un día en el que hayan médicos disponibles", App.Current?.Windows[0].Page);
                    }
                }
                else
                {
                    ToastMaker.Make("La fecha actual no se a capturado correctamente, intente de nuevo más tarde", App.Current?.Windows[0].Page);
                }
            }
            catch (Exception ex)
            {
                ExceptionMessageMaker.Make("Error al enviar las visitas", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
            }
        }

        //metodo para enviar datos por api rest si el tipo de visita corresponde al ID de 
        private async void EnviarDatos(TablaVisitasPendientes visitas, int actualizar_ubicacion)
        {
            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
            {
                IDSolicitud = Preferences.Default.Get("ModeTipoVisitas", -1) == 1 ?
                                    App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA017").ToList().Count
                                    : App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA038").ToList().Count,
                OperacionID = Preferences.Default.Get("ModeTipoVisitas", -1) == 1 ?
                                    $"VMedicA017" //insertar visitas no efectivas u otras visitas
                                    : $"VMedicA038",//insertar visitas ENTRADA SALIDA
                Parametros = Preferences.Default.Get("ModeTipoVisitas", -1) == 1 ?
                                    $"'{visitas.CodVendedor}','{visitas.CodCliente}','{IDTiposVisitas}','{visitas.Comentarios}','{visitas.FechaGPS}','{visitas.Longitud}','{visitas.Latitud}','{Medico?.Latitud}','{Medico?.Longitud}',{actualizar_ubicacion}"
                                    : $"'{visitas.CodVendedor}','{visitas.CodLugar}','{IDTiposVisitas}','{visitas.Comentarios}','{visitas.FechaGPS}','{visitas.Longitud}','{visitas.Latitud}'",
                ClavesVacias = 0,
                TipoRestService = 1,
                CodigoCliente = visitas.CodCliente,
                ModuloSolicitud = 1
            };

            var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"{SolicitudEnviar.OperacionID}/{SolicitudEnviar.Parametros}", null))?.FirstOrDefault();
            if (datos is not null)
            {
                switch (datos.MSG)
                {
                    case "1":
                        ToastMaker.Make("Datos enviados con éxito", App.Current?.Windows[0].Page);
                        break;
                    case "2":
                        ToastMaker.Make("Médico no existente, enviar datos de médico primero", App.Current?.Windows[0].Page);
                        break;
                    case "3":
                        ToastMaker.Make("No tiene permisos para el registro de visitas", App.Current?.Windows[0].Page);
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
                    App.Current?.Windows[0].Page?.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");
                    App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                    DatosCompartidos.CantidadIntentos = 0;
                }
                else
                {
                    ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, App.Current?.Windows[0].Page);
                }
            }
        }

        public static double CalcularDistancia(double LatA, double LonA, double? LatB, double? LonB)
        {
            if (LatB is not null && LonB is not null)
            {
                const double radioTierraKm = 6371.0;

                double lat1Rad = LatA * Math.PI / 180.0;
                double lat2Rad = (double)LatB * Math.PI / 180.0;

                double deltaLat = ((double)LatB - LatA) * Math.PI / 180.0;
                double deltaLon = ((double)LonB - LonA) * Math.PI / 180.0;

                double a =
                    Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1Rad) *
                    Math.Cos(lat2Rad) *
                    Math.Sin(deltaLon / 2) *
                    Math.Sin(deltaLon / 2);

                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                return radioTierraKm * c;
            }

            return 0.0;
        }
    }
}