using VMedic.Global;
using VMedic.MVVM.ViewModels.Médicos;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Médicos;

public partial class InformacionMedicoView : ContentPage
{
	public InformacionMedicoView(string? cODIGO_DE_CLIENTE)
	{
		InitializeComponent();
		BindingContext = new InformacionMedicoViewModel(cODIGO_DE_CLIENTE);
        InsertarMapa();
        PressedPreferences.EndPressed();
    }

    private async void InsertarMapa()
    { 
        await Task.Delay(1000);
        Grid.SetRow(DatosCompartidos.MapaUbicaiconMedico, 8);
        Grid.SetColumn(DatosCompartidos.MapaUbicaiconMedico, 0);
        Grid.SetColumnSpan(DatosCompartidos.MapaUbicaiconMedico, 2);
        ContenedorInfo.Children.Add(DatosCompartidos.MapaUbicaiconMedico);
    }

    private void btn_editarMedico_Clicked(object sender, EventArgs e)
    {
        var vm = (InformacionMedicoViewModel)BindingContext;
        Shell.Current.Navigation.PushAsync(new EditarMedicoView(vm.CodigoCliente));
    }

    private void btn_compartirUbicacion_Clicked(object sender, EventArgs e)
    {
        var vm = (InformacionMedicoViewModel)BindingContext;
        vm.CompartirUbicacionMedico();
    }
}