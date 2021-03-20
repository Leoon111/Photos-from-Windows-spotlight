using System;
using System.Windows;
using ImagesWindowsSpotlight.lib;
using ImagesWindowsSpotlight.lib.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotoFromScreensaver.ViewModels;

namespace PhotoFromScreensaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _Hosting;

        public static IHost Hosting => _Hosting
            ??= Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                .ConfigureServices(ConfigureServices)
                .Build();

        public static IServiceProvider Services => Hosting.Services;

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MyWindowsViewModel>();
            services.AddTransient<IImagesService, ImageService>();
        }
    }
}
