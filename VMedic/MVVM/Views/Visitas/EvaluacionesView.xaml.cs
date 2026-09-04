using Mopups.Services;
using VMedic.Global;
using VMedic.MVVM.Models.DataBase;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class EvaluacionesView : ContentPage
{
    private TablaVisitasPendientes? Visitas { get; set; }
    private bool Cargado { get; set; } = false;
    public EvaluacionesView(TablaVisitasPendientes visitas, int actualizar_ubicacion)
    {
        InitializeComponent();
        DatosCompartidos.ListaEvaluaciones = List_Evaluaciones;
        BindingContext = new EvaluacionesViewModel(visitas, actualizar_ubicacion);
        Visitas = visitas;
        nombre_doctor.Text = App.Doctores?.GetItems()?.Where(D => D.CODIGO_DE_CLIENTE == Visitas?.CodCliente).Select(D => D.CODIGO_DE_CLIENTE + " - " + D.NOMBRE_COMERCIAL).FirstOrDefault();
        var firmaEvaluacion = App.Evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == visitas.CodCliente)?.Base64Image;
        if (firmaEvaluacion is not null && firmaEvaluacion != "")
        {
            containerbtn_agregarevaluacion.IsEnabled = false;
            containerbtn_agregarevaluacion.BackgroundColor = Colors.DarkGray;
            btn_sign.IsEnabled = false;
        }
    }

    private void SCROLL_Scrolled(object sender, ScrolledEventArgs e)
    {

    }

    private void AgregarEvaluacion_Tapped(object sender, TappedEventArgs e)
    {
        var encabezado = App.Evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == Visitas?.CodCliente);
        if (encabezado is null)
        {
            var tablaEncabezadoEvaluacion = new TablaEncabezadoEvaluacion
            {
                IdCliente = Visitas?.CodCliente,
            };
            App.Evaluacionencabezado?.InsertItem(tablaEncabezadoEvaluacion);
        }
        Shell.Current.Navigation.PushAsync(new NuevaEvaluacionView(Visitas?.CodCliente));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Cargado)
        {
            var vm = (EvaluacionesViewModel)BindingContext;
            vm.MostrarEvaluaciones();
        }

        Cargado = true;
    }

    private async void btn_sign_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            if (App.Evaluaciondetalles is not null)
            {
                if (!App.Evaluaciondetalles.IsEmpty())
                {
                    var MainPage = App.Current?.Windows[0].Page;
                    if (MainPage is not null)
                    {
                        var confirmar = await MainPage.DisplayAlert("Aviso", "Al agregar una firma ya no podrá agregar o editar evaluaciones.\n¿Desea firmar las evaluaciones?", "SI", "NO");
                        if (confirmar)
                        {
                            await MopupService.Instance.PushAsync(new CapturarFirmaView(Visitas?.CodCliente, containerbtn_agregarevaluacion, btn_sign, (EvaluacionesViewModel)BindingContext));
                        }
                        else
                        {
                            PressedPreferences.EndPressed();
                        }
                    }
                }
                else
                {
                    ToastMaker.Make("Debe agregar al menos un producto para firmar", App.Current?.Windows[0].Page);
                }
            }
        }
    }

    private void btn_enviarEvaluaciones_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            var vm = (EvaluacionesViewModel)BindingContext;
            vm.EnviarEvaluaciones(containerbtn_agregarevaluacion, btn_sign);
        }
    }

    private void refresh_Refreshing(object sender, EventArgs e)
    {
        var vm = (EvaluacionesViewModel)BindingContext;
        vm.Refresh();
    }
}