using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMedic.Global;
using VMedic.Interfaces;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.Views.Visitas;
using VMedic.Servicios;
using VMedic.Utilidades;
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

        private readonly RestService Servicio = new();
        private TablaVisitasPendientes? VisitasPendientes { get; set; }
        private List<dynamic>? Evaluaciones { get; set; }
        private int ActualizarUbicacion { get; set; }
        public EvaluacionesViewModel(TablaVisitasPendientes visitas, int actualizar)
        {
            _isStatus = false;
            _textoAviso = false;
            VisitasPendientes = visitas;
            ActualizarUbicacion = actualizar;
            var ListaEvaluacionesPendientes = App.SolicitudesPendientes?.GetItems()?.Where(SP => SP.OperacionID == "VMedicA046" && SP.CodigoCliente == VisitasPendientes?.CodCliente).ToList();
            if (ListaEvaluacionesPendientes?.Count > 0)
            {
                App.Current?.Dispatcher.Dispatch(delegate
                {
                    ToastMaker.Make("Tienes evaluaciones de este médico pendientes a enviar", App.Current?.Windows[0].Page);
                });
            }
            MostrarEvaluaciones();
            PressedPreferences.EndPressed();
        }

        //metodo para refrescar la lista
        public void Refresh()
        {
            IsRefreshing = true;
            MostrarEvaluaciones();
        }

        //metodo que genera la lista de las evaluaciones de muestras
        public async void MostrarEvaluaciones()
        {
            IsStatus = true;
            TextoAviso = false;
            var ListaDetallesdeEvaluacion = App.Evaluaciondetalles?.GetItems()?.Where(DE => DE.IdCliente == VisitasPendientes?.CodCliente).ToList();
            var ListaMuestra = await SincronizacionDataBase.ObtenerMuestras();
            DatosCompartidos.ListaEvaluaciones?.Children.Clear();
            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
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
                            var productosEvaluacionesPendientes = App.SolicitudesPendientes?.GetItems()?.Where(SP => SP.OperacionID == "VMedicA046" && SP.CodigoCliente == VisitasPendientes?.CodCliente).Select(SP => SP.Parametros?.Split("','")[1].Split(CaracteresEspeciales.SECCION)).ToList();
                            Evaluaciones = [.. ListaEvaluaciones.Cast<dynamic>()];
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
                        ExceptionMessageMaker.Make("Error carga evaluaciones", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
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

        //metodo para personalizar la vista de los registros
        public void GenerarListaCustom(int i)
        {
            if (Evaluaciones is not null)
            {
                var firmaEvaluacion = App.Evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente)?.Base64Image;
                foreach (var evaluacion in Evaluaciones.Skip(i).ToList())
                {
                    var borderMain = new Border
                    {
                        Margin = new Thickness(5, 15, 5, 0),
                        Padding = 10,
                        BackgroundColor = Colors.White,
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10,
                        },
                        Shadow = new Shadow
                        {
                            Brush = Colors.Black,
                            Opacity = 0.3f,
                            Offset = new Point(5, 5),
                            Radius = 5
                        },
                    };

                    var gridContainer = new Grid();

                    gridContainer.ColumnDefinitions.Add(new ColumnDefinition());
                    gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = 50 });
                    gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = 50 });

                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    gridContainer.RowDefinitions.Add(new RowDefinition { Height = 45 });

                    var lbl_CodigoProducto = new Label
                    {
                        Text = $"Código {evaluacion.IdProducto}",
                        HorizontalTextAlignment = TextAlignment.Center,
                        FontSize = 16,
                        BindingContext = evaluacion,
                    };

                    Grid.SetColumnSpan(lbl_CodigoProducto, 3);
                    Grid.SetRow(lbl_CodigoProducto, 0);

                    var lbl_NombreProducto = new Label
                    {
                        Text = evaluacion.DESCRIPCION_MUESTRA,
                        HorizontalTextAlignment = TextAlignment.Start,
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
                        HeightRequest = 50,
                        WidthRequest = 50,
                        Padding = 12.5,
                        Margin = new Thickness(5, 10, 5, 0),
                        IsEnabled = firmaEvaluacion is null,
                        BindingContext = evaluacion,
                    };

                    btn_eliminar.Clicked += async (sender, args) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(sender);

                            var MainPage = App.Current?.Windows[0].Page;

                            if (MainPage is not null)
                            {
                                var eliminar = await MainPage.DisplayAlert("Información", "¿Desea eliminar esta evaluación?", "SI", "NO");
                                if (eliminar)
                                {
                                    if (sender is ImageButton boton)
                                    {
                                        dynamic contexto = boton.BindingContext;

                                        var MuestraSeleccionada = App.Muestras?.GetItems()?.Where(M => M.CODIGO_MUESTRA == contexto.IdProducto).FirstOrDefault();
                                        var EvaluacionSeleciconada = App.Evaluaciondetalles?.GetItems()?.Where(ED => ED.TableID == contexto.TableID).FirstOrDefault();
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

                                        App.Muestras?.UpdateITEM(MuestraSeleccionada);
                                        App.Evaluaciondetalles?.DeleteItem(EvaluacionSeleciconada);

                                        if (DatosCompartidos.ListaEvaluaciones?.Children.Count == 0)
                                        {
                                            TextoAviso = true;
                                        }

                                        ToastMaker.Make("Evaluación eliminada con éxito", App.Current?.Windows[0].Page);
                                    }
                                }
                            }

                            PressedPreferences.EndPressed();
                        }
                    };

                    Grid.SetColumn(btn_eliminar, 2);
                    Grid.SetRow(btn_eliminar, 2);

                    var btn_editar = new ImageButton
                    {
                        Source = "edit.png",
                        HeightRequest = 50,
                        WidthRequest = 50,
                        Padding = 12.5,
                        Margin = new Thickness(5, 10, 5, 0),
                        IsEnabled = firmaEvaluacion is null,
                        BindingContext = evaluacion,
                    };

                    btn_editar.Clicked += (sender, args) =>
                    {
                        if (PressedPreferences.ValidatePressing())
                        {
                            PressedPreferences.Pressing(sender);

                            if (sender is ImageButton boton)
                            {
                                DatosCompartidos.EvaluacionEditar = boton.BindingContext;

                                Shell.Current.Navigation.PushAsync(new NuevaEvaluacionView(VisitasPendientes?.CodCliente));
                            }
                        }
                    };

                    Grid.SetColumn(btn_editar, 1);
                    Grid.SetRow(btn_editar, 2);

                    gridContainer.Children.Add(lbl_CodigoProducto);
                    gridContainer.Children.Add(lbl_NombreProducto);
                    gridContainer.Children.Add(lbl_Cantidad);
                    gridContainer.Children.Add(btn_eliminar);
                    gridContainer.Children.Add(btn_editar);

                    borderMain.Content = gridContainer;

                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        DatosCompartidos.ListaEvaluaciones?.Children.Add(borderMain);
                    });
                }
            }
        }

        //metodo para enviar las evaluaciones
        public async void EnviarEvaluaciones(Border containerbtn_agregarevaluacion, ImageButton btn_sign)
        {
            try
            {
                if (Evaluaciones is not null)
                {
                    var EvaluacionEnc = App.Evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente);

                    var Medico = App.Doctores?.GetItems()?.FirstOrDefault(M => M.CODIGO_DE_CLIENTE == VisitasPendientes?.CodCliente);

                    var SolicitudEnviar = new TablaSolicitudesNoEnviadas
                    {
                        IDSolicitud = App.SolicitudesPendientes?.GetItems()?.Where(S => S.OperacionID == "VMedicA046").ToList().Count,
                        OperacionID = $"VMedicA046",
                        Parametros = $"'{VisitasPendientes?.CodCliente}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.IdProducto}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}{E.Cantidad}"))}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.Observaciones}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}"))}','{VisitasPendientes?.CodVendedor}','{VisitasPendientes?.IDTipoVisita}','{VisitasPendientes?.Comentarios}','{VisitasPendientes?.Latitud}','{VisitasPendientes?.Longitud}','{VisitasPendientes?.FechaGPS}','{EvaluacionEnc?.Base64Image}','{string.Join(CaracteresEspeciales.SECCION, Evaluaciones.Select(E => $"{E.IdProducto}{CaracteresEspeciales.BARRA_VERTICAL_ROTA}{E.Presentacion}"))}','{Medico?.LATITUD}','{Medico?.LONGITUD}',{ActualizarUbicacion}",
                        ClavesVacias = 1,
                        TipoRestService = 2,
                        CodigoCliente = VisitasPendientes?.CodCliente,
                        ModuloSolicitud = 1
                    };

                    var datos = (await Servicio.ResultadoPOST(SolicitudEnviar.OperacionID, SolicitudEnviar.Parametros, valores => new Resultado
                        {
                            Id = valores[0],
                            MSG = valores[1],
                            Codigo = valores[2]
                        })
                    )?.FirstOrDefault();

                    if (datos is not null)
                    {
                        var Codigos = datos.Codigo?.Split(CaracteresEspeciales.SECCION);
                        if (Codigos is not null)
                        {
                            foreach (var codigo in Codigos)
                            {
                                var muestraActualizar = App.Muestras?.GetItems()?.FirstOrDefault(M => M.CODIGO_MUESTRA == codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[0]);

                                if (muestraActualizar is not null)
                                {
                                    muestraActualizar.CANT_DISPONIBLE = int.Parse(codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[1]);

                                    App.Muestras?.UpdateITEM(muestraActualizar);

                                    var detallesEliminar = App.Evaluaciondetalles?.GetItems()?.Where(Edet => Edet.IdCliente == VisitasPendientes?.CodCliente).ToList();
                                    var encabezadoEliminar = App.Evaluacionencabezado?.GetItems()?.Where(Eenc => Eenc.IdCliente == VisitasPendientes?.CodCliente).ToList();

                                    if (detallesEliminar is not null && encabezadoEliminar is not null)
                                    {
                                        App.Evaluaciondetalles?.DeleteItems(detallesEliminar);
                                        App.Evaluacionencabezado?.DeleteItems(encabezadoEliminar);

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
                                ToastMaker.Make(datos.MSG, App.Current?.Windows[0].Page);
                                await Shell.Current.Navigation.PopAsync();
                                break;
                            case "2":
                                ToastMaker.Make(datos.MSG, App.Current?.Windows[0].Page);
                                break;
                            case "3":
                                ToastMaker.Make(datos.MSG, App.Current?.Windows[0].Page);
                                break;
                            default:
                                ToastMaker.Make($"Lo sentimos, ha ocurrido un error inesperado: {datos.MSG}", App.Current?.Windows[0].Page);
                                break;
                        }
                    }
                    else if (DatosCompartidos.ErrorResponseValue is not null)
                    {
                        DatosCompartidos.CantidadIntentos++;

                        if (DatosCompartidos.CantidadIntentos == 4)
                        {
                            var MainPage = App.Current?.Windows[0].Page;

                            if (MainPage is not null)
                            {
                                await MainPage.DisplayAlert("No fue posible completar el envío", "Después de 3 intentos, el registro se ha almacenado de forma local en el módulo de sincronización, ubicación en la que se podrá enviar nuevamente más tarde", "OK");

                                containerbtn_agregarevaluacion.IsEnabled = true;
                                containerbtn_agregarevaluacion.BackgroundColor = (Color?)Application.Current?.Resources["Primary"];
                                btn_sign.IsEnabled = true;

                                App.Evaluaciondetalles?.DeleteItems();
                                App.Evaluacionencabezado?.DeleteItems();

                                MostrarEvaluaciones();

                                App.SolicitudesPendientes?.InsertItem(SolicitudEnviar);
                                DatosCompartidos.CantidadIntentos = 0;
                            }
                        }
                        else
                        {
                            ToastMaker.Make(DatosCompartidos.ErrorResponseValue.FirstOrDefault().Value, App.Current?.Windows[0].Page);
                        }
                    }
                }

                PressedPreferences.EndPressed();
            }
            catch (Exception ex)
            {
                ExceptionMessageMaker.Make("Error al elviar las evaluaciones", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
            }
        }
    }
}
