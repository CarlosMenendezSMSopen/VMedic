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
        public static Dictionary<int, string>? ErrorResponseValue { get; set; }
        public static int CantidadIntentos { get; set; } = 0;
        public static VerticalStackLayout? ListaEvaluaciones { get; set; }
        public static dynamic? EvaluacionEditar { get; set; } = null;
        public static VerticalStackLayout? ListaMedicos { get; set; }
        public static string? TextoBusquedaMedicos { get; set; } = "";
        public static string? TextoBusquedaProductos { get; set; } = "";
        public static Map? MapaUbicaiconMedico { get; set; }
        public static Label? Lbl_CatntidadPendientes_Medicos { get; set; }
        public static string[] OperacionesIDVisitas { get; set; } = ["VMedicA017", "VMedicA038", "VMedicA046", "VMedicA043"];
        public static string[] OperacionesIDMedicos { get; set; } = ["VMedicA014", "VMedicA048", "VMedicA041", "VMedicA042"];
        public static string[] OperacionesIDPlanifiacion { get; set; } = ["VMedicA047", "VMedicA054"];
        public static VerticalStackLayout? ListaVisitasPendientes { get; set; }
        public static VerticalStackLayout? ListaMedicosPendientes { get; set; }
        public static VerticalStackLayout? ListaProductos { get; set; }
        public static VerticalStackLayout? ListaSolicitudesPendientes { get; set; }
    }
}
