using System.Net;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;

namespace VMedic
{
    public partial class App : Application
    {
        public static BaseRepository<TablaUsuario>? usuario { get; private set; }
        public static BaseRepository<TablaDoctores>? doctores { get; private set; }
        public static BaseRepository<TablaNiveles>? niveles { get; private set; }
        public static BaseRepository<TablaCategorias>? categorias { get; private set; }
        public static BaseRepository<TablaSubCategorias>? subcategorias { get; private set; }
        public static BaseRepository<TablaTiposVisitas>? tiposvisitas { get; private set; }
        public static BaseRepository<TablaVisitasMensuales>? visitasmensuales { get; private set; }
        public static BaseRepository<TablaLugaresVenta>? lugaresventas { get; private set; }
        public static BaseRepository<TablaVisitasPendientes>? visitas { get; private set; }
        public static BaseRepository<TablaMateriales>? materiales { get; private set; }
        public static BaseRepository<TablaMuestras>? muestras { get; private set; }
        public static BaseRepository<TablaDetallesEvaluacion>? evaluaciondetalles { get; private set; }
        public static BaseRepository<TablaSKUProducto>? skuproductos { get; private set; }
        public static BaseRepository<TablaEncabezadoEvaluacion>? evaluacionencabezado { get; private set; }
        public static BaseRepository<TablaSolicitudesNoEnviadas>? SolicitudesPendientes { get; private set; }
        public App(
            BaseRepository<TablaUsuario> repo,
            BaseRepository<TablaDoctores> repo3,
            BaseRepository<TablaNiveles> repo4,
            BaseRepository<TablaCategorias> repo5,
            BaseRepository<TablaSubCategorias> repo6,
            BaseRepository<TablaTiposVisitas> repo18,
            BaseRepository<TablaVisitasMensuales> repo19,
            BaseRepository<TablaLugaresVenta> repo37,
            BaseRepository<TablaMateriales> repo44,
            BaseRepository<TablaMuestras> repo45,
            BaseRepository<TablaSKUProducto> repo53,
            BaseRepository<TablaVisitasPendientes> repoL1,
            BaseRepository<TablaDetallesEvaluacion> repoL2,
            BaseRepository<TablaEncabezadoEvaluacion> repoL3,
            BaseRepository<TablaSolicitudesNoEnviadas> repoL4
        )
        {
            InitializeComponent();

            usuario = repo;
            doctores = repo3;
            niveles = repo4;
            categorias = repo5;
            subcategorias = repo6;
            tiposvisitas = repo18;
            visitasmensuales = repo19;
            lugaresventas = repo37;
            materiales = repo44;
            muestras = repo45;
            skuproductos = repo53;
            visitas = repoL1;
            evaluaciondetalles = repoL2;
            evaluacionencabezado = repoL3;
            SolicitudesPendientes = repoL4;

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            if (Application.Current is not null)
                Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }

        private void Connectivity_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;
            RestService servicio = new RestService();

            try
            {
                if (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular))
                {
                    WebRequest tRequest = WebRequest.Create("https://www.google.com/");
                    tRequest.Method = "GET";
                    tRequest.Timeout = 120000;
                    tRequest.ContentType = "application/json";
                    using (HttpWebResponse response = (HttpWebResponse)tRequest.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            ToastMaker.Make("Se reestableció la conexión a Internet", App.Current?.MainPage);
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
                                                                        var DoctorSeleciconado = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente).FirstOrDefault();
                                                                        if (DoctorSeleciconado is not null)
                                                                        {
                                                                            DoctorSeleciconado.Visitas = 1;
                                                                            App.doctores?.UpdateITEM(DoctorSeleciconado);
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
                                                                                var muestraActualizar = App.muestras?.GetItems()?.FirstOrDefault(M => M.CODIGO_MUESTRA == codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[0]);
                                                                                var clienteActualizar = App.doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente);

                                                                                if (muestraActualizar is not null && clienteActualizar is not null)
                                                                                {
                                                                                    muestraActualizar.CANT_DISPONIBLE = int.Parse(codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[1]);
                                                                                    clienteActualizar.Visitas = 1;

                                                                                    App.muestras?.UpdateITEM(muestraActualizar);
                                                                                    App.doctores?.UpdateITEM(clienteActualizar);

                                                                                    var detallesEliminar = App.evaluaciondetalles?.GetItems()?.Where(Edet => Edet.IdCliente == solicitud.CodigoCliente).ToList();
                                                                                    var encabezadoEliminar = App.evaluacionencabezado?.GetItems()?.Where(Eenc => Eenc.IdCliente == solicitud.CodigoCliente).ToList();

                                                                                    if (detallesEliminar is not null && encabezadoEliminar is not null)
                                                                                    {
                                                                                        App.evaluaciondetalles?.DeleteItems(detallesEliminar);
                                                                                        App.evaluacionencabezado?.DeleteItems(encabezadoEliminar);
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
                            Console.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("⚠️ Se ha perdido la conexión a Internet.");
            }
        }
    }
}
