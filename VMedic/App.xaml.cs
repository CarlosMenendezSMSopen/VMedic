using System.Diagnostics;
using System.Net;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic
{
    public partial class App : Application
    {
        public static BaseRepository<TablaUsuario>? Usuario { get; private set; }
        public static BaseRepository<TablaDoctores>? Doctores { get; private set; }
        public static BaseRepository<TablaNiveles>? Niveles { get; private set; }
        public static BaseRepository<TablaCategorias>? Categorias { get; private set; }
        public static BaseRepository<TablaSubCategorias>? Subcategorias { get; private set; }
        public static BaseRepository<TablaProductos>? Productos { get; private set; }
        public static BaseRepository<TablaPresentaciones>? Presentaciones { get; private set; }
        public static BaseRepository<TablaTiposVisitas>? Tiposvisitas { get; private set; }
        public static BaseRepository<TablaVisitasMensuales>? Visitasmensuales { get; private set; }
        public static BaseRepository<TablaNivelesPrecio>? NivelesPrecio { get; private set; }
        public static BaseRepository<TablaLugaresVenta>? Lugaresventas { get; private set; }
        public static BaseRepository<TablaVisitasPendientes>? Visitas { get; private set; }
        public static BaseRepository<TablaMateriales>? Materiales { get; private set; }
        public static BaseRepository<TablaMuestras>? Muestras { get; private set; }
        public static BaseRepository<TablaDetallesEvaluacion>? Evaluaciondetalles { get; private set; }
        public static BaseRepository<TablaSKUProducto>? Skuproductos { get; private set; }
        public static BaseRepository<TablaEncabezadoEvaluacion>? Evaluacionencabezado { get; private set; }
        public static BaseRepository<TablaSolicitudesNoEnviadas>? SolicitudesPendientes { get; private set; }
        public static BaseRepository<TablaClasesEspecializaciones>? Especialidades { get; private set; }
        public static BaseRepository<TablaCategoriasMedico>? Categoriasmedico { get; private set; }
        public static BaseRepository<TablaProductoPreferencia>? Productospreferencias { get; private set; }
        public static BaseRepository<TablaMedicoProductoPreferencia>? Medicoprodpreferencias { get; private set; }
        public static BaseRepository<TablaAgenda>? Agenda { get; private set; }
        public App
        (
            BaseRepository<TablaUsuario> repo,
            BaseRepository<TablaDoctores> repo3,
            BaseRepository<TablaNiveles> repo4,
            BaseRepository<TablaCategorias> repo5,
            BaseRepository<TablaSubCategorias> repo6,
            BaseRepository<TablaProductos> repo8,
            BaseRepository<TablaPresentaciones> repo11,
            BaseRepository<TablaTiposVisitas> repo18,
            BaseRepository<TablaVisitasMensuales> repo19,
            BaseRepository<TablaNivelesPrecio> repo22,
            BaseRepository<TablaLugaresVenta> repo37,
            BaseRepository<TablaClasesEspecializaciones> repo40,
            BaseRepository<TablaMateriales> repo44,
            BaseRepository<TablaMuestras> repo45,
            BaseRepository<TablaAgenda> repo47,
            BaseRepository<TablaCategoriasMedico> repo50,
            BaseRepository<TablaProductoPreferencia> repo51,
            BaseRepository<TablaMedicoProductoPreferencia> repo52,
            BaseRepository<TablaSKUProducto> repo53,
            BaseRepository<TablaVisitasPendientes> repoL1,
            BaseRepository<TablaDetallesEvaluacion> repoL2,
            BaseRepository<TablaEncabezadoEvaluacion> repoL3,
            BaseRepository<TablaSolicitudesNoEnviadas> repoL4
        )
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH5fcnVdRWJdVUB/XUVWYEg=");
            InitializeComponent();

            Usuario = repo;
            Doctores = repo3;
            Niveles = repo4;
            Categorias = repo5;
            Subcategorias = repo6;
            Productos = repo8;
            Presentaciones = repo11;
            Tiposvisitas = repo18;
            Visitasmensuales = repo19;
            NivelesPrecio = repo22;
            Lugaresventas = repo37;
            Especialidades = repo40;
            Materiales = repo44;
            Muestras = repo45;
            Agenda = repo47;
            Categoriasmedico = repo50;
            Productospreferencias = repo51;
            Medicoprodpreferencias = repo52;
            Skuproductos = repo53;
            Visitas = repoL1;
            Evaluaciondetalles = repoL2;
            Evaluacionencabezado = repoL3;
            SolicitudesPendientes = repoL4;

            if (Application.Current is not null)
                Application.Current.UserAppTheme = AppTheme.Light;

            //Windows[0].Page = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;
            RestService servicio = new();

            try
            {
                if (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular))
                {
                    WebRequest tRequest = WebRequest.Create("https://www.google.com/");
                    tRequest.Method = "GET";
                    tRequest.Timeout = 120000;
                    tRequest.ContentType = "application/json";
                    using HttpWebResponse response = (HttpWebResponse)tRequest.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ToastMaker.Make("Se reestableció la conexión a Internet", App.Current?.Windows[0].Page);
                        Task.Run(async () =>
                        {
                            if (SolicitudesPendientes is not null)
                                if (!SolicitudesPendientes.IsEmpty())
                                {
                                    var listadeSolicitudes = SolicitudesPendientes.GetItems();
                                    if (listadeSolicitudes is not null)
                                        foreach (var solicitud in listadeSolicitudes)
                                        {
                                            if (solicitud.TipoRestService is 1)
                                            {
                                                var datos = solicitud.ClavesVacias == 0
                                                    ? (await servicio.ResultadoGET<Resultado>($"{solicitud.OperacionID}/{solicitud.Parametros}", null))?.FirstOrDefault()
                                                    : (await servicio.ResultadoGET($"{solicitud.OperacionID}/{solicitud.Parametros}", valores => new Resultado
                                                    {
                                                        Id = valores[0],
                                                        MSG = valores[1],
                                                        Codigo = valores[2]
                                                    }))?.FirstOrDefault();

                                                if (datos is not null)
                                                {
                                                    switch (datos.MSG)
                                                    {
                                                        case "1":
                                                            switch (solicitud.OperacionID)
                                                            {
                                                                case "VMedicA017" or "VMedicA038" or "VMedicA043":
                                                                    var DoctorSeleciconado = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente).FirstOrDefault();
                                                                    if (DoctorSeleciconado is not null)
                                                                    {
                                                                        DoctorSeleciconado.Visitas = 1;
                                                                        App.Doctores?.UpdateITEM(DoctorSeleciconado);
                                                                    }
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                            break;
                                                        case "2":

                                                            break;
                                                        case "3":

                                                            break;
                                                        default:

                                                            break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var datos = solicitud.ClavesVacias == 0
                                                    ? (await servicio.ResultadoPOST<Resultado>(solicitud.OperacionID, solicitud.Parametros, null))?.FirstOrDefault()
                                                    : (await servicio.ResultadoPOST(solicitud.OperacionID, solicitud.Parametros, valores => new Resultado
                                                    {
                                                        Id = valores[0],
                                                        MSG = valores[1],
                                                        Codigo = valores[2]
                                                    }))?.FirstOrDefault();

                                                if (datos is not null)
                                                {
                                                    switch (datos.MSG)
                                                    {
                                                        case "1":
                                                            switch (solicitud.OperacionID)
                                                            {
                                                                case "VMedicA046":
                                                                    var Codigos = datos.Codigo?.Split(CaracteresEspeciales.SECCION);
                                                                    if (Codigos is not null)
                                                                    {
                                                                        foreach (var codigo in Codigos)
                                                                        {
                                                                            var muestraActualizar = App.Muestras?.GetItems()?.FirstOrDefault(M => M.CODIGO_MUESTRA == codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[0]);
                                                                            var clienteActualizar = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente);

                                                                            if (muestraActualizar is not null && clienteActualizar is not null)
                                                                            {
                                                                                muestraActualizar.CANT_DISPONIBLE = int.Parse(codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[1]);
                                                                                clienteActualizar.Visitas = 1;

                                                                                App.Muestras?.UpdateITEM(muestraActualizar);
                                                                                App.Doctores?.UpdateITEM(clienteActualizar);

                                                                                var detallesEliminar = App.Evaluaciondetalles?.GetItems()?.Where(Edet => Edet.IdCliente == solicitud.CodigoCliente).ToList();
                                                                                var encabezadoEliminar = App.Evaluacionencabezado?.GetItems()?.Where(Eenc => Eenc.IdCliente == solicitud.CodigoCliente).ToList();

                                                                                if (detallesEliminar is not null && encabezadoEliminar is not null)
                                                                                {
                                                                                    App.Evaluaciondetalles?.DeleteItems(detallesEliminar);
                                                                                    App.Evaluacionencabezado?.DeleteItems(encabezadoEliminar);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                            break;
                                                        case "2":

                                                            break;
                                                        case "3":

                                                            break;
                                                        default:

                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                }
                        });
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
                    }
                }
                else
                {
                    Debug.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
            }
        }
    }
}