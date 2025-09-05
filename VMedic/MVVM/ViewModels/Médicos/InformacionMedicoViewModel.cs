using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using PropertyChanged;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VMedic.Behaviors;
using VMedic.Global;
using VMedic.MVVM.Models.CustomMapObjects;
using VMedic.MVVM.Models.DatabaseTables;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace VMedic.MVVM.ViewModels.Médicos
{
    [AddINotifyPropertyChangedInterface]
    public partial class InformacionMedicoViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string? _medicoName = "";

        [ObservableProperty]
        private string? _medicoEspecialidad = "";

        [ObservableProperty]
        private string? _medicoContact = "";

        [ObservableProperty]
        private string? _medicoTelefono = "";

        [ObservableProperty]
        private string? _medicoColor = "";

        [ObservableProperty]
        private string? _medicoColorNombre = "";

        [ObservableProperty]
        private string? _medicoCategoria = "";

        [ObservableProperty]
        private string? _medicoAdaptacion = "";

        [ObservableProperty]
        private string? _medicoDireccion = "";

        [ObservableProperty]
        private bool _medicoContactVisible;

        [ObservableProperty]
        private bool _medicoTelefonoVisible;

        [ObservableProperty]
        private bool _medicoCategoriaVisible;

        [ObservableProperty]
        private bool _medicoAdaptacionVisible;

        [ObservableProperty]
        private bool _medicoColorVisible;

        [ObservableProperty]
        private bool _medicoDireccionVisible;

        [ObservableProperty]
        private int _paddingImagen;
        public string? CodigoCliente { get; set; } = "";
        private Assembly? assembly { get; set; }
        private TablaDoctores? Medico { get; set; }
        private Dictionary<string, double> ZonasCalor { get; set; } = new Dictionary<string, double>
        {
            { "#150000FF", TransformarRadio(1) },
            { "#150033FF", TransformarRadio(2) },
            { "#150066FF", TransformarRadio(3) },
            { "#150099FF", TransformarRadio(4) },
            { "#1500CCFF", TransformarRadio(5) },
            { "#1500E0FF", TransformarRadio(6) },
            { "#1500FFFF", TransformarRadio(7) },
            { "#1500FFD5", TransformarRadio(8) },
            { "#1500FFCC", TransformarRadio(9) },
            { "#1500FF99", TransformarRadio(10) },
            { "#1500FF00", TransformarRadio(11) },
            { "#1540FF00", TransformarRadio(12) },
            { "#157FFF00", TransformarRadio(13) },
            { "#15A0FF00", TransformarRadio(14) },
            { "#15ADFF2F", TransformarRadio(15) },
            { "#15D0FF00", TransformarRadio(16) },
            { "#15FFFF00", TransformarRadio(17) },
            { "#15FFE000", TransformarRadio(18) },
            { "#15FFD700", TransformarRadio(19) },
            { "#15FFBF00", TransformarRadio(20) },
            { "#15FFA500", TransformarRadio(21) },
            { "#15FF8C00", TransformarRadio(22) },
            { "#15FF7F00", TransformarRadio(23) },
            { "#15FF6347", TransformarRadio(24) },
            { "#15FF5A00", TransformarRadio(25) },
            { "#15FF4500", TransformarRadio(26) },
            { "#15FF2200", TransformarRadio(27) },
            { "#15FF0000", TransformarRadio(28) },
        };
        public InformacionMedicoViewModel(string? cODIGO_DE_CLIENTE)
        {
            CodigoCliente = cODIGO_DE_CLIENTE;
            assembly = Assembly.GetExecutingAssembly();
            DatosCompartidos.MapaUbicaiconMedico = new Map();
            DatosCompartidos.MapaUbicaiconMedico.IsShowingUser = true;
            DatosCompartidos.MapaUbicaiconMedico.MapType = MapType.Street;
            _paddingImagen = DeviceInfo.Platform == DevicePlatform.Android ? 6 : 10;
            MostrarDetallesMedico();
        }

        private static double TransformarRadio(int v)
        {
            double PrimerRadio = 15;
            double UltimoRadio = 1;
            return PrimerRadio + (((UltimoRadio - PrimerRadio) / 27) * (v - 1));
        }

        private async void MostrarDetallesMedico()
        {
            await Task.Delay(1000);
            Medico = App.doctores?.GetItems()?.FirstOrDefault(D => D.CODIGO_DE_CLIENTE == CodigoCliente);
            var especialidadMedico = App.especialidades?.GetItems()?.FirstOrDefault(E => E.CODIGO_DE_CLASE == Medico?.CODIGO_DE_CLASE);
            var categoriaMedico = App.categoriasmedico?.GetItems()?.FirstOrDefault(C => C.CATEGORIAID == Medico?.CATEGORIAID);
            MedicoName = Medico?.NOMBRE_COMERCIAL;
            MedicoEspecialidad = especialidadMedico?.DESCRIPCION_CLASE;
            MedicoContact = Medico?.CONTACTO_CLIENTE;
            MedicoContactVisible = !string.IsNullOrEmpty(MedicoContact);
            MedicoTelefono = Medico?.TELEFONO_CLIENTE;
            MedicoTelefonoVisible = !string.IsNullOrEmpty(MedicoTelefono);
            MedicoCategoria = categoriaMedico?.CATEGORIA;
            MedicoCategoriaVisible = !string.IsNullOrEmpty(MedicoCategoria);
            MedicoAdaptacion = Medico?.ESCALA_ADAPTACION;
            MedicoAdaptacionVisible = !string.IsNullOrEmpty(MedicoAdaptacion);
            MedicoColorNombre = Medico?.COLOR;
            MedicoColor = ObtenerColorMedico(Medico?.COLOR);
            MedicoColorVisible = !string.IsNullOrEmpty(Medico?.COLOR);
            MedicoDireccion = Medico?.DIRECCION_CLIENTE;
            MedicoDireccionVisible = !string.IsNullOrEmpty(MedicoDireccion);

            InsertarUbicacion();
        }

        private void InsertarUbicacion()
        {
            if (Medico is not null)
            {
                var place = new CustomPin
                {
                    ClassId = "1",
                    Label = "",
                    Address = "",
                    Location = new Location(Medico.LATITUD, Medico.LONGITUD),
                    Icon = ImageSource.FromStream(() =>
                    {
                        var resource = $"VMedic.Resources.Images.medic_part1.png";
                        Stream? stream = assembly?.GetManifestResourceStream(resource);
                        if (stream != null)
                        {
                            using (stream)
                            {
                                var image = SKBitmap.Decode(stream);
                                var resizedImage = image.Resize(new SKImageInfo(DeviceInfo.Platform == DevicePlatform.Android ? 76 : 34, DeviceInfo.Platform == DevicePlatform.Android ? 76 : 41), SKSamplingOptions.Default);
                                return new MemoryStream(resizedImage.Encode(SKEncodedImageFormat.Png, 120).ToArray());
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Advertencia: El archivo de imagen no se encontró: {resource}");
                            return null;
                        }
                    }),
                    Anchor = new Point(0.5, 0.875),
                };

                var place2 = new CustomPin
                {
                    ClassId = "2",
                    Label = "",
                    Address = "",
                    Location = new Location(Medico.LATITUD, Medico.LONGITUD),
                    Icon = ImageSource.FromStream(() =>
                    {
                        var resource = $"VMedic.Resources.Images.medic_part2.png";
                        Stream? stream = assembly?.GetManifestResourceStream(resource);
                        if (stream != null)
                        {
                            using (stream)
                            {
                                var image = SKBitmap.Decode(stream);
                                var resizedImage = image.Resize(new SKImageInfo(DeviceInfo.Platform == DevicePlatform.Android ? 64 : 34, DeviceInfo.Platform == DevicePlatform.Android ? 64 : 41), SKSamplingOptions.Default);
                                return new MemoryStream(resizedImage.Encode(SKEncodedImageFormat.Png, 120).ToArray());
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Advertencia: El archivo de imagen no se encontró: {resource}");
                            return null;
                        }
                    }),
                    Anchor = new Point(0.5, 0.870),
                };

                foreach (var calor in ZonasCalor)
                {
                    var circle = new Circle
                    {
                        Center = new Location(Medico.LATITUD, Medico.LONGITUD),
                        Radius = Distance.FromMeters(calor.Value),
                        StrokeColor = Colors.Transparent,
                        FillColor = Color.FromArgb(calor.Key),
                    };

                    DatosCompartidos.MapaUbicaiconMedico?.MapElements.Add(circle);
                }

                DatosCompartidos.MapaUbicaiconMedico?.Pins.Add(place);
                DatosCompartidos.MapaUbicaiconMedico?.Pins.Add(place2);
                DatosCompartidos.MapaUbicaiconMedico?.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Medico.LATITUD, Medico.LONGITUD), Distance.FromMeters(100)));
            }
        }

        private string? ObtenerColorMedico(string? cOLOR)
        {
            switch (cOLOR)
            {
                case "Rojo":
                    return Colors.Red.ToHex();
                case "Azul":
                    return Colors.Blue.ToHex();
                case "Amarillo":
                    return Colors.Yellow.ToHex();
                case "Verde":
                    return Colors.Green.ToHex();
                default:
                    return Colors.Black.ToHex();
            }
        }

        public async void CompartirUbicacionMedico()
        {
            try
            {
                if (Medico is not null)
                {
                    string url = $"https://www.google.com/maps?q={Medico.LATITUD},{Medico.LONGITUD}";

                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Text = $"Ubicación del Dr(a). {Medico.NOMBRE_COMERCIAL}:\n\n{url}",
                        Title = "Compartir ubicación"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo ubicación: {ex.Message}");
            }
        }
    }
}
