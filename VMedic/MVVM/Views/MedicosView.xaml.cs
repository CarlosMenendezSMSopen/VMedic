using System.Timers;
using VMedic.Converters;
using VMedic.Global;
using VMedic.MVVM.ViewModels;
using VMedic.Services;
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
            containerbtn_agregaremedicos.CornerRadius = 50;
            btn_cancel_text.Padding = 5;
        }
        else
        {
            containerbtn_agregaremedicos.CornerRadius = 27;
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
                Console.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
                await vm.CargarMasMedicos(status, itemcount);
            }
        }
        else if (HeightRecorido == screenheight && itemcount < vm.Medicos?.Count)
        {
            Console.WriteLine($"Scroll: {scroll?.Height} + {scroll?.ScrollY} = {HeightRecorido} == {screenheight}");
            await vm.CargarMasMedicos(status, itemcount);
        }
    }

    private void AgregarMedicos_Tapped(object sender, TappedEventArgs e)
    {

    }
}