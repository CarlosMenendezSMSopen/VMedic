using MapKit;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace VMedic;

public class CustomAnnotation : MKPointAnnotation
{
	public Guid Identifier { get; init; }
	public string? ClassID { get; init; }
    public UIImage? Image { get; init; }
	public required IMapPin Pin { get; init; }
	public Point Anchor { get; init; }
	public bool IsVisible { get; init; }
	public bool IsinFollow { get; init; }
}
