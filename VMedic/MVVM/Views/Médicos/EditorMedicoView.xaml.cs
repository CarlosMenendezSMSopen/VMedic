using System.Threading.Tasks;
using VMedic.Global;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.ViewModels.Médicos;
using VMedic.Services;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Médicos;

public partial class EditorMedicoView : ContentPage
{
    public EditorMedicoView(int ModoEditor, string? cODIGO_DE_CLIENTE)
    {
        try
        {
            InitializeComponent();
            SincronizacionDataBase.ObtenerCategoriasMedico();
            SincronizacionDataBase.ObtenerProductosPreferencias();
            BindingContext = new EditorMedicoViewModel(ModoEditor, cODIGO_DE_CLIENTE);
            PressedPreferences.EndPressed();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            InsertarMapa();
        }
    }

    private async void InsertarMapa()
    {
        await Task.Delay(1000);
        Grid.SetRow(DatosCompartidos.MapaUbicaiconMedico, 8);
        Grid.SetColumn(DatosCompartidos.MapaUbicaiconMedico, 0);
        Grid.SetColumnSpan(DatosCompartidos.MapaUbicaiconMedico, 2);
        ContenedorVerInfo.Children.Add(DatosCompartidos.MapaUbicaiconMedico);
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

    private void Preferencia_Tapped(object sender, TappedEventArgs e)
    {
        /*SelectPreferencias.Unfocus();
        SelectPreferencias.Focus();*/
    }

    private void Adopcion_Tapped(object sender, TappedEventArgs e)
    {
        SelectEscalaAdopcion.Unfocus();
        SelectEscalaAdopcion.Focus();
    }

    private void swt_repetirVisita_Toggled(object sender, ToggledEventArgs e)
    {
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.EnableRepetir = e.Value;
    }

    private void Rojo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.Position = 0;
        vm.ColorSeleccionado = "Rojo";
    }

    private void Azul_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.Position = 1;
        vm.ColorSeleccionado = "Azul";
    }

    private void Amarillo_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.Position = 2;
        vm.ColorSeleccionado = "Amarillo";
    }

    private void Verde_Tapped(object sender, TappedEventArgs e)
    {
        ColorSeleccionado.IsVisible = true;
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.Position = 3;
        vm.ColorSeleccionado = "Verde";
    }

    private void Cancelar_Clicked(object sender, EventArgs e)
    {
        Shell.Current.Navigation.PopAsync();
    }

    private void Guardar_Clicked(object sender, EventArgs e)
    {
        var vm = (EditorMedicoViewModel)BindingContext;
        vm.GuardarNuevoMedico();
    }

    private void searchbox_preferencias_DropDownClosed(object sender, EventArgs e)
    {
        var vm = (EditorMedicoViewModel)BindingContext;
        var seleccionados = searchbox_preferencias.SelectedItems?
                                  .Cast<TablaProductoPreferencia>()
                                  .Select(e => e.ID_PRODUCTO_PREFERENCIA)
                                  .ToList();
        if (seleccionados is not null)
        {
            vm.IdsPreferencias = string.Join(",", seleccionados);
        }
    }
}