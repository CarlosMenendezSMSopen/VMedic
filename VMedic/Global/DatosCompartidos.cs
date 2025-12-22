using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace VMedic.Global
{
    public static class DatosCompartidos
    {
        public static Label? Lbl_UsuarioNombre { get; set; }
        public static object? Sender { get; set; } = null;
        public static string? ErrorResponseValue { get; set; }
        public static VerticalStackLayout? ListaEvaluaciones { get; set; }
        public static int StatusVolver { get; set; } = 0;
        public static dynamic? EvaluacionEditar { get; set; } = null;
        public static VerticalStackLayout? ListaMedicos { get; set; }
        public static string? TextoBusquedaMedicos { get; set; } = "";
        public static string? TextoBusquedaProductos { get; set; } = "";
        public static Map? MapaUbicaiconMedico { get; set; }
        public static Label? Lbl_CatntidadPendientes_Visitas { get; set; }
        public static Label? Lbl_CatntidadPendientes_Medicos { get; set; }
        public static string[] OperacionesIDVisitas { get; set; } = ["VMedicA017", "VMedicA038", "VMedicA046", "VMedicA043"];
        public static string[] OperacionesIDMedicos { get; set; } = ["VMedicA014", "VMedicA021", "VMedicA041", "VMedicA042"];
        public static string[] OperacionesIDPlanifiacion { get; set; } = ["VMedicA047", "VMedicA048"];
        public static VerticalStackLayout? ListaVisitasPendientes { get; set; }
        public static VerticalStackLayout? ListaMedicosPendientes { get; set; }
        public static SfScheduler? CalendarioPlanificacion { get; internal set; }
        public static Grid? ContenedorCuentaPlanificacion { get; internal set; }
        public static Label? LabelContarPendientesPlanificacion { get; internal set; }
        public static VerticalStackLayout? ListaProductos { get; set; }
    }
}
