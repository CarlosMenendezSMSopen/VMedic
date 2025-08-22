using Mopups.Pages;
using Mopups.Services;
using VMedic.MVVM.Views.Médicos;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Menus;

public partial class MenuMedicos : PopupPage
{
	public MenuMedicos()
	{
		InitializeComponent();
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
            Shell.Current.Navigation.PushAsync(new EditorMedicoView(1));
        }
    }

    private void EnviarPendientes_Tapped(object sender, TappedEventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);
        }
    }
}