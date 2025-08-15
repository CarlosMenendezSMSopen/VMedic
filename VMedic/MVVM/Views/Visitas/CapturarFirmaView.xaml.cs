using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics.Platform;
using Mopups.Pages;
using Mopups.Services;
using System.Xml.Serialization;
using VMedic.MVVM.Models;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.Utilities;

namespace VMedic.MVVM.Views.Visitas;

public partial class CapturarFirmaView : PopupPage
{
    private DrawingDrawable drawable { get; set; }
    private List<PointF>? currentLine { get; set; }
    private string? IDCliente { get; set; }
    private Frame? AgregarEvaluacion { get; set; }
    private ImageButton? btnFirmar { get; set; }
    private EvaluacionesViewModel? Context { get; set; }
    public CapturarFirmaView(string? codCliente, Frame containerbtn_agregarevaluacion, ImageButton btn_sign, EvaluacionesViewModel bindingContext)
    {
        InitializeComponent();
        IDCliente = codCliente;
        AgregarEvaluacion = containerbtn_agregarevaluacion;
        btnFirmar = btn_sign;
        Context = bindingContext;
        drawable = new DrawingDrawable();
        dibujoFirma.Drawable = drawable;
        PressedPreferences.EndPressed();
    }


    private void Close_Clicked(object sender, EventArgs e)
    {
        MopupService.Instance.PopAllAsync();
    }

    private void dibujoFirma_StartInteraction(object sender, TouchEventArgs e)
    {
        currentLine = new List<PointF> { new PointF(e.Touches[0].X, e.Touches[0].Y) };
        drawable.Lines.Add(currentLine);
        dibujoFirma.Invalidate();
        btn_firmar.IsEnabled = true;
    }

    private void dibujoFirma_DragInteraction(object sender, TouchEventArgs e)
    {
        currentLine?.Add(new PointF(e.Touches[0].X, e.Touches[0].Y));
        dibujoFirma.Invalidate();
    }

    private void dibujoFirma_EndInteraction(object sender, TouchEventArgs e)
    {

    }   

    private void btn_limpiar_Clicked(object sender, EventArgs e)
    {
        drawable.Lines.Clear();
        dibujoFirma.Invalidate();
    }

    private async void btn_firmar_Clicked(object sender, EventArgs e)
    {
        var Encabezado = App.evaluacionencabezado?.GetItems()?.FirstOrDefault(Eenc => Eenc.IdCliente == IDCliente);
        if (Encabezado is not null)
        {
            Encabezado.Base64Image = await DibujoaBase64();

            App.evaluacionencabezado?.UpdateITEM(Encabezado);

            await MopupService.Instance.PopAllAsync();

            if (AgregarEvaluacion is not null && btnFirmar is not null)
            {
                AgregarEvaluacion.IsEnabled = false;
                AgregarEvaluacion.BackgroundColor = Colors.DarkGray;
                btnFirmar.IsEnabled = false;

                Context?.MostrarEvaluaciones();
            }

            ToastMaker.Make("Evaluaciones firmadas con éxito", App.Current?.MainPage);
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