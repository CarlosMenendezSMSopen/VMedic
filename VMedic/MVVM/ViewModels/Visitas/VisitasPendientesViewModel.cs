using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.ViewModels.Visitas
{
    [AddINotifyPropertyChangedInterface]
    public partial class VisitasPendientesViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _indicador;

        [ObservableProperty]
        private bool _isRefreshing;
        public List<TablaSolicitudesNoEnviadas>? VisitasPendientes { get; set; }

        private readonly RestService servicio = new();
        public VisitasPendientesViewModel()
        {
            MostrarVisitasPendientes();
        }

        public void Refresh()
        {
            IsRefreshing = true;
            MostrarVisitasPendientes();
        }

        public async void MostrarVisitasPendientes()
        {
            Indicador = true;
            DatosCompartidos.ListaVisitasPendientes?.Children.Clear();
            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    VisitasPendientes = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDVisitas.Contains(SP.OperacionID)).ToList();
                    GenerateListaCustom(0);
                }
                catch (Exception ex)
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        Indicador = false;
                        IsRefreshing = false;
                        Debug.WriteLine(ex);
                        ExceptionMessageMaker.Make("Error carga visitas pendientes", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                    });
                }
                finally
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        Indicador = false;
                        IsRefreshing = false;
                    });
                }
            });
        }

        public void GenerateListaCustom(int i)
        {
            if (VisitasPendientes is not null)
            {
                var lista = DeviceInfo.Platform == DevicePlatform.Android ? [.. VisitasPendientes.Skip(i).Take(30)] : VisitasPendientes;
                if (lista is not null)
                {
                    foreach (var visita in lista)
                    {
                        var container = new Grid
                        {
                            Padding = 10,
                            Margin = 5,
                            Shadow = new Shadow
                            {
                                Brush = Colors.Black,
                                Opacity = 0.3f,
                                Offset = new Point(5, 5),
                                Radius = 5
                            },
                            ColumnSpacing = 5,
                            BindingContext = visita,
                        };

                        container.ColumnDefinitions.Add(new ColumnDefinition());
                        container.ColumnDefinitions.Add(new ColumnDefinition());

                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                        var lbl_tipodevisita = new Label
                        {
                            Margin = new Thickness(5),
                            FontAttributes = FontAttributes.Bold,
                        };

                        var lbl_fechaGPS = new Label
                        {
                            Margin = new Thickness(5),
                            HorizontalTextAlignment = TextAlignment.End,
                        };

                        var lbl_nombres = new Label
                        {
                            Margin = new Thickness(5)
                        };

                        var lbl_especificaciones = new Label
                        {
                            Margin = new Thickness(5)
                        };
                        
                        var lbl_mensajeserror = new Label
                        {
                            Margin = new Thickness(5),
                            FontAttributes = FontAttributes.Italic,
                            TextColor = Colors.Red,
                        };

                        Grid.SetColumn(lbl_tipodevisita, 0);
                        Grid.SetRow(lbl_tipodevisita, 0);

                        Grid.SetColumn(lbl_fechaGPS, 1);
                        Grid.SetRow(lbl_fechaGPS, 0);

                        Grid.SetColumn(lbl_nombres, 0);
                        Grid.SetColumnSpan(lbl_nombres, 2);
                        Grid.SetRow(lbl_nombres, 1);

                        Grid.SetColumn(lbl_especificaciones, 0);
                        Grid.SetColumnSpan(lbl_especificaciones, 2);
                        Grid.SetRow(lbl_especificaciones, 2);
                        
                        Grid.SetColumn(lbl_mensajeserror, 0);
                        Grid.SetColumnSpan(lbl_mensajeserror, 2);
                        Grid.SetRow(lbl_mensajeserror, 3);

                        container.Children.Add(lbl_tipodevisita);
                        container.Children.Add(lbl_fechaGPS);
                        container.Children.Add(lbl_nombres);
                        container.Children.Add(lbl_especificaciones);
                        container.Children.Add(lbl_mensajeserror);

                        switch (visita.OperacionID)
                        {
                            case "VMedicA017":
                                container.BackgroundColor = Color.FromArgb("#FFEEE5");

                                var datos017 = visita.Parametros?.Split("','");
                                var tipoVisita017 = App.Tiposvisitas?.GetItems()?.FirstOrDefault(TV => TV.CODIGO_TIPO_VISITA == datos017?[2])?.DESCRIPCION;
                                var fechaGPS017 = datos017?[4] != "" ? DateTime.ParseExact(datos017?[4] ?? "", "yyyyMMdd HH:mm:ss", new CultureInfo("es-ES")).ToString("yyyy-MM-dd HH:mm:ss") : "";
                                var Nombre017 = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == datos017?[1])?.NOMBRE_COMERCIAL;
                                var Motivo017 = datos017?[3];

                                lbl_tipodevisita.Text = tipoVisita017;
                                lbl_fechaGPS.Text = fechaGPS017;
                                lbl_nombres.Text = Nombre017;
                                lbl_especificaciones.Text = Motivo017;

                                lbl_mensajeserror.IsVisible = false;

                                App.Current?.Dispatcher.Dispatch(delegate
                                {
                                    DatosCompartidos.ListaVisitasPendientes?.Children.Add(container);
                                });

                                break;
                            case "VMedicA038":
                                container.BackgroundColor = Color.FromArgb("#FFF8D2");

                                var datos038 = visita.Parametros?.Split("','");
                                var tipoVisita038 = App.Tiposvisitas?.GetItems()?.FirstOrDefault(TV => TV.CODIGO_TIPO_VISITA == datos038?[2])?.DESCRIPCION;
                                var fechaGPS038 = datos038?[4] != "" ? DateTime.ParseExact(datos038?[4] ?? "", "yyyyMMdd HH:mm:ss", new CultureInfo("es-ES")).ToString("yyyy-MM-dd HH:mm:ss") : "";
                                var Nombre038 = App.Lugaresventas?.GetItems()?.FirstOrDefault(D => D.CODIGO_LUGAR == datos038?[1])?.DESCRIPCION;
                                var Motivo038 = datos038?[3].Split(". ")[0];

                                lbl_tipodevisita.Text = tipoVisita038;
                                lbl_fechaGPS.Text = fechaGPS038;
                                lbl_nombres.Text = Nombre038;
                                lbl_especificaciones.Text = Motivo038;

                                lbl_mensajeserror.IsVisible = false;

                                App.Current?.Dispatcher.Dispatch(delegate
                                {
                                    DatosCompartidos.ListaVisitasPendientes?.Children.Add(container);
                                });

                                break;
                            case "VMedicA046":
                                container.BackgroundColor = Color.FromArgb("#DAECF0");
                                container.Children.Remove(lbl_tipodevisita);

                                var datos046 = visita.Parametros?.Split("','");
                                var tipoVisita046 = App.Tiposvisitas?.GetItems()?.FirstOrDefault(TV => TV.CODIGO_TIPO_VISITA == datos046?[4])?.DESCRIPCION;
                                var Cliente = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == datos046?[0].Substring(1));
                                var EvaluacionesText = "";

                                lbl_tipodevisita.Text = tipoVisita046;
                                lbl_nombres.Text = Cliente?.NOMBRE_COMERCIAL;

                                var ListaDetallesdeEvaluacion = App.Evaluaciondetalles?.GetItems()?.Where(ED => ED.IdCliente == datos046?[0].Substring(1)).ToList();
                                var ListaMuestra = App.Muestras?.GetItems();
                                if (ListaDetallesdeEvaluacion is not null && ListaMuestra is not null)
                                {
                                    var ListaEvaluacionesPendientes = (from a in ListaDetallesdeEvaluacion
                                                                       join b in ListaMuestra on a.IdProducto equals b.CODIGO_MUESTRA
                                                                       select new
                                                                       {
                                                                           b.DESCRIPCION_MUESTRA,
                                                                           a.Cantidad,
                                                                       }).ToList();

                                    for (int j = 0; j < ListaEvaluacionesPendientes.Count; j++)
                                    {
                                        var evaluaciones = ListaEvaluacionesPendientes[j];
                                        if (j > 0)
                                        {
                                            EvaluacionesText += $"\n        {(j + 1)}. {evaluaciones.DESCRIPCION_MUESTRA} - Cant: {evaluaciones.Cantidad}";
                                        }
                                        else
                                        {
                                            EvaluacionesText += $"        {(j + 1)}. {evaluaciones.DESCRIPCION_MUESTRA} - Cant: {evaluaciones.Cantidad}";
                                        }
                                    }
                                }

                                lbl_especificaciones.Text = EvaluacionesText;

                                lbl_mensajeserror.IsVisible = false;

                                Grid.SetColumn(lbl_tipodevisita, 0);
                                Grid.SetRow(lbl_tipodevisita, 0);
                                Grid.SetColumnSpan(lbl_tipodevisita, 2);

                                container.Children.Add(lbl_tipodevisita);

                                App.Current?.Dispatcher.Dispatch(delegate
                                {
                                    DatosCompartidos.ListaVisitasPendientes?.Children.Add(container);
                                });

                                break;
                            case "VMedicA043":
                                container.BackgroundColor = Color.FromArgb("#E0FDE2");

                                var datos043 = visita.Parametros?.Split("','");
                                var tipoVisita043 = App.Tiposvisitas?.GetItems()?.FirstOrDefault(TV => TV.CODIGO_TIPO_VISITA == datos043?[4])?.DESCRIPCION;
                                var fechaGPS043 = datos043?[8].Split("'")[0] != "" ? DateTime.ParseExact(datos043?[8].Split("'")[0] ?? "", "yyyyMMdd HH:mm:ss", new CultureInfo("es-ES")).ToString("yyyy-MM-dd HH:mm:ss") : "";
                                var Nombre043 = App.Materiales?.GetItems()?.FirstOrDefault(M => M.CODIGO_MATERIAL == datos043?[2])?.NOMBRE_MATERIAL;
                                var Motivo043 = datos043?[5];

                                lbl_tipodevisita.Text = tipoVisita043;
                                lbl_fechaGPS.Text = fechaGPS043;
                                lbl_nombres.Text = Nombre043;
                                lbl_especificaciones.Text = Motivo043;

                                lbl_mensajeserror.IsVisible = false;

                                App.Current?.Dispatcher.Dispatch(delegate
                                {
                                    DatosCompartidos.ListaVisitasPendientes?.Children.Add(container);
                                });
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public async void EnviarSolicitudes()
        {
            var Mensaje = "";
            var envioCorrecto = 0;
            var ConteoMensaje = 0;

            if (IsInternet.Avilable())
            {
                if (VisitasPendientes is not null)
                {
                    for (int i = 0; i < VisitasPendientes.Count; i++)
                    {
                        var solicitud = VisitasPendientes[i];

                        switch (solicitud.OperacionID)
                        {
                            case "VMedicA017":
                                var datosA017 = (await servicio.ResultadoGET<Resultado>($"{solicitud.OperacionID}/{solicitud.Parametros}", null))?.FirstOrDefault();
                                if (datosA017 is not null)
                                {
                                    switch (datosA017.MSG)
                                    {
                                        case "1":
                                            envioCorrecto++;
                                            var DoctorSeleciconado = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente).FirstOrDefault();
                                            
                                            if (DoctorSeleciconado is not null)
                                            {
                                                DoctorSeleciconado.Visitas = 1;
                                                App.Doctores?.UpdateITEM(DoctorSeleciconado);
                                            }

                                            DatosCompartidos.ListaVisitasPendientes?.Children.RemoveAt(i);
                                            App.SolicitudesPendientes?.DeleteItem(solicitud);

                                            break;
                                        case "2":
                                            MostrarMensaje(i, "Médico no existente al tratar de enviar visita no efectiva");
                                            break;
                                        case "3":
                                            MostrarMensaje(i, "No tiene permisos para el registro de visita no efectiva");
                                            break;
                                        default:
                                            MostrarMensaje(i, "Lo sentimos, ha ocurrido un error inesperado al tratar de enviar visita no efectiva");
                                            break;
                                    }
                                }

                                break;
                            case "VMedicA038":
                                var datosA038 = (await servicio.ResultadoGET<Resultado>($"{solicitud.OperacionID}/{solicitud.Parametros}", null))?.FirstOrDefault();
                                if (datosA038 is not null)
                                {
                                    switch (datosA038.MSG)
                                    {
                                        case "1":
                                            envioCorrecto++;
                                            var DoctorSeleciconado = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == solicitud.CodigoCliente).FirstOrDefault();
                                            if (DoctorSeleciconado is not null)
                                            {
                                                DoctorSeleciconado.Visitas = 1;
                                                App.Doctores?.UpdateITEM(DoctorSeleciconado);
                                            }

                                            DatosCompartidos.ListaVisitasPendientes?.Children.RemoveAt(i);
                                            App.SolicitudesPendientes?.DeleteItem(solicitud);
                                            break;
                                        case "2":
                                            MostrarMensaje(i, "Médico no existente al tratar de enviar visita a lugares o eventos");
                                            break;
                                        case "3":
                                            MostrarMensaje(i, "No tiene permisos para el registro de visita a lugares o eventos");
                                            break;
                                        default:
                                            MostrarMensaje(i, "Lo sentimos, ha ocurrido un error inesperado al tratar de enviar visita a lugares o eventos");
                                            break;
                                    }
                                }

                                break;
                            case "VMedicA046":
                                var datosA046 = (await servicio.ResultadoPOST(solicitud.OperacionID, solicitud.Parametros, valores => new Resultado
                                {
                                    Id = valores[0],
                                    MSG = valores[1],
                                    Codigo = valores[2]
                                }))?.FirstOrDefault();

                                if (datosA046 is not null)
                                {
                                    var Codigos = datosA046.Codigo?.Split(CaracteresEspeciales.SECCION);
                                    if (Codigos is not null)
                                    {
                                        foreach (var codigo in Codigos)
                                        {
                                            var muestraActualizar = App.Muestras?.GetItems()?.FirstOrDefault(M => M.CODIGO_MUESTRA == codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[0]);
                                            var clienteActualizar = App.Doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == solicitud?.CodigoCliente);

                                            if (muestraActualizar is not null && clienteActualizar is not null)
                                            {
                                                muestraActualizar.CANT_DISPONIBLE = int.Parse(codigo.Split(CaracteresEspeciales.BARRA_VERTICAL_ROTA)[1]);
                                                clienteActualizar.Visitas = 1;

                                                App.Muestras?.UpdateITEM(muestraActualizar);
                                                App.Doctores?.UpdateITEM(clienteActualizar);

                                                var detallesEliminar = App.Evaluaciondetalles?.GetItems()?.Where(Edet => Edet.IdCliente == solicitud?.CodigoCliente).ToList();
                                                var encabezadoEliminar = App.Evaluacionencabezado?.GetItems()?.Where(Eenc => Eenc.IdCliente == solicitud?.CodigoCliente).ToList();

                                                if (detallesEliminar is not null && encabezadoEliminar is not null)
                                                {
                                                    App.Evaluaciondetalles?.DeleteItems(detallesEliminar);
                                                    App.Evaluacionencabezado?.DeleteItems(encabezadoEliminar);
                                                }
                                            }
                                        }
                                    }

                                    switch (datosA046.Id)
                                    {
                                        case "1":
                                            envioCorrecto++;
                                            DatosCompartidos.ListaVisitasPendientes?.Children.RemoveAt(i);
                                            App.SolicitudesPendientes?.DeleteItem(solicitud);
                                            break;
                                        case "2":
                                            MostrarMensaje(i, $"{datosA046.MSG} al enviar las evaluaciones");
                                            break;
                                        case "3":
                                            MostrarMensaje(i, $"{datosA046.MSG} al enviar las evaluaciones");
                                            break;
                                        default:
                                            MostrarMensaje(i, $"Lo sentimos, ha ocurrido un error inesperado al enviar las evaluaciones: {datosA046.MSG}");
                                            break;
                                    }
                                }

                                break;
                            case "VMedicA043":
                                var datosA043 = (await servicio.ResultadoGET<Resultado>($"{solicitud.OperacionID}/{solicitud.Parametros}", null))?.FirstOrDefault();
                                if (datosA043 is not null)
                                {
                                    switch (datosA043.MSG)
                                    {
                                        case "1":
                                            DatosCompartidos.ListaVisitasPendientes?.Children.RemoveAt(i);
                                            envioCorrecto++;
                                            var DoctorSeleciconado = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == solicitud?.CodigoCliente).FirstOrDefault();
                                            if (DoctorSeleciconado is not null)
                                            {
                                                DoctorSeleciconado.Visitas = 1;
                                                App.Doctores?.UpdateITEM(DoctorSeleciconado);
                                            }

                                            DatosCompartidos.ListaVisitasPendientes?.Children.RemoveAt(i);
                                            App.SolicitudesPendientes?.DeleteItem(solicitud);
                                            break;
                                        case "2":
                                            MostrarMensaje(i, "Médico no existente para enviar el promocinal");
                                            break;
                                        case "3":
                                            MostrarMensaje(i, "No tiene permisos para el registro de materiales");
                                            break;
                                        default:
                                            MostrarMensaje(i, "Lo sentimos, ha ocurrido un error inesperado al tratar de enviar el promocional");
                                            break;
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                    }

                    if (envioCorrecto == VisitasPendientes.Count)
                    {
                        ToastMaker.Make("Datos sincronizados correctamente", App.Current?.Windows[0].Page);
                    }
                    else
                    {
                        ToastMaker.Make("Aviso de Fallo de Sincronización", App.Current?.Windows[0].Page);
                    }

                    MostrarVisitasPendientes();
                    if (DatosCompartidos.Lbl_CatntidadPendientes_Visitas is not null)
                    {
                        DatosCompartidos.Lbl_CatntidadPendientes_Visitas.Text = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDVisitas.Contains(SP.OperacionID)).ToList().Count.ToString();
                    }
                }
            }
        }

        private void MostrarMensaje(int i, string mensaje)
        {
            var Registro = ((DatosCompartidos.ListaVisitasPendientes?.Children as IEnumerable<object>)?.OfType<Grid>().ToList()[i])?.Children.FirstOrDefault(LVP => !(LVP as Label).IsVisible) as Label;
            if (Registro is not null)
            {
                Registro.IsVisible = true;
                Registro.Text = mensaje;
            }
        }
    }
}