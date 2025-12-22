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
    }
}
