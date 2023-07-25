//
//  MainWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2023
//
using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using SmtpClient.Mailer;

namespace SmtpClient
{
    public partial class MainWindow : Window
    {
        private readonly MailSession mailSession;

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            mailSession = new();

            InitializeFields();
        }

        private void AttachFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileNames.Length > 0)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        mailSession.Message.AttachmentFiles.Add(fileName);
                    }
                }
                else if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    mailSession.Message.AttachmentFiles.Add(openFileDialog.FileName);
                }
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsValidateInput())
            {
                return;
            }

            GetMailSessionData();

            try
            {
                MailClient.SendSmtpMail(mailSession);

                DisplayInformationMessage($"Email sent to {mailSession.Message.To}.");
            }
            catch (MailClientException mce)
            {
                DisplayExceptionMessage(mce.Message, mce.InnerException.Message);
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            InitializeFields();
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

        private void InitializeFields()
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

        private void GetMailSessionData()
        {
            mailSession.Parameters.ServerName          = ServerNameText.Text;
            mailSession.Parameters.PortNumber          = PortNumber();
            mailSession.Parameters.UserName            = UserNameText.Text;
            mailSession.Parameters.Password            = PasswordBox.Password;
            mailSession.Parameters.UseAuthentication   = UseAuthenticationCheckBox.IsChecked == true;
            mailSession.Parameters.EnableTls           = UseTlsCheckBox.IsChecked == true;
            mailSession.Parameters.Timeout             = Timeout();
            mailSession.Parameters.ProtocolLogFilePath = ProtocolLogFilePath();
            mailSession.Message.From                   = FromAddressText.Text;
            mailSession.Message.FromDisplayName        = FromDisplayText.Text;
            mailSession.Message.To                     = ToAddressText.Text;
            mailSession.Message.ToDisplayName          = ToDisplayText.Text;
            mailSession.Message.Cc                     = CcAddressText.Text;
            mailSession.Message.Bcc                    = BccAddressText.Text;
            mailSession.Message.Subject                = SubjectText.Text;
            mailSession.Message.Body                   = BodyText.Text;
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
            var configurationValue = ConfigurationManager.AppSettings["MailTimeout"];
            if (string.IsNullOrEmpty(configurationValue))
            {
                return 5000;
            }
            else
            {
                return int.TryParse(configurationValue, out var number) ? number : 5000;
            }
        }

        private static string ProtocolLogFilePath()
        {
            var configurationValue = ConfigurationManager.AppSettings["MailProtocolLogFilePath"];
            if (string.IsNullOrEmpty(configurationValue))
            {
                return null;
            }
            else
            {
                var dateTimeSuffix = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", DateTime.Now);
                return string.Format(CultureInfo.InvariantCulture, $"{configurationValue}\\SmtpClient.{dateTimeSuffix}.log");
            }
        }

        private static void DisplayInformationMessage(string message)
        {
            _ = MessageBox.Show(message, "Information");
        }

        private static void DisplayErrorMessage(string message)
        {
            _ = MessageBox.Show(message, "Error");
        }

        private static void DisplayExceptionMessage(string exceptionMessage, string innerExceptionMessage)
        {
            StringBuilder message = new();

            _ = message.Append(CultureInfo.InvariantCulture, $"{exceptionMessage}").Append(Environment.NewLine);
            _ = message.Append(CultureInfo.InvariantCulture, $"{innerExceptionMessage}");

            _ = MessageBox.Show(message.ToString(), "Exception");
        }
    }
}