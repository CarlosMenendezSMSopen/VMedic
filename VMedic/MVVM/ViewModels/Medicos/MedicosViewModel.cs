using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using Mopups.Services;
using PropertyChanged;
using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Views.Médicos;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.ViewModels.Medicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class MedicosViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool _indicador;

        [ObservableProperty]
        private bool _textoAviso;

        [ObservableProperty]
        private bool _isRefreshing;
        public List<dynamic>? Medicos { get; set; }
        SfScheduler? Calendario { get; set; }
        public MedicosViewModel()
        {
            MostrarMedicos();
        }

        public void Refresh()
        {
            IsRefreshing = true;
            MostrarMedicos();
        }

        //metodo para formar la lista de los medicos desde 2 registros de la base de datos local
        public async void MostrarMedicos()
        {
            Indicador = true;
            TextoAviso = false;
            Medicos?.Clear();
            DatosCompartidos.ListaMedicos?.Children.Clear();
            var ListaEspecialidad = await SincronizacionDataBase.ObtenerEspecialidades();
            var ListaClientes = await SincronizacionDataBase.ObtenerDoctores();

            await Task.Delay(250);
            await Task.Run(() =>
            {
                try
                {
                    if (ListaEspecialidad is not null && ListaClientes is not null && DatosCompartidos.TextoBusquedaMedicos is not null)
                    {
                        var ListaMedicos = (from c in ListaClientes
                                            join e in ListaEspecialidad on c.CODIGO_DE_CLASE?.Trim() equals e.CODIGO_DE_CLASE
                                            where DatosCompartidos.TextoBusquedaMedicos == "" || (c.NOMBRE_COMERCIAL is not null && c.NOMBRE_COMERCIAL.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase)) || (e.DESCRIPCION_CLASE is not null && e.DESCRIPCION_CLASE.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase))
                                            select new
                                            {
                                                c.CODIGO_DE_CLIENTE,
                                                c.NOMBRE_COMERCIAL,
                                                e.CODIGO_DE_CLASE,
                                                e.DESCRIPCION_CLASE,
                                                c.LATITUD,
                                                c.LONGITUD,
                                                c.COLOR,
                                                c.DIRECCION_EMAIL,
                                                c.TELEFONO_CLIENTE,
                                                c.DIRECCION_CLIENTE
                                            }).ToList();
                        Medicos = [.. ListaMedicos.Cast<dynamic>()];
                        GenerarListaCustom(0);
                    }
                }
                catch (Exception ex)
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        Indicador = false;
                        IsRefreshing = false;
                        ExceptionMessageMaker.Make("Error carga medicos", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                    });
                }
                finally
                {
                    Indicador = false;
                    IsRefreshing = false;
                    if (Medicos?.Count == 0)
                    {
                        TextoAviso = true;
                    }
                }
            });
        }

        //metodo para personalizar la vista de los registros de medico
        public void GenerarListaCustom(int i)
        {
            if (Medicos is not null)
            {
                var lista = DeviceInfo.Platform == DevicePlatform.Android ? [.. Medicos.Skip(i).Take(30)] : Medicos;
                if (lista is not null)
                {
                    foreach (var medico in lista)
                    {
                        var borderContainer = new Border
                        {
                            Margin = new Thickness(15, 5),
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
                            StrokeShape = new RoundRectangle
                            {
                                CornerRadius = 10,
                            },
                            BindingContext = medico,
                        };

                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += (s, e) =>
                        {
                            if (PressedPreferences.ValidatePressing())
                            {
                                PressedPreferences.Pressing(s);

                                dynamic? medicoContext = ((Border?)s)?.BindingContext;

                                MopupService.Instance.PopAllAsync();
                                Shell.Current.Navigation.PushAsync(new InformacionMedicoView(medicoContext?.CODIGO_DE_CLIENTE));
                            }
                        };

                        borderContainer.GestureRecognizers.Add(tapGestureRecognizer);

                        var gridMain = new Grid
                        {
                            ColumnSpacing = 5,
                            RowSpacing = 5,
                        };

                        gridMain.AddColumnDefinition(new ColumnDefinition { Width = 15 });
                        gridMain.AddColumnDefinition(new ColumnDefinition());
                        gridMain.AddColumnDefinition(new ColumnDefinition { Width = GridLength.Auto });
                        gridMain.AddColumnDefinition(new ColumnDefinition { Width = 10 });

                        gridMain.AddRowDefinition(new RowDefinition());
                        gridMain.AddRowDefinition(new RowDefinition());
                        gridMain.AddRowDefinition(new RowDefinition());
                        gridMain.AddRowDefinition(new RowDefinition());

                        var GridColor = new Grid
                        {
                            BackgroundColor = medico.COLOR == "Azul" ? Colors.Blue : medico.COLOR == "Rojo" ? Colors.Red : medico.COLOR == "Amarillo" ? Colors.Yellow : medico.COLOR == "Verde" ? Colors.Green : Colors.White,
                            Margin = new Thickness(0, 0, 7.5, 0),
                        };

                        Grid.SetColumn(GridColor, 0);
                        Grid.SetRow(GridColor, 0);
                        Grid.SetRowSpan(GridColor, 4);

                        var lblNombre = new Label
                        {
                            Text = medico.NOMBRE_COMERCIAL,
                            Margin = new Thickness(0, 10, 0, 0),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 15,
                        };

                        Grid.SetColumn(lblNombre, 1);
                        Grid.SetRow(lblNombre, 0);
                        Grid.SetColumnSpan(lblNombre, 2);

                        var lblEspecialidad = new Label
                        {
                            Text = medico.DESCRIPCION_CLASE,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 12,
                        };

                        Grid.SetColumn(lblEspecialidad, 1);
                        Grid.SetRow(lblEspecialidad, 1);

                        var lblCorreo = new Label
                        {
                            Text = medico.DIRECCION_EMAIL,
                            FontSize = 13,
                        };

                        Grid.SetColumn(lblCorreo, 1);
                        Grid.SetRow(lblCorreo, 2);

                        var lblTelefono = new Label
                        {
                            Text = medico.TELEFONO_CLIENTE,
                            FontSize = 13,
                        };

                        Grid.SetColumn(lblTelefono, 2);
                        Grid.SetRow(lblTelefono, 2);

                        var lblDireccion = new Label
                        {
                            Margin = new Thickness(0, 0, 0, 10),
                            Text = medico.DIRECCION_CLIENTE,
                            FontSize = 14,
                        };

                        Grid.SetColumn(lblDireccion, 1);
                        Grid.SetColumnSpan(lblDireccion, 2);
                        Grid.SetRow(lblDireccion, 3);

                        gridMain.Add(GridColor);
                        gridMain.Add(lblNombre);
                        gridMain.Add(lblEspecialidad);
                        gridMain.Add(lblCorreo);
                        gridMain.Add(lblTelefono);
                        gridMain.Add(lblDireccion);

                        borderContainer.Content = gridMain;

                        App.Current?.Dispatcher.Dispatch(() =>
                        {
                            if (borderContainer is not null)
                                DatosCompartidos.ListaMedicos?.Children.Add(borderContainer);
                        });
                    }
                }
            }
        }

        //funcion tarea para cargar el siguiente lote de medicos en caso de que en Android la cantidad de registros supere el limite de vista por lote
        public async Task CargarMasMedicos(int itemcount)
        {
            Indicador = true;
            await Task.Delay(1000);

            await Task.Run(() =>
            {
                GenerarListaCustom(itemcount);

                App.Current?.Dispatcher.Dispatch(() =>
                {
                    Indicador = false;
                });
            });

        }

        public void ObtenerMedicosPrueba(SfScheduler? calendario)
        {
            App.Current?.Dispatcher.Dispatch(delegate
            {

            });
        }
    }
}
