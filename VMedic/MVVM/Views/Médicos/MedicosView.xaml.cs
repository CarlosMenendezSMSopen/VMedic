using Mopups.Services;
using System.Diagnostics;
using System.Timers;
using VMedic.Conversores;
using VMedic.Global;
using VMedic.MVVM.ViewModels;
using VMedic.MVVM.ViewModels.Medicos;
using VMedic.MVVM.Views.Menus;
using VMedic.Servicios;
using Timer = System.Timers.Timer;

namespace VMedic.MVVM.Views;

public partial class MedicosView : ContentPage
{
    private Timer? SearchMedicosTimer { get; set; }
    public MedicosView()
	{
		InitializeComponent();
        DatosCompartidos.ListaMedicos = List_Medicos;
        SincronizacionDataBase.ObtenerEspecialidades();

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            btn_cancel_text.Padding = 5;
        }
        else
        {
            btn_cancel_text.Padding = 12;
        }

        BindingContext = new MedicosViewModel();

        SearchMedicosTimer = new Timer(2500);
        SearchMedicosTimer.Elapsed += SearchMedicosElapsed;
    }

    private void SearchMedicosElapsed(object? sender, ElapsedEventArgs e)
    {
        SearchMedicosTimer?.Stop();
        App.Current?.Dispatcher.Dispatch(delegate
        {
            var vm = (MedicosViewModel)BindingContext;
            vm.MostrarMedicos();
        });
    }

    private void searchBox_Medico_TextChanged(object sender, TextChangedEventArgs e)
    {
        DatosCompartidos.TextoBusquedaMedicos = e.NewTextValue?.Trim() ?? "";
        SearchMedicosTimer?.Stop();
        SearchMedicosTimer?.Start();
    }

    private void btn_cancel_text_Clicked(object sender, EventArgs e)
    {
        titulo.IsVisible = true;
        btn_habilitarBuscar.IsVisible = true;
        SearchMedico.IsVisible = false;
        searchBox_Medico.Text = "";
        searchBox_Medico.Unfocus();
    }

    private void btn_habilitarBuscar_Clicked(object sender, EventArgs e)
    {
        titulo.IsVisible = false;
        btn_habilitarBuscar.IsVisible = false;
        SearchMedico.IsVisible = true;
        searchBox_Medico.Focus();
    }

    private async void scroll_container_Scrolled(object sender, ScrolledEventArgs e)
    {
        var vm = (MedicosViewModel)BindingContext;
        var scroll = sender as ScrollView;

        int itemcount = (List_Medicos.Children as IEnumerable<object>).Count();

        double screenheight = ConversorDouble.Parse(scroll?.ContentSize.Height.ToString("0.############"));
        double HeightRecorido = ConversorDouble.Parse((scroll?.Height + scroll?.ScrollY)?.ToString("0.############"));

        if (DatosCompartidos.TextoBusquedaMedicos != "")
        {
            var lista = vm.Medicos?.Where(M => M.NOMBRE_COMERCIAL.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase) || M.DESCRIPCION_CLASE.Contains(DatosCompartidos.TextoBusquedaMedicos, StringComparison.OrdinalIgnoreCase)).ToList();
            if (HeightRecorido == screenheight && itemcount != lista?.Count)
            {
                Debug.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
                await vm.CargarMasMedicos(itemcount);
            }
        }
        else if (HeightRecorido == screenheight && itemcount < vm.Medicos?.Count)
        {
            Debug.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
            await vm.CargarMasMedicos(itemcount);
        }
    }

    private void AgregarMedicos_Tapped(object sender, TappedEventArgs e)
    {
        MopupService.Instance.PushAsync(new MenuMedicos());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (DatosCompartidos.StatusVolver == 1)
        {
            var vm = (MedicosViewModel)BindingContext;
            vm.MostrarMedicos();
            DatosCompartidos.StatusVolver = 0;
        }
    }

    private void refresh_Refreshing(object sender, EventArgs e)
    {
        var vm = (MedicosViewModel)BindingContext;
        vm.Refresh();
    }
}