using System.Diagnostics;
using System.Threading.Tasks;
using VMedic.Global;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Médicos;

public partial class NuevoMedicoView : ContentPage
{
    public NuevoMedicoView()
    {
        try
        {
            InitializeComponent();
            BindingContext = new NuevoMedicoViewModel(List_FormularioNuevoMedico);

            PressedPreferences.EndPressed();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private void Preferencia_Tapped(object sender, TappedEventArgs e)
    {
        //SelectPreferencias.Unfocus();
        //SelectPreferencias.Focus();
    }

    private void Cancelar_Clicked(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PopAsync();
    }

    private void Guardar_Clicked(object sender, EventArgs e)
    {
        var vm = (NuevoMedicoViewModel)BindingContext;
        vm.GuardarNuevoMedico();
    }

    private void searchbox_preferencias_DropDownClosed(object sender, EventArgs e)
    {
        var vm = (NuevoMedicoViewModel)BindingContext;
        var seleccionados = searchbox_preferencias.SelectedItems?
                                  .Cast<TablaProductoPreferencia>()
                                  .Select(e => e.ID_PRODUCTO_PREFERENCIA)
                                  .ToList();
        if (seleccionados is not null)
        {
            vm.IdsPreferencias = string.Join(",", seleccionados);
        }
    }

    private void searchbox_especialidad_DropDownClosed(object sender, EventArgs e)
    {

    }

    private void searchbox_categoria_DropDownClosed(object sender, EventArgs e)
    {

    }

    private void searchbox_adaptacion_DropDownClosed(object sender, EventArgs e)
    {

    }

    private void searchbox_pais_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        frame_selectDepartamento.Opacity = 1;
        searchbox_departamento.IsEnabled = true;

        var vm = (NuevoMedicoViewModel)BindingContext;
        vm.MostrarDatosDepartamentos();
    }

    private void searchbox_departamento_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        frame_selectMunicipio.Opacity = 1;
        searchbox_municipio.IsEnabled = true;

        var vm = (NuevoMedicoViewModel)BindingContext;
        vm.MostrarDatosMunicipios();
    }

    private void date_FechaInicial_DateSelected(object sender, DateChangedEventArgs e)
    {

    }

    private void sw_repetir_StateChanged(object sender, Syncfusion.Maui.Buttons.SwitchStateChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            date_FechaInicial.IsEnabled = !(bool)e.NewValue;
            var vm = (NuevoMedicoViewModel)BindingContext;
            vm.EnableRepetir = (bool)e.NewValue;
        }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await Task.Yield();

        formulario.IsVisible = true;
    }
}