using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Effects
{
    public static class ButtonEffect
    {
        //metodo tarea que, dependiendo el tipo de objeto, este actua como un boton cuando se presiona
        public static async void PressedEffectsON(object? sender)
        {
            await Task.Run(async () =>
            {
                if (sender is ImageButton)
                {
                    var btn = (ImageButton)sender;
                    await btn.ScaleTo(0.80, 50, Easing.CubicInOut);
                }
                else if (sender is Label)
                {
                    var btn = (Label)sender;
                    await btn.ScaleTo(0.80, 50, Easing.CubicInOut);
                }
                else if (sender is Image)
                {
                    var btn = (Image)sender;
                    await btn.ScaleTo(0.80, 50, Easing.CubicInOut);
                }
                else if (sender is Button)
                {
                    var btn = (Button)sender;
                    await btn.ScaleTo(0.80, 50, Easing.CubicInOut);
                }
                else if (sender is Grid)
                {
                    var btn = (Grid)sender;
                    await btn.ScaleTo(0.90, 50, Easing.CubicInOut);
                }
                else if (sender is Frame)
                {
                    var btn = (Frame)sender;
                    var label = btn.FindByName<Label>("lbl_cerrar");
                    if (label != null)
                    {
                        await label.ScaleTo(0.85, 50, Easing.CubicInOut);
                    }
                    else
                    {
                        await btn.ScaleTo(0.80, 50, Easing.CubicInOut);
                    }
                }
            });

            await Task.Delay(1000);
        }

        //metodo tarea que, dependiendo el tipo de objeto, este actua como un boton cuando regresa a su anterior estado
        public static async Task PressedEffectsOFF(object? sender)
        {
            await Task.Run(async () =>
            {
                if (sender is ImageButton)
                {
                    var btn = (ImageButton)sender;
                    await btn.ScaleTo(1, 50, Easing.CubicInOut);
                }
                else if (sender is Label)
                {
                    var btn = (Label)sender;
                    await btn.ScaleTo(1, 50, Easing.CubicInOut);
                }
                else if (sender is Image)
                {
                    var btn = (Image)sender;
                    await btn.ScaleTo(1, 50, Easing.CubicInOut);
                }
                else if (sender is Button)
                {
                    var btn = (Button)sender;
                    await btn.ScaleTo(1, 50, Easing.CubicInOut);
                }
                else if (sender is Grid)
                {
                    var btn = (Grid)sender;
                    await btn.ScaleTo(1, 50, Easing.CubicInOut);
                }
                else if (sender is Frame)
                {
                    var btn = (Frame)sender;
                    var label = btn.FindByName<Label>("lbl_cerrar");
                    if (label != null)
                    {
                        await label.ScaleTo(1, 50, Easing.CubicInOut);
                    }
                    else
                    {
                        await btn.ScaleTo(1, 50, Easing.CubicInOut);
                    }

                }
            });

            await Task.Delay(1000);
        }
    }
}
