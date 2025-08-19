using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.Views.Visitas;
using VMedic.Services;
using VMedic.Utilities;
using BaseViewModel = VMedic.Behaviors.BaseViewModel;

namespace VMedic.MVVM.ViewModels.Visitas
{
    [AddINotifyPropertyChangedInterface]
    public partial class EvaluacionesViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isStatus;

        [ObservableProperty]
        private bool _textoAviso;

        private RestService servicio = new RestService();
        public ICommand? RefreshCommand { get; private set; }
        private TablaVisitasPendientes? VisitasPendientes { get; set; }
        private string? NiveldePrecio { get; set; }
        private List<dynamic>? Evaluaciones { get; set; }
        public EvaluacionesViewModel(TablaVisitasPendientes visitas, string? nivelPrecio)
        {
            _isStatus = false;
            _textoAviso = false;
            VisitasPendientes = visitas;
            NiveldePrecio = nivelPrecio;
            SincronizacionDataBase.ObtenerMuestras();
            MostrarEvaluaciones();
            RefreshCommand = new Command(Refresh);
            PressedPreferences.EndPressed();
        }

        public void Refresh()
        {
            IsRefreshing = true;
            MostrarEvaluaciones();
        }

        public async void MostrarEvaluaciones()
        {
            IsStatus = true;
            TextoAviso = false;
            DatosCompartidos.ListaEvaluaciones?.Children.Clear();
            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    var ListaDetallesdeEvaluacion = App.evaluaciondetalles?.GetItems()?.Where(DE => DE.IdCliente == VisitasPendientes?.CodCliente).ToList();
                    var ListaMuestra = App.muestras?.GetItems();
                    if (ListaDetallesdeEvaluacion is not null && ListaMuestra is not null)
                    {
                        var ListaEvaluaciones = (from a in ListaDetallesdeEvaluacion
                                                 join b in ListaMuestra on a.IdProducto equals b.CODIGO_MUESTRA
                                                 select new
                                                 {
                                                     a.TableID,
                                                     a.Observaciones,
                                                     a.Cantidad,
                                                     a.IdProducto,
                                                     b.DESCRIPCION_MUESTRA,
                                                     a.Presentacion,
                                                 }).ToList();
                        if (ListaEvaluaciones.Count > 0)
                        {
                            Evaluaciones = ListaEvaluaciones.Cast<dynamic>().ToList();
                            GenerarListaCustom(0);
                        }
                        else
                        {
                            TextoAviso = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        IsStatus = false;
                        IsRefreshing = false;
                        ExceptionMessageMaker.Make("Error carga evaluaciones", ex.ToString(), ex.Message, App.Current?.MainPage);
                    });
                }
                finally
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        IsStatus = false;
                        IsRefreshing = false;
                    });
                }
            });
        }

        public void GenerarListaCustom(int i)
        {
            if (Evaluaciones is not null)
            {
                var firmaEvaluacion = App.evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente)?.Base64Image;
                foreach (var evaluacion in Evaluaciones.Skip(i).ToList())
                {
                    var gridContainer = new Grid
                    {
                        Padding = 10,
                        Margin = 5,
                        Background = Colors.White,
                        Shadow = new Shadow
                        {
                            Brush = Colors.Black,
                            Opacity = 0.3f,
                            Offset = new Point(5, 5),
                            Radius = 5
                        },
                        ColumnSpacing = 5,
                        BindingContext = evaluacion,
                    };

                    gridContainer.ColumnDefinitions.Add(new ColumnDefinition());
                    gridContainer.ColumnDefinitions.Add(new ColumnDefinition());

                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    var lbl_CodigoProducto = new Label
                    {
                        Text = $"Código {evaluacion.IdProducto}",
                        HorizontalTextAlignment = TextAlignment.Center,
                        BindingContext = evaluacion,
                    };

                    Grid.SetColumnSpan(lbl_CodigoProducto, 3);
                    Grid.SetRow(lbl_CodigoProducto, 0);

                    var lbl_NombreProducto = new Label
                    {
                        Text = evaluacion.DESCRIPCION_MUESTRA,
                        HorizontalTextAlignment = TextAlignment.Start,
                        FontSize = 16,
                        FontAttributes = FontAttributes.Bold,
                        BindingContext = evaluacion,
                    };

                    Grid.SetColumnSpan(lbl_NombreProducto, 3);
                    Grid.SetRow(lbl_NombreProducto, 1);


                    var lbl_Cantidad = new Label
                    {
                        Text = $"Cantidad: {evaluacion.Cantidad}",
                        HorizontalTextAlignment = TextAlignment.Start,
                        BindingContext = evaluacion,
                    };

                    Grid.SetColumnSpan(lbl_Cantidad, 3);
                    Grid.SetRow(lbl_Cantidad, 2);

                    var btn_eliminar = new ImageButton
                    {
                        Source = "delete.png",
                        HeightRequest = 30,
                        WidthRequest = 30,
                        Margin = 10,
                        IsEnabled = firmaEvaluacion is null,
                        BindingContext = evaluacion,
                    };

                    btn_eliminar.Clicked += async (sender, args) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(sender);

                            if (App.Current?.MainPage is not null)
                            {
                                var eliminar = await App.Current.MainPage.DisplayAlert("Información", "¿Desea eliminar esta evaluación?", "SI", "NO");
                                if (eliminar)
                                {
                                    if (sender is ImageButton boton)
                                    {
                                        dynamic contexto = boton.BindingContext;

                                        var MuestraSeleccionada = App.muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == contexto.IdProducto).FirstOrDefault();
                                        var EvaluacionSeleciconada = App.evaluaciondetalles?.GetItems()?.Where(ED => ED.TableID == contexto.TableID).FirstOrDefault();
                                        var IDDetalleEvaluacion = EvaluacionSeleciconada?.TableID;
                                        var CantidadMuestra = MuestraSeleccionada?.CANT_DISPONIBLE;
                                        var Resultado = CantidadMuestra + int.Parse(contexto.Cantidad);

                                        if (MuestraSeleccionada is not null && EvaluacionSeleciconada is not null)
                                        {
                                            MuestraSeleccionada.CANT_DISPONIBLE = Resultado;
                                        }

                                        var GridsDynamic = (DatosCompartidos.ListaEvaluaciones?.Children as IEnumerable<object>)?.OfType<Grid>().Where(grid =>
                                        {
                                            dynamic contexto = grid.BindingContext;

                                            return contexto.TableID == IDDetalleEvaluacion;
                                        }).FirstOrDefault();

                                        DatosCompartidos.ListaEvaluaciones?.Children.Remove(GridsDynamic);

                                        App.muestras?.UpdateITEM(MuestraSeleccionada);
                                        App.evaluaciondetalles?.DeleteItem(EvaluacionSeleciconada);

                                        if (DatosCompartidos.ListaEvaluaciones?.Children.Count == 0)
                                        {
                                            TextoAviso = true;
                                        }

                                        ToastMaker.Make("Evaluación eliminada con éxito", App.Current.MainPage);
                                    }
                                }
                            }

                            PressedPreferences.EndPressed();
                        }
                    };

                    Grid.SetColumn(btn_eliminar, 0);
                    Grid.SetRow(btn_eliminar, 3);

                    var btn_editar = new ImageButton
                    {
                        Source = "edit.png",
                        HeightRequest = 30,
                        WidthRequest = 30,
                        Margin = 10,
                        IsEnabled = firmaEvaluacion is null,
                    };

                    btn_editar.Clicked += (sender, args) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(sender);

                            if (sender is ImageButton boton)
                            {
                                DatosCompartidos.EvaluacionEditar = boton.BindingContext;

                                Shell.Current.Navigation.PushAsync(new NuevaEvaluacionView(VisitasPendientes?.CodCliente, NiveldePrecio));
                            }
                        }
                    };

                    Grid.SetColumn(btn_editar, 1);
                    Grid.SetRow(btn_editar, 3);

                    gridContainer.Children.Add(lbl_CodigoProducto);
                    gridContainer.Children.Add(lbl_NombreProducto);
                    gridContainer.Children.Add(lbl_Cantidad);
                    gridContainer.Children.Add(btn_eliminar);
                    gridContainer.Children.Add(btn_editar);

                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        DatosCompartidos.ListaEvaluaciones?.Children.Add(gridContainer);
                    });
                }
            }
        }

        public async void EnviarEvaluaciones(Frame containerbtn_agregarevaluacion, ImageButton btn_sign)
        {
            try
            {
                if (Evaluaciones is not null)
                {
                    var EvaluacionEnc = App.evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente);
                    var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                    {
                        OperacionID = $"VMedicA046",
                        Parametros = $"'{VisitasPendientes?.CodCliente}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.IdProducto}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}{E.Cantidad}"))}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.Observaciones}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}"))}','{VisitasPendientes?.CodVendedor}','{VisitasPendientes?.IDTipoVisita}','{VisitasPendientes?.Comentarios}','{VisitasPendientes?.Latitud}','{VisitasPendientes?.Longitud}','{VisitasPendientes?.FechaGPS}','{EvaluacionEnc?.Base64Image}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.IdProducto}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}{E.Presentacion}"))}'",
                        ClavesVacias = 1,
                        TipoRestService = 2,
                        CodigoCliente = VisitasPendientes?.CodCliente,
                    };

                    if (IsInternet.Avilable())
                    {
                        var datos = (await servicio.ResultadoPOST(SolicitudEnviar.OperacionID, SolicitudEnviar.Parametros, valores => new Resultado
                        {
                            Id = valores[0],
                            MSG = valores[1],
                            Codigo = valores[2]
                        }))?.FirstOrDefault();

                        if (datos is not null)
                        {
                            var Codigos = datos.Codigo?.Split(CaracteresEspeciales.SECCION);
                            if (Codigos is not null)
                            {
                                foreach (var codigo in Codigos)
                                {
                                    var muestraActualizar = App.muestras?.GetItems()?.FirstOrDefault(M => M.CODIGO_MUESTRA == codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[0]);
                                    var clienteActualizar = App.doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == VisitasPendientes?.CodCliente);

                                    if (muestraActualizar is not null && clienteActualizar is not null)
                                    {
                                        muestraActualizar.CANT_DISPONIBLE = int.Parse(codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[1]);
                                        clienteActualizar.Visitas = 1;

                                        App.muestras?.UpdateITEM(muestraActualizar);
                                        App.doctores?.UpdateITEM(clienteActualizar);

                                        var detallesEliminar = App.evaluaciondetalles?.GetItems()?.Where(Edet => Edet.IdCliente == VisitasPendientes?.CodCliente).ToList();
                                        var encabezadoEliminar = App.evaluacionencabezado?.GetItems()?.Where(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente).ToList();

                                        if (detallesEliminar is not null && encabezadoEliminar is not null)
                                        {
                                            App.evaluaciondetalles?.DeleteItems(detallesEliminar);
                                            App.evaluacionencabezado?.DeleteItems(encabezadoEliminar);

                                            containerbtn_agregarevaluacion.IsEnabled = true;
                                            containerbtn_agregarevaluacion.BackgroundColor = (Color?)Application.Current?.Resources["Primary"];
                                            btn_sign.IsEnabled = true;

                                            MostrarEvaluaciones();
                                        }
                                    }
                                }
                            }

                            switch (datos.Id)
                            {
                                case "1":
                                    ToastMaker.Make(datos.MSG, App.Current?.MainPage);
                                    break;
                                case "2":
                                    ToastMaker.Make(datos.MSG, App.Current?.MainPage);
                                    break;
                                case "3":
                                    ToastMaker.Make(datos.MSG, App.Current?.MainPage);
                                    break;
                                default:
                                    ToastMaker.Make($"Lo sentimos, ha ocurrido un error inesperado: {datos.MSG}", App.Current?.MainPage);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        ToastMaker.Make("No hay conexión a Internet, verifique su plan de datos para enviar la evaluación automáticamente", App.Current?.MainPage);
                        App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                    }
                }

                PressedPreferences.EndPressed();
            }
            catch (Exception ex)
            {
                ExceptionMessageMaker.Make("Error al elviar las evaluaciones", ex.ToString(), ex.Message, App.Current?.MainPage);
            }
        }
    }
}
