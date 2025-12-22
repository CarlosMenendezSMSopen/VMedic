using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Efectos;
using VMedic.Global;

namespace VMedic.Utilidades
{
    public static class PressedPreferences
    {
        public static bool ValidatePressing()
        {
            if (Preferences.Default.ContainsKey("press"))
            {
                if (Preferences.Default.Get("press", 0) == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Preferences.Default.Set("press", 0);
                return true;
            }
        }

        public static async void Pressing(object? sender)
        {
            Preferences.Default.Set("press", 1);
            if (sender != null)
            {
                DatosCompartidos.Sender = sender;
                ButtonEffect.PressedEffectsON(sender);
            }
            await Task.Delay(1000);
        }

        public static async void EndPressed()
        {
            await Task.Delay(1000);
            Preferences.Default.Set("press", 0);
            if (DatosCompartidos.Sender != null)
            {
                await ButtonEffect.PressedEffectsOFF(DatosCompartidos.Sender);
                DatosCompartidos.Sender = null;
            }
        }
    }
}
