//
//  MainWindow.xaml.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using SmtpClient.Interlink;

[assembly: CLSCompliant(true)]
namespace SmtpClient
{
    public partial class MainWindow : Window
    {
        private MailData mailData = new();

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
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

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            var mailData = GetMailData();
            if (!ValidateData(mailData))
            {
                return;
            }

            try
            {
                MailClient.SendSmtpMail(mailData);

                DisplayInformationMessage($"Email sent to {mailData.To}.");
            }
            catch (MailClientException mce)
            {
                MessageText.Text = $"exception-> {mce.Message}. {mce.InnerException}";
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            ServerNameText.Text = string.Empty;
            PortNumberText.Text = "25";
            UserNameText.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            UseAuthenticationCheckBox.IsChecked = false;
            UseTlsCheckBox.IsChecked = false;
            FromAddressText.Text = string.Empty;
            FromDisplayText.Text = string.Empty;
            ToAddressText.Text = string.Empty;
            ToDisplayText.Text = string.Empty;
            CcAddressText.Text = string.Empty;
            BccAddressText.Text = string.Empty;
            SubjectText.Text = string.Empty;
            BodyText.Text = string.Empty;
            MessageText.Text = string.Empty;
            MessageText.Foreground = Brushes.MediumBlue;
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private MailData GetMailData()
        {
            mailData = new()
            {
                ServerName = ServerNameText.Text,
                PortNumber = PortNumber(),
                UserName = UserNameText.Text,
                Password = PasswordBox.Password,
                UseAuthentication = UseAuthenticationCheckBox.IsChecked == true,
                UseEnableTls = UseTlsCheckBox.IsChecked == true,
                From = FromAddressText.Text,
                FromDisplayName = FromDisplayText.Text,
                To = ToAddressText.Text,
                ToDisplayName = ToDisplayText.Text,
                Cc = CcAddressText.Text,
                Bcc = BccAddressText.Text,
                Subject = SubjectText.Text,
                Body = BodyText.Text,
            };
            return mailData;
        }

        private bool ValidateData(MailData mailData)
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
            if (string.IsNullOrEmpty(mailData.Subject))
            {
                DisplayErrorMessage("Subject is missing or empty.");
                return false;
            }
            return true;
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