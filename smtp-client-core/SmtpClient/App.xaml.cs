//
//  App.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2023
//
using System;
using System.Windows;

[assembly: CLSCompliant(true)]
namespace SmtpClient
{
    public partial class App : Application
    {
        void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
            }
        }
    }
}