using System.Diagnostics;
using System.Timers;
using VMedic.Conversores;
using VMedic.Global;
using VMedic.MVVM.ViewModels.Inventario;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.Utilidades;
using Timer = System.Timers.Timer;

namespace VMedic.MVVM.Views.Inventario;

public partial class InventarioView : ContentPage
{
    private Timer? SearcProductosTimer { get; set; }
    public InventarioView()
	{
		InitializeComponent();
        try
        {
            DatosCompartidos.ListaProductos = List_Productos;
            BindingContext = new InventarioViewModel();

            SearcProductosTimer = new Timer(2500);
            SearcProductosTimer.Elapsed += SearchMedicosElapsed;
        }
        catch (Exception ex)
        {
            ToastMaker.Make("Error Inventario: " + ex, App.Current?.Windows[0].Page);
        }
    }

    //metodo evento que ejecuta la muestra de procudtos depues de una busqueda
    private void SearchMedicosElapsed(object? sender, ElapsedEventArgs e)
    {
        SearcProductosTimer?.Stop();
        App.Current?.Dispatcher.Dispatch(delegate
        {
            var vm = (InventarioViewModel)BindingContext;
            vm.MostrarProductos();
        });
    }

    //metodo evento que obtiene un siguiente lote de registros que no aparecen en la lista
    private async void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {
        var vm = (InventarioViewModel)BindingContext;
        var scroll = sender as ScrollView;

        int itemcount = (List_Productos.Children as IEnumerable<object>).Count();

        double screenheight = ConversorDouble.Parse(scroll?.ContentSize.Height.ToString("0.############"));
        double HeightRecorido = ConversorDouble.Parse((scroll?.Height + scroll?.ScrollY)?.ToString("0.############"));

        if (DatosCompartidos.TextoBusquedaProductos != "")
        {
            var lista = vm.Productos?.Where(P => P.DESCRIPCION_PROD.Contains(DatosCompartidos.TextoBusquedaProductos, StringComparison.OrdinalIgnoreCase) || P.PRODUCTO.Contains(DatosCompartidos.TextoBusquedaProductos, StringComparison.OrdinalIgnoreCase)).ToList();
            if (HeightRecorido == screenheight && itemcount != lista?.Count)
            {
                Debug.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
                await vm.CargarMasProductos(itemcount);
            }
        }
        else if (HeightRecorido == screenheight && itemcount < vm.Productos?.Count)
        {
            Debug.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
            await vm.CargarMasProductos (itemcount);
        }
    }

    //metodo evento para ejecutar el refrescado de la lista
    private void refreshvie_Refreshing(object sender, EventArgs e)
    {
        var vm = (InventarioViewModel)BindingContext;
        vm.Refresh();
    }

    //metodo evento que ejecuta un tiempo de espera despues de que se haya cambiado el texto de un searchbox
    private void searchBox_Producto_TextChanged(object sender, TextChangedEventArgs e)
    {
        DatosCompartidos.TextoBusquedaProductos = e.NewTextValue?.Trim() ?? "";
        SearcProductosTimer?.Stop();
        SearcProductosTimer?.Start();
    }

    //metodo evento que cierra la caja de texto de busqueda
    private void btn_cancel_text_Clicked(object sender, EventArgs e)
    {
        titulo.IsVisible = true;
        btn_habilitarBusqueda.IsVisible = true;
        SearchProducto.IsVisible = false;
        searchBox_Producto.Text = "";
        searchBox_Producto.Unfocus();
    }

    //metodo evento que habilita la caja de texto de busqueda
    private void btn_habilitarBusqueda_Clicked(object sender, EventArgs e)
    {
        titulo.IsVisible = false;
        btn_habilitarBusqueda.IsVisible = false;
        SearchProducto.IsVisible = true;
        searchBox_Producto.Focus();
    }
}