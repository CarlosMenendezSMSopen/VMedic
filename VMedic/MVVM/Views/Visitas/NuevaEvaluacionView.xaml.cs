using VMedic.Global;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Visitas;

public partial class NuevaEvaluacionView : ContentPage
{
	public NuevaEvaluacionView(string? codCliente, string? nivelPRecio)
	{
		InitializeComponent();
		BindingContext = new NuevaEvaluacionViewModel(codCliente, nivelPRecio);
        Medico_nombre.Text = App.doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == codCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
        if (DatosCompartidos.EvaluacionEditar is not null)
        {
            btn_guardar.Text = "Editar";
        }
        PressedPreferences.EndPressed();
    }

    private void btn_cancelar_Clicked(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PopAsync();
    }

    private void btn_guardar_Clicked(object sender, EventArgs e)
    {
        if (DatosCompartidos.EvaluacionEditar is not null)
        {
            var vm = (NuevaEvaluacionViewModel)BindingContext;
            vm.EditarEvaluacion();
        }
        else
        {
            var vm = (NuevaEvaluacionViewModel)BindingContext;
            vm.GuardarEvaluacion();
        }
    }

    private void searchbox_productos_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (NuevaEvaluacionViewModel)BindingContext;
        vm.MostrarPresentaciones();
    }
}