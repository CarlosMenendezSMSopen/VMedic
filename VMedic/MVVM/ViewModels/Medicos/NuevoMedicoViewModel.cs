using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Medicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class NuevoMedicoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _indicador;

        [ObservableProperty]
        private bool _enableRepetir;

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
        private string? _medicoDUI = "";

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
        private ObservableRangeCollection<TablaDatosPais>? _paises;

        [ObservableProperty]
        private TablaDatosPais? _pais;

        [ObservableProperty]
        private ObservableRangeCollection<TablaDatosPais>? _departamentos;

        [ObservableProperty]
        private TablaDatosPais? _departamento;

        [ObservableProperty]
        private ObservableRangeCollection<TablaDatosPais>? _municipios;

        [ObservableProperty]
        private TablaDatosPais? _municipio;

        [ObservableProperty]
        private ObservableRangeCollection<string>? _escalasAdaptacion = ["Dogmático", "Pragmático"];

        [ObservableProperty]
        private string? _adaptacion;

        [ObservableProperty]
        private List<TablaProductoPreferencia>? _preferenciasSeleccionadas;

        [ObservableProperty]
        private bool _positionVisibilidad;

        [ObservableProperty]
        private ObservableRangeCollection<string> _repetir = ["Una Sola Visita", "Días Hábiles", "Una Vez a la Semana", "Una Vez al Mes"];

        [ObservableProperty]
        private int? _selectedRepetir;

        [ObservableProperty]
        private string? _numberEnables;

        [ObservableProperty]
        private DateTime? _fechaInicial;

        [ObservableProperty]
        private DateTime? _fechaFinal;

        [ObservableProperty]
        private ObservableRangeCollection<string> _colores = ["Rojo", "Azul", "Amarillo", "Verde"];

        [ObservableProperty]
        private string? _selectedColor;

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
        private bool _checkD;

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

        private readonly RestService servicio = new();
        public string? CodigoCliente { get; set; } = "";
        public string? IdsPreferencias { get; set; } = "";
        private TablaDoctores? Medico { get; set; }
        private Location? LocalizacionUsuario { get; set; }
        private VerticalStackLayout? Stack_FormularioNuevoMedico { get; set; }
        public NuevoMedicoViewModel(VerticalStackLayout list_FormularioNuevoMedico)
        {
            _indicador = true;
            _enableRepetir = true;
            Stack_FormularioNuevoMedico = list_FormularioNuevoMedico;

            GeolocationsPermissions();

            MostrarEspecialidad();
            MostrarPreferenciasdeProducto();
            MostrarCategoriasMedico();
            MostrarDatosPais();
        }

        private async void MostrarDatosPais()
        {
            Paises = new ObservableRangeCollection<TablaDatosPais>(await SincronizacionDataBase.ObtenerDatosPaises(1, null));
            Pais = Paises.FirstOrDefault(P => P.Id == Medico?.CODIGO_DE_PAIS);

            Indicador = false;
        }

        public async void MostrarDatosDepartamentos()
        {
            Departamentos = new ObservableRangeCollection<TablaDatosPais>(await SincronizacionDataBase.ObtenerDatosPaises(2, Pais?.Id));
            Departamento = Departamentos.FirstOrDefault(P => P.Id == Medico?.CODIGO_DEPARTAMENTO);
        }

        public async void MostrarDatosMunicipios()
        {
            Municipios = new ObservableRangeCollection<TablaDatosPais>(await SincronizacionDataBase.ObtenerDatosPaises(3, Departamento?.Id));
            Municipio = Municipios.FirstOrDefault(P => P.Id == Medico?.CODIGO_MUNICIPIO);
        }

        public async void MostrarPreferenciasdeProducto()
        {
            var listaMedicoProdPref = (await SincronizacionDataBase.ObtenerMedicosProductosPreferencias())?.Where(MPP => MPP.CODIGO_DE_CLIENTE.ToString() == Medico?.CODIGO_DE_CLIENTE).ToList();

            if (listaMedicoProdPref is not null)
            {
                var listaProdPref = (await SincronizacionDataBase.ObtenerProductosPreferencias())?.Select(PP =>
                {
                    PP.DESCRIPCION_PROD_PREFERENCIA = PP.DESCRIPCION_PROD_PREFERENCIA?.Trim();

                    return PP;
                }).ToList();

                await Task.Delay(50);

                if (listaProdPref is not null)
                {
                    if (listaProdPref.Count > 0)
                    {
                        Preferencias = new ObservableRangeCollection<TablaProductoPreferencia>(listaProdPref);
                    }
                    else
                    {
                        Preferencia = "No hay Categorías disponibles";
                    }
                }
            }
        }

        private async void MostrarCategoriasMedico()
        {
            var listaCategorias = await SincronizacionDataBase.ObtenerCategoriasMedico();
            await Task.Delay(50);
            if (listaCategorias is not null)
            {
                if (listaCategorias.Count > 0)
                {
                    Categorias = new ObservableRangeCollection<TablaCategoriasMedico>(listaCategorias);
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
            var listaEspecialidades = await SincronizacionDataBase.ObtenerEspecialidades();
            await Task.Delay(50);
            if (listaEspecialidades is not null)
            {
                if (listaEspecialidades.Count > 0)
                {
                    Especialidades = new ObservableRangeCollection<TablaClasesEspecializaciones>(listaEspecialidades);
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
                App.Current?.Windows[0].Page?.DisplayAlert("Permiso denegado", "No se puede acceder a la ubicación.", "OK");
                return;
            }
            else
            {
                LocalizacionUsuario = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            }
        }

        public async void GuardarNuevoMedico()
        {
            try
            {
                if (MedicoName != "")
                {
                    if (MedicoDireccion != "")
                    {
                        if (MedicoTelefono != "")
                        {
                            if (MedicoMail != "")
                            {
                                if (Especialidad is not null)
                                {
                                    GeolocationsPermissions();

                                    await Task.Delay(1000);

                                    var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                                    {
                                        IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA014").ToList().Count,
                                        OperacionID = "VMedicA014",
                                        Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoDUI}','{Pais?.Id}','{Departamento?.Id}','{Municipio?.Id}','{Especialidad?.CODIGO_DE_CLASE}','{MedicoMail}','{LocalizacionUsuario?.Latitude.ToString().Replace(",", ".")}','{LocalizacionUsuario?.Longitude.ToString().Replace(",", ".")}','{SelectedColor}','{Adaptacion}','{MedicoJVPM}','{Categoria?.CATEGORIAID}','{IdsPreferencias}'",
                                        ClavesVacias = 0,
                                        TipoRestService = 1,
                                        ModuloSolicitud = 2
                                    };

                                    var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                                    if (datos is not null)
                                    {
                                        switch (datos.MSG)
                                        {
                                            case "1":
                                                var SolicitudActualizarControlVisita = new TablaSolicitudesNoEnviadas
                                                {
                                                    IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA048").ToList().Count,
                                                    OperacionID = "VMedicA048",
                                                    Parametros = $"'{App.Usuario?.GetItem().UsuarioName}',{datos.COD},{(EnableRepetir ? "NULL" : FechaInicial is not null ? $"'{FechaInicial.Value.ToString("yyyyMMdd")} {DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss")}'" : "NULL")},'{ObtenerCadenaSemDia(1)}','{ObtenerCadenaSemDia(2)}'",
                                                    ClavesVacias = 0,
                                                    TipoRestService = 1,
                                                    IDSolicitudPadre = SolicitudEnviar.IDSolicitud,
                                                    ModuloSolicitud = 2
                                                };

                                                var resultados = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudActualizarControlVisita.OperacionID + "/" + SolicitudActualizarControlVisita.Parametros, null))?.FirstOrDefault();
                                                if (resultados is not null)
                                                {
                                                    switch (resultados.MSG)
                                                    {
                                                        case "1":
                                                            ToastMaker.Make("El médico fue agregado con éxito", App.Current?.Windows[0].Page);

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
                                                else if (DatosCompartidos.ErrorResponseValue is not null)
                                                {
                                                    DatosCompartidos.CantidadIntentos++;

                                                    if (DatosCompartidos.CantidadIntentos == 4)
                                                    {
                                                        var MainPage = App.Current?.Windows[0].Page;

                                                        if (MainPage is not null)
                                                            await MainPage.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");

                                                        App.SolicitudesPendientes?.InsertItem(SolicitudActualizarControlVisita);
                                                        DatosCompartidos.CantidadIntentos = 0;

                                                        await Shell.Current.Navigation.PopAsync();
                                                    }
                                                    else
                                                    {
                                                        ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, App.Current?.Windows[0].Page);
                                                    }
                                                }

                                                break;
                                            case "2":
                                                ToastMaker.Make("El usuario no tiene permisos para modificar médicos", App.Current?.Windows[0].Page);
                                                break;
                                            case "3":
                                                ToastMaker.Make("Ha ocurrido un error inesperado al guardar el médico", App.Current?.Windows[0].Page);
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
                                            var MainPage = App.Current?.Windows[0].Page;

                                            if (MainPage is not null)
                                                await MainPage.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");

                                            var SolicitudActualizarControlVisita = new TablaSolicitudesNoEnviadas
                                            {
                                                IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA048").ToList().Count,
                                                OperacionID = "VMedicA048",
                                                Parametros = $"'{App.Usuario?.GetItem().UsuarioName}',-1,{(EnableRepetir ? "NULL" : FechaInicial is not null ? $"'{FechaInicial.Value.ToString("yyyyMMdd")} {DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss")}'" : "NULL")},'{ObtenerCadenaSemDia(1)}','{ObtenerCadenaSemDia(2)}'",
                                                ClavesVacias = 0,
                                                TipoRestService = 1,
                                                IDSolicitudPadre = SolicitudEnviar.IDSolicitud,
                                                ModuloSolicitud = 2
                                            };

                                            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                                            App.SolicitudesPendientes?.InsertItem(SolicitudActualizarControlVisita);

                                            await Shell.Current.Navigation.PopAsync();

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
                                ToastMaker.Make("Favor digite el correo electrónico del médico", App.Current?.Windows[0].Page);
                            }
                        }
                        else
                        {
                            ToastMaker.Make("Favor digite el teléfono del médico", App.Current?.Windows[0].Page);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private string ObtenerCadenaSemDia(int v)
        {
            if (EnableRepetir)
            {
                if (v == 1)
                {
                    List<string> Semanas = [];

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

                    return Semanas.Count > 0 ? string.Join(",", Semanas) : "0";
                }
                else
                {
                    List<string> Dias = [];

                    if (CheckD)
                    {
                        Dias.Add("1");
                    }

                    if (CheckL)
                    {
                        Dias.Add("2");
                    }

                    if (CheckM)
                    {
                        Dias.Add("3");
                    }

                    if (CheckMi)
                    {
                        Dias.Add("4");
                    }

                    if (CheckJ)
                    {
                        Dias.Add("5");
                    }

                    if (CheckV)
                    {
                        Dias.Add("6");
                    }

                    if (CheckS)
                    {
                        Dias.Add("7");
                    }

                    return Dias.Count > 0 ? string.Join(",", Dias) : "0";
                }
            }
            else
            {
                return "0";
            }
        }
    }
}
