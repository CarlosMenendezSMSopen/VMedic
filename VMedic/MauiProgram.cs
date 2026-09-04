using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Mopups.PreBaked.PopupPages.Login;
using Syncfusion.Maui.Core.Hosting;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.MVVM.Views;
using VMedic.MVVM.Views.Planificacion;
using VMedic.MVVM.Views.Visitas;
using VMedic.Utilidades;

namespace VMedic
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureMopups()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fontello.ttf", "Icon");
                    fonts.AddFont("fontello2.ttf", "SecondIcon");
                    fonts.AddFont("fontello3.ttf", "ThirdIcon");
                })
                .ConfigureSyncfusionCore()
                .ConfigureEssentials()
                .UseMauiMaps();

            builder.Services.AddScoped(typeof(BaseRepository<>));

            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.ConfigureMauiHandlers(handlers =>
            {
#if ANDROID || IOS
                handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, CustomMapHandler>();
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
