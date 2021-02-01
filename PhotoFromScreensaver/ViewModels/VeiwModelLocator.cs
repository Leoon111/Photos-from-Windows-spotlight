using Microsoft.Extensions.DependencyInjection;

namespace PhotoFromScreensaver.ViewModels
{
    class VeiwModelLocator
    {
        public MyWindowsViewModel MyWindowsViewModel => App.Services.GetRequiredService<MyWindowsViewModel>();
    }
}
