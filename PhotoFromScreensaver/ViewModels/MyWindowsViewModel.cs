using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photos_Windows_spotlight.ViewModels.Base;

namespace PhotoFromScreensaver.ViewModels
{
    class MyWindowsViewModel : ViewModel
    {
        private string _Title = "Фото с заставки Windows. версия 0.6";

        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
    }
}
