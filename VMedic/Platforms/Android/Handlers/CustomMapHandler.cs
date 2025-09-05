using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Firebase;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMedic.MVVM.Models.CustomMapObjects;
using VMedic.Platforms.Android.Handlers;
using Circle = Microsoft.Maui.Controls.Maps.Circle;
using IMap = Microsoft.Maui.Maps.IMap;

namespace VMedic;
public class CustomMapHandler : MapHandler
{
    public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper =
new PropertyMapper<IMap, IMapHandler>(Mapper)
{
    [nameof(IMap.Pins)] = MapPins,
};

    public CustomMapHandler() : base(CustomMapper, CommandMapper)
    {
    }

    public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(
        mapper ?? CustomMapper, commandMapper ?? CommandMapper)
    {
    }

    public List<Marker> Markers { get; } = new();
    public List<Element> Elements { get; } = new();

    private static readonly Dictionary<string, Bitmap> _bitmapCache = new();
    public int _previousPosition { get; set; } = -1;
    public static Handler handlerAnimate { get; set; } = new Handler(Looper.MainLooper);
    private static long startTime { get; set; } = SystemClock.UptimeMillis();

    protected override void ConnectHandler(MapView platformView)
    {
        base.ConnectHandler(platformView);
        var mapReady = new MapCallbackHandler(this);
        PlatformView.GetMapAsync(mapReady);
    }

    private static new void MapPins(IMapHandler handler, IMap map)
    {
        if (handler is CustomMapHandler mapHandler)
        {
            foreach (var marker in mapHandler.Markers)
            {
                marker.Remove();
                floatingMarkers.Remove(marker);
            }

            isFloatingAnimationRunning = false;


            mapHandler.AddPins(map.Pins);
        }
    }

    private void AddPins(IEnumerable<IMapPin> mapPins)
    {
        if (Map is null || MauiContext is null)
        {
            return;
        }

        foreach (var pin in mapPins)
        {
            var pinHandler = pin.ToHandler(MauiContext);
            if (pinHandler is IMapPinHandler mapPinHandler)
            {
                var markerOption = mapPinHandler.PlatformView;
                if (pin is CustomPin cp)
                {
                    cp.Icon.LoadImage(MauiContext, result =>
                    {
                        if (result?.Value is BitmapDrawable bitmapDrawable)
                        {
                            markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmapDrawable.Bitmap));
                        }

                        markerOption.Anchor((float)cp.Anchor.X, (float)cp.Anchor.Y);

                        AddMarker(Map, pin, Markers, markerOption);
                    });
                }
                else
                {
                    AddMarker(Map, pin, Markers, markerOption);
                }
            }
        }
    }

    private static void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption)
    {
        var marker = map.AddMarker(markerOption);
        pin.MarkerId = marker.Id;
        if (((CustomPin)pin).ClassId == "1")
            AnimateFloating(marker);

        markers.Add(marker);
    }

    private static readonly List<Marker> floatingMarkers = new();
    private static bool isFloatingAnimationRunning = false;
    private static long animationLastTime = SystemClock.UptimeMillis();
    private static readonly long animationInterval = 50;
    private static readonly float animationAmplitude = 0.1f;

    private static void AnimateFloating(Marker marker)
    {
        // Solo lo añadimos si no está ya
        if (!floatingMarkers.Contains(marker))
        {
            floatingMarkers.Add(marker);
        }

        Console.WriteLine(floatingMarkers.Count);

        // Si la animación ya está corriendo, no hacemos nada más
        if (isFloatingAnimationRunning)
            return;

        isFloatingAnimationRunning = true;

        void Run()
        {
            // Solo actualizamos si ha pasado el intervalo deseado
            long currentTime = SystemClock.UptimeMillis();
            long elapsedTime = currentTime - animationLastTime;

            if (elapsedTime < animationInterval)
            {
                handlerAnimate.PostDelayed(Run, animationInterval - elapsedTime);
                return;
            }

            animationLastTime = currentTime;

            // Animación flotante
            long elapsed = currentTime - startTime;
            float angle = (elapsed % 2000) / 2000f * 2 * (float)Math.PI;
            float offset = (float)Math.Sin(angle) * animationAmplitude;

            // Actualizamos solo los marcadores visibles para reducir la carga
            foreach (var floatingMarker in floatingMarkers.ToList()) // copia por seguridad
            {
                try
                {
                    floatingMarker.SetAnchor(0.5f, 1f + offset);
                }
                catch (Java.Lang.Exception e)
                {
                    Console.WriteLine("Error animando marcador flotante: " + e.Message);
                    floatingMarkers.Remove(floatingMarker);
                }
            }

            // Continuamos la animación solo si hay marcadores flotantes
            if (floatingMarkers.Count > 0)
            {
                handlerAnimate.PostDelayed(Run, animationInterval);
            }
            else
            {
                isFloatingAnimationRunning = false; // Detenemos la animación si no hay marcadores flotantes
            }
        }

        handlerAnimate.Post(Run);
    }
}