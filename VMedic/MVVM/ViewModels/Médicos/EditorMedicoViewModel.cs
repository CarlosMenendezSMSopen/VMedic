using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VMedic.Behaviors;
using VMedic.Converters;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;
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
        private ObservableRangeCollection<string>? _escalasAdopcion = new ObservableRangeCollection<string> { "Dogmático", "Pragmático" };

        [ObservableProperty]
        private string? _adopcion;

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
        private int _position;

        private RestService servicio = new RestService();
        public string? ColorSeleccionado { get; set; } = "";
        public int ModeEditor { get; set; }
        private Location? LocalizacionUsuario { get; set; }
        public string? IdsPreferencias { get; set; } = "";
        public EditorMedicoViewModel(int modoEditor)
        {
            _tituloPagina = modoEditor == 1 ? "Agregar Médico" : modoEditor == 2 ? "Ver en Mapa" : "Editar Médico";
            _visibilidadAgregar = modoEditor == 1 || modoEditor == 3;
            _visibilidadNuevo = modoEditor == 1;
            _visibilidadInfo = modoEditor == 2;
            _visibilidadActualizar = modoEditor == 3;
            _enableRepetir = true;
            ModeEditor = modoEditor;
            MostrarEspecialidad();
            MostrarCategoriasMedico();
            MostrarPreferenciasdeProducto();
            _adopcion = _escalasAdopcion?.FirstOrDefault();
        }

        private async void MostrarPreferenciasdeProducto()
        {
            await Task.Delay(2000);
            var listaProdPref = App.productospreferencias?.GetItems();
            if (listaProdPref is not null)
            {
                if (listaProdPref.Count > 0)
                {
                    Preferencias = new ObservableRangeCollection<TablaProductoPreferencia>(listaProdPref);
                    Preferencia = Preferencias.FirstOrDefault()?.DESCRIPCION_PROD_PREFERENCIA;
                }
                else
                {
                    Preferencia = "No hay Categgorías disponibles";
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

        public void GuardarNuevoMedico()
        {
            if (ModeEditor == 1)
            {
                if (MedicoName != "")
                {
                    if (MedicoDireccion != "")
                    {
                        var CodigoMilis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                                      .ToString()
                                      .Substring(9);
                        var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                        {
                            OperacionID = "VMedicA014",
                            Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{CodigoMilis}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoMail}','{MedicoJVPM}','','','','','','','','{ConversorDouble.Parse(LocalizacionUsuario?.Latitude.ToString())}','{ConversorDouble.Parse(LocalizacionUsuario?.Longitude.ToString())}','{Especialidad?.CODIGO_DE_CLASE}','{ColorSeleccionado}','','{Adopcion}','{Categoria?.CATEGORIAID}','{IdsPreferencias}'",
                            ClavesVacias = 0,
                            TipoRestService = 1,
                        };

                        if (IsInternet.Avilable())
                        {
                            //var datos = await servicio.ResultadoGET<Resultado>(SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null);
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
    }
}
