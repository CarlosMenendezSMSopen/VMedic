using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Utilidades
{
    public static class CerrarTeclado
    {
        public static void Close()
        {
#if ANDROID
            var imm = (Android.Views.InputMethods.InputMethodManager?)Android.App.Application.Context.GetSystemService(Android.Content.Context.InputMethodService);

            var activity = Platform.CurrentActivity;

            if (activity?.CurrentFocus != null)
            {
                imm?.HideSoftInputFromWindow(
                    activity.CurrentFocus.WindowToken,
                    Android.Views.InputMethods.HideSoftInputFlags.None);
            }

            activity?.CurrentFocus?.ClearFocus();
#endif
        }
    }
}
