using CommunityToolkit.Maui.Alerts;
using Mopups.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Utilities
{
    public static class ToastMaker
    {
        //metodo para enviar un mensaje de alerta dependiendo de la plataforma en que se este ejecutando la aplicación
        public static async void Make(string? texto, object? contentPage)
        {
            if (texto != null)
                if (DeviceInfo.Platform == DevicePlatform.Android && texto.Length <= 66)
                {
                    await Toast.Make(texto, CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                }
                else
                {
                    await Task.Delay(1000);
                    if (contentPage is Page page)
                    {
                        await page.DisplayAlert("Aviso", texto, "OK");
                    }
                    else if (contentPage is ContentPage content)
                    {
                        await content.DisplayAlert("Aviso", texto, "OK");
                    }
                    else if (contentPage is PopupPage popup)
                    {
                        await popup.DisplayAlert("Aviso", texto, "OK");
                    }
                }
        }
    }
}
