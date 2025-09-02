using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMedic.Behaviors;
using VMedic.Converters;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Timer = System.Timers.Timer;

namespace VMedic.MVVM.ViewModels.Médicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class EditorMedicoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? _tituloPagina;

        [ObservableProperty]
        private bool _visibilidadInfo;

        [ObservableProperty]
        private bool _visibilidadAgregar;

        [ObservableProperty]
        private bool _visibilidadActualizar;

        [ObservableProperty]
        private bool _visibilidadNuevo;

        [ObservableProperty]
        private bool _checkedUbicacion;

        [ObservableProperty]
        private string? _medicoName = "";

        [ObservableProperty]
        private string? _medicoEspecialidad = "";

        [ObservableProperty]
        private string? _medicoContact = "";

        [ObservableProperty]
        private string? _medicoDireccion = "";
        
        [ObservableProperty]
        private string? _medicoColor = "";
        
        [ObservableProperty]
        private string? _medicoColorNombre = "";

        [ObservableProperty]
        private string? _medicoTelefono = "";
        
        [ObservableProperty]
        private string? _medicoCategoria = "";
        
        [ObservableProperty]
        private string? _medicoAdaptacion = "";

        [ObservableProperty]
        private string? _medicoMail = "";

        [ObservableProperty]
        private string? _medicoJVPM = "";

        [ObservableProperty]
        private string? _fechaVisita = "";

        [ObservableProperty]
        private ObservableRangeCollection<TablaClasesEspecializaciones>? _especialidades;

        [ObservableProperty]
        private TablaClasesEspecializaciones? _especialidad;

        [ObservableProperty]
        private ObservableRangeCollection<TablaCategoriasMedico>? _categorias;

        [ObservableProperty]
        private TablaCategoriasMedico? _categoria;

        [ObservableProperty]
        private ObservableRangeCollection<TablaProductoPreferencia>? _preferencias;

        [ObservableProperty]
        private string? _preferencia;

        [ObservableProperty]
        private ObservableRangeCollection<string>? _escalasAdaptacion = new ObservableRangeCollection<string> { "Dogmático", "Pragmático" };

        [ObservableProperty]
        private string? _adaptacion;

        [ObservableProperty]
        private bool _enableRepetir;

        [ObservableProperty]
        private bool _checkS1;

        [ObservableProperty]
        private bool _checkS2;

        [ObservableProperty]
        private bool _checkS3;

        [ObservableProperty]
        private bool _checkS4;

        [ObservableProperty]
        private bool _checkS5;

        [ObservableProperty]
        private bool _checkL;

        [ObservableProperty]
        private bool _checkM;

        [ObservableProperty]
        private bool _checkMi;

        [ObservableProperty]
        private bool _checkJ;

        [ObservableProperty]
        private bool _checkV;

        [ObservableProperty]
        private bool _checkS;

        [ObservableProperty]
        private bool _checkD;

        [ObservableProperty]
        private bool _medicoContactVisible;

        [ObservableProperty]
        private bool _medicoTelefonoVisible;

        [ObservableProperty]
        private bool _medicoCategoriaVisible;

        [ObservableProperty]
        private bool _medicoAdaptacionVisible;

        [ObservableProperty]
        private bool _medicoColorVisible;

        [ObservableProperty]
        private bool _medicoDireccionVisible;

        [ObservableProperty]
        private int _position;
        
        [ObservableProperty]
        private int _paddingImagen;

        private RestService servicio = new RestService();
        public string? ColorSeleccionado { get; set; } = "";
        public int ModeEditor { get; set; }
        private Location? LocalizacionUsuario { get; set; }
        public string? IdsPreferencias { get; set; } = "";
        public string? CodigoCliente { get; set; } = "";
        private TablaDoctores? Medico { get; set; }
        public EditorMedicoViewModel(int modoEditor, string? cODIGO_DE_CLIENTE)
        {
            _tituloPagina = modoEditor == 1 ? "Agregar Médico" : modoEditor == 2 ? "Información de Médico" : "Editar Médico";
            _visibilidadAgregar = modoEditor == 1 || modoEditor == 3;
            _visibilidadNuevo = modoEditor == 1;
            _visibilidadInfo = modoEditor == 2;
            _visibilidadActualizar = modoEditor == 3;
            ModeEditor = modoEditor;
            if (modoEditor == 1)
            {
                _enableRepetir = true;
                MostrarEspecialidad();
                MostrarCategoriasMedico();
                MostrarPreferenciasdeProducto();
                _adaptacion = _escalasAdaptacion?.FirstOrDefault();
                _fechaVisita = DateTime.Today.AddDays(1).ToString("ddd dd MMM yyyy", new CultureInfo("es-ES"));
            }
            else if (modoEditor == 2)
            {
                CodigoCliente = cODIGO_DE_CLIENTE;
                DatosCompartidos.MapaUbicaiconMedico = new Map();
                DatosCompartidos.MapaUbicaiconMedico.IsShowingUser = true;
                DatosCompartidos.MapaUbicaiconMedico.MapType = MapType.Street;
                _paddingImagen = DeviceInfo.Platform == DevicePlatform.Android ? 6 : 10;
                MostrarDetallesMedico();
            }
        }

        private void InsertarUbicacion()
        {
            if (Medico is not null)
            {
                var place = new Pin
                {
                    Label = "Medico",
                    Address = Medico.DIRECCION_CLIENTE + "",
                    Location = new Location(Medico.LATITUD, Medico.LONGITUD)
                };

                DatosCompartidos.MapaUbicaiconMedico?.Pins.Add(place);
                DatosCompartidos.MapaUbicaiconMedico?.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Medico.LATITUD, Medico.LONGITUD), Distance.FromMiles(0.3)));
            }
        }

        private async void MostrarDetallesMedico()
        {
            await Task.Delay(1000);
            Medico = App.doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == CodigoCliente);
            var especialidadMedico = App.especialidades?.GetItems()?.FirstOrDefault(E => E.CODIGO_DE_CLASE == Medico?.CODIGO_DE_CLASE);
            var categoriaMedico = App.categoriasmedico?.GetItems()?.FirstOrDefault(C => C.CATEGORIAID == Medico?.CATEGORIAID);
            MedicoName = Medico?.NOMBRE_COMERCIAL;
            MedicoEspecialidad = especialidadMedico?.DESCRIPCION_CLASE;
            MedicoContact = Medico?.CONTACTO_CLIENTE;
            MedicoContactVisible = !string.IsNullOrEmpty(MedicoContact);
            MedicoTelefono = Medico?.TELEFONO_CLIENTE;
            MedicoTelefonoVisible = !string.IsNullOrEmpty(MedicoTelefono);
            MedicoCategoria = categoriaMedico?.CATEGORIA;
            MedicoCategoriaVisible = !string.IsNullOrEmpty(MedicoCategoria);
            MedicoAdaptacion = Medico?.ESCALA_ADAPTACION;
            MedicoAdaptacionVisible = !string.IsNullOrEmpty(MedicoAdaptacion);
            MedicoColorNombre = Medico?.COLOR;
            MedicoColor = ObtenerColorMedico(Medico?.COLOR);
            MedicoColorVisible = !string.IsNullOrEmpty(Medico?.COLOR);
            MedicoDireccion = Medico?.DIRECCION_CLIENTE;
            MedicoDireccionVisible = !string.IsNullOrEmpty(MedicoDireccion);

            InsertarUbicacion();
        }

        private string? ObtenerColorMedico(string? cOLOR)
        {
            switch (cOLOR)
            {
                case "Rojo":
                    return Colors.Red.ToHex();
                case "Azul":
                    return Colors.Blue.ToHex();
                case "Amarillo":
                    return Colors.Yellow.ToHex();
                case "Verde":
                    return Colors.Green.ToHex();
                default:
                    return Colors.Black.ToHex();
            }
        }

        private async void MostrarPreferenciasdeProducto()
        {
            await Task.Delay(2000);
            var listaProdPref = App.productospreferencias?.GetItems()?.Select(PP =>
            {
                PP.DESCRIPCION_PROD_PREFERENCIA = PP.DESCRIPCION_PROD_PREFERENCIA?.Trim();

                return PP;
            }).ToList();
            if (listaProdPref is not null)
            {
                if (listaProdPref.Count > 0)
                {
                    Preferencias = new ObservableRangeCollection<TablaProductoPreferencia>(listaProdPref);
                    Preferencia = Preferencias.FirstOrDefault()?.DESCRIPCION_PROD_PREFERENCIA;
                }
                else
                {
                    Preferencia = "No hay Categorías disponibles";
                }
            }
        }

        private async void MostrarCategoriasMedico()
        {
            await Task.Delay(1000);
            var listaCategorias = App.categoriasmedico?.GetItems();
            if (listaCategorias is not null)
            {
                if (listaCategorias.Count > 0)
                {
                    Categorias = new ObservableRangeCollection<TablaCategoriasMedico>(listaCategorias);
                    Categoria = Categorias.FirstOrDefault();
                }
                else
                {
                    Categoria = new TablaCategoriasMedico
                    {
                        CATEGORIA = "No hay Categgorías disponibles"
                    };
                }   
            }
        }

        private async void MostrarEspecialidad()
        {
            await Task.Delay(1000);
            var listaEspecialidades = App.especialidades?.GetItems();
            if (listaEspecialidades is not null)
            {
                if (listaEspecialidades.Count > 0)
                {
                    Especialidades = new ObservableRangeCollection<TablaClasesEspecializaciones>(listaEspecialidades);
                    Especialidad = Especialidades.FirstOrDefault();
                }
                else
                {
                    Especialidad = new TablaClasesEspecializaciones
                    {
                        DESCRIPCION_CLASE = "No hay especialidades disponibles"
                    };
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
                LocalizacionUsuario = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            }
        }

        public async void GuardarNuevoMedico()
        {
            if (ModeEditor == 1)
            {
                if (MedicoName != "")
                {
                    if (MedicoDireccion != "")
                    {
                        GeolocationsPermissions();
                        var CodigoMilis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                      .ToString()
                                      .Substring(9);
                        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                        {
                            OperacionID = "VMedicA014",
                            Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoMail}','{MedicoJVPM}','','','','','','','','{LocalizacionUsuario?.Latitude.ToString().Replace(",",".")}','{LocalizacionUsuario?.Longitude.ToString().Replace(",", ".")}','{Especialidad?.CODIGO_DE_CLASE}','{ColorSeleccionado}','','{Adaptacion}',{Categoria?.CATEGORIAID},'{IdsPreferencias}'",
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
                                        var SolicitudEnviarControlVisita = new TablaSolicitudesNoEnviadas();
                                        if (EnableRepetir)
                                        {
                                            SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                            SolicitudEnviarControlVisita.Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{datos.COD}','{ObtenerSemanasSeleciconadas()}','{ObtenerDiasSeleccionados()}','10'";
                                            SolicitudEnviarControlVisita.ClavesVacias = 0;
                                            SolicitudEnviarControlVisita.TipoRestService = 1;
                                        }
                                        else
                                        {
                                            SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                            SolicitudEnviarControlVisita.Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{datos.COD}','0','0','10','{DateTime.ParseExact(FechaVisita ?? "", "ddd dd MMM yyyy", new CultureInfo("es-ES")).ToString("dd/MM/yyyy hh:mm:ss a")}'";
                                            SolicitudEnviarControlVisita.ClavesVacias = 0;
                                            SolicitudEnviarControlVisita.TipoRestService = 1;
                                        }

                                        if (IsInternet.Avilable())
                                        {
                                            var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudEnviarControlVisita.OperacionID + "/" + SolicitudEnviarControlVisita.Parametros, null))?.FirstOrDefault();
                                            if (resultados is not null)
                                            {
                                                switch (resultados.MSG)
                                                {
                                                    case "1":
                                                        ToastMaker.Make("El médico fue agregado con éxito", App.Current?.MainPage);
                                                        await Shell.Current.Navigation.PopAsync();
                                                        break;
                                                    case "2":
                                                        ToastMaker.Make("El usuario no tiene permisos para agregar control de visitas", App.Current?.MainPage);
                                                        break;
                                                    case "3":
                                                        ToastMaker.Make("Ha ocurrido un error inesperado al guardar el control de visitas", App.Current?.MainPage);
                                                        break;
                                                    case "5":
                                                        ToastMaker.Make("No se ha encontrado el médico indicado", App.Current?.MainPage);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para guardar el control de visita automáticamente", App.Current?.MainPage);
                                            App.SolicitudesPendientes?.InsertItem(SolicitudEnviarControlVisita);
                                        }
                                        break;
                                    case "2":
                                        ToastMaker.Make("El usuario no tiene permisos para agregar médicos", App.Current?.MainPage);
                                        break;
                                    case "3":
                                        ToastMaker.Make("Ha ocurrido un error inesperado al guardar el médico", App.Current?.MainPage);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para guardar el médico automáticamente", App.Current?.MainPage);
                            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                        }
                    }
                    else
                    {
                        ToastMaker.Make("Favor digite la dirección del médico", App.Current?.MainPage);
                    }
                }
                else
                {
                    ToastMaker.Make("Favor digite el nombre del médico", App.Current?.MainPage);
                }
            }
            else if (ModeEditor == 3)
            {

            }
        }

        private string ObtenerDiasSeleccionados()
        {
            var Dias = new List<string>();

            if (CheckL)
            {
                Dias.Add("1");
            }

            if (CheckM)
            {
                Dias.Add("2");
            }

            if (CheckMi)
            {
                Dias.Add("3");
            }

            if (CheckJ)
            {
                Dias.Add("4");
            }

            if (CheckV)
            {
                Dias.Add("5");
            }
            
            if (CheckS)
            {
                Dias.Add("6");
            }
            
            if (CheckD)
            {
                Dias.Add("7");
            }

            return string.Join(",", Dias);
        }

        private string ObtenerSemanasSeleciconadas()
        {
            var Semanas = new List<string>();

            if (CheckS1)
            {
                Semanas.Add("1");
            }

            if (CheckS2)
            {
                Semanas.Add("2");
            }

            if (CheckS3)
            {
                Semanas.Add("3");
            }

            if (CheckS4)
            {
                Semanas.Add("4");
            }

            if (CheckS5)
            {
                Semanas.Add("5");
            }

            return string.Join(",", Semanas);   
        }
    }
}
