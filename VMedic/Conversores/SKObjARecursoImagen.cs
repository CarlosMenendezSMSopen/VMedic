using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.Utilidades;

namespace VMedic.Conversores
{
    public static class SKObjARecursoImagen
    {
        //funcion que retorna un ImageSource desde el SKBitmap que se le mande como parametro
        public static ImageSource? GridImageSource(SKBitmap? bitmap)
        {
            try
            {
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
                using (var stream = new MemoryStream())
                {
                    if (stream is not null)
                    {
                        data.SaveTo(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        return ImageSource.FromStream(() =>
                        {
                            var newStream = new MemoryStream(stream.ToArray());
                            return newStream;
                        });
                    }
                    else
                    {
                        Debug.WriteLine("no se pudo cargar la imagen");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                PressedPreferences.EndPressed();
                ExceptionMessageMaker.Make("Error Pintar Pin", ex.ToString(), ex.Message, App.Current?.Windows[0].Page);
                return null;
            }
        }
    }
}
