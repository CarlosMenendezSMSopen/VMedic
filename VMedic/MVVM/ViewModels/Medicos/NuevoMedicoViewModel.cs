using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using MvvmHelpers;
using PropertyChanged;
using SkiaSharp;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Timers;
using VMedic.Behaviors;
using VMedic.Conversores;
using VMedic.Global;
using VMedic.Interfaces;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.CustomMapObjects;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Point = Microsoft.Maui.Graphics.Point;
using Timer = System.Timers.Timer;

namespace VMedic.MVVM.ViewModels.Medicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class NuevoMedicoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _checkedUbicacion;

        [ObservableProperty]
        private string? _medicoName = "";

        [ObservableProperty]
        private string? _medicoContact = "";

        [ObservableProperty]
        private string? _medicoDireccion = "";

        [ObservableProperty]
        private string? _medicoTelefono = "";

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
        private ObservableRangeCollection<string>? _escalasAdaptacion = ["Dogmático", "Pragmático"];

        /*[ObservableProperty]
        private IList? _preferenciasSeleccionadas;*/

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
        private bool _checkS6;

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
        private int _position;

        private readonly RestService servicio = new();
        public string? ColorSeleccionado { get; set; } = "";
        private Location? LocalizacionUsuario { get; set; }
        public string? IdsPreferencias { get; set; } = "";
        public string? CodigoCliente { get; set; } = "";
        private List<string>? Dias { get; set; }
        private string? DiasSeleccionados { get; set; }
        private List<string>? Semanas { get; set; }
        private string? SemanasSeleccionadas { get; set; }

        public NuevoMedicoViewModel()
        {
            try
            {
                _enableRepetir = true;
                GeolocationsPermissions();
                MostrarEspecialidad();
                MostrarCategoriasMedico();
                MostrarPreferenciasdeProducto();
                _adaptacion = _escalasAdaptacion?.FirstOrDefault();
                _fechaVisita = DateTime.Today.AddDays(1).ToString("ddd dd MMM yyyy", new CultureInfo("es-ES"));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //metodo que llena la lista desplegable de preferencias
        private async void MostrarPreferenciasdeProducto()
        {
            var listaProdPref = App.Productospreferencias?.GetItems()?.Select(PP =>
            {
                PP.DESCRIPCION_PROD_PREFERENCIA = PP.DESCRIPCION_PROD_PREFERENCIA?.Trim();

                return PP;
            }).ToList();

            await Task.Delay(1000);

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

        //metodo que llenaa la lista desplegable de categorías de medico
        private async void MostrarCategoriasMedico()
        {
            SincronizacionDataBase.ObtenerCategoriasMedico();
            await Task.Delay(1000);
            var listaCategorias = App.Categoriasmedico?.GetItems();
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

        //metodo que llena la lista desplegable de las especialidades para desginar el nuevo medico
        private async void MostrarEspecialidad()
        {
            SincronizacionDataBase.ObtenerEspecialidades();
            await Task.Delay(1000);
            var listaEspecialidades = App.Especialidades?.GetItems();
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

        //metodo que obtiene los permisos de localización
        private async void GeolocationsPermissions()
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                App.Current?.Windows[0].Page?.DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación.", "OK");
                return;
            }
            else
            {
                LocalizacionUsuario = await Geolocation.Default.GetLastKnownLocationAsync();
            }

        }

        //metodo que envía los datos de medico a registrar
        public async void GuardarNuevoMedico()
        {
            if (MedicoName != "")
            {
                if (MedicoDireccion != "")
                {
                    ObtenerSemanasSeleciconadas();
                    if (SemanasSeleccionadas != "")
                    {
                        ObtenerDiasSeleccionados();
                        if (DiasSeleccionados != "")
                        {
                            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                            {
                                OperacionID = "VMedicA014",
                                Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoMail}','{MedicoJVPM}','{LocalizacionUsuario?.Latitude.ToString().Replace(",", ".")}','{LocalizacionUsuario?.Longitude.ToString().Replace(",", ".")}','{Especialidad?.CODIGO_DE_CLASE}','{ColorSeleccionado}','','{Adaptacion}',{Categoria?.CATEGORIAID},'{IdsPreferencias}'",
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
                                            if (EnableRepetir && datos.COD is not null)
                                            {
                                                SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                                SolicitudEnviarControlVisita.Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{datos.COD}','{SemanasSeleccionadas}','{DiasSeleccionados}','10'";
                                                SolicitudEnviarControlVisita.ClavesVacias = 0;
                                                SolicitudEnviarControlVisita.TipoRestService = 1;
                                                SolicitudEnviarControlVisita.CodigoCliente = datos.COD.ToString();
                                                SolicitudEnviarControlVisita.IDSolicitudPadre = datos.COD.ToString();
                                                SolicitudEnviarControlVisita.OperacionIdPadre = SolicitudEnviar.OperacionID;
                                            }
                                            else if (datos.COD is not null)
                                            {
                                                SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                                SolicitudEnviarControlVisita.Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{datos.COD}','0','0','10','{DateTime.ParseExact(FechaVisita ?? "", "ddd dd MMM yyyy", new CultureInfo("es-ES")):yyyyMMdd HH:mm:ss}'";
                                                SolicitudEnviarControlVisita.ClavesVacias = 0;
                                                SolicitudEnviarControlVisita.TipoRestService = 1;
                                                SolicitudEnviarControlVisita.CodigoCliente = datos.COD.ToString();
                                                SolicitudEnviarControlVisita.IDSolicitudPadre = datos.COD.ToString();
                                                SolicitudEnviarControlVisita.OperacionIdPadre = SolicitudEnviar.OperacionID;
                                            }

                                            if (IsInternet.Avilable())
                                            {
                                                var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudEnviarControlVisita.OperacionID + "/" + SolicitudEnviarControlVisita.Parametros, null))?.FirstOrDefault();
                                                if (resultados is not null)
                                                {
                                                    switch (resultados.MSG)
                                                    {
                                                        case "1":
                                                            ToastMaker.Make("El médico fue agregado con éxito", App.Current?.Windows[0].Page);
                                                            DatosCompartidos.StatusVolver = 1;
                                                            await Shell.Current.Navigation.PopAsync();
                                                            break;
                                                        case "2":
                                                            ToastMaker.Make("El usuario no tiene permisos para agregar control de visitas", App.Current?.Windows[0].Page);
                                                            break;
                                                        case "3":
                                                            ToastMaker.Make("Ha ocurrido un error inesperado al guardar el control de visitas", App.Current?.Windows[0].Page);
                                                            break;
                                                        case "5":
                                                            ToastMaker.Make("No se ha encontrado el médico indicado", App.Current?.Windows[0].Page);
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para guardar el control de visita automáticamente", App.Current?.Windows[0].Page);
                                                SolicitudEnviarControlVisita.OperacionIdPadre = null;
                                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviarControlVisita);
                                            }
                                            break;
                                        case "2":
                                            ToastMaker.Make("El usuario no tiene permisos para agregar médicos", App.Current?.Windows[0].Page);
                                            break;
                                        case "3":
                                            ToastMaker.Make("Ha ocurrido un error inesperado al guardar el médico", App.Current?.Windows[0].Page);
                                            break;
                                        case "4":
                                            ToastMaker.Make(datos.ErrorMessage, App.Current?.Windows[0].Page);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para guardar el médico automáticamente", App.Current?.Windows[0].Page);
                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);

                                var SolicitudEnviarControlVisita = new TablaSolicitudesNoEnviadas();
                                if (EnableRepetir)
                                {
                                    SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                    SolicitudEnviarControlVisita.Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','','{SemanasSeleccionadas}','{DiasSeleccionados}','10'";
                                    SolicitudEnviarControlVisita.ClavesVacias = 0;
                                    SolicitudEnviarControlVisita.TipoRestService = 1;
                                    SolicitudEnviarControlVisita.IDSolicitudPadre = SolicitudEnviar.TableID.ToString();
                                    SolicitudEnviarControlVisita.OperacionIdPadre = SolicitudEnviar.OperacionID;
                                }
                                else
                                {
                                    SolicitudEnviarControlVisita.OperacionID = "VMedicA021";
                                    SolicitudEnviarControlVisita.Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','','0','0','10','{DateTime.ParseExact(FechaVisita ?? "", "ddd dd MMM yyyy", new CultureInfo("es-ES")):yyyyMMdd HH:mm:ss}'";
                                    SolicitudEnviarControlVisita.ClavesVacias = 0;
                                    SolicitudEnviarControlVisita.TipoRestService = 1;
                                    SolicitudEnviarControlVisita.IDSolicitudPadre = SolicitudEnviar.TableID.ToString();
                                    SolicitudEnviarControlVisita.OperacionIdPadre = SolicitudEnviar.OperacionID;
                                }
                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviarControlVisita);
                            }

                            await Shell.Current.Navigation.PopAsync();
                        }
                        else
                        {
                            ToastMaker.Make("Favor seleccione los días de visita", App.Current?.Windows[0].Page);
                        }
                    }
                    else
                    {
                        ToastMaker.Make("Favor seleccione la semana de visita", App.Current?.Windows[0].Page);
                    }
                }
                else
                {
                    ToastMaker.Make("Favor digite la dirección del médico", App.Current?.Windows[0].Page);
                }
            }
            else
            {
                ToastMaker.Make("Favor digite el nombre del médico", App.Current?.Windows[0].Page);
            }
        }

        //metodo que asigna a un array los numeros segun los checkbox seleccionados para los días de visita
        private void ObtenerDiasSeleccionados()
        {
            Dias = [];

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

            DiasSeleccionados = !EnableRepetir ? "0" : string.Join(",", Dias);
        }

        //metodo que asigna a un array los numeros segun los checkbox seleccionados para las semanas de visita
        private void ObtenerSemanasSeleciconadas()
        {
            Semanas = [];

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
            
            if (CheckS6)
            {
                Semanas.Add("6");
            }

            SemanasSeleccionadas = !EnableRepetir ? "0" : string.Join(",", Semanas);
        }
    }
}
