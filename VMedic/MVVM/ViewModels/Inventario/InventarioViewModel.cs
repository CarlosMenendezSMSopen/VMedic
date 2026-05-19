using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Views.Médicos;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.ViewModels.Inventario
{
    [AddINotifyPropertyChangedInterface]
    public partial class InventarioViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _indicador;

        [ObservableProperty]
        private bool _refreshing;

        [ObservableProperty]
        private bool _visibilidadAviso;
        public List<dynamic>? Productos { get; set; }
        public InventarioViewModel()
        {
            MostrarProductos();
        }

        //metodo para general la lista de los productos fusionando los registros de 3 bases de datos
        public async void MostrarProductos()
        {
            try
            {
                await Task.Delay(1000);
                Indicador = true;
                DatosCompartidos.ListaProductos?.Children.Clear();
                await Task.Run(() =>
                {
                    try
                    {
                        SincronizacionDataBase.ObtenerProductos();
                        SincronizacionDataBase.ObtenerPresentaciones();
                        SincronizacionDataBase.ObtenerNivelesdePrecios();

                        var ListaProductos = App.Productos?.GetItems();
                        var ListaPresentaciones = App.Presentaciones?.GetItems();
                        var ListaNivelesPrecios = App.NivelesPrecio?.GetItems();

                        if (ListaNivelesPrecios is not null && ListaProductos is not null && DatosCompartidos.TextoBusquedaProductos is not null)
                        {
                            var ListadeProductos = (from pre in ListaPresentaciones
                                                    join np in ListaNivelesPrecios on pre.NIVEL_PRECIO equals np.NIVEL_PRECIO
                                                    join pro in ListaProductos on pre.PRODUCTO equals pro.PRODUCTO
                                                    orderby np.NIVEL_PRECIO, pre.FACTOR_DE_CONVERSION
                                                    where DatosCompartidos.TextoBusquedaProductos == "" || (pro.DESCRIPCION_PROD is not null && pro.DESCRIPCION_PROD.Contains(DatosCompartidos.TextoBusquedaProductos, StringComparison.OrdinalIgnoreCase)) || (pro.PRODUCTO is not null && pro.PRODUCTO.Contains(DatosCompartidos.TextoBusquedaProductos, StringComparison.OrdinalIgnoreCase))
                                                    select new
                                                    {
                                                        pre.PRODUCTO,
                                                        pro.DESCRIPCION_PROD,
                                                        pro.PRECIOU,
                                                        pre.DESCRP_UNIDAD_VENTA,
                                                        np.DESCRIPCION,
                                                        np.NIVEL_PRECIO,
                                                        pro.CANTIDAD,
                                                    }).ToList();

                            Productos = [.. ListadeProductos.Cast<dynamic>()];

                            GenerarCustomLista(0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }

                });
            }
            catch (Exception ex)
            {
                Indicador = false;
                Refreshing = false;
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error Lista de Productos", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
            }
            finally
            {
                Indicador = false;
                Refreshing = false;
                VisibilidadAviso = DatosCompartidos.ListaProductos?.Children.Count == 0;
            }
        }

        //metodo para general la vista de la lista de productos
        public void GenerarCustomLista(int v)
        {
            var ListaProductos = DeviceInfo.Platform == DevicePlatform.Android ? Productos?.Skip(v).Take(30).ToList() : Productos;
            if (ListaProductos is not null)
            {
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    var producto = ListaProductos[i];

                    var container = new Grid
                    {
                        BindingContext = producto,
                        BackgroundColor = Color.FromArgb("#102693ff"),
                    };

                    container.AddColumnDefinition(new ColumnDefinition());

                    container.AddRowDefinition(new RowDefinition());
                    container.AddRowDefinition(new RowDefinition());

                    var ContenedorVisible = new Grid
                    {
                        ClassId = i + "",
                        BindingContext = producto,
                        Padding = 10,
                        ColumnSpacing = 10,
                    };

                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });
                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition { Width = 1 });
                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });
                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition());

                    ContenedorVisible.AddRowDefinition(new RowDefinition());
                    ContenedorVisible.AddRowDefinition(new RowDefinition { Height = 1 });
                    ContenedorVisible.AddRowDefinition(new RowDefinition());
                    ContenedorVisible.AddRowDefinition(new RowDefinition { Height = 1 });
                    ContenedorVisible.AddRowDefinition(new RowDefinition());

                    var Separador1 = new Grid
                    {
                        Background = Colors.Gray
                    };

                    Grid.SetColumn(Separador1, 1);
                    Grid.SetRowSpan(Separador1, 5);

                    var Separador2 = new Grid
                    {
                        Background = Colors.Gray
                    };

                    Grid.SetColumn(Separador2, 2);
                    Grid.SetRow(Separador2, 1);
                    Grid.SetColumnSpan(Separador2, 3);

                    var Separador3 = new Grid
                    {
                        Background = Colors.Gray
                    };

                    Grid.SetColumn(Separador3, 3);
                    Grid.SetRowSpan(Separador3, 3);

                    var lblCodigoProducto = new Label
                    {
                        Text = producto.PRODUCTO,
                        FontAttributes = FontAttributes.Bold,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(lblCodigoProducto, 0);
                    Grid.SetRow(lblCodigoProducto, 0);
                    Grid.SetRowSpan(lblCodigoProducto, 5);

                    var label1 = new Label
                    {
                        Text = "Descripción: ",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(label1, 2);
                    Grid.SetRow(label1, 0);

                    var lblDescripcion = new Label
                    {
                        Text = producto.DESCRIPCION_PROD,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(lblDescripcion, 4);
                    Grid.SetRow(lblDescripcion, 0);

                    var label2 = new Label
                    {
                        Text = "Cantidad: ",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(label2, 2);
                    Grid.SetRow(label2, 2);

                    var lblCantidad = new Label
                    {
                        Text = producto.CANTIDAD + "",
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(lblCantidad, 4);
                    Grid.SetRow(lblCantidad, 2);

                    var label3 = new Label
                    {
                        Text = "Precio U.: ",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(label3, 2);
                    Grid.SetRow(label3, 4);

                    var lblPrecio = new Label
                    {
                        Text = "$" + producto.PRECIOU,
                        VerticalTextAlignment = TextAlignment.Center,
                    };

                    Grid.SetColumn(lblPrecio, 4);
                    Grid.SetRow(lblPrecio, 4);

                    ContenedorVisible.Children.Add(Separador1);
                    ContenedorVisible.Children.Add(Separador2);
                    ContenedorVisible.Children.Add(lblCodigoProducto);
                    ContenedorVisible.Children.Add(label1);
                    ContenedorVisible.Children.Add(lblDescripcion);
                    ContenedorVisible.Children.Add(label2);
                    ContenedorVisible.Children.Add(lblCantidad);
                    ContenedorVisible.Children.Add(label3);
                    ContenedorVisible.Children.Add(lblPrecio);

                    var gradient = new LinearGradientBrush
                    {
                        StartPoint = new Point(0.5, 0),
                        EndPoint = new Point(0.5, 10),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop { Color = Color.FromArgb("#252693ff"), Offset = 0.0f },
                            new GradientStop { Color = Color.FromArgb("#50007fff"), Offset = 1.0f }
                        }
                    };

                    var ContenedorDetalles = new Grid
                    {
                        BindingContext = producto,
                        IsVisible = false,
                        Padding = new Thickness(20, 10),
                        ColumnSpacing = 10,
                        RowSpacing = 5,
                        BackgroundColor = Color.FromArgb("#252693ff"),
                    };

                    ContenedorDetalles.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });
                    ContenedorDetalles.AddColumnDefinition(new ColumnDefinition());
                    ContenedorDetalles.AddRowDefinition(new RowDefinition());
                    ContenedorDetalles.AddRowDefinition(new RowDefinition());

                    var label4 = new Label
                    {
                        Text = $"Nivel {producto.NIVEL_PRECIO}: ",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold
                    };

                    Grid.SetColumn(label4, 0);
                    Grid.SetRow(label4, 0);

                    var lblNivel = new Label
                    {
                        Text = producto.DESCRIPCION,
                    };

                    Grid.SetColumn(lblNivel, 1);
                    Grid.SetRow(lblNivel, 0);

                    var label5 = new Label
                    {
                        Text = "Presentación: ",
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold,
                    };

                    Grid.SetColumn(label5, 0);
                    Grid.SetRow(label5, 1);

                    var lblPresentacion = new Label
                    {
                        Text = producto.DESCRP_UNIDAD_VENTA,
                    };

                    Grid.SetColumn(lblPresentacion, 1);
                    Grid.SetRow(lblPresentacion, 1);

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += async (s, e) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(null);
                            Grid? Contenedor = (Grid?)s;
                            if (Contenedor is not null)
                            {
                                ContenedorDetalles.IsVisible = !ContenedorDetalles.IsVisible;
                                //Contenedor.Background = ContenedorDetalles.IsVisible ? Colors.LightGray : int.Parse(Contenedor.ClassId) % 2 == 0 ? Color.FromArgb("#252693ff") : Color.FromArgb("#50007fff");
                                //Contenedor.Background = ContenedorDetalles.IsVisible ? Colors.LightGray : Colors.Transparent;
                                PressedPreferences.EndPressed();
                            }
                        }
                    };

                    ContenedorVisible.GestureRecognizers.Add(tapGestureRecognizer);

                    Grid.SetColumn(ContenedorVisible, 0);
                    Grid.SetRow(ContenedorVisible, 0);

                    ContenedorDetalles.Children.Add(label4);
                    ContenedorDetalles.Children.Add(lblNivel);
                    ContenedorDetalles.Children.Add(label5);
                    ContenedorDetalles.Children.Add(lblPresentacion);

                    Grid.SetColumn(ContenedorDetalles, 0);
                    Grid.SetRow(ContenedorDetalles, 1);

                    container.Children.Add(ContenedorVisible);
                    container.Children.Add(ContenedorDetalles);

                    App.Current?.Dispatcher.Dispatch(() =>
                    {
                        DatosCompartidos.ListaProductos?.Children.Add(container);
                    });
                }
            }
        }

        public void Refresh()
        {
            Refreshing = true;
            MostrarProductos();
        }

        public async Task CargarMasProductos(int itemcount)
        {
            Indicador = true;
            await Task.Delay(1000);

            await Task.Run(() =>
            {
                GenerarCustomLista(itemcount);

                App.Current?.Dispatcher.Dispatch(() =>
                {
                    Indicador = false;
                });
            });
        }
    }
}
