using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Syncfusion.Maui.Core.Hosting;
using VMedic.MVVM.Models.DatabaseTables;
using VMedic.MVVM.ViewModels;
using VMedic.MVVM.ViewModels.Visitas;
using VMedic.MVVM.Views;
using VMedic.MVVM.Views.Visitas;
using VMedic.Utilities;

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

            builder.Services.AddTransient<VisitasView>();
            builder.Services.AddTransient<VisitasViewModel>();
            builder.Services.AddTransient<MedicosView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
