using VMedic.Global;
using VMedic.MVVM.ViewModels.Sincronizaciones;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Sincronizaciones;

public partial class SincronizacionView : ContentPage
{
    private int positionList = 0;
    private int CantidadModulos;
    public SincronizacionView()
    {
        InitializeComponent();
        DatosCompartidos.ListaSolicitudesPendientes = List_Pendientes;
        BindingContext = new SincronizacionViewModel();
    }

    private async void btn_bajar_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(null);

            if (positionList < CantidadModulos)
            {
                positionList++;

                var targetModulo = List_Pendientes.Children[positionList] as Border;

                if (targetModulo is not null)
                {
                    PressedPreferences.EndPressed();
                    await Scroll.ScrollToAsync(0, targetModulo.Y - 10, true);
                }
            }
        }
    }

    private async void btnSubir_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(null);
            if (positionList > 0)
            {
                positionList--;

                var targetModulo = List_Pendientes.Children[positionList] as Border;

                if (targetModulo is not null)
                {
                    PressedPreferences.EndPressed();
                    await Scroll.ScrollToAsync(0, targetModulo.Y - 10, true);
                }
            }
        }
    }

    private void Scroll_SizeChanged(object sender, EventArgs e)
    {
        int AltoPantalla = (int)(DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density);

        CantidadModulos = List_Pendientes.Children.OfType<Border>().ToList().Count;

        if (List_Pendientes.Height > AltoPantalla && CantidadModulos > 1)
        {
            Container_Btns.IsVisible = true;
        }
    }
}