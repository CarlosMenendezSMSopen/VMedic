using Mopups.Pages;
using Mopups.Services;
using VMedic.Global;
using VMedic.MVVM.Views.Médicos;
using VMedic.MVVM.Views.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Menus;

public partial class MenuMedicos : PopupPage
{
	public MenuMedicos()
	{
		InitializeComponent();
        lbl_cantidadPendientes.Text = App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDMedicos.Contains(SP.OperacionID)).ToList().Count + "";

        if (DeviceInfo.Platform == DevicePlatform.Android)
		{
            Gridcontenedor.Margin = new Thickness(0, 0, 10, 60);
        }
		else
		{
			Gridcontenedor.Margin = new Thickness(0, 0, 10, 95);
        }
	}

    private void NewMedico_Tapped(object sender, TappedEventArgs e)
    {
		if (PressedPreferences.ValidatePressing())
		{
			PressedPreferences.Pressing(sender);

            MopupService.Instance.PopAllAsync();
            Shell.Current.Navigation.PushAsync(new NuevoMedicoView());
        }
    }

    private void EnviarPendientes_Tapped(object sender, TappedEventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            if (App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDMedicos.Contains(SP.OperacionID)).ToList().Count > 0)
            {
                MopupService.Instance.PopAllAsync();
                Shell.Current.Navigation.PushAsync(new MedicosPendientesView());
            }
            else
            {
                ToastMaker.Make("No hay medicos pendientes por enviar", App.Current?.Windows[0].Page);
                PressedPreferences.EndPressed();
            }
        }
    }
}