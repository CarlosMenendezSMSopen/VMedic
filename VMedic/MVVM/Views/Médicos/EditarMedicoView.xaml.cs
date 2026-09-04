using System.Diagnostics;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.Servicios;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Médicos;

public partial class EditarMedicoView : ContentPage
{
	public EditarMedicoView(string? cODIGO_DE_CLIENTE)
	{
		InitializeComponent();    
        BindingContext = new EditarMedicoViewModel(cODIGO_DE_CLIENTE);
        //InsertarPreferencias();
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
        //var vm = (EditarMedicoViewModel)BindingContext;
        //vm.;
        //await Task.Delay(1000);
        //searchbox_preferencias.SelectedItems?.Clear();
        //if (vm.PreferenciasSeleccionadas is not null)
        //{
        //    foreach (var preferencia in vm.PreferenciasSeleccionadas)
        //    {
        //        searchbox_preferencias.SelectedItems?.Add(preferencia);
        //    }
        //}
    }

    protected override bool OnBackButtonPressed()
    {
        if (searchbox_preferencias.IsDropDownOpen)
        {
            searchbox_preferencias.IsDropDownOpen = false;
        }
        return base.OnBackButtonPressed();
    }

    private void searchbox_pais_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.MostrarDatosDepartamentos();
    }

    private void searchbox_departamento_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        var vm = (EditarMedicoViewModel)BindingContext;
        vm.MostrarDatosMunicipios();
    }

    public void sw_repetir_StateChanged(object? sender, Syncfusion.Maui.Buttons.SwitchStateChangedEventArgs? e)
    {
        if (e?.NewValue is not null)
        {
            date_FechaInicial.IsEnabled = !(bool)e.NewValue;
            var vm = (EditarMedicoViewModel)BindingContext;
            vm.EnableRepetir = (bool)e.NewValue;
        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            //if (sender is not Entry entry)
            //    return;

            //string texto = new string(e.NewTextValue?
            //    .Where(char.IsDigit)
            //    .ToArray() ?? []);

            //if (texto.Length > 9)
            //    texto = texto[..9];

            //string formateado = texto.Length > 4
            //    ? $"{texto[..4]} {texto[4..]}"
            //    : texto;

            //if (entry.Text != formateado)
            //    entry.Text = formateado;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}