using Mopups.Services;
using VMedic.Global;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Visitas;

public partial class EvaluacionesView : ContentPage
{
    private TablaVisitasPendientes? Visitas { get; set; }
    private string? NivelPRecio { get; set; }
    public EvaluacionesView(TablaVisitasPendientes visitas, string? nivelPrecio)
    {
        InitializeComponent();
        DatosCompartidos.ListaEvaluaciones = List_Evaluaciones;
        BindingContext = new EvaluacionesViewModel(visitas, nivelPrecio);
        containerbtn_agregarevaluacion.CornerRadius = DeviceInfo.Platform == DevicePlatform.Android ? 50 : 27;
        Visitas = visitas;
        NivelPRecio = nivelPrecio;
        nombre_doctor.Text = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
        var firmaEvaluacion = App.evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == visitas.CodCliente)?.Base64Image;
        if (firmaEvaluacion is not null && firmaEvaluacion != "")
        {
            containerbtn_agregarevaluacion.IsEnabled = false;
            containerbtn_agregarevaluacion.BackgroundColor = Colors.DarkGray;
            btn_sign.IsEnabled = false;
        }
    }

    private void SCROLL_Scrolled(object sender, ScrolledEventArgs e)
    {

    }

    private void AgregarEvaluacion_Tapped(object sender, TappedEventArgs e)
    {
        var encabezado = App.evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == Visitas?.CodCliente);
        if (encabezado is null)
        {
            var tablaEncabezadoEvaluacion = new TablaEncabezadoEvaluacion
            {
                IdCliente = Visitas?.CodCliente,
            };
            App.evaluacionencabezado?.InsertItem(tablaEncabezadoEvaluacion);
        }
        Shell.Current.Navigation.PushAsync(new NuevaEvaluacionView(Visitas?.CodCliente, NivelPRecio));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (DatosCompartidos.StatusVolver == 1)
        {
            var vm = (EvaluacionesViewModel)BindingContext;
            vm.MostrarEvaluaciones();
            DatosCompartidos.StatusVolver = 0;
        }
    }

    private async void FirmarTapped(object sender, TappedEventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            if (App.evaluaciondetalles is not null)
            {
                if (!App.evaluaciondetalles.IsEmpty())
                {
                    if (App.Current?.MainPage is not null)
                    {
                        var confirmar = await App.Current.MainPage.DisplayAlert("Aviso", "Al agregar una firma ya no podrá agregar o editar evaluaciones.\n¿Desea firmar las evaluaciones?", "SI", "NO");
                        if (confirmar)
                        {
                            await MopupService.Instance.PushAsync(new CapturarFirmaView(Visitas?.CodCliente, containerbtn_agregarevaluacion, btn_sign, (EvaluacionesViewModel)BindingContext));
                        }
                        else
                        {
                            PressedPreferences.EndPressed();
                        }
                    }
                }
                else
                {
                    ToastMaker.Make("Debe agregar al menos un producto para firmar", App.Current?.MainPage);
                }
            }
        }
    }

    private void EnviarEvaluacionesTapped(object sender, TappedEventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            var vm = (EvaluacionesViewModel)BindingContext;
            vm.EnviarEvaluaciones(containerbtn_agregarevaluacion, btn_sign);
        }
    }
}