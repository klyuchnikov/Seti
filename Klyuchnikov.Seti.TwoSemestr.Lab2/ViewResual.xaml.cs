﻿using System;
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
    /// Логика взаимодействия для ViewResual.xaml
    /// </summary>
    public partial class ViewResual : Window
    {
        public ViewResual()
        {
            InitializeComponent();
        }

        private void listBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var form = new ViewResultTask(listBox2.SelectedItem as Document);
            form.ShowDialog();
        }
    }
}
