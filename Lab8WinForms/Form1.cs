using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab8WinForms
{
    // Класс основной формы
    public partial class Form1 : Form, IView
    {
        // Конструктор. Тут очень важно инициализировать Presenter, иначе ничего работать не будет
        public Form1()
        {
            InitializeComponent();
            Presenter programPresenter = new Presenter(this);
        }
        public event EventHandler<EventArgs> SyncronizeDirectoriesEvent;
        public event EventHandler<EventArgs> Inversion;
        string IView.FirstDirectory() { return firstDirectoryTextBox.Text; }
        string IView.SecondDirectory() { return secondDirectoryTextBox.Text; }

        // Попытка синхронизации - сюда приходит данные, которые были получены в модели и переданы презентером
        void IView.TrySynchronize(List<string> message)
        {
            ResultList.Items.Clear(); // Обновление списка

            List<string> outputList = message;

            foreach (string output in outputList)
            {
                ResultList.Items.Add(output);
            }
            
        }

        // Нажатие на кнопку "Синхронизировать" и что после этого произойдет.
        private void buttonStartSynchronize_Click(object sender, EventArgs inputEvent)
        {
            SyncronizeDirectoriesEvent(sender,inputEvent);
        }

        // Нажатие на кнопку "Синхронизировать" и что после этого произойдет.
        private void inversion_CheckedChanged(object sender, EventArgs inputEvent)
        {
            Inversion(sender, inputEvent);
        }
    }
}
