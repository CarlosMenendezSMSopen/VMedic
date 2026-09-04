using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
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
                Indicador = true;
                DatosCompartidos.ListaProductos?.Children.Clear();
                var ListaProductos = await SincronizacionDataBase.ObtenerProductos();
                var ListaPresentaciones = await SincronizacionDataBase.ObtenerPresentaciones();
                var ListaNivelesPrecios = await SincronizacionDataBase.ObtenerNivelesdePrecios();
                await Task.Delay(500);
                await Task.Run(() =>
                {
                    try
                    {
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
            var ListaProductos = DeviceInfo.Platform == DevicePlatform.Android ? Productos?.Skip(v).Take(20).ToList() : Productos;
            if (ListaProductos is not null)
            {
                for (int i = 0; i < ListaProductos.Count; i++)
                {
                    var producto = ListaProductos[i];

                    var Mainborder = new Border
                    {
                        Padding = 10,
                        Margin = new Thickness(15, 5),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10,
                        },
                        Background = new LinearGradientBrush
                        {
                            StartPoint = new Point(0, 0),
                            EndPoint = new Point(1, 0),
                            GradientStops =
                            {
                                new GradientStop
                                {
                                    Color = Color.FromArgb("#f4fbfd"),
                                    Offset = 0.0f
                                },
                                new GradientStop
                                {
                                    Color = Color.FromArgb("#f8fdff"),
                                    Offset = 1.0f
                                },
                            }
                        },
                    };

                    var container = new Grid
                    {
                        BindingContext = producto,
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
                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition());
                    ContenedorVisible.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });

                    ContenedorVisible.AddRowDefinition(new RowDefinition());
                    ContenedorVisible.AddRowDefinition(new RowDefinition());
                    ContenedorVisible.AddRowDefinition(new RowDefinition { Height = 15 });
                    ContenedorVisible.AddRowDefinition(new RowDefinition());

                    var lblCodigoProducto = new Label
                    {
                        Text = $"ID: {producto.PRODUCTO}",
                        FontAttributes = FontAttributes.Bold,
                        VerticalTextAlignment = TextAlignment.Start,
                    };

                    Grid.SetColumn(lblCodigoProducto, 0);
                    Grid.SetRow(lblCodigoProducto, 0);

                    var lblDescripcion = new Label
                    {
                        Text = producto.DESCRIPCION_PROD.Split(" - ")[1],
                        VerticalTextAlignment = TextAlignment.Start,
                    };

                    Grid.SetColumn(lblDescripcion, 0);
                    Grid.SetRow(lblDescripcion, 1);
                    Grid.SetColumnSpan(lblDescripcion, 3);

                    var lblCantidad = new Label
                    {
                        Text = $"Cantidad: {producto.CANTIDAD}",
                        VerticalTextAlignment = TextAlignment.Start,
                    };

                    Grid.SetColumn(lblCantidad, 0);
                    Grid.SetRow(lblCantidad, 3);

                    var lblPrecioU = new Label
                    {
                        Text = $"${producto.PRECIOU}/u",
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                    };

                    Grid.SetColumn(lblPrecioU, 2);
                    Grid.SetRow(lblPrecioU, 0);

                    ContenedorVisible.Children.Add(lblCodigoProducto);
                    ContenedorVisible.Children.Add(lblDescripcion);
                    ContenedorVisible.Children.Add(lblCantidad);
                    ContenedorVisible.Children.Add(lblPrecioU);

                    Grid.SetColumn(ContenedorVisible, 0);
                    Grid.SetRow(ContenedorVisible, 0);

                    var gradient = new LinearGradientBrush
                    {
                        StartPoint = new Point(0.5, 0),
                        EndPoint = new Point(0.5, 10),
                        GradientStops =
                        [
                            new GradientStop
                            {
                                Color = Color.FromArgb("#252693ff"),
                                Offset = 0.0f
                            },
                            new GradientStop
                            {
                                Color = Color.FromArgb("#50007fff"),
                                Offset = 1.0f
                            }
                        ]
                    };

                    var borderDetalles = new Border
                    {
                        IsVisible = false,
                        Padding = 15,
                        BackgroundColor = Color.FromArgb("#252693ff"),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10,
                        }
                    };

                    var btnDetalles = new ImageButton
                    {
                        HeightRequest = 40,
                        WidthRequest = 40,
                        Padding = 7.5,
                        VerticalOptions = LayoutOptions.End,
                        Source = new FontImageSource
                        {
                            FontFamily = "Icon",
                            Glyph = "\uE806",
                            Size = 20,
                            Color = (Color?)Application.Current?.Resources["Primary"],
                        }
                    };

                    Grid.SetColumn(btnDetalles, 2);
                    Grid.SetRow(btnDetalles, 2);
                    Grid.SetRowSpan(btnDetalles, 2);

                    btnDetalles.Clicked += async (s, e) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(null);
                            ImageButton? Contenedor = (ImageButton?)s;
                            if (Contenedor is not null)
                            {
                                borderDetalles.IsVisible = !borderDetalles.IsVisible;
                                await Contenedor.RotateTo(borderDetalles.IsVisible ? 180 : 0, 1000, Easing.CubicInOut);

                                PressedPreferences.EndPressed();
                            }
                        }
                    };

                    ContenedorVisible.Children.Add(btnDetalles);

                    var ContenedorDetalles = new Grid
                    {
                        BindingContext = producto,
                        ColumnSpacing = 10,
                        RowSpacing = 5,
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

                    ContenedorDetalles.Children.Add(label4);
                    ContenedorDetalles.Children.Add(lblNivel);
                    ContenedorDetalles.Children.Add(label5);
                    ContenedorDetalles.Children.Add(lblPresentacion);

                    Grid.SetColumn(borderDetalles, 0);
                    Grid.SetRow(borderDetalles, 1);

                    borderDetalles.Content = ContenedorDetalles;

                    container.Children.Add(ContenedorVisible);
                    container.Children.Add(borderDetalles);

                    Mainborder.Content = container;

                    App.Current?.Dispatcher.Dispatch(() =>
                    {
                        DatosCompartidos.ListaProductos?.Children.Add(Mainborder);
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
