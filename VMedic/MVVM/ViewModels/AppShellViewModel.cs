using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;

namespace VMedic.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class AppShellViewModel : BaseViewModel
    {
        public AppShellViewModel()
        {

        }

        public void SignOut()
        {
            if (App.Current is not null)
            {
                Preferences.Default.Clear();
                App.usuario?.DeleteItems();
                App.doctores?.DeleteItems();
                App.niveles?.DeleteItems();
                App.categorias?.DeleteItems();
                App.subcategorias?.DeleteItems();
                App.tiposvisitas?.DeleteItems();
                App.visitasmensuales?.DeleteItems();
                App.lugaresventas?.DeleteItems();
                App.materiales?.DeleteItems();
                App.muestras?.DeleteItems();
                App.skuproductos?.DeleteItems();
                App.visitas?.DeleteItems();
                App.evaluaciondetalles?.DeleteItems();
                App.evaluacionencabezado?.DeleteItems();
                App.SolicitudesPendientes?.DeleteItems();

                App.Current.MainPage = new AppShell();
            }
        }
    }
}
