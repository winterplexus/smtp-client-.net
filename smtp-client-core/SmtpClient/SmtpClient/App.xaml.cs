﻿//
//  App.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System.Windows;

namespace SmtpClient
{
    public partial class App : Application
    {
        void AppicationStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
            }
        }
    }
}