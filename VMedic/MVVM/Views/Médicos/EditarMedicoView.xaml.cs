using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.Servicios;

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

    //metodo evento que habilita la lista desplegable de las especialidades
    private void Especialidad_Tapped(object sender, TappedEventArgs e)
    {
        SelectEspecialidades.Unfocus();
        SelectEspecialidades.Focus();
    }

    //metodo evento que habilira la lista desplegable de las categorias de visita
    private void Categoria_Tapped(object sender, TappedEventArgs e)
    {
        SelectCategorias.Unfocus();
        SelectCategorias.Focus();
    }

    //metodo evento que concatena la selección de las rpeferencias de visita
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

    //metodo evento que asigna variables cuando se selecciona el tipo de visita en Rojo
    private void Rojo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 0;
        vm.ColorSeleccionado = "Rojo";
    }

    //metodo evento que asigna variables cuando se selecciona el tipo de visita en Azul
    private void Azul_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 1;
        vm.ColorSeleccionado = "Azul";
    }

    //metodo evento que asigna variables cuando se selecciona el tipo de visita en Amarillo
    private void Amarillo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 2;
        vm.ColorSeleccionado = "Amarillo";
    }

    //metodo evento que asigna variables cuando se selecciona el tipo de visita en Verde
    private void Verde_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.Position = 3;
        vm.ColorSeleccionado = "Verde";
    }

    //metodo evento que habilita la lista desplegable de la forma de adaptación de visita
    private void Adopcion_Tapped(object sender, TappedEventArgs e)
    {
        SelectEscalaAdopcion.Unfocus();
        SelectEscalaAdopcion.Focus();
    }

    //metodo evento que cierra el formulario completo al precionar el boton de cancelar
    private async void Cancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
        await Shell.Current.Navigation.PopAsync();
    }

    //metodo evento que activa el envío de los cambios de los datos hechos al medico
    private void Actualizar_Clicked(object sender, EventArgs e)
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.ActualizarMedico();
    }

    //metodo que concatena las preferencias seleccionadas
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