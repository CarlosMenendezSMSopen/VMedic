using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System.Diagnostics;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Medicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class EditarMedicoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _checkedUbicacion;

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
        private List<string>? Dias { get; set; }
        private string? DiasSeleccionados { get; set; }
        private List<string>? Semanas { get; set; }
        private string? SemanasSeleccionadas { get; set; }
        public EditarMedicoViewModel(string? cODIGO_DE_CLIENTE)
        {
            _enableRepetir = true;
            CodigoCliente = cODIGO_DE_CLIENTE;
            Medico = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == CodigoCliente);
            _medicoName = Medico?.NOMBRE_COMERCIAL;
            _medicoContact = Medico?.CONTACTO_CLIENTE;
            _medicoDireccion = Medico?.DIRECCION_CLIENTE;
            _medicoTelefono = Medico?.TELEFONO_CLIENTE;
            _medicoMail = Medico?.DIRECCION_EMAIL;
            _medicoJVPM = Medico?.JVPM;
            _medicoDUI = Medico?.DUI_CLIENTE;
            MostrarEspecialidad();
            MostrarCategoriasMedico();
            MostrarPreferenciasdeProducto();
            MostrarVisitaMensualMedico();
            MostrarDatosPais();
            _positionVisibilidad = Medico?.COLOR != "";
            _selectedColor = Medico?.COLOR switch { "Rojo" => _colores[0], "Azul" => _colores[1], "Amarillo" => _colores[2], "Verde" => _colores[3], "" => "", _ => "" };
            _adaptacion = Medico?.ESCALA_ADAPTACION;
        }

        private async void MostrarDatosPais()
        {
            Paises = new ObservableRangeCollection<TablaDatosPais>(await SincronizacionDataBase.ObtenerDatosPaises(1, null));
            Pais = Paises.FirstOrDefault(P => P.Id == Medico?.CODIGO_DE_PAIS);
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

        //metodo para chequear los dias y semanas programados para las visitas al medico
        private async void MostrarVisitaMensualMedico()
        {
            var VisitasMensualMedico = await SincronizacionDataBase.ObtenerVisitasMensuales(Medico?.CODIGO_DE_CLIENTE);
            if (VisitasMensualMedico is not null)
            {
                if (VisitasMensualMedico.FirstOrDefault()?.TIPO_CONTROL == 1)
                {
                    EnableRepetir = false;
                    var fecha = VisitasMensualMedico.FirstOrDefault()?.FECHA?.Replace("T", " ");
                    if (fecha is not null)
                        FechaInicial = DateTime.Parse(fecha);
                }
                else
                {
                    EnableRepetir = true;

                    FechaInicial = DateTime.Today;

                    foreach (var visita in VisitasMensualMedico)
                    {
                        if (visita.SEMANA == 1)
                            CheckS1 = true;

                        if (visita.SEMANA == 2)
                            CheckS2 = true;

                        if (visita.SEMANA == 3)
                            CheckS3 = true;

                        if (visita.SEMANA == 4)
                            CheckS4 = true;

                        if (visita.SEMANA == 5)
                            CheckS5 = true;

                        if (visita.SEMANA == 6)
                            CheckS6 = true;

                        if (visita.DIA == 1)
                            CheckD = true;

                        if (visita.DIA == 2)
                            CheckL = true;

                        if (visita.DIA == 3)
                            CheckM = true;

                        if (visita.DIA == 4)
                            CheckMi = true;

                        if (visita.DIA == 5)
                            CheckJ = true;

                        if (visita.DIA == 6)
                            CheckV = true;

                        if (visita.DIA == 7)
                            CheckS = true;
                    }
                }
                //SelectedRepetir = VisitaMensualMedico.TIPO_CONTROL - 1;
                //if (VisitaMensualMedico.FECHA is not null && VisitaMensualMedico.FECHAFINAL is not null)
                //{
                //    FechaInicial = DateTime.Parse(VisitaMensualMedico.FECHA.Replace("T", " "));
                //    FechaFinal = DateTime.Parse(VisitaMensualMedico.FECHAFINAL.Replace("T", " "));
                //    if (VisitaMensualMedico.TIPO_CONTROL == 3)
                //    {
                //        NumberEnables = ((DateTime.Parse(VisitaMensualMedico.FECHAFINAL.Replace("T", " ")) - DateTime.Parse(VisitaMensualMedico.FECHA.Replace("T", " "))).Days / 7)  + "";
                //    }
                //    else if (VisitaMensualMedico.TIPO_CONTROL == 4)
                //    {
                //        NumberEnables = (DateTime.Parse(VisitaMensualMedico.FECHAFINAL.Replace("T", " ")).Month - DateTime.Parse(VisitaMensualMedico.FECHA.Replace("T", " ")).Month) + "";
                //    }
                //}
            }
        }

        //metodo para llenar la lista desplegable de seleccion multiple con las preferencias de la programación
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
                        PreferenciasSeleccionadas = [.. listaProdPref.Where(LPP => listaMedicoProdPref.Any(LMPP => LMPP.ID_PRODUCTO_PREFERENCIA == LPP.ID_PRODUCTO_PREFERENCIA))];
                    }
                    else
                    {
                        Preferencia = "No hay Categorías disponibles";
                    }
                }
            }
        }

        //metodo que llena la lista desplegable de las categorías del médico
        private async void MostrarCategoriasMedico()
        {
            var listaCategorias = await SincronizacionDataBase.ObtenerCategoriasMedico();
            await Task.Delay(50);
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

        //metodo que llena la lista desplegable de las especialidades
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
                    Especialidad = Especialidades.FirstOrDefault(E => E.CODIGO_DE_CLASE == Medico?.CODIGO_DE_CLASE?.Trim());
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

        //metodo que consume api rest para actualizar la información del medico seleccionado
        public async void ActualizarMedico()
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
                                        IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA042").ToList().Count,
                                        OperacionID = "VMedicA042",
                                        Parametros = $"'{App.Usuario?.GetItem().UsuarioName}','{Medico?.CODIGO_DE_CLIENTE}','{MedicoName}','{MedicoContact}','{MedicoDireccion}','{MedicoTelefono}','{MedicoDUI}','{Pais?.Id}','{Departamento?.Id}','{Municipio?.Id}','{Especialidad?.CODIGO_DE_CLASE}','{MedicoMail}','{LocalizacionUsuario?.Latitude.ToString().Replace(",", ".")}','{LocalizacionUsuario?.Longitude.ToString().Replace(",", ".")}','{SelectedColor}','{Adaptacion}','{MedicoJVPM}','{Categoria?.CATEGORIAID}','{IdsPreferencias}'",
                                        ClavesVacias = 0,
                                        TipoRestService = 1,
                                        ModuloSolicitud = 2
                                    };

                                    var SolicitudActualizarControlVisita = new TablaSolicitudesNoEnviadas
                                    {
                                        IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA048").ToList().Count,
                                        OperacionID = "VMedicA048",
                                        Parametros = $"'{App.Usuario?.GetItem().UsuarioName}',{Medico?.CODIGO_DE_CLIENTE},{(EnableRepetir ? "NULL" : FechaInicial is not null ? $"'{FechaInicial.Value.ToString("yyyyMMdd")} {DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss")}'" : "NULL")},'{ObtenerCadenaSemDia(1)}','{ObtenerCadenaSemDia(2)}'",
                                        ClavesVacias = 0,
                                        TipoRestService = 1,
                                        IDSolicitudPadre = SolicitudEnviar.IDSolicitud,
                                        ModuloSolicitud = 2
                                    };

                                    var datos = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudEnviar.OperacionID + "/" + SolicitudEnviar.Parametros, null))?.FirstOrDefault();
                                    if (datos is not null)
                                    {
                                        switch (datos.MSG)
                                        {
                                            case "1":
                                                var resultados = (await servicio.ResultadoGET<Resultado>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, SolicitudActualizarControlVisita.OperacionID + "/" + SolicitudActualizarControlVisita.Parametros, null))?.FirstOrDefault();
                                                if (resultados is not null)
                                                {
                                                    switch (resultados.MSG)
                                                    {
                                                        case "1":
                                                            ToastMaker.Make("El médico fue actualizado con éxito", App.Current?.Windows[0].Page);

                                                            await Shell.Current.Navigation.PopAsync();
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

                                            App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                                            App.SolicitudesPendientes?.InsertItem(SolicitudActualizarControlVisita);

                                            await Shell.Current.Navigation.PopAsync();
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
