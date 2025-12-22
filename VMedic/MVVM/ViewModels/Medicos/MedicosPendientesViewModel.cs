using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models;
using VMedic.MVVM.Models.DataBase;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.ViewModels.Medicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class MedicosPendientesViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _indicador;

        [ObservableProperty]
        private bool _isRefreshing;
        public List<TablaSolicitudesNoEnviadas>? MedicosPendientes { get; set; }

        private readonly RestService servicio = new();
        private List<string> Semanas { get; set; } = ["Semana 1", "Semana 2", "Semana 3", "Semana 4", "Semana 5"];
        private List<string> Dias { get; set; } = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"];
        public MedicosPendientesViewModel()
        {
            MostrarMedicosPendientes();
        }

        public void Refresh()
        {
            IsRefreshing = true;
            MostrarMedicosPendientes();
        }

        public async void MostrarMedicosPendientes()
        {
            Indicador = true;
            DatosCompartidos.ListaMedicosPendientes?.Children.Clear();
            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    MedicosPendientes = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDMedicos.Contains(SP.OperacionID)).ToList();
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

        private void GenerateListaCustom(int i)
        {
            if (MedicosPendientes is not null)
            {
                var lista = DeviceInfo.Platform == DevicePlatform.Android ? [.. MedicosPendientes.Skip(i).Take(30)] : MedicosPendientes;
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

                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                        var lbl_procedimientomedico = new Label
                        {
                            Margin = new Thickness(5),
                            FontAttributes = FontAttributes.Bold,
                        };

                        var lbl_Titlenombres = new Label
                        {
                            Margin = new Thickness(5, 5, 5, 0),
                            FontAttributes = FontAttributes.Bold,
                        };

                        var lbl_nombres = new Label
                        {
                            Margin = new Thickness(5, 0, 5, 5)
                        };

                        var lbl_Titleespecificaciones = new Label
                        {
                            FontAttributes = FontAttributes.Bold,
                            Margin = new Thickness(5, 5, 5, 0),
                        };

                        var lbl_especificaciones = new Label
                        {
                            Margin = new Thickness(5, 0, 5, 5)
                        };

                        var lbl_mensajeserror = new Label
                        {
                            Margin = new Thickness(5),
                            FontAttributes = FontAttributes.Italic,
                            TextColor = Colors.Red,
                        };

                        Grid.SetColumn(lbl_procedimientomedico, 0);
                        Grid.SetRow(lbl_procedimientomedico, 0);

                        Grid.SetColumn(lbl_Titlenombres, 0);
                        Grid.SetRow(lbl_Titlenombres, 1);

                        Grid.SetColumn(lbl_nombres, 0);
                        Grid.SetRow(lbl_nombres, 2);

                        Grid.SetColumn(lbl_Titleespecificaciones, 0);
                        Grid.SetRow(lbl_Titleespecificaciones, 3);

                        Grid.SetColumn(lbl_especificaciones, 0);
                        Grid.SetRow(lbl_especificaciones, 4);

                        Grid.SetColumn(lbl_mensajeserror, 0);
                        Grid.SetRow(lbl_mensajeserror, 5);

                        container.Children.Add(lbl_procedimientomedico);
                        container.Children.Add(lbl_Titlenombres);
                        container.Children.Add(lbl_nombres);
                        container.Children.Add(lbl_Titleespecificaciones);
                        container.Children.Add(lbl_especificaciones);
                        container.Children.Add(lbl_mensajeserror);

                        if (visita.OperacionID == "VMedicA014")
                        {
                            container.BackgroundColor = Color.FromArgb("#FFEEE5");
                            var datos014 = visita.Parametros?.Split(',');
                            lbl_procedimientomedico.Text = "Agregar Nuevo Médico";

                            lbl_Titlenombres.Text = "Nombre:";
                            lbl_nombres.Text = datos014?[1].Split("'")[1];
                            lbl_Titleespecificaciones.Text = "Especialización: ";
                            lbl_especificaciones.Text = App.Especialidades?.GetItems()?.FirstOrDefault(E => E.CODIGO_DE_CLASE == datos014?[10].Split("'")[1])?.DESCRIPCION_CLASE;
                            lbl_mensajeserror.IsVisible = false;

                            App.Current?.Dispatcher.Dispatch(delegate
                            {
                                DatosCompartidos.ListaMedicosPendientes?.Children.Add(container);
                            });
                        }

                        if (visita.OperacionID == "VMedicA042")
                        {
                            container.BackgroundColor = Color.FromArgb("#DAECF0");
                            var datos042 = visita.Parametros?.Split(',');
                            lbl_procedimientomedico.Text = "Actualizar Médico";

                            lbl_Titlenombres.Text = "Nombre: ";
                            lbl_nombres.Text = datos042?[2].Split("'")[1];
                            lbl_Titleespecificaciones.Text = "Especialización:";
                            lbl_especificaciones.Text = App.Especialidades?.GetItems()?.FirstOrDefault(E => E.CODIGO_DE_CLASE == datos042?[14].Split("'")[1])?.DESCRIPCION_CLASE;
                            lbl_mensajeserror.IsVisible = false;

                            App.Current?.Dispatcher.Dispatch(delegate
                            {
                                DatosCompartidos.ListaMedicosPendientes?.Children.Add(container);
                            });
                        }

                        if (visita.OperacionID == "VMedicA021" && visita.OperacionIdPadre == "VMedicA014")
                        {
                            var secondcontainer = new Grid();

                            secondcontainer.ColumnDefinitions.Add(new ColumnDefinition { Width = 75 });
                            secondcontainer.ColumnDefinitions.Add(new ColumnDefinition());

                            secondcontainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                            var imagen = new Image
                            {
                                Source = "tree_arrow.png",
                                WidthRequest = 50,
                                HorizontalOptions = LayoutOptions.End,
                            };

                            Grid.SetColumn(imagen, 0);
                            Grid.SetRow(imagen, 0);

                            container.BackgroundColor = Color.FromArgb("#FFF8D2");

                            var datos014 = visita.Parametros?.Split("','");

                            lbl_procedimientomedico.Text = "Control de Visitas";
                            if (datos014?[2] == "0" && datos014?[3] == "0")
                            {
                                lbl_Titlenombres.Text = "Fecha Visita: ";
                                lbl_nombres.Text = $"{datos014?[5]}";
                                lbl_Titleespecificaciones.IsVisible = false;
                                lbl_especificaciones.IsVisible = false;
                            }
                            else
                            {
                                lbl_Titlenombres.Text = "Semanas Visita: ";
                                lbl_nombres.Text = MostrarSemanas(datos014?[2]);
                                lbl_Titleespecificaciones.Text = "Días Visita:";
                                lbl_especificaciones.Text = MostrarDias(datos014?[3]);
                            }

                            lbl_mensajeserror.IsVisible = false;

                            Grid.SetColumn(container, 1);
                            Grid.SetRow(container, 0);

                            secondcontainer.Children.Add(imagen);
                            secondcontainer.Children.Add(container);

                            App.Current?.Dispatcher.Dispatch(delegate
                            {
                                DatosCompartidos.ListaMedicosPendientes?.Children.Add(secondcontainer);
                            });
                        }
                        else if (visita.OperacionID == "VMedicA021" && visita.OperacionIdPadre == "VMedicA042")
                        {
                            var secondcontainer = new Grid();

                            secondcontainer.ColumnDefinitions.Add(new ColumnDefinition { Width = 75 });
                            secondcontainer.ColumnDefinitions.Add(new ColumnDefinition());

                            secondcontainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                            var imagen = new Image
                            {
                                Source = "tree_arrow.png",
                                WidthRequest = 50,
                                HorizontalOptions = LayoutOptions.End,
                            };

                            Grid.SetColumn(imagen, 0);
                            Grid.SetRow(imagen, 0);

                            container.BackgroundColor = Color.FromArgb("#FFF8D2");

                            var datos042 = visita.Parametros?.Split("','");

                            lbl_procedimientomedico.Text = "Control de Visitas";
                            if (datos042?[2] == "0" && datos042?[3] == "0")
                            {
                                lbl_Titlenombres.Text = "Fecha Visita:";
                                lbl_nombres.Text = $"{datos042?[5]}";
                                lbl_Titleespecificaciones.IsVisible = false;
                                lbl_especificaciones.IsVisible = false;
                            }
                            else
                            {
                                lbl_Titlenombres.Text = "Semanas Visita:";
                                lbl_nombres.Text = MostrarSemanas(datos042?[2]);
                                lbl_Titleespecificaciones.Text = "Días Visita";
                                lbl_especificaciones.Text = MostrarDias(datos042?[3]);
                            }

                            lbl_mensajeserror.IsVisible = false;

                            Grid.SetColumn(container, 1);
                            Grid.SetRow(container, 0);

                            secondcontainer.Children.Add(imagen);
                            secondcontainer.Children.Add(container);

                            App.Current?.Dispatcher.Dispatch(delegate
                            {
                                DatosCompartidos.ListaMedicosPendientes?.Children.Add(secondcontainer);
                            });
                        }
                        else if (visita.OperacionID == "VMedicA021" && visita.OperacionIdPadre is null)
                        {
                            container.BackgroundColor = Color.FromArgb("#FFF8D2");

                            var datos021 = visita.Parametros?.Split("','");

                            lbl_procedimientomedico.Text = "Control de Visitas";
                            if (datos021?[2] == "0" && datos021?[3] == "0")
                            {
                                lbl_Titlenombres.Text = "Fecha Visita:";
                                lbl_nombres.Text = $"{datos021?[5]}";
                                lbl_Titleespecificaciones.IsVisible = false;
                                lbl_especificaciones.IsVisible = false;
                            }
                            else
                            {
                                lbl_Titlenombres.Text = "Semanas Visita:";
                                lbl_nombres.Text = MostrarSemanas(datos021?[2]);
                                lbl_Titleespecificaciones.Text = "Días Visita";
                                lbl_especificaciones.Text = MostrarDias(datos021?[3]);
                            }

                            lbl_mensajeserror.IsVisible = false;

                            Grid.SetColumn(container, 1);
                            Grid.SetRow(container, 0);

                            App.Current?.Dispatcher.Dispatch(delegate
                            {
                                DatosCompartidos.ListaMedicosPendientes?.Children.Add(container);
                            });
                        }
                    }
                }
            }
        }

        private string MostrarDias(string? dias)
        {
            if (dias is not null)
                if (dias.Contains(','))
                {
                    var diasText = "";
                    var diasList = dias.Split(",");
                    for (int i = 1; i <= diasList.Length; i++)
                    {
                        if (i == diasList.Length)
                        {
                            diasText += $" y {Dias[i]}";
                        }
                        else if (i == 1)
                        {
                            diasText += $"{Dias[i]}";
                        }
                        else
                        {
                            diasText += $", {Dias[i]}";
                        }
                    }

                    return diasText;
                }
                else
                {
                    int pos = int.Parse(dias);
                    return Dias[pos];
                }

            return "";
        }

        private string MostrarSemanas(string? semanas)
        {
            if (semanas is not null)
                if (semanas.Contains(','))
                {
                    var SemanaText = "";
                    var semanaList = semanas.Split(',');
                    for (int i = 1; i <= semanaList.Length; i++)
                    {
                        if (i == semanaList.Length)
                        {
                            SemanaText += $" y {Semanas[i]}";
                        }
                        else if (i == 1)
                        {
                            SemanaText += $"{Semanas[i]}";
                        }
                        else
                        {
                            SemanaText += $", {Semanas[i]}";
                        }
                    }

                    return SemanaText;
                }
                else
                {
                    int pos = int.Parse(semanas);
                    return Semanas[pos];
                }

            return "";
        }

        public async void EnviarMedicosPendientes()
        {
            if (MedicosPendientes is not null)
            {
                var PendientesPadre = MedicosPendientes.Where(MP => MP.OperacionIdPadre is null).ToList();
                if (PendientesPadre is not null)
                {
                    var envioCorrecto = 0;
                    for (int i = 0; i < PendientesPadre.Count; i++)
                    {
                        var SolicitudPendiente = PendientesPadre[i];
                        if (SolicitudPendiente.OperacionID == "VMedicA014")
                        {
                            var datos = (await servicio.ResultadoGET<Resultado>(SolicitudPendiente.OperacionID + "/" + SolicitudPendiente.Parametros, null))?.FirstOrDefault();
                            if (datos is not null)
                            {
                                switch (datos.MSG)
                                {
                                    case "1":
                                        envioCorrecto++;
                                        DatosCompartidos.ListaMedicosPendientes?.Children.RemoveAt(i);
                                        App.SolicitudesPendientes?.DeleteItem(SolicitudPendiente);

                                        var SolicitudHija = MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.TableID.ToString()) ?? MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.CodigoCliente);
                                        var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudHija?.OperacionID + "/" + SolicitudHija?.Parametros, null))?.FirstOrDefault();
                                        if (resultados is not null)
                                        {
                                            var IndiceMedicoPendiente = MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.TableID.ToString()) ?? MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.CodigoCliente);
                                            if (IndiceMedicoPendiente is not null)
                                            {
                                                switch (resultados.MSG)
                                                {
                                                    case "1":
                                                        envioCorrecto++;
                                                        DatosCompartidos.ListaMedicosPendientes?.Children.RemoveAt(i);
                                                        App.SolicitudesPendientes?.DeleteItem(SolicitudHija);
                                                        break;
                                                    case "2":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "El usuario no tiene permisos para agregar control de visitas");
                                                        break;
                                                    case "3":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "Ha ocurrido un error inesperado al guardar el control de visitas");
                                                        break;
                                                    case "5":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "No se ha encontrado el médico indicado");
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                    case "2":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "El usuario no tiene permisos para agregar médicos");
                                        break;
                                    case "3":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "Ha ocurrido un error inesperado al guardar el médico");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else if (SolicitudPendiente.OperacionID == "VMedicA042")
                        {
                            var datos = (await servicio.ResultadoGET<Resultado>(SolicitudPendiente.OperacionID + "/" + SolicitudPendiente.Parametros, null))?.FirstOrDefault();
                            if (datos is not null)
                            {
                                switch (datos.MSG)
                                {
                                    case "1":
                                        envioCorrecto++;
                                        DatosCompartidos.ListaMedicosPendientes?.Children.RemoveAt(i);
                                        App.SolicitudesPendientes?.DeleteItem(SolicitudPendiente);

                                        var SolicitudHija = MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.TableID.ToString()) ?? MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.CodigoCliente);
                                        var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudHija?.OperacionID + "/" + SolicitudHija?.Parametros, null))?.FirstOrDefault();
                                        if (resultados is not null)
                                        {
                                            var IndiceMedicoPendiente = MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.TableID.ToString()) ?? MedicosPendientes.FirstOrDefault(MP => MP.IDSolicitudPadre == SolicitudPendiente.CodigoCliente);
                                            if (IndiceMedicoPendiente is not null)
                                            {
                                                switch (resultados.MSG)
                                                {
                                                    case "1":
                                                        envioCorrecto++;
                                                        DatosCompartidos.ListaMedicosPendientes?.Children.RemoveAt(i);
                                                        App.SolicitudesPendientes?.DeleteItem(SolicitudHija);
                                                        break;
                                                    case "2":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "El usuario no tiene permisos para agregar control de visitas");
                                                        break;
                                                    case "3":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "Ha ocurrido un error inesperado al guardar el control de visitas");
                                                        break;
                                                    case "5":
                                                        MostrarMensajePendientesHijo(MedicosPendientes.IndexOf(IndiceMedicoPendiente), "No se ha encontrado el médico indicado");
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                    case "2":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "El usuario no tiene permisos para agregar médicos");
                                        break;
                                    case "3":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "Ha ocurrido un error inesperado al guardar el médico");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else if (SolicitudPendiente.OperacionID == "VMedicA021")
                        {
                            var resultados = (await servicio.ResultadoGET<Resultado>(SolicitudPendiente.OperacionID + "/" + SolicitudPendiente.Parametros, null))?.FirstOrDefault();
                            if (resultados is not null)
                            {
                                switch (resultados.MSG)
                                {
                                    case "1":
                                        envioCorrecto++;
                                        DatosCompartidos.ListaMedicosPendientes?.Children.RemoveAt(i);
                                        App.SolicitudesPendientes?.DeleteItem(SolicitudPendiente);
                                        break;
                                    case "2":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "El usuario no tiene permisos para agregar control de visitas");
                                        break;
                                    case "3":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "Ha ocurrido un error inesperado al guardar el control de visitas");
                                        break;
                                    case "5":
                                        MostrarMensajePendientesPadre(MedicosPendientes.IndexOf(SolicitudPendiente), "No se ha encontrado el médico indicado");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    if (envioCorrecto == MedicosPendientes.Count)
                    {
                        ToastMaker.Make("Datos sincronizados correctamente", App.Current?.Windows[0].Page);
                    }
                    else
                    {
                        ToastMaker.Make("Aviso de Fallo de Sincronización", App.Current?.Windows[0].Page);
                    }
                }
            }
        }

        private void MostrarMensajePendientesPadre(int i, string mensaje)
        {
            var Registro = ((DatosCompartidos.ListaMedicosPendientes?.Children as IEnumerable<object>)?.OfType<Grid>().ToList()[i])?.Children.FirstOrDefault(LVP => !(LVP as Label).IsVisible) as Label;
            if (Registro is not null)
            {
                Registro.IsVisible = true;
                Registro.Text = mensaje;
            }
        }

        private void MostrarMensajePendientesHijo(int i, string mensaje)
        {
            var Registro = (((DatosCompartidos.ListaMedicosPendientes?.Children as IEnumerable<object>)?.OfType<Grid>().ToList()[i])?.Children as IEnumerable<object>)?.OfType<Grid>().FirstOrDefault()?.Children.FirstOrDefault(LVP => !(LVP as Label).IsVisible) as Label;
            if (Registro is not null)
            {
                Registro.IsVisible = true;
                Registro.Text = mensaje;
            }
        }
    }
}
