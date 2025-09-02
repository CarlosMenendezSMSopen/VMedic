using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMedic.MVVM.Models
{
    public class CustomPin : Pin
    {
        //nuevo atributo de PIN para personalizar el icono
        public static readonly BindableProperty ImageSourceProperty
            = BindableProperty.Create(nameof(Icon), typeof(ImageSource), typeof(CustomPin));

        //nuevo atributo de PIN para posicionar el icono en cualquier lado, en base al Location como eje
        public static readonly BindableProperty AnchorProperty
            = BindableProperty.Create(nameof(Anchor), typeof(Point), typeof(CustomPin));

        //nuevo atributo en PIN para conocer el nombre del recurso del icono.
        public static readonly BindableProperty ResourceNameProperty
            = BindableProperty.Create(nameof(ResourceName), typeof(string), typeof(CustomPin));

        public ImageSource? Icon
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public Point Anchor
        {
            get => (Point)GetValue(AnchorProperty);
            set => SetValue(AnchorProperty, value);
        }

        public string ResourceName
        {
            get => (string)GetValue(ResourceNameProperty);
            set => SetValue(ResourceNameProperty, value);
        }

        public static readonly BindableProperty? title;
    }
}
