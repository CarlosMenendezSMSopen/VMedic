using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.Services;
using VMedic.Utilities;
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
        private string? NiveldePRecio { get; set; }
        public NuevaEvaluacionViewModel(string? codCliente, string? nivelPRecio)
        {
            CodigoCliente = codCliente;
            NiveldePRecio = nivelPRecio;
            SincronizacionDataBase.ObtenerSKUProductos();
            MostrarProductos();
            ValidarEditar();
        }

        private async void MostrarProductos()
        {
            var ListaMuestras = App.muestras?.GetItems();
            if (ListaMuestras is not null)
            {
                Productos = new ObservableRangeCollection<dynamic>(ListaMuestras.Select(m => new
                {
                    Descripcion = m.DESCRIPCION_MUESTRA,
                    ID = m.CODIGO_MUESTRA,
                    Cantidad = m.CANT_DISPONIBLE,
                }).ToList());
                await Task.Delay(1000);
                Producto = Productos.FirstOrDefault();
            }
        }

        public async void MostrarPresentaciones()
        {
            var ListaPresentaciones = App.skuproductos?.GetItems()?.Where(SKU => SKU.PRODUCTO == Producto?.ID).Select(SKU => SKU.CODIGO_UNIDAD_VENTA).ToList();
            if (ListaPresentaciones is not null)
            {
                SKUs = new ObservableRangeCollection<string?>(ListaPresentaciones);
                await Task.Delay(1000);
                SKU = SKUs.FirstOrDefault();
            }
        }

        private async void ValidarEditar()
        {
            await Task.Delay(1000);
            if (DatosCompartidos.EvaluacionEditar is not null)
            {
                Producto = Productos?.FirstOrDefault(P => P.ID == DatosCompartidos.EvaluacionEditar.IdProducto);
                SKU = SKUs?.FirstOrDefault(sku => sku == DatosCompartidos.EvaluacionEditar.Presentacion);
                Cantidad = DatosCompartidos.EvaluacionEditar.Cantidad;
                Observaciones = DatosCompartidos.EvaluacionEditar.Observaciones;
            }
        }

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

                        var MuestraSeleciconada = App.muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == Producto?.ID).FirstOrDefault();
                        if (MuestraSeleciconada is not null)
                        {
                            MuestraSeleciconada.CANT_DISPONIBLE = Restante;

                            App.evaluaciondetalles?.InsertItem(NuevaEvaluacion);
                            App.muestras?.UpdateITEM(MuestraSeleciconada);

                            if (App.Current?.MainPage is not null)
                            {
                                bool confirmar = await App.Current.MainPage.DisplayAlert("Información", "Datos ingresados con éxito.\n¿Desea ingresar una nueva evaluación?", "SI", "NO");
                                if (confirmar)
                                {
                                    MostrarProductos();
                                    Cantidad = "";
                                    Observaciones = "";
                                }
                                else
                                {
                                    DatosCompartidos.StatusVolver = 1;
                                    await Shell.Current.Navigation.PopAsync();
                                }
                            }
                        }
                    }
                    else
                    {
                        ToastMaker.Make("No hay muestras suficientes para entregar, reduzca la cantidad de muestras", App.Current?.MainPage);
                    }
                }
                else
                {
                    ToastMaker.Make("Ingrese la cantidad de muestras a entregar", App.Current?.MainPage);
                }
        }

        public async void EditarEvaluacion()
        {
            if (Cantidad is not null)
                if (Cantidad != "")
                {
                    var MismoProducto = false;
                    var EvaluacionEditada = App.evaluaciondetalles?.GetItems()?.FirstOrDefault(Edet => Edet.TableID == DatosCompartidos.EvaluacionEditar?.TableID);
                    if (EvaluacionEditada is not null)
                    {
                        MismoProducto = EvaluacionEditada.IdProducto == Producto?.ID;

                        var MuestraSeleciconada = App.muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == Producto?.ID).FirstOrDefault();
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

                                App.evaluaciondetalles?.UpdateITEM(EvaluacionEditada);
                                App.muestras?.UpdateITEM(MuestraSeleciconada);

                                App.Current?.MainPage?.DisplayAlert("Información", "Datos actualizados con éxito.", "ACEPTAR");

                                DatosCompartidos.StatusVolver = 1;
                                await Shell.Current.Navigation.PopAsync();
                            }
                            else
                            {
                                ToastMaker.Make("No hay muestras suficientes para entregar, reduzca la cantidad de muestras", App.Current?.MainPage);
                            }
                        }
                    }

                }
                else
                {
                    ToastMaker.Make("Ingrese la cantidad de muestras a entregar", App.Current?.MainPage);
                }
        }
    }
}
