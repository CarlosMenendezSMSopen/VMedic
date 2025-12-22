using VMedic.Global;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Médicos;

public partial class MedicosPendientesView : ContentPage
{
	public MedicosPendientesView()
	{
		InitializeComponent();
        DatosCompartidos.ListaMedicosPendientes = List_MedicosPendientes;
        BindingContext = new MedicosPendientesViewModel();
        PressedPreferences.EndPressed();
	}

    private void btn_enviarPendientes_Clicked(object sender, EventArgs e)
    {
        var vm = (MedicosPendientesViewModel)BindingContext;
        vm.EnviarMedicosPendientes();
    }

    private void scroll_container_Scrolled(object sender, ScrolledEventArgs e)
    {

    }

    private void refresh_Refreshing(object sender, EventArgs e)
    {
        var vm = (MedicosPendientesViewModel)BindingContext;
        vm.Refresh();
    }
}