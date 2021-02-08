using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWindowsSpotlight.lib;
using Photos_Windows_spotlight.Infrastructure.Commands;
using Photos_Windows_spotlight.ViewModels.Base;

namespace PhotoFromScreensaver.ViewModels
{
    class MyWindowsViewModel : ViewModel
    {
        private readonly IImagesService _imagesService;
        // путь к файлу где в Виндовс находятся картинки для заставки
        private string _pathToPicturesLocal =
            @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private string _Title = "Фото с заставки Windows. версия 0.6";

        public MyWindowsViewModel(IImagesService imagesService)
        {
            _imagesService = imagesService;
            
        }

        #region Command SearchImagesInFolder - поиск картинок папке для заставок экрана

        /// <summary>Поиск картинок в системной папке для заставки виндовс</summary>
        private ICommand _searchImagesInFolderCommand;

        public ICommand SearchImagesInFolder => _searchImagesInFolderCommand
            ??= new LambdaCommand(OnSearchImagesInFolderExecuted, CanSearchImagesInFolderExecute);

        private bool CanSearchImagesInFolderExecute(object p) => true;

        private void OnSearchImagesInFolderExecuted(object p)
        {
            var newImagesList = _imagesService.SearchImagesInFolder(_pathToPicturesLocal);
            if (newImagesList.Count > 0)
            {
                OutputForWin = "Найдены изображения:";
                foreach (var image in newImagesList)
                {
                    OutputForWin = String.Concat(image.Name, ", ", image.DateOfCreation, ", ", image.Resolution);
                }
            }
            else 
                OutputForWin = "Изображений в системной папке не найдено";
        }

        #endregion

        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }

        private string _OutputForWin = "Старт\n";

        public string OutputForWin
        {
            get => _OutputForWin;
            set => Set(ref _OutputForWin, String.Concat(_OutputForWin, "\n", value));
        }

  
    }
}
