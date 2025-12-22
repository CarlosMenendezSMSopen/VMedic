using VMedic.Global;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class VisitasView : ContentPage
{
    public VisitasView()
    {
        InitializeComponent();
        DatosCompartidos.Lbl_CatntidadPendientes_Visitas = lbl_cantidadPendientes;
        BindingContext = new VisitasViewModel();
    }

    private void Semanas_Tapped(object sender, TappedEventArgs e)
    {
        SelectSemanas.Unfocus();
        SelectSemanas.Focus();
    }

    private void DiasSemana_Tapped(object sender, TappedEventArgs e)
    {
        SelectDiasSemana.Unfocus();
        SelectDiasSemana.Focus();
    }

    private void TipoVisita_Tapped(object sender, TappedEventArgs e)
    {
        SelectTiposVisitas.Unfocus();
        SelectTiposVisitas.Focus();
    }

    private void SelectTiposVisitas_SelectedIndexChanged(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.ChangeTipoVisitas();
    }

    private void SelectSemanas_SelectedIndexChanged(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.MostrarMedicos();
    }

    private void SelectDiasSemana_SelectedIndexChanged(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.MostrarMedicos();
    }

    private void Btn_enviar_Clicked(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.EnviarVisitas();
    }

    private void SelectLugaresVenta_SelectedIndexChanged(object sender, EventArgs e)
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
}