using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.Models.WebserviceModels;

namespace VMedic.Services
{
    public static class SincronizacionDataBase
    {
        private static TablaUsuario? UsuarioIniciado = App.usuario?.GetItem();
        private static RestService servicio = new RestService();
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

        public static void ObtenerDoctores()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaDoctores>($"VMedicA003/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert Doctores");
                    if (App.doctores is not null)
                    {
                        App.doctores.DeleteItems();
                        App.doctores?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerNiveles()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaNiveles>($"VMedicA004/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert Niveles");
                    if (App.niveles is not null)
                    {
                        App.niveles.DeleteItems();
                        App.niveles?.InsertItems(data);
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
                    Console.WriteLine("Insert Categorias");
                    if (App.categorias is not null)
                    {
                        App.categorias.DeleteItems();
                        App.categorias?.InsertItems(data);
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
                    Console.WriteLine("Insert Sub Categorias");
                    if (App.subcategorias is not null)
                    {
                        App.subcategorias.DeleteItems();
                        App.subcategorias?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerTiposVisitas()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaTiposVisitas>($"VMedicA018/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert Tipos de Visitas");
                    if (App.tiposvisitas is not null)
                    {
                        App.tiposvisitas.DeleteItems();
                        App.tiposvisitas?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerVisitasMensuales()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaVisitasMensuales>($"VMedicA019/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert Visitas Mensuales");
                    if (App.visitasmensuales is not null)
                    {
                        App.visitasmensuales.DeleteItems();
                        App.visitasmensuales?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerLugaresdeVentas()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaLugaresVenta>($"VMedicA037/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert Lugares de Venta");
                    if (App.lugaresventas is not null)
                    {
                        App.lugaresventas.DeleteItems();
                        App.lugaresventas?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerEspecialidades()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaClasesEspecializaciones>($"VMedicA040/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert especialidades");
                    if (App.especialidades is not null)
                    {
                        App.especialidades.DeleteItems();
                        App.especialidades?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerMateriales()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaMateriales>($"VMedicA044/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert materiales");
                    if (App.materiales is not null)
                    {
                        App.materiales.DeleteItems();
                        App.materiales?.InsertItems(data);
                    }
                }
            });
        }

        public static void ObtenerMuestras()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaMuestras>($"VMedicA045/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert materiales");
                    if (App.muestras is not null)
                    {
                        if (App.muestras.IsEmpty())
                        {
                            App.muestras.DeleteItems();
                            App.muestras?.InsertItems(data);
                        }
                    }
                }
            });
        }

        public static void ObtenerSKUProductos()
        {
            Task.Run(async () =>
            {
                var data = await servicio.ResultadoGET<TablaSKUProducto>($"VMedicA053/'{UsuarioIniciado?.UsuarioName}'", null);
                if (data is not null)
                {
                    Console.WriteLine("Insert SKU productos");
                    if (App.skuproductos is not null)
                    {
                        App.skuproductos.DeleteItems();
                        App.skuproductos?.InsertItems(data);
                    }
                }
            });
        }
    }
}
