using VMedic.MVVM.ViewModels.Visitas;

namespace VMedic.MVVM.Views.Visitas;

public partial class VisitasView : ContentPage
{
	public VisitasView()
	{
		InitializeComponent();
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

    private void btn_enviar_Clicked(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.EnviarVisitas();
    }

    private void SelectLugaresVenta_SelectedIndexChanged(object sender, EventArgs e)
    {
        var vm = (VisitasViewModel)BindingContext;
        vm.SeleccionarLugarID();
    }
}