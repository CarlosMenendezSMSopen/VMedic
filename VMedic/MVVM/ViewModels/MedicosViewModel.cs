using CommunityToolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.Utilities;

namespace VMedic.MVVM.ViewModels
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
        public ICommand? RefreshCommand { get; private set; }
        public MedicosViewModel()
        {
            MostrarMedicos();
            RefreshCommand = new Command(Refresh);
        }

        private void Refresh()
        {
            IsRefreshing = true;
            MostrarMedicos();
        }

        public async void MostrarMedicos()
        {
            Indicador = true;
            TextoAviso = false;
            DatosCompartidos.ListaMedicos?.Children.Clear();
            await Task.Delay(1000);
            await Task.Run(() =>
            {
                try
                {
                    var ListaEspecialidad = App.especialidades?.GetItems();
                    var ListaClientes = App.doctores?.GetItems();
                    if (ListaEspecialidad is not null && ListaClientes is not null && DatosCompartidos.TextoBusquedaMedicos is not null)
                    {
                        var ListaMedicos = (from c in ListaClientes
                                            join e in ListaEspecialidad on c.CODIGO_DE_CLASE equals e.CODIGO_DE_CLASE
                                            orderby c.NOMBRE_COMERCIAL
                                            where DatosCompartidos.TextoBusquedaMedicos != "" 
                                                ? c.NOMBRE_COMERCIAL is not null 
                                                    ? c.NOMBRE_COMERCIAL.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase) 
                                                    : false
                                                ||
                                                    e.DESCRIPCION_CLASE is not null 
                                                    ? e.DESCRIPCION_CLASE.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase)
                                                    : false
                                                : true
                                            select new
                                            {
                                                c.CODIGO_DE_CLIENTE,
                                                c.NOMBRE_COMERCIAL,
                                                e.CODIGO_DE_CLASE,
                                                e.DESCRIPCION_CLASE,
                                                c.LATITUD,
                                                c.LONGITUD
                                            }).ToList();
                        Medicos = ListaMedicos.Cast<dynamic>().ToList();
                        GenerarListaCustom(0);
                    }
                }
                catch (Exception ex)
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        Indicador = false;
                        IsRefreshing = false;
                        ExceptionMessageMaker.Make("Error carga evaluaciones", ex.ToString(), ex.Message, App.Current?.MainPage);
                    });
                }
                finally
                {
                    App.Current?.Dispatcher.Dispatch(delegate
                    {
                        Indicador = false;
                        IsRefreshing = false;
                        if (DatosCompartidos.ListaMedicos?.Children.Count == 0)
                        {
                            TextoAviso = true;
                        }
                    });
                }
            });
        }

        public void GenerarListaCustom(int i)
        {
            if (Medicos is not null)
            {
                var lista = DeviceInfo.Platform == DevicePlatform.Android ? Medicos.Skip(i).Take(30).ToList() : Medicos;
                if (lista is not null)
                {
                    foreach (var medico in lista)
                    {
                        var container = new Grid
                        {
                            Margin = new Thickness(15, 0)
                        };
                        container.ColumnDefinitions.Add(new ColumnDefinition());

                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        container.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                        var lbl_NombreMedico = new Label
                        {
                            Text = medico.NOMBRE_COMERCIAL,
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center
                        };

                        Grid.SetRow(lbl_NombreMedico, 0);
                        Grid.SetColumn(lbl_NombreMedico, 0);

                        var lbl_especialidad = new Label
                        {
                            Text = medico.DESCRIPCION_CLASE,
                            FontSize = 12,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center
                        };

                        Grid.SetRow(lbl_especialidad, 1);
                        Grid.SetColumn(lbl_especialidad, 0);

                        if (DeviceInfo.Platform == DevicePlatform.Android)
                        {
                            var frame = new Button
                            {
                                BackgroundColor = Colors.Black,
                                Margin = new Thickness(0, 10),
                                HeightRequest = 1.5,
                            };

                            Grid.SetRow(frame, 2);
                            Grid.SetColumn(frame, 0);

                            container.Children.Add(frame);
                        }
                        else
                        {
                            var frame = new Frame
                            {
                                BorderColor = Colors.Black,
                                BackgroundColor = Colors.Black,
                                Margin = new Thickness(0, 10),
                                HeightRequest = 1.5,
                            };

                            Grid.SetRow(frame, 2);
                            Grid.SetColumn(frame, 0);

                            container.Children.Add(frame);
                        }

                        container.Children.Add(lbl_NombreMedico);
                        container.Children.Add(lbl_especialidad);
                        

                        App.Current?.Dispatcher.Dispatch(() =>
                        {
                            DatosCompartidos.ListaMedicos?.Children.Add(container);
                        });
                    }
                }
            }
        }

        public async Task CargarMasMedicos(ActivityIndicator status, int itemcount)
        {
            status.IsVisible = true;
            await Task.Delay(1000);

            await Task.Run(() =>
            {
                GenerarListaCustom(itemcount);

                App.Current?.Dispatcher.Dispatch(() =>
                {
                    status.IsVisible = false;
                });
            });

        }
    }
}
