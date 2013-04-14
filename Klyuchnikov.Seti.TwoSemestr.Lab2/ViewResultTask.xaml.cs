using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Klyuchnikov.Seti.TwoSemestr.CommonLibrary;

namespace Klyuchnikov.Seti.TwoSemestr.Lab2
{
    /// <summary>
    /// Логика взаимодействия для ViewResultTask.xaml
    /// </summary>
    public partial class ViewResultTask : Window
    {
        public ViewResultTask(Document document)
        {
            InitializeComponent();
            DataContext = document;
        }
    }
}
