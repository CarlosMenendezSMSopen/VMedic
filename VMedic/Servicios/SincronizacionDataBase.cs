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

        public static async void ObtenerDoctores()
        {
            var data = await servicio.ResultadoGET<TablaDoctores>($"VMedicA003/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Doctores");
                if (App.Doctores is not null)
                {
                    App.Doctores.DeleteItems();
                    App.Doctores?.InsertItems(data);
                }
            }
        }

        public static void ObtenerNiveles()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaNiveles>($"VMedicA004/'{UsuarioIniciado?.UsuarioName}'", null);
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
                var data = await servicio.ResultadoGET<TablaCategorias>($"VMedicA005/'{UsuarioIniciado?.UsuarioName}'", null);
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
                var data = await servicio.ResultadoGET<TablaSubCategorias>($"VMedicA006/'{UsuarioIniciado?.UsuarioName}'", null);
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

        public static async void ObtenerProductos()
        {
            //Task.Run(async () =>
            //{
            var data = await servicio.ResultadoGET<TablaProductos>($"VMedicA008/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Productos");
                if (App.Productos is not null)
                {
                    App.Productos.DeleteItems();
                    App.Productos?.InsertItems(data);
                }
            }
            // });
        }

        public static async void ObtenerPresentaciones()
        {
            //Task.Run(async () =>
            //{
            var data = await servicio.ResultadoGET<TablaPresentaciones>($"VMedicA011/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Presentaciones");
                if (App.Presentaciones is not null)
                {
                    App.Presentaciones.DeleteItems();
                    App.Presentaciones?.InsertItems(data);
                }
            }
            //});
        }

        public static async void ObtenerTiposVisitas()
        {
            var data = await servicio.ResultadoGET<TablaTiposVisitas>($"VMedicA018/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Tipos de Visitas");
                if (App.Tiposvisitas is not null)
                {
                    App.Tiposvisitas.DeleteItems();
                    App.Tiposvisitas?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerVisitasMensuales()
        {
            var data = await servicio.ResultadoGET<TablaVisitasMensuales>($"VMedicA019/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Visitas Mensuales");
                if (App.Visitasmensuales is not null)
                {
                    App.Visitasmensuales.DeleteItems();
                    App.Visitasmensuales?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerNivelesdePrecios()
        {
            //Task.Run(async () =>
            //{
            var data = await servicio.ResultadoGET<TablaNivelesPrecio>($"VMedicA022/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Niveles de Precio");
                if (App.NivelesPrecio is not null)
                {
                    App.NivelesPrecio.DeleteItems();
                    App.NivelesPrecio?.InsertItems(data);
                }
            }
            //});
        }

        public static async void ObtenerLugaresdeVentas()
        {
            var data = await servicio.ResultadoGET<TablaLugaresVenta>($"VMedicA037/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert Lugares de Venta");
                if (App.Lugaresventas is not null)
                {
                    App.Lugaresventas.DeleteItems();
                    App.Lugaresventas?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerEspecialidades()
        {
            var data = await servicio.ResultadoGET<TablaClasesEspecializaciones>($"VMedicA040/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert especialidades");
                if (App.Especialidades is not null)
                {
                    App.Especialidades.DeleteItems();
                    App.Especialidades?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerMateriales()
        {
            var data = await servicio.ResultadoGET<TablaMateriales>($"VMedicA044/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert materiales");
                if (App.Materiales is not null)
                {
                    App.Materiales.DeleteItems();
                    App.Materiales?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerMuestras()
        {
            var data = await servicio.ResultadoGET<TablaMuestras>($"VMedicA045/'{UsuarioIniciado?.UsuarioName}'", null);
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
            }
        }

        public static void ObtenerAgendaCompleta()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaAgenda>($"VMedicA047/'{UsuarioIniciado?.UsuarioName}'", null);
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

        public static async void ObtenerCategoriasMedico()
        {
            var data = await servicio.ResultadoGET<TablaCategoriasMedico>($"VMedicA050/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert categorias medico");
                if (App.Categoriasmedico is not null)
                {
                    App.Categoriasmedico.DeleteItems();
                    App.Categoriasmedico?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerProductosPreferencias()
        {
            var data = await servicio.ResultadoGET<TablaProductoPreferencia>($"VMedicA051/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert productos preferencias");
                if (App.Productospreferencias is not null)
                {
                    App.Productospreferencias.DeleteItems();
                    App.Productospreferencias?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerMedicosProductosPreferencias()
        {
            var data = await servicio.ResultadoGET<TablaMedicoProductoPreferencia>($"VMedicA052/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert productos preferencias de medico");
                if (App.Medicoprodpreferencias is not null)
                {
                    App.Medicoprodpreferencias.DeleteItems();
                    App.Medicoprodpreferencias?.InsertItems(data);
                }
            }
        }

        public static async void ObtenerSKUProductos()
        {
            var data = await servicio.ResultadoGET<TablaSKUProducto>($"VMedicA053/'{UsuarioIniciado?.UsuarioName}'", null);
            if (data is not null)
            {
                Debug.WriteLine("Insert SKU productos");
                if (App.Skuproductos is not null)
                {
                    App.Skuproductos.DeleteItems();
                    App.Skuproductos?.InsertItems(data);
                }
            }
        }
    }
}
