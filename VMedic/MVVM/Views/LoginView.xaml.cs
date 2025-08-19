using CommunityToolkit.Maui.Behaviors;
using Mopups.Services;
using VMedic.MVVM.ViewModels;
using VMedic.Utilities;

namespace VMedic.MVVM.Views;

public partial class LoginView : ContentPage
{
    public LoginView()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
        PressedPreferences.EndPressed();

        IList<Behavior> behaviors = new List<Behavior>
        {
            new StatusBarBehavior
            {
                StatusBarColor = (Color)Application.Current.Resources["PrimaryDark"],
                StatusBarStyle = CommunityToolkit.Maui.Core.StatusBarStyle.LightContent
            }
        };
    }

    //metodo evento para habilitar el dialogo de configuracion de licencia
    private void btn_configuracion_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PushAsync(new ConfiguracionView());
    }
}