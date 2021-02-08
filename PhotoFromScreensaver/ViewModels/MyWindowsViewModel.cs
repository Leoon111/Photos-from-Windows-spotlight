﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ImagesWindowsSpotlight.lib;
using ImagesWindowsSpotlight.lib.Models;
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
        private List<ImageInfo> _newImagesList;

        public MyWindowsViewModel(IImagesService imagesService)
        {
            _imagesService = imagesService;

        }

        #region Команды

        #region Command SearchImagesInFolder - поиск картинок папке для заставок экрана

        /// <summary>Поиск картинок в системной папке для заставки виндовс</summary>
        private ICommand _SearchImagesInFolderCommand;
        /// <summary>Поиск картинок в системной папке для заставки виндовс</summary>
        public ICommand SearchImagesInFolderCommand => _SearchImagesInFolderCommand
            ??= new LambdaCommand(OnSearchImagesInFolderExecuted, CanSearchImagesInFolderExecute);
        /// <summary>Проверка возможности выполнения - Поиск картинок</summary>
        private bool CanSearchImagesInFolderExecute(object p) => true;
        /// <summary>Логика выполнения - Поиск картинок</summary>
        private void OnSearchImagesInFolderExecuted(object p)
        {
            _newImagesList = _imagesService.SearchImagesInFolder(_pathToPicturesLocal);
            if (_newImagesList.Count > 0)
            {
                OutputForWin = "Найдены изображения:";
                foreach (var image in _newImagesList)
                {
                    OutputForWin = String.Concat(image.Name, ", ", image.DateOfCreation, ", ", image.Resolution);
                }
            }
            else
                OutputForWin = "Изображений в системной папке не найдено";
        }

        #endregion  

        #endregion

        #region Свойства

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

        #endregion


    }
}