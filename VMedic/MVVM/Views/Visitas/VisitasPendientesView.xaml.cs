using VMedic.Global;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class VisitasPendientesView : ContentPage
{
	public VisitasPendientesView()
	{
		InitializeComponent();
		DatosCompartidos.ListaVisitasPendientes = List_VisitasPendientes;
		BindingContext = new VisitasPendientesViewModel();
		PressedPreferences.EndPressed();
	}

    private void Scroll_container_Scrolled(object sender, ScrolledEventArgs e)
    {

    }

    private void refresh_Refreshing(object sender, EventArgs e)
    {
		var vm = (VisitasPendientesViewModel)BindingContext;
		vm.Refresh();
    }

    private void btn_enviarPendientes_Clicked(object sender, EventArgs e)
    {
        var vm = (VisitasPendientesViewModel)BindingContext;
        vm.EnviarSolicitudes();
    }
}