using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.ViewModels.Médicos;
using VMedic.Services;

namespace VMedic.MVVM.Views.Médicos;

public partial class EditarMedicoView : ContentPage
{
	public EditarMedicoView(string? cODIGO_DE_CLIENTE)
	{
		InitializeComponent();
        SincronizacionDataBase.ObtenerEspecialidades();
        SincronizacionDataBase.ObtenerCategoriasMedico();
        SincronizacionDataBase.ObtenerProductosPreferencias();
        SincronizacionDataBase.ObtenerMedicosProductosPreferencias();
        SincronizacionDataBase.ObtenerVisitasMensuales();
        BindingContext = new EditarMedicoViewModel(cODIGO_DE_CLIENTE);
        InsertarPreferencias();
    }

    private void Especialidad_Tapped(object sender, TappedEventArgs e)
    {
        SelectEspecialidades.Unfocus();
        SelectEspecialidades.Focus();
    }

    private void Categoria_Tapped(object sender, TappedEventArgs e)
    {
        SelectCategorias.Unfocus();
        SelectCategorias.Focus();
    }

    private void searchbox_preferencias_DropDownClosed(object sender, EventArgs e)
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        var seleccionados = searchbox_preferencias.SelectedItems?
                                  .Cast<TablaProductoPreferencia>()
                                  .Select(e => e.ID_PRODUCTO_PREFERENCIA)
                                  .ToList();
        if (seleccionados is not null)
        {
            vm.IdsPreferencias = string.Join(",", seleccionados);
        }
    }

    private void Rojo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 0;
        vm.ColorSeleccionado = "Rojo";
    }

    private void Azul_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 1;
        vm.ColorSeleccionado = "Azul";
    }

    private void Amarillo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 2;
        vm.ColorSeleccionado = "Amarillo";
    }

    private void Verde_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 3;
        vm.ColorSeleccionado = "Verde";
    }

    private void Adopcion_Tapped(object sender, TappedEventArgs e)
    {
        SelectEscalaAdopcion.Unfocus();
        SelectEscalaAdopcion.Focus();
    }

    private async void Cancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
        await Shell.Current.Navigation.PopAsync();
    }

    private void Actualizar_Clicked(object sender, EventArgs e)
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.ActualizarMedico();
    }

    private async void InsertarPreferencias()
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.MostrarPreferenciasdeProducto();
        await Task.Delay(1000);
        searchbox_preferencias.SelectedItems?.Clear();
        if (vm.PreferenciasSeleccionadas is not null)
        {
            foreach (var preferencia in vm.PreferenciasSeleccionadas)
            {
                searchbox_preferencias.SelectedItems?.Add(preferencia);
            }
        }
    }
}