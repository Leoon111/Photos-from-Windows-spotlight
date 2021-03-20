﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ImagesWindowsSpotlight.lib;
using ImagesWindowsSpotlight.lib.Models;
using Photos_Windows_spotlight.Infrastructure.Commands;
using Photos_Windows_spotlight.ViewModels.Base;

namespace PhotoFromScreensaver.ViewModels
{
    class MyWindowsViewModel : ViewModel, IDataErrorInfo
    {
        private readonly IImagesService _imagesService;
        // путь к файлу где в Виндовс находятся картинки для заставки
        private string _pathToPicturesLocal =
            @"Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets";
        private string _Title = "Фото с заставки Windows. версия 0.6";
        /// <summary>Коллекция полученных изображений</summary>
        private List<PHashAndDataImage> _newImagesList;
        private List<PHashAndDataImage> _oldImagesList;
        private CancellationTokenSource _pHashCancellation;
        private string _pathFolderMyImages = @"D:\OneDrive\Новые фотографии\1\test\1\";
        /// <summary>Токен, которой сообщает о процессе вычисления поиска хеша и сравнения с другими картинками</summary> 
        private bool _comparisonToken = false;


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
        private bool CanSearchImagesInFolderExecute(object p)
        {
            return !_comparisonToken;
        }

        /// <summary>Логика выполнения - Поиск картинок</summary>
        private void OnSearchImagesInFolderExecuted(object p)
        {
            _newImagesList = _imagesService.SearchImagesInFolder(_pathToPicturesLocal);
            if (_newImagesList.Count > 0)
            {
                OutputForWin = "Найдены изображения:";
                foreach (var image in _newImagesList)
                {
                    OutputForWin = String.Concat(image.Name, ", ", image.DateOfCreation, ", ", image.Resolution.Width, "X", image.Resolution.Height);
                }
            }
            else
                OutputForWin = "Изображений в системной папке не найдено";
        }

        #endregion  

        #region Command ComparisonOfNewWithCurrent - сравнение найденных с имеющимися

        /// <summary>Сравнение найденных картинок с имеющимися</summary>
        private ICommand _ComparisonOfNewWithCurrentCommand;

        /// <summary>Сравнение найденных картинок с имеющимися</summary>
        public ICommand ComparisonOfNewWithCurrentCommand => _ComparisonOfNewWithCurrentCommand
            ??= new LambdaCommand(OnComparisonOfNewWithCurrentExecuted, CanComparisonOfNewWithCurrentExecute);

        /// <summary>Проверка возможности выполнения - Сравнение картинок</summary>
        private bool CanComparisonOfNewWithCurrentExecute(object p)
        {
            bool canCompaison = _pathFolderMyImages is not null && _newImagesList is not null && _comparisonToken == false;
            return canCompaison;
        }

        /// <summary>Логика выполнения - Сравнение картинок</summary>
        private async void OnComparisonOfNewWithCurrentExecuted(object p)
        {
            // todo создать методы из длинного кода
            try
            {
                _comparisonToken = true;
                _pHashCancellation = new CancellationTokenSource();
                var cancellation = _pHashCancellation.Token;

                //var newImagesSelected = new List<PHashAndDataImage>();

                // удаляем изображения разрешением меньше 1000 в длину, т.к. они нам не нужны
                for (int i = _newImagesList.Count - 1; i > -1; i--)
                {
                    if (_newImagesList[i].Resolution.Width < 1000)
                        _newImagesList.Remove(_newImagesList[i]);
                }

                // получить коллекцию перцептивного хеша полученных изображений
                await Task.Run(() =>
                   {
                       Parallel.ForEach(_newImagesList, imageData =>
                        {
                            _imagesService.GetPerceptualHashOfImageData(imageData);

                        });
                   }
                );


                // получить коллекцию перцептивного хеша, имеющихся в папке
                _oldImagesList = new List<PHashAndDataImage>();
                var pathOldImages = new DirectoryInfo(_pathFolderMyImages).GetFiles().ToList();
                //oldImagesPHash = _imagesService.GetPerceptualHashOfImagesList(pathOldImages);

                OutputForWin = "\nПроизводится анализ имеющихся изображений в выбранной папке";
                try
                {

                    await Task.Run(() =>
                    {
                        var timer = Stopwatch.StartNew();
                        if (_imagesService != null)
                            _oldImagesList =
                                _imagesService.GetPerceptualHashOfImagesList(pathOldImages, cancellation);
                        timer.Stop();
                        OutputForWin =
                            $"\nИзображения проанализированы, потраченное время: {timer.Elapsed.TotalSeconds}," +
                            $"Количество изображений {_oldImagesList.Count}";
                    });
                }
                catch (OperationCanceledException)
                {
                    OutputForWin = $"Операция отменена пользователем";
                }
                catch (AggregateException ex)
                {
                    OutputForWin =
                        ex.InnerException is OperationCanceledException
                            ? $"Операция принудительно отмена пользователем {ex.InnerException.Message}"
                            : $"Обработка выдала несколько исключений {ex.InnerExceptions}";
                }
                catch (Exception e)
                {
                    OutputForWin = $"Произошла ошибка нахождения хеша изображения. {e.Message}";
                }

                // todo сохранять коллекцию хеша в файл ассоциируя их с именем изображения и записываем дату последнего изменения

                // сравнить между собой
                await Task.Run(() =>
                {
                    var timer = Stopwatch.StartNew();
                    bool identityToken;
                    bool[] imagesNemberArray = new bool[_newImagesList.Count];

                    foreach (var newImage in _newImagesList)
                    {
                        // изначально изображение идентичное
                        identityToken = true;
                        foreach (var oldImage in _oldImagesList)
                        {

                            if ((newImage.Resolution.Width / newImage.Resolution.Height) == (oldImage.Resolution.Width / oldImage.Resolution.Height))
                            {
                                // Счетчик совпадений байтов в массиве перцептивного хеша.
                                int byteMatchCounter = 0;
                                for (int i = 0; i < newImage.PerceptualHash.Length; i++)
                                {
                                    if (newImage.PerceptualHash[i] == oldImage.PerceptualHash[i])
                                        byteMatchCounter++;
                                }

                                // при полном совпадении - это одна и та же картинка.
                                if (byteMatchCounter == 64)
                                {
                                    OutputForWin =
                                        $"\nТочное совпадение картинки {newImage.Name} \nС картинкой {oldImage.Name}";
                                    identityToken = false;
                                }

                                if (byteMatchCounter == 63)
                                    OutputForWin =
                                        $"\nСовпадение 63 из 64 картинки {newImage.Name} \nС картинкой {oldImage.Name}";
                                if (byteMatchCounter == 62)
                                    OutputForWin =
                                        $"\nСовпадение 62 из 64 картинки {newImage.Name} \nС картинкой {oldImage.Name}";
                            }
                        }

                        if (identityToken)
                            imagesNemberArray[_newImagesList.IndexOf(newImage)] = true;
                        else
                            imagesNemberArray[_newImagesList.IndexOf(newImage)] = false;
                    }
                    timer.Stop();
                    //var c = imagesNemberArray.Count(x => x == true);
                    OutputForWin =
                        $"\nИзображения проанализированы, потраченное время: {timer.Elapsed.TotalSeconds},\n" +
                        $"Количество новых изображений {imagesNemberArray.Count(x => x == true)}";
                });




                // подготовить список изображений, которые новые без совпадений
            }

            catch (Exception e)
            {
                OutputForWin = $"Произошла ошибка во время обработки. {e.Message}";
            }
            finally
            {
                _comparisonToken = false;
                // Forcing the CommandManager to raise the RequerySuggested event
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region Command CancelingOperation - Отмена операции

        /// <summary>Отмена операций</summary>
        private ICommand _CancelingOperationCommand;

        /// <summary>Отмена операций</summary>
        public ICommand CancelingOperationCommand => _CancelingOperationCommand
            ??= new LambdaCommand(OnCancelingOperationExecuted, CanCancelingOperationExecute);

        /// <summary>Проверка возможности выполнения - Отмена операций</summary>
        private bool CanCancelingOperationExecute(object p)
        {
            return _comparisonToken;
        }

        /// <summary>Логика выполнения - Отмена операций</summary>
        private void OnCancelingOperationExecuted(object p)
        {
            _pHashCancellation?.Cancel();
        }

        #endregion

        #region Command ExitingProgram - Выход из приложения

        /// <summary>Выход из приложения</summary>
        private ICommand _ExitingProgramCommand;

        /// <summary>Выход из приложения</summary>
        public ICommand ExitingProgramCommand => _ExitingProgramCommand
            ??= new LambdaCommand(OnExitingProgramExecuted, CanExitingProgramExecute);

        /// <summary>Проверка возможности выполнения - Выход из приложения</summary>
        private bool CanExitingProgramExecute(object p)
        {
            return !_comparisonToken;
        }

        /// <summary>Логика выполнения - Выход из приложения</summary>
        private void OnExitingProgramExecuted(object p)
        {
            App.Current.MainWindow.Close();
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

        /// <summary>
        /// Выводит в текст бокс текст событий и не только (временное решение)
        /// </summary>
        public string OutputForWin
        {
            get => _OutputForWin;
            set
            {
                if (Dispatcher.CurrentDispatcher.CheckAccess())
                    Set(ref _OutputForWin, String.Concat(_OutputForWin, "\n", value));
                else
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                        Set(ref _OutputForWin, String.Concat(_OutputForWin, "\n", value)));
            }
        }

        public string PathFolderMyImages
        {
            get => _pathFolderMyImages;
            set
            {
                //if (!Directory.Exists(_pathFolderMyImages))
                //    throw new ArgumentException("Данной папки не существует", nameof(value));

                if (Directory.Exists(_pathFolderMyImages))
                    Set(ref _pathFolderMyImages, value);
            }
        }

        #endregion

        string IDataErrorInfo.Error { get; } = null;

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    default: return null;

                    case nameof(PathFolderMyImages):
                        if (!Directory.Exists(_pathFolderMyImages))
                            return "Данной папки не существует";

                        return null;
                }
            }
        }
    }
}
