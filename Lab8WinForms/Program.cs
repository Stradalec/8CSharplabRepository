using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab8WinForms
{

    // Интерфейс (он же вид). Отображает данные из модели, передает команды пользователя презентеру
    interface IView 
    {
        string FirstDirectory();
        string SecondDirectory();

        void TrySynchronize(List<string> message);

        event EventHandler<EventArgs> SyncronizeDirectoriesEvent;

        event EventHandler<EventArgs> Inversion;
    }
    

    // Модель. Основная часть работы программы происходит здесь
    class Model 
    {
        private bool isInverted = false;

        // Для кнопки "инверсия", делающий главной директорию 2, и для возвращения к обычному виду работы программы при повторном нажатии
        public void Invert ()
        {
            isInverted = !isInverted;
        }

        // Основной код по синхронизации директорий
        public List<string> SynchronizeDirectories(string firstDirectory, string secondDirectory)
        {
            DirectoryInfo mainDirectoryInfo = new DirectoryInfo(firstDirectory);
            DirectoryInfo targetDirectoryInfo  = new DirectoryInfo(secondDirectory);
            List<string> resultofSynchronize;

            // Проверка наличия инверсии
            if (!isInverted)
            {
                resultofSynchronize = InnerSynchronizeDirectories(mainDirectoryInfo, targetDirectoryInfo);
                return resultofSynchronize;
            }
            else
            {
                resultofSynchronize = InnerSynchronizeDirectories(targetDirectoryInfo , mainDirectoryInfo);
                return resultofSynchronize;
            }
            
        }

        private List<string> InnerSynchronizeDirectories(DirectoryInfo mainDirectoryInfo, DirectoryInfo targetDirectoryInfo )
        {
            List<string> innerResultOfSynchronize = new List<string>();
            bool isNeedToSynchronize = false;

            // Поиск и (при необходимости) замена файлов из "целевой" директории файлами из "главной" директории
            foreach (FileInfo directoryFile in mainDirectoryInfo.GetFiles())
            {
                FileInfo targetFileInOtherDirectory = new FileInfo(Path.Combine(targetDirectoryInfo .FullName, directoryFile.Name));  // Combine есть объединение строк в одну

                if (!targetFileInOtherDirectory.Exists || targetFileInOtherDirectory.LastWriteTime < directoryFile.LastWriteTime) // Не существует или записан ранее - заменяем
                {
                    File.Copy(directoryFile.FullName, targetFileInOtherDirectory.FullName, true);
                    innerResultOfSynchronize.Add($"Файл {directoryFile.Name} изменен");
                    isNeedToSynchronize = true;
                }
            }

            // Если в "главной" директории файла нет, удаляем его в "целевой"
            foreach (FileInfo directoryFile in targetDirectoryInfo.GetFiles())
            {
                FileInfo fileInMainDirectory = new FileInfo(Path.Combine(mainDirectoryInfo.FullName, directoryFile.Name));

                if (!fileInMainDirectory.Exists)
                {
                    directoryFile.Delete();
                    innerResultOfSynchronize.Add($"Файл {directoryFile.Name} удален");
                    isNeedToSynchronize = true;
                }
            }

            // Если не было попыток синхронизировать, сообщаем, что это и не требовалось.
            if (!isNeedToSynchronize) 
            {
                innerResultOfSynchronize.Add("Не нужно синхронизировать");
            }

            return innerResultOfSynchronize;
        }
    }

    // Презентер. Извлекает данные из модели, передает в вид. Обрабатывает события
    class Presenter 
    {
        private IView mainView;
        private Model model;

        public Presenter(IView inputView) 
        {
            mainView = inputView;
            model = new Model();

            mainView.SyncronizeDirectoriesEvent += new EventHandler<EventArgs>(Synchronize);
            mainView.Inversion += new EventHandler<EventArgs> (Inversion);
        }

        // Обработка события "Инверсия"
        private void Inversion(object sender, EventArgs inputEvent)
        {
            model.Invert();
        }

        // Обработка события "Синхронизация"
        private void Synchronize(object sender, EventArgs inputEvent) 
        {
            List<string> resultOfSynchronization = model.SynchronizeDirectories(mainView.FirstDirectory(),mainView.SecondDirectory());

            mainView.TrySynchronize(resultOfSynchronization);
        }
    }
    // Тут производится запуск
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
