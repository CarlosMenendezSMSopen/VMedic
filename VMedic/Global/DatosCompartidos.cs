using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.Global
{
    public static class DatosCompartidos
    {
        public static object? sender { get; set; } = null;
        public static string? ErrorResponseValue { get; set; }
        public static VerticalStackLayout? ListaEvaluaciones { get; set; }
        public static int StatusVolver { get; set; } = 0;
        public static dynamic? EvaluacionEditar { get; set; } = null;
    }
}
