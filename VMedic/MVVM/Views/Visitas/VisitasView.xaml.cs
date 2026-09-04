using VMedic.Global;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class VisitasView : ContentPage
{
    private bool ClaseCargada = false;
    public VisitasView()
    {
        InitializeComponent();
        BindingContext = new VisitasViewModel();
    }

    private void SelectTiposVisitas_SelectedIndexChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.ChangeTipoVisitas();
    }

    private void SelectSemanas_SelectedIndexChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.MostrarMedicos();
    }

    private void SelectDiasSemana_SelectedIndexChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.MostrarMedicos();
    }

    private async void Btn_enviar_Clicked(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.EnviarVisitas();
    }

    private void SelectLugaresVenta_SelectedIndexChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.SeleccionarLugarID();
    }

    private void VerVisitasPendientesTapped(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(null);

            if (App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDVisitas.Contains(SP.OperacionID)).ToList().Count > 0)
            {
                Shell.Current.Navigation.PushAsync(new VisitasPendientesView());
            }
            else
            {
                ToastMaker.Make("No hay visitas pendientes por enviar", App.Current?.Windows[0].Page);
                PressedPreferences.EndPressed();
            }
        }
    }

    private async void searchbox_medicos_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;

        if (vm.Medico is not null)
        {
            searchbox_medicos.TextColor = vm.Medico.ColorEstado;
        }

        if (searchbox_medicos.Text != "")
        {
            await Task.Delay(250);
            CerrarTeclado.Close();
            searchbox_medicos.Unfocus();
        }
    }

    protected async override void OnAppearing()
    {
        if (ClaseCargada)
        {
            busyIndicator.IsRunning = true;
            await Task.Delay(300);
            BindingContext = new VisitasViewModel();
        }

        ClaseCargada = true;

        base.OnAppearing();
    }

    private async void btn_actualizar_Clicked(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.MostrarMedicos();

        ToastMaker.Make("Actualizando lista de médicos...", App.Current?.Windows[0].Page);
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var radiobtn = sender as RadioButton;

        if (radiobtn is not null)
        {
            var vm = (VisitasViewModel)BindingContext;

            if ((string)radiobtn.Value == "A" && e.Value == true)
            {
                vm.EntradaOp = true;
                vm.SalidaOp = false;
            }
            else if ((string)radiobtn.Value == "B" && e.Value == true)
            {
                vm.EntradaOp = false;
                vm.SalidaOp = true;
            }
        }
    }
}