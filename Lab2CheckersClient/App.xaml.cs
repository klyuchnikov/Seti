using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace Lab2CheckersClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                File.AppendAllLines("log_" + Client.Current.Guid + ".txt", new string[] { e.Exception.Message, e.Exception.StackTrace });
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Current.Exit += delegate
                                {
                                    Client.Current.Close();
                                };

        }
    }
}
