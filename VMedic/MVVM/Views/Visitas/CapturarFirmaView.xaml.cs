using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics.Platform;
using Mopups.Pages;
using Mopups.Services;
using System.Xml.Serialization;
using VMedic.MVVM.Models;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Visitas;

public partial class CapturarFirmaView : PopupPage
{
    private DrawingDrawable? Drawable { get; set; } = new();
    private List<PointF>? CurrentLine { get; set; }
    private string? IDCliente { get; set; }
    private Border? AgregarEvaluacion { get; set; }
    private ImageButton? BtnFirmar { get; set; }
    private EvaluacionesViewModel? Context { get; set; }
    public CapturarFirmaView(string? codCliente, Border containerbtn_agregarevaluacion, ImageButton btn_sign, EvaluacionesViewModel bindingContext)
    {
        InitializeComponent();
        IDCliente = codCliente;
        AgregarEvaluacion = containerbtn_agregarevaluacion;
        BtnFirmar = btn_sign;
        Context = bindingContext;
        dibujoFirma.Drawable = Drawable;
        PressedPreferences.EndPressed();
    }


    private void Close_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAllAsync();
    }

    private void DibujoFirma_StartInteraction(object sender, TouchEventArgs e)
    {
        var p = e.Touches[0];
        CurrentLine = new List<PointF> { p };
        Drawable?.Lines.Add(CurrentLine);
        dibujoFirma.Invalidate();
        btn_firmar.IsEnabled = true;
    }

    private void DibujoFirma_DragInteraction(object sender, TouchEventArgs e)
    {
        var p = e.Touches[0];
        CurrentLine?.Add(p);
        dibujoFirma.Invalidate();
    }

    private void DibujoFirma_EndInteraction(object sender, TouchEventArgs e)
    {

    }   

    private void Btn_limpiar_Clicked(object sender, EventArgs e)
    {
        Drawable?.Lines.Clear();
        dibujoFirma.Invalidate();
    }

    private async void Btn_firmar_Clicked(object sender, EventArgs e)
    {
        var Encabezado = App.Evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == IDCliente);
        if (Encabezado is not null)
        {
            Encabezado.Base64Image = await DibujoaBase64();

            App.Evaluacionencabezado?.UpdateITEM(Encabezado);

            await MopupService.Instance.PopAllAsync();

            if (AgregarEvaluacion is not null && BtnFirmar is not null)
            {
                AgregarEvaluacion.IsEnabled = false;
                AgregarEvaluacion.BackgroundColor = Colors.DarkGray;
                BtnFirmar.IsEnabled = false;

                Context?.MostrarEvaluaciones();
            }

            ToastMaker.Make("Evaluaciones firmadas con éxito", App.Current?.Windows[0].Page);
        }
    }

    private async Task<string> DibujoaBase64()
    {
        var screenshotResult = await dibujoFirma.CaptureAsync();

        if (screenshotResult != null)
        {
            using var ms = new MemoryStream();
            await screenshotResult.CopyToAsync(ms);
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        return "";
    }
}