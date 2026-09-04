using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Visitas
{
    [AddINotifyPropertyChangedInterface]
    public partial class NuevaEvaluacionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ObservableRangeCollection<dynamic>? _productos;

        [ObservableProperty]
        private string? _productoSeleccionado;

        [ObservableProperty]
        private ObservableRangeCollection<string?>? _sKUs;

        [ObservableProperty]
        private string? _sKU;

        [ObservableProperty]
        private string? _cantidad;

        [ObservableProperty]
        private string? _observaciones;
        public dynamic? Producto { get; set; }
        private string? CodigoCliente { get; set; }
        public NuevaEvaluacionViewModel(string? codCliente)
        {
            CodigoCliente = codCliente;
            MostrarProductos();
            ValidarEditar();
        }

        //metodo para llenar la lista desplegable de los productos
        private async void MostrarProductos()
        {
            var ListaMuestras = await SincronizacionDataBase.ObtenerMuestras();
            await Task.Delay(1000);
            if (ListaMuestras is not null)
            {
                Productos = new ObservableRangeCollection<dynamic>(ListaMuestras.Select(m => new
                {
                    Descripcion = m.DESCRIPCION_MUESTRA,
                    ID = m.CODIGO_MUESTRA,
                    Cantidad = m.CANT_DISPONIBLE,
                }).ToList());
                await Task.Delay(1000);
                Producto = Productos.FirstOrDefault(P => DatosCompartidos.EvaluacionEditar is not null ? P.ID == DatosCompartidos.EvaluacionEditar.IdProducto : true);
            }
        }

        //metodo para llenar la lista desplegable de las presentaciones SKU
        public async void MostrarPresentaciones()
        {
            var ListaPresentaciones = (await SincronizacionDataBase.ObtenerSKUProductos())?.Where(SKU => SKU.PRODUCTO == Producto?.ID).Select(SKU => SKU.CODIGO_UNIDAD_VENTA).ToList();
            await Task.Delay(1000);
            if (ListaPresentaciones is not null)
            {
                SKUs = new ObservableRangeCollection<string?>(ListaPresentaciones);
                await Task.Delay(1000);
                SKU = SKUs.FirstOrDefault(SKU => DatosCompartidos.EvaluacionEditar is not null ? SKU == DatosCompartidos.EvaluacionEditar.Presentacion : true);
            }
        }

        //metodo que valida si se puede editar
        private async void ValidarEditar()
        {
            await Task.Delay(50);
            if (DatosCompartidos.EvaluacionEditar is not null)
            {
                Cantidad = DatosCompartidos.EvaluacionEditar.Cantidad;
                Observaciones = DatosCompartidos.EvaluacionEditar.Observaciones;
            }
        }

        //metodo para enviar los datos de registros de evaluacion de muestras al servidor
        public async void GuardarEvaluacion()
        {
            if (Cantidad is not null)
                if (Cantidad != "")
                {
                    if (int.Parse(Cantidad) <= Producto?.Cantidad)
                    {
                        var Restante = Producto?.Cantidad - int.Parse(Cantidad);

                        var NuevaEvaluacion = new TablaDetallesEvaluacion
                        {
                            IdCliente = CodigoCliente,
                            IdProducto = Producto?.ID,
                            Observaciones = Observaciones,
                            Cantidad = Cantidad,
                            Presentacion = SKU
                        };

                        var MuestraSeleciconada = App.Muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == Producto?.ID).FirstOrDefault();
                        if (MuestraSeleciconada is not null)
                        {
                            MuestraSeleciconada.CANT_DISPONIBLE = Restante;

                            App.Evaluaciondetalles?.InsertItem(NuevaEvaluacion);
                            App.Muestras?.UpdateITEM(MuestraSeleciconada);

                            var MainPage = App.Current?.Windows[0].Page;

                            if (MainPage is not null)
                            {
                                bool confirmar = await MainPage.DisplayAlert("Información", "Datos ingresados con éxito.\n¿Desea ingresar una nueva evaluación?", "SI", "NO");
                                if (confirmar)
                                {
                                    MostrarProductos();
                                    Cantidad = "";
                                    Observaciones = "";
                                }
                                else
                                {
                                    await Shell.Current.Navigation.PopAsync();
                                }
                            }
                        }
                    }
                    else
                    {
                        ToastMaker.Make("No hay muestras suficientes para entregar, reduzca la cantidad de muestras", App.Current?.Windows[0].Page);
                    }
                }
                else
                {
                    ToastMaker.Make("Ingrese la cantidad de muestras a entregar", App.Current?.Windows[0].Page);
                }
        }

        //metodo para cambiar los registros de evaluaciones de muestras
        public async void EditarEvaluacion()
        {
            if (Cantidad is not null)
                if (Cantidad != "")
                {
                    var MismoProducto = false;
                    var EvaluacionEditada = App.Evaluaciondetalles?.GetItems()?.FirstOrDefault(Edet => Edet.TableID == DatosCompartidos.EvaluacionEditar?.TableID);
                    if (EvaluacionEditada is not null)
                    {
                        MismoProducto = EvaluacionEditada.IdProducto == Producto?.ID;

                        var MuestraSeleciconada = App.Muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == Producto?.ID).FirstOrDefault();
                        if (MuestraSeleciconada is not null)
                        {
                            var CantidadMuestra = MuestraSeleciconada.CANT_DISPONIBLE;
                            var Agregado = CantidadMuestra + int.Parse(EvaluacionEditada.Cantidad + "");
                            if (int.Parse(Cantidad) <= (MismoProducto ? Agregado : Producto?.Cantidad))
                            {
                                var Restante = MismoProducto ? Agregado - int.Parse(Cantidad) : Producto?.Cantidad - int.Parse(Cantidad);

                                EvaluacionEditada.IdProducto = Producto?.ID;
                                EvaluacionEditada.Observaciones = Observaciones;
                                EvaluacionEditada.Cantidad = Cantidad;
                                EvaluacionEditada.Presentacion = SKU;

                                MuestraSeleciconada.CANT_DISPONIBLE = Restante;

                                App.Evaluaciondetalles?.UpdateITEM(EvaluacionEditada);
                                App.Muestras?.UpdateITEM(MuestraSeleciconada);

                                await App.Current?.Windows[0].Page?.DisplayAlert("Información", "Datos actualizados con éxito.", "ACEPTAR");

                                DatosCompartidos.EvaluacionEditar = null;

                                await Shell.Current.Navigation.PopAsync();
                            }
                            else
                            {
                                ToastMaker.Make("No hay muestras suficientes para entregar, reduzca la cantidad de muestras", App.Current?.Windows[0].Page);
                            }
                        }
                    }

                }
                else
                {
                    ToastMaker.Make("Ingrese la cantidad de muestras a entregar", App.Current?.Windows[0].Page);
                }
        }
    }
}
