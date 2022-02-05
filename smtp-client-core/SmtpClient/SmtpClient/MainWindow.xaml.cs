//
//  MainWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using SmtpClient.Interlink;

namespace SmtpClient
{
    public partial class MainWindow : Window
    {
        private readonly MailData mailData;

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            mailData = new();
        }

        private void AttachFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length > 0)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        mailData.AttachmentFiles.Add(fileName);
                    }
                }
                else if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    mailData.AttachmentFiles.Add(openFileDialog.FileName);
                }
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsValidateInput())
            {
                return;
            }

            try
            {
                GetMailData();
                MailClient.SendSmtpMail(mailData);

                DisplayInformationMessage($"Email sent to {mailData.To}.");
            }
            catch (MailClientException mce)
            {
                MessageText.Text = $"exception-> {mce.Message}. {mce.InnerException}";
            }
        }

        private bool IsValidateInput()
        {
            if (string.IsNullOrEmpty(ServerNameText.Text))
            {
                DisplayErrorMessage("Server name is missing or empty.");
                return false;
            }
            if (string.IsNullOrEmpty(PortNumberText.Text))
            {
                DisplayErrorMessage("Port number is missing or empty.");
                return false;
            }
            if (UseAuthenticationCheckBox.IsChecked == true)
            {
                if (string.IsNullOrEmpty(UserNameText.Text))
                {
                    DisplayErrorMessage("User name is missing or empty.");
                    return false;
                }
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    DisplayErrorMessage("Password is missing or empty.");
                    return false;
                }
            }
            if (string.IsNullOrEmpty(SubjectText.Text))
            {
                DisplayErrorMessage("Subject is missing or empty.");
                return false;
            }
            return true;
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            ServerNameText.Text                 = string.Empty;
            PortNumberText.Text                 = "25";
            UserNameText.Text                   = string.Empty;
            PasswordBox.Password                = string.Empty;
            UseAuthenticationCheckBox.IsChecked = false;
            UseTlsCheckBox.IsChecked            = false;
            FromAddressText.Text                = string.Empty;
            FromDisplayText.Text                = string.Empty;
            ToAddressText.Text                  = string.Empty;
            ToDisplayText.Text                  = string.Empty;
            CcAddressText.Text                  = string.Empty;
            BccAddressText.Text                 = string.Empty;
            SubjectText.Text                    = string.Empty;
            BodyText.Text                       = string.Empty;
            MessageText.Text                    = string.Empty;
            MessageText.Foreground              = Brushes.MediumBlue;
        }

        private void GetMailData()
        {
            mailData.ServerName        = ServerNameText.Text;
            mailData.PortNumber        = PortNumber();
            mailData.UserName          = UserNameText.Text;
            mailData.Password          = PasswordBox.Password;
            mailData.UseAuthentication = UseAuthenticationCheckBox.IsChecked == true;
            mailData.EnableTls         = UseTlsCheckBox.IsChecked == true;
            mailData.Timeout           = Timeout();
            mailData.From              = FromAddressText.Text;
            mailData.FromDisplayName   = FromDisplayText.Text;
            mailData.To                = ToAddressText.Text;
            mailData.ToDisplayName     = ToDisplayText.Text;
            mailData.Cc                = CcAddressText.Text;
            mailData.Bcc               = BccAddressText.Text;
            mailData.Subject           = SubjectText.Text;
            mailData.Body              = BodyText.Text;           
        }

        private int PortNumber()
        {
            int portNumber = 25;

            try
            {
                if (!int.TryParse(PortNumberText.Text, out portNumber))
                {
                    DisplayErrorMessage("Port number is not a number.");
                }
            }
            catch (OverflowException)
            {
            }
            catch (FormatException)
            {
            }

            return portNumber;
        }

        private static int Timeout()
        {
            var appSettings = ConfigurationManager.AppSettings;
            if (appSettings.Count == 0)
            {
                return 5000;
            }
            else
            {
                var configurationValue = appSettings["MailTimeout"];
                return int.TryParse(configurationValue, out var number) ? number : 0;
            }
        }

        private void AboutButtonClick(object sender, RoutedEventArgs e)
        {
            StringBuilder message = new();

            var assembly = Assembly.GetExecutingAssembly();

            var descriptionAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (descriptionAttributes.Length > 0)
            {
                _ = message.Append(((AssemblyDescriptionAttribute)descriptionAttributes[0]).Description).Append(Environment.NewLine);
                _ = message.Append(CultureInfo.InvariantCulture, $"v{assembly.GetName().Version}").Append(Environment.NewLine);
            }

            var copyrightAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (copyrightAttributes.Length > 0)
            {
                _ = message.Append(((AssemblyCopyrightAttribute)copyrightAttributes[0]).Copyright);
            }

            _ = MessageBox.Show(message.ToString(), "About");
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplayInformationMessage(string message)
        {
            MessageText.Text = message;
            MessageText.Foreground = Brushes.MediumBlue;
        }

        private void DisplayErrorMessage(string message)
        {
            MessageText.Text = message;
            MessageText.Foreground = Brushes.OrangeRed;
        }
    }
}