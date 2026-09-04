using VMedic.Global;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class NuevaEvaluacionView : ContentPage
{
    public NuevaEvaluacionView(string? codCliente)
    {
        InitializeComponent();
        BindingContext = new NuevaEvaluacionViewModel(codCliente);
        Medico_nombre.Text = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == codCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
        if (DatosCompartidos.EvaluacionEditar is not null)
        {
            searchbox_productos.IsEnabled = false;
            frame_selectProducto.Opacity = 0.75;
            btn_guardar.Text = "Editar";
        }
        PressedPreferences.EndPressed();
    }

    private void Btn_cancelar_Clicked(object sender, EventArgs e)
    {
        DatosCompartidos.EvaluacionEditar = null;
        Shell.Current.Navigation.PopAsync();
    }

    private void Btn_guardar_Clicked(object sender, EventArgs e)
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

    private async void Searchbox_productos_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (NuevaEvaluacionViewModel)BindingContext;
        vm.MostrarPresentaciones();

        if (searchbox_productos.Text != "")
        {
            await Task.Delay(250);

            CerrarTeclado.Close();
            searchbox_productos.Unfocus();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        DatosCompartidos.EvaluacionEditar = null;
        Shell.Current.Navigation.PopAsync();

        return true;
    }
}