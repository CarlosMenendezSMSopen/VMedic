using Microsoft.Maui.Controls.Shapes;
using MvvmHelpers;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Global;

namespace VMedic.MVVM.ViewModels.Sincronizaciones
{
    [AddINotifyPropertyChangedInterface]
    public partial class SincronizacionViewModel : BaseViewModel
    {
        private readonly string[] OperadoresVisitas = [ "VMedicA017", "VMedicA038", "VMedicA043", "VMedicA046" ];
        private readonly string[] OperadoresMedicos = [ "VMedicA014", "VMedicA048", "VMedicA042", "VMedicA054" ];
        private readonly string[] OperadoresPlanificación = [ "VMedicA054", "VMedicA049" ];
        public SincronizacionViewModel()
        {
            MostrarSolicitudesPendientes();
        }

        private void MostrarSolicitudesPendientes()
        {
            var SolicitudesPendientes = App.SolicitudesPendientes?.GetItems();

            DatosCompartidos.ListaSolicitudesPendientes?.Children.Clear();

            if (SolicitudesPendientes is not null)
            {
                //var actualizarsolicitud1 = SolicitudesPendientes[0];
                //var actualizarsolicitud2 = SolicitudesPendientes[1];
                //var actualizarsolicitud3 = SolicitudesPendientes[2];
                //var actualizarsolicitud4 = SolicitudesPendientes[3];
                //var actualizarsolicitud5 = SolicitudesPendientes[4];
                //var actualizarsolicitud6 = SolicitudesPendientes[5];
                //var actualizarsolicitud7 = SolicitudesPendientes[6];
                //var actualizarsolicitud8 = SolicitudesPendientes[7];
                //var actualizarsolicitud9 = SolicitudesPendientes[8];
                //var actualizarsolicitud10 = SolicitudesPendientes[9];
                //var actualizarsolicitud11 = SolicitudesPendientes[10];
                //var actualizarsolicitud12 = SolicitudesPendientes[11];
                //var actualizarsolicitud13 = SolicitudesPendientes[12];

                //actualizarsolicitud1.ModuloSolicitud = 1;
                //actualizarsolicitud1.IDSolicitud = 0;

                //actualizarsolicitud2.ModuloSolicitud = 1;
                //actualizarsolicitud2.IDSolicitud = 0;

                //actualizarsolicitud3.ModuloSolicitud = 1;
                //actualizarsolicitud3.IDSolicitud = 0;

                //actualizarsolicitud4.ModuloSolicitud = 1;
                //actualizarsolicitud4.IDSolicitud = 0;

                //actualizarsolicitud5.ModuloSolicitud = 1;
                //actualizarsolicitud5.IDSolicitud = 0;

                //actualizarsolicitud6.ModuloSolicitud = 2;
                //actualizarsolicitud6.IDSolicitud = 0;

                //actualizarsolicitud7.ModuloSolicitud = 2;
                //actualizarsolicitud7.IDSolicitud = 0;
                //actualizarsolicitud7.IDSolicitudPadre = 0;

                //actualizarsolicitud8.ModuloSolicitud = 2;
                //actualizarsolicitud8.IDSolicitud = 1;

                //actualizarsolicitud9.ModuloSolicitud = 2;
                //actualizarsolicitud9.IDSolicitud = 1;
                //actualizarsolicitud9.IDSolicitudPadre = 1;
                //actualizarsolicitud9.OperacionIdPadre = "VMedicA014";

                //actualizarsolicitud10.ModuloSolicitud = 2;
                //actualizarsolicitud10.IDSolicitud = 0;

                //actualizarsolicitud11.ModuloSolicitud = 2;
                //actualizarsolicitud11.IDSolicitud = 0;
                //actualizarsolicitud11.IDSolicitudPadre = 0;

                //actualizarsolicitud12.ModuloSolicitud = 3;
                //actualizarsolicitud12.IDSolicitud = 1;

                //actualizarsolicitud13.ModuloSolicitud = 3;
                //actualizarsolicitud13.IDSolicitud = 0;

                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud1);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud2);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud3);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud4);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud5);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud6);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud7);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud8);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud9);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud10);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud11);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud12);
                //App.SolicitudesPendientes?.UpdateITEM(actualizarsolicitud13);

                var SolicitudesVisitasPendientes = SolicitudesPendientes.Where(S => OperadoresVisitas.Any(OV => S.OperacionID is not null && S.OperacionID.Contains(OV))).ToList();
                var SolicitudesMedicosPendientes = SolicitudesPendientes.Where(S => OperadoresMedicos.Any(OM => S.OperacionID is not null && S.OperacionID.Contains(OM))).ToList();
                var SolicitudesPlanificacionesPendientes = SolicitudesPendientes.Where(S => OperadoresPlanificación.Any(OP => S.OperacionID is not null && S.OperacionID.Contains(OP))).ToList();

                if (SolicitudesVisitasPendientes is not null)
                {
                    if (SolicitudesVisitasPendientes.Count > 0)
                    {
                        var MainContainerBorder = new Border
                        {
                            Margin = new Thickness(15, 10),
                            Stroke = Colors.Transparent,
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
                            }
                        };

                        var GridContainer = new Grid();

                        GridContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                        GridContainer.RowDefinitions.Add(new RowDefinition());

                        var TitleBorder = new Border
                        {
                            Stroke = Colors.Transparent,
                            BackgroundColor = Color.FromRgba("#42b7a6"),
                            Content = new Label
                            {
                                Text = $"VISITAS ({SolicitudesVisitasPendientes.Count})",
                                Margin = 10,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 16,
                                TextColor = Colors.White,
                            },
                        };

                        Grid.SetColumn(TitleBorder, 0);
                        Grid.SetRow(TitleBorder, 0);

                        GridContainer.Children.Add(TitleBorder);

                        foreach (var visitasPendientes in SolicitudesVisitasPendientes)
                        {

                        }

                        MainContainerBorder.Content = GridContainer;

                        DatosCompartidos.ListaSolicitudesPendientes?.Children.Add(MainContainerBorder);
                    }
                }
            }
        }
    }
}
