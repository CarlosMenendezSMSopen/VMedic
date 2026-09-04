using Mopups.Services;
using Syncfusion.Maui.Scheduler;
using System.Diagnostics;
using VMedic.Global;
using VMedic.MVVM.ViewModels.Planificacion;
using VMedic.Utilidades;

namespace VMedic.MVVM.Views.Planificacion;

public partial class PlanificacionView : ContentPage
{
    private bool Cargado { get; set; } = false;
    public PlanificacionView()
    {
        InitializeComponent();
        busyIndicator.IsRunning = true;
        BindingContext = new PlanificacionViewModel();
    }

    private void Calendario_QueryAppointments(object sender, SchedulerQueryAppointmentsEventArgs e)
    {
        calendario.SuspendAppointmentViewUpdate();
        var vm = (PlanificacionViewModel)BindingContext;
        vm.ObtenerPlanificaciones(calendario, AgendaTareas);
    }

    public void Btn_actualizar_Clicked(object? sender, EventArgs? e)
    {
        busyIndicator.IsRunning = true;
        calendario.SuspendAppointmentViewUpdate();
        var vm = (PlanificacionViewModel)BindingContext;
#pragma warning disable CS8625 // No se puede convertir un literal NULL en un tipo de referencia que no acepta valores NULL.
        AgendaTareas.AppointmentsSource = null;
#pragma warning restore CS8625 // No se puede convertir un literal NULL en un tipo de referencia que no acepta valores NULL.
        vm.ObtenerPlanificaciones(calendario, AgendaTareas);
    }

    private void Btn_agregarVisita_Clicked(object sender, EventArgs e)
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
                    vm.EliminarVisitaAgenda((SchedulerAppointment?)e.Appointments.FirstOrDefault(), this);
                }
            }
            else
            {
                PressedPreferences.EndPressed();
            }
        }
    }

    protected override async void OnAppearing()
    {
        if (Cargado)
        {
            busyIndicator.IsRunning = true;
            await Task.Delay(500);
            Btn_actualizar_Clicked(null, null);
        }

        Cargado = true;

        base.OnAppearing();
    }

    private void AgendaTareas_ChildAdded(object sender, ElementEventArgs e)
    {

    }

    private void AgendaTareas_DescendantAdded(object sender, ElementEventArgs e)
    {

    }

    private void AgendaTareas_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Debug.WriteLine("propiedades: " + e.PropertyName);
    }

    private async void AgendaTareas_ViewChanged(object sender, SchedulerViewChangedEventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Task.Yield();

            await Task.Delay(5000);

            busyIndicator.IsRunning = false;
        });
    }

    private void Calendario_SelectionChanged(object sender, SchedulerSelectionChangedEventArgs e)
    {
        if (e.NewValue is not null)
        {
            AgendaTareas.DisplayDate = (DateTime)e.NewValue;

            var vm = (PlanificacionViewModel)BindingContext;
            vm.VisualizarTareas(calendario, AgendaTareas, new DateTimeOffset(e.NewValue.Value.ToUniversalTime()).ToUnixTimeMilliseconds());
        }
    }
}