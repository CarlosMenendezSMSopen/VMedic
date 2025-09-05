using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Médicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class EditarMedicoViewModel : BaseViewModel
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
        private ObservableRangeCollection<string>? _escalasAdaptacion = new ObservableRangeCollection<string> { "Dogmático", "Pragmático" };

        [ObservableProperty]
        private string? _adaptacion;

        [ObservableProperty]
        private bool _checkS1 = false;

        [ObservableProperty]
        private bool _checkS2 = false;

        [ObservableProperty]
        private bool _checkS3 = false;

        [ObservableProperty]
        private bool _checkS4 = false;

        [ObservableProperty]
        private bool _checkS5 = false;

        [ObservableProperty]
        private bool _checkL = false;

        [ObservableProperty]
        private bool _checkM = false;

        [ObservableProperty]
        private bool _checkMi = false;

        [ObservableProperty]
        private bool _checkJ = false;

        [ObservableProperty]
        private bool _checkV = false;

        [ObservableProperty]
        private bool _checkS = false;

        [ObservableProperty]
        private bool _checkD = false;
        
        [ObservableProperty]
        private bool _positionVisibilidad;

        [ObservableProperty]
        private int _position;

        private readonly RestService servicio = new();
        public string? CodigoCliente { get; set; } = "";
        public string? ColorSeleccionado { get; set; } = "";
        public string? IdsPreferencias { get; set; } = "";
        private TablaDoctores? Medico { get; set; }
        private Location? LocalizacionUsuario { get; set; }
        public List<TablaProductoPreferencia>? PreferenciasSeleccionadas { get; set; }
        private List<string>? Dias { get; set; }
        private string? DiasSeleccionados { get; set; }
        private List<string>? Semanas { get; set; }
        private string? SemanasSeleccionadas { get; set; }
        public EditarMedicoViewModel(string? cODIGO_DE_CLIENTE)
        {
            CodigoCliente = cODIGO_DE_CLIENTE;
            Medico = App.doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == CodigoCliente);
            _medicoName = Medico?.NOMBRE_COMERCIAL;
            _medicoContact = Medico?.CONTACTO_CLIENTE;
            _medicoDireccion = Medico?.DIRECCION_CLIENTE;
            _medicoTelefono = Medico?.TELEFONO_CLIENTE;
            _medicoMail = Medico?.DIRECCION_EMAIL;
            _medicoJVPM = Medico?.JVPM;
            MostrarEspecialidad();
            MostrarCategoriasMedico();
            _positionVisibilidad = Medico?.COLOR != "";
            _position = Medico?.COLOR switch { "Rojo" => 0, "Azul" => 1, "Amarillo" => 2, "Verde" => 3, "" => 0, _ => -1 };
            ColorSeleccionado = Medico?.COLOR;
            _adaptacion = Medico?.ESCALA_ADAPTACION;
            MostrarVisitaMensualMedico();
        }

        private void MostrarVisitaMensualMedico()
        {
            var VisitaMensualMedico = App.visitasmensuales?.GetItems()?.Where(VM => VM.CODIGO_DE_CLIENTE.ToString() == Medico?.CODIGO_DE_CLIENTE).ToList();
            if (VisitaMensualMedico is not null)
            {
                var SemanasVisita = VisitaMensualMedico.Select(VM => VM.SEMANA).ToList();
                var DiasVisita = VisitaMensualMedico.Select(VM => VM.DIA).ToList();
                if (SemanasVisita is not null)
                {
                    foreach (var semana in SemanasVisita)
                    {
                        switch (semana)
                        {
                            case 1:
                                CheckS1 = true;
                                break;
                            case 2:
                                CheckS2 = true;
                                break;
                            case 3:
                                CheckS3 = true;
                                break;
                            case 4:
                                CheckS4 = true;
                                break;
                            case 5:
                                CheckS5 = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (DiasVisita is not null)
                {
                    foreach (var dia in DiasVisita)
                    {
                        switch (dia)
                        {
                            case 1:
                                CheckL = true;
                                break;
                            case 2:
                                CheckM = true;
                                break;
                            case 3:
                                CheckMi = true;
                                break;
                            case 4:
                                CheckJ = true;
                                break;
                            case 5:
                                CheckV = true;
                                break;
                            case 6:
                                CheckS = true;
                                break;
                            case 7:
                                CheckD = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void MostrarPreferenciasdeProducto()
        {
            var listaMedicoProdPref = App.medicoprodpreferencias?.GetItems()?.Where(MPP => MPP.CODIGO_DE_CLIENTE.ToString() == Medico?.CODIGO_DE_CLIENTE).ToList();
            if (listaMedicoProdPref is not null)
            {
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
                        PreferenciasSeleccionadas = [.. listaProdPref.Where(LPP => listaMedicoProdPref.Any(LMPP => LMPP.ID_PRODUCTO_PREFERENCIA == LPP.ID_PRODUCTO_PREFERENCIA))];
                    }
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
                    Categoria = Categorias.FirstOrDefault(C => C.CATEGORIAID == Medico?.CATEGORIAID);
                }
                else
                {
                    Categoria = new TablaCategoriasMedico
                    {
                        CATEGORIA = "No hay Categorías disponibles"
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
                    Especialidad = Especialidades.FirstOrDefault(E => E.CODIGO_DE_CLASE == Medico?.CODIGO_DE_CLASE);
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
                if (CheckedUbicacion)
                {
                    LocalizacionUsuario = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
                }
                else if (Medico is not null)
                {
                    LocalizacionUsuario = new Location(Medico.LATITUD, Medico.LONGITUD);
                }
            }
        }

        public async void ActualizarMedico()
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
                            GeolocationsPermissions();

                            await Task.Delay(1000);

                            var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                            {
                                OperacionID = "VMedicA042",
                                Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{Medico?.CODIGO_DE_CLIENTE}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoMail}','{MedicoJVPM}','{LocalizacionUsuario?.Latitude.ToString().Replace(",", ".")}','{LocalizacionUsuario?.Longitude.ToString().Replace(",", ".")}','{ColorSeleccionado}','','{Adaptacion}','{Especialidad?.CODIGO_DE_CLASE}',{Categoria?.CATEGORIAID},'{IdsPreferencias}'",
                                ClavesVacias = 0,
                                TipoRestService = 1,
                            };

                            var SolicitudEnviarAEliminar = new TablaSolicitudesNoEnviadas
                            {
                                OperacionID = "VMedicA041",
                                Parametros = $"'{Medico?.CODIGO_DE_CLIENTE}','{App.usuario?.GetItem().UsuarioName}'",
                                ClavesVacias = 0,
                                TipoRestService = 1,
                                SolicitudPadre = new Dictionary<int, string?> { { int.Parse(Medico?.CODIGO_DE_CLIENTE ?? ""), SolicitudEnviar.OperacionID } }
                            };

                            var SolicitudEnviarControlVisita = new TablaSolicitudesNoEnviadas
                            {
                                OperacionID = "VMedicA021",
                                Parametros = $"'{App.usuario?.GetItem().UsuarioName}','{Medico?.CODIGO_DE_CLIENTE}','{SemanasSeleccionadas}','{DiasSeleccionados}','10'",
                                ClavesVacias = 0,
                                TipoRestService = 1,
                                SolicitudPadre = new Dictionary<int, string?> { { int.Parse(Medico?.CODIGO_DE_CLIENTE ?? ""), SolicitudEnviar.OperacionID } },
                            };

                            if (IsInternet.Avilable())
                            {
                                var eliminar = (await servicio.ResultadoGET<Resultado>(SolicitudEnviarAEliminar.OperacionID + "/" + SolicitudEnviarAEliminar.Parametros, null))?.FirstOrDefault();
                                var datos = (await servicio.ResultadoGET<Resultado>(SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                                if (datos is not null)
                                {
                                    switch (datos.MSG)
                                    {
                                        case "1":
                                            if (IsInternet.Avilable())
                                            {
                                                var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudEnviarControlVisita.OperacionID + "/" + SolicitudEnviarControlVisita.Parametros, null))?.FirstOrDefault();
                                                if (resultados is not null)
                                                {
                                                    switch (resultados.MSG)
                                                    {
                                                        case "1":
                                                            ToastMaker.Make("El médico fue actualizado con éxito", App.Current?.MainPage);
                                                            DatosCompartidos.StatusVolver = 1;
                                                            await Shell.Current.Navigation.PopAsync();
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
                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviarAEliminar);
                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviarControlVisita);
                            }
                        }
                        else
                        {
                            ToastMaker.Make("Favor seleccione los días de visita", App.Current?.MainPage);
                        }
                    }
                    else
                    {
                        ToastMaker.Make("Favor seleccione la semana de visita", App.Current?.MainPage);
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

            DiasSeleccionados = string.Join(",", Dias);
        }

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

            SemanasSeleccionadas = string.Join(",", Semanas);
        }
    }
}
