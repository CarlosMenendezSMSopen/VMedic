using VMedic.Global;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.Utilidades;

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

    //metodo para insertar el mapa de forma asincrona en orden sin interrupciones
    private async void InsertarMapa()
    { 
        await Task.Delay(1000);
        Grid.SetRow(DatosCompartidos.MapaUbicaiconMedico, 8);
        Grid.SetColumn(DatosCompartidos.MapaUbicaiconMedico, 0);
        Grid.SetColumnSpan(DatosCompartidos.MapaUbicaiconMedico, 2);
        ContenedorInfo.Children.Add(DatosCompartidos.MapaUbicaiconMedico);
    }

    //metodo evento que navega la aplicación a la pantalla para editar la información de médico seleciconado
    private void btn_editarMedico_Clicked(object sender, EventArgs e)
    {
        var vm = (InformacionMedicoViewModel)BindingContext;
        Shell.Current.Navigation.PushAsync(new EditarMedicoView(vm.CodigoCliente));
    }

    //metodo evento que ejecuta el metodo para compartir la ubicación del usuario
    private void btn_compartirUbicacion_Clicked(object sender, EventArgs e)
    {
        var vm = (InformacionMedicoViewModel)BindingContext;
        vm.CompartirUbicacionMedico();
    }
}