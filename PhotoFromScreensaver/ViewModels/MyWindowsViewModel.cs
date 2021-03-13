using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private List<PHashAndDataImage> _oldImagesPHash;
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
                var newImagesSelected = new List<PHashAndDataImage>();
                // получить коллекцию перцептивного хеша полученных изображений

                // получить коллекцию перцептивного хеша, имеющихся в папке


                _oldImagesPHash = new List<PHashAndDataImage>();
                var pathOldImages = new DirectoryInfo(_pathFolderMyImages).GetFiles().ToList();
                //oldImagesPHash = _imagesService.GetPerceptualHashOfImagesList(pathOldImages);

                OutputForWin = "Производится анализ имеющихся изображений в выбранной папке";
                try
                {
                    _pHashCancellation = new CancellationTokenSource();
                    var cancellation = _pHashCancellation.Token;
                    await Task.Run(() =>
                    {
                        var timer = Stopwatch.StartNew();
                        if (_imagesService != null)
                            _oldImagesPHash =
                                _imagesService.GetPerceptualHashOfImagesList(pathOldImages, cancellation);
                        timer.Stop();
                        OutputForWin =
                            $"Изображения проанализированы, потраченное время: {timer.Elapsed.TotalSeconds}," +
                            $" количество изображений {_oldImagesPHash.Count}";
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



                // сохранять коллекцию хеша в файл ассоциируя их с именем изображения и записываем дату последнего изменения

                // сравнить между собой

                // подготовить список изображений, которые новые без совпадений
            }

            catch (Exception e)
            {
                OutputForWin = $"Произошла ошибка во время обработки. {e.Message}";
            }
            finally
            {
                _comparisonToken = false;
                
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
