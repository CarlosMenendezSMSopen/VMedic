using Mopups.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic;

namespace VMedic.Utilities
{
    public static class ExceptionMessageMaker
    {
        //metodo que muestra un mensaje de error por Exception de un try catch
        public static void Make(string Title, string exceptionString, string exeMessage, object? contentPage)
        {
			try
			{
                var Ats = exceptionString.Split("at ");
                var erroresEn = string.Join("\n", exceptionString.Contains("Base de datos") ? Ats : Ats.Where(a => a.Contains("FMP.")));
                var texto = $"{exeMessage} \n\n {erroresEn}";

                if (contentPage is Page page)
                {
                    App.Current?.Dispatcher.Dispatch(async delegate
                    {
                        await page.DisplayAlert(Title, texto, "OK");
                    });
                }
                else if (contentPage is ContentPage content)
                {
                    App.Current?.Dispatcher.Dispatch(async delegate
                    {
                        await content.DisplayAlert(Title, texto, "OK");
                    });
                }
                else if (contentPage is PopupPage popup)
                {
                    App.Current?.Dispatcher.Dispatch(async delegate
                    {
                        await popup.DisplayAlert(Title, texto, "OK");
                    });
                }
            }
			catch (Exception ex)
			{
                Console.WriteLine("Error texto: " + ex.ToString());
			}
        }
    }
}
