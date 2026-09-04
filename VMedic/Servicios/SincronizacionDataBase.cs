using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.DataBase;

namespace VMedic.Servicios
{
    public static class SincronizacionDataBase
    {
        private static readonly TablaUsuario? UsuarioIniciado = App.Usuario?.GetItem();
        private static readonly RestService servicio = new();
        public static void SincronizarTodo()
        {
            //ObtenerDoctores();
            ObtenerNiveles();
            ObtenerCategorias();
            ObtenerSubCategorias();
            //ObtenerTiposVisitas();
            //ObtenerVisitasMensuales();
            //ObtenerLugaresdeVentas();
            //ObtenerMateriales();
            //ObtenerMuestras();
            //ObtenerSKUProductos();
        }

        public static void EliminarTodo()
        {
            App.Usuario?.DeleteItems();
            App.Doctores?.DeleteItems();
            App.Niveles?.DeleteItems();
            App.Categorias?.DeleteItems();
            App.Subcategorias?.DeleteItems();
            App.Tiposvisitas?.DeleteItems();
            App.Visitasmensuales?.DeleteItems();
            App.Lugaresventas?.DeleteItems();
            App.Especialidades?.DeleteItems();
            App.Materiales?.DeleteItems();
            App.Muestras?.DeleteItems();
            App.Categoriasmedico?.DeleteItems();
            App.Productospreferencias?.DeleteItems();
            App.Medicoprodpreferencias?.DeleteItems();
            App.Skuproductos?.DeleteItems();
            App.Visitas?.DeleteItems();
            App.Evaluaciondetalles?.DeleteItems();
            App.Evaluacionencabezado?.DeleteItems();
            App.SolicitudesPendientes?.DeleteItems();
        }

        public static async Task<List<TablaDoctores>> ObtenerDoctores()
        {
            var data = await servicio.ResultadoGET<TablaDoctores>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA003/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Doctores");
                if (App.Doctores is not null)
                {
                    App.Doctores.DeleteItems();
                    App.Doctores?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static void ObtenerNiveles()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaNiveles>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA004/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Debug.WriteLine("Insert Niveles");
                    if (App.Niveles is not null)
                    {
                        App.Niveles.DeleteItems();
                        App.Niveles?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerCategorias()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaCategorias>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA005/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Debug.WriteLine("Insert Categorias");
                    if (App.Categorias is not null)
                    {
                        App.Categorias.DeleteItems();
                        App.Categorias?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerSubCategorias()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaSubCategorias>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA006/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Debug.WriteLine("Insert Sub Categorias");
                    if (App.Subcategorias is not null)
                    {
                        App.Subcategorias.DeleteItems();
                        App.Subcategorias?.InsertItems(data);
                    }
                }
            });
        }

        public static async Task<List<TablaProductos>> ObtenerProductos()
        {
            var data = await servicio.ResultadoGET<TablaProductos>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA008/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Productos");
                if (App.Productos is not null)
                {
                    App.Productos.DeleteItems();
                    App.Productos?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaPresentaciones>> ObtenerPresentaciones()
        {
            var data = await servicio.ResultadoGET<TablaPresentaciones>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA011/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Presentaciones");
                if (App.Presentaciones is not null)
                {
                    App.Presentaciones.DeleteItems();
                    App.Presentaciones?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaTiposVisitas>> ObtenerTiposVisitas()
        {
            var data = await servicio.ResultadoGET<TablaTiposVisitas>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA018/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Tipos de Visitas");
                if (App.Tiposvisitas is not null)
                {
                    App.Tiposvisitas.DeleteItems();
                    App.Tiposvisitas?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaVisitasMensuales>> ObtenerVisitasMensuales(string? cODIGO_DE_CLIENTE)
        {
            var data = await servicio.ResultadoGET<TablaVisitasMensuales>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA019/'{UsuarioIniciado?.UsuarioName}'{(cODIGO_DE_CLIENTE is not null ? $",{cODIGO_DE_CLIENTE}" : "")}", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Visitas Mensuales");
                if (App.Visitasmensuales is not null)
                {
                    App.Visitasmensuales.DeleteItems();
                    App.Visitasmensuales?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaNivelesPrecio>> ObtenerNivelesdePrecios()
        {
            var data = await servicio.ResultadoGET<TablaNivelesPrecio>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA022/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Niveles de Precio");
                if (App.NivelesPrecio is not null)
                {
                    App.NivelesPrecio.DeleteItems();
                    App.NivelesPrecio?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaLugaresVenta>> ObtenerLugaresdeVentas()
        {
            var data = await servicio.ResultadoGET<TablaLugaresVenta>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA037/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Lugares de Venta");
                if (App.Lugaresventas is not null)
                {
                    App.Lugaresventas.DeleteItems();
                    App.Lugaresventas?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaClasesEspecializaciones>> ObtenerEspecialidades()
        {
            var data = await servicio.ResultadoGET<TablaClasesEspecializaciones>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA040/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert especialidades");
                if (App.Especialidades is not null)
                {
                    App.Especialidades.DeleteItems();
                    App.Especialidades?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaMateriales>> ObtenerMateriales()
        {
            var data = await servicio.ResultadoGET<TablaMateriales>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA044/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert materiales");
                if (App.Materiales is not null)
                {
                    App.Materiales.DeleteItems();
                    App.Materiales?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaMuestras>> ObtenerMuestras()
        {
            var data = await servicio.ResultadoGET<TablaMuestras>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA045/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert materiales");
                if (App.Muestras is not null)
                {
                    if (App.Muestras.IsEmpty())
                    {
                        App.Muestras.DeleteItems();
                        App.Muestras?.InsertItems(data);
                    }
                }

                return [.. data];
            }
            return [];
        }

        public static void ObtenerAgendaCompleta()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaAgenda>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA047/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Debug.WriteLine("Insert agenda");
                    if (App.Agenda is not null)
                    {
                        if (App.Agenda.IsEmpty())
                        {
                            App.Agenda.DeleteItems();
                            App.Agenda?.InsertItems(data);
                        }
                    }
                }
            });
        }

        public static async Task<List<TablaCategoriasMedico>> ObtenerCategoriasMedico()
        {
            var data = await servicio.ResultadoGET<TablaCategoriasMedico>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA050/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert categorias medico");
                if (App.Categoriasmedico is not null)
                {
                    App.Categoriasmedico.DeleteItems();
                    App.Categoriasmedico?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaProductoPreferencia>> ObtenerProductosPreferencias()
        {
            var data = await servicio.ResultadoGET<TablaProductoPreferencia>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA051/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert productos preferencias");
                if (App.Productospreferencias is not null)
                {
                    App.Productospreferencias.DeleteItems();
                    App.Productospreferencias?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaMedicoProductoPreferencia>> ObtenerMedicosProductosPreferencias()
        {
            var data = await servicio.ResultadoGET<TablaMedicoProductoPreferencia>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA052/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert productos preferencias de medico");
                if (App.Medicoprodpreferencias is not null)
                {
                    App.Medicoprodpreferencias.DeleteItems();
                    App.Medicoprodpreferencias?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }

        public static async Task<List<TablaSKUProducto>> ObtenerSKUProductos()
        {
            var data = await servicio.ResultadoGET<TablaSKUProducto>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, $"VMedicA053/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert SKU productos");
                if (App.Skuproductos is not null)
                {
                    App.Skuproductos.DeleteItems();
                    App.Skuproductos?.InsertItems(data);
                }

                return [.. data];
            }
            return [];
        }
        
        public static async Task<List<TablaDatosPais>> ObtenerDatosPaises(int Tipo, int? IdParent)
        {
            var parametros = $"{(Tipo == 1 ? "VMedicA055" : Tipo == 2 ? "VMedicA056" : "VMedicA057")}/{UsuarioIniciado?.Codigo_COMPANIA}{(IdParent is not null ? $",{IdParent}" : "")}";
            var data = await servicio.ResultadoGET<TablaDatosPais>(App.Usuario?.GetItems()?.FirstOrDefault()?.DominioIP, parametros, null);
            if (data is not null)
            {
                return [.. data];
            }
            return [];
        }

        public static async void ObtenerDireccion(double Lat, double Lon)
        {
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            string jsonResponse = await client.GetStringAsync($"https://nominatim.openstreetmap.org/reverse?lat={Lat}&lon={Lon}&format=jsonv2");
        }
    }
}
