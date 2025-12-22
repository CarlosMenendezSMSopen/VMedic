using Mopups.Services;
using Syncfusion.Maui.Scheduler;
using VMedic.Global;
using VMedic.MVVM.ViewModels.Planificacion;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Planificacion;

public partial class PlanificacionView : ContentPage
{
	public PlanificacionView()
	{
		InitializeComponent();
        //calendario.HeightRequest = DeviceDisplay.Current.MainDisplayInfo.Height / DeviceDisplay.Current.MainDisplayInfo.Density;
        DatosCompartidos.ContenedorCuentaPlanificacion = container_cuenta;
        DatosCompartidos.LabelContarPendientesPlanificacion = lbl_PlanificacionesPendientes;
        DatosCompartidos.CalendarioPlanificacion = calendario;
        BindingContext = new PlanificacionViewModel();
	}

    private async void calendario_QueryAppointments(object sender, SchedulerQueryAppointmentsEventArgs e)
    {
        status.IsRunning = true;
        calendario.SuspendAppointmentViewUpdate();
        var vm  = (PlanificacionViewModel)BindingContext;
        if (AgendaTareas.IsVisible)
        {
            vm.ShowAgendaDia(false);
            calendario.SelectedDate = null;
            await Task.Delay(1000);
        }
        await vm.ObtenerPlanificaciones(calendario, status);
    }

    private async void AgendaTareas_QueryAppointments(object sender, SchedulerQueryAppointmentsEventArgs e)
    {
        calendario.ResumeAppointmentViewUpdate();
        DateTime[] RangeCalendarDisplay = [ calendario.DisplayDate.Date, calendario.DisplayDate.Date.AddDays(20) ];

        /*if (AgendaTareas.DisplayDate < RangeCalendarDisplay[0] || AgendaTareas.DisplayDate > RangeCalendarDisplay[1])
        {
            calendario.SuspendAppointmentViewUpdate();
            calendario.DisplayDate = AgendaTareas.DisplayDate.AddDays(-14);

            await Task.Delay(1000);
        }*/

        calendario.SelectedDate = AgendaTareas.DisplayDate;
        
        AgendaTareas.SuspendAppointmentViewUpdate();
        var vm = (PlanificacionViewModel)BindingContext;
        vm.VisualizarTareas(calendario, AgendaTareas, AgendaTareas.DisplayDate.Date, status);
    }

    public async void btn_actualizar_Clicked(object? sender, EventArgs? e)
    {
        status.IsRunning = true;
        calendario.SuspendAppointmentViewUpdate();
        var vm = (PlanificacionViewModel)BindingContext;
        if (App.SolicitudesPendientes?.GetItems()?.Where(SP => DatosCompartidos.OperacionesIDPlanifiacion.Contains(SP.OperacionID)).ToList()?.Count > 0)
        {
            vm.EnviarSolicitudesPendientes();
        }
        await vm.ObtenerPlanificaciones(calendario, status);
    }

    private void calendario_SelectionChanged(object sender, SchedulerSelectionChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            var vm = (PlanificacionViewModel)BindingContext;
            vm.VisualizarTareas(calendario, AgendaTareas, (DateTime)e.NewValue, status);
        }
    }

    private void btn_closeAgenda_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            var vm = (PlanificacionViewModel)BindingContext;
            vm.CerrarVistaAgenda(calendario, AgendaTareas, status);
        }
    }

    private void btn_agregarVisita_Clicked(object sender, EventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            MopupService.Instance.PushAsync(new NuevaVisitaPlanificacionView(this));
        }
    }

    private void AgendaTareas_Tapped(object sender, SchedulerTappedEventArgs e)
    {
        if (PressedPreferences.ValidatePressing())
        {
            PressedPreferences.Pressing(sender);

            if (e.Appointments is not null)
            {
                if (e.Appointments.Count > 0)
                {
                    var vm = (PlanificacionViewModel)BindingContext;
                    vm.EliminarVisitaAgenda((SchedulerAppointment?)e.Appointments.FirstOrDefault());
                }
            }
            else
            {
                PressedPreferences.EndPressed();
            }
            
        }
    }

    private void calendario_ViewChanged(object sender, SchedulerViewChangedEventArgs e)
    {

    }
}