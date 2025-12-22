using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Behaviors
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        public bool _isBusy;
        [ObservableProperty]
        public string? _title;

        public void SignOut()
        {
            try
            {
                if (App.Current is not null)
                {
                    Preferences.Default.Clear();
                    App.Usuario?.DeleteItems();
                    App.Doctores?.DeleteItems();
                    App.Niveles?.DeleteItems();
                    App.Categorias?.DeleteItems();
                    App.Subcategorias?.DeleteItems();
                    App.Tiposvisitas?.DeleteItems();
                    App.Visitasmensuales?.DeleteItems();
                    App.Lugaresventas?.DeleteItems();
                    App.Materiales?.DeleteItems();
                    App.Muestras?.DeleteItems();
                    App.Skuproductos?.DeleteItems();
                    App.Visitas?.DeleteItems();
                    App.Evaluaciondetalles?.DeleteItems();
                    App.Evaluacionencabezado?.DeleteItems();
                    App.SolicitudesPendientes?.DeleteItems();

                    App.Current.Windows[0].Page = new AppShell();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
