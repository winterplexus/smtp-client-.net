//
//  MailClient.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2023
//
using System;
using MimeKit;
using MailKit;
using MailKit.Security;

[assembly: CLSCompliant(true)]
namespace SmtpClient.Mailer
{
    public static class MailClient
    {
        public static void SendSmtpMail(MailSession mailSession)
        {
            if (mailSession == null)
            {
                throw new ArgumentNullException(nameof(mailSession));
            }

            try
            {
                if (string.IsNullOrEmpty(mailSession.Parameters.ProtocolLogFilePath))
                {
                    ProcessMailRequest(mailSession);
                }
                else
                {
                    ProcessMailRequestWithLogging(mailSession);
                }
            }
            catch (MailKit.ServiceNotConnectedException mksnce)
            {
                throw new MailClientException("unable to connect to SMTP server", mksnce);
            }
            catch (MailKit.ServiceNotAuthenticatedException mksnae)
            {
                throw new MailClientException("invalid SMTP authentication credentials", mksnae);
            }
            catch (MailKit.Net.Smtp.SmtpCommandException mknssce)
            {
                throw new MailClientException("invalid SMTP command", mknssce);
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException mknsspe)
            {
                throw new MailClientException("invalid SMTP protocol", mknsspe);
            }
            catch (Exception ex)
            {
                throw new MailClientException("unable to send message", ex);
            }
        }

        private static void ProcessMailRequest(MailSession mailSession)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.Timeout = mailSession.Parameters.Timeout;

            if (mailSession.Parameters.EnableTls)
            {
                client.Connect(mailSession.Parameters.ServerName, mailSession.Parameters.PortNumber, SecureSocketOptions.StartTls);
            }
            else
            {
                client.Connect(mailSession.Parameters.ServerName, mailSession.Parameters.PortNumber, SecureSocketOptions.None);
            }
            if (mailSession.Parameters.UseAuthentication)
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(mailSession.Parameters.UserName, mailSession.Parameters.Password);
            }

            using (var message = CreateMimeMessage(mailSession))
            {
                client.Send(message);
            }
            client.Disconnect(true);
        }

        private static void ProcessMailRequestWithLogging(MailSession mailSession)
        {
            using var logger = new ProtocolLogger(mailSession.Parameters.ProtocolLogFilePath);
            using var client = new MailKit.Net.Smtp.SmtpClient(logger);

            client.Timeout = mailSession.Parameters.Timeout;

            if (mailSession.Parameters.EnableTls)
            {
                client.Connect(mailSession.Parameters.ServerName, mailSession.Parameters.PortNumber, SecureSocketOptions.StartTls);
            }
            else
            {
                client.Connect(mailSession.Parameters.ServerName, mailSession.Parameters.PortNumber, SecureSocketOptions.None);
            }
            if (mailSession.Parameters.UseAuthentication)
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(mailSession.Parameters.UserName, mailSession.Parameters.Password);
            }

            using (var message = CreateMimeMessage(mailSession))
            {
                client.Send(message);
            }
            client.Disconnect(true);
        }

        private static MimeMessage CreateMimeMessage(MailSession mailSession)
        {
            var message = new MimeMessage();

            if (!string.IsNullOrEmpty(mailSession.Message.FromDisplayName))
            {
                message.From.Add(MailboxAddress.Parse(mailSession.Message.From));
            }
            else
            {
                message.From.Add(new MailboxAddress(mailSession.Message.FromDisplayName, mailSession.Message.From));
            }
            if (!string.IsNullOrEmpty(mailSession.Message.ToDisplayName))
            {
                message.To.Add(MailboxAddress.Parse(mailSession.Message.To));
            }
            else
            {
                message.To.Add(new MailboxAddress(mailSession.Message.ToDisplayName, mailSession.Message.To));
            }
            if (!string.IsNullOrEmpty(mailSession.Message.Cc))
            {
                message.Cc.Add(MailboxAddress.Parse(mailSession.Message.Cc));
            }
            if (!string.IsNullOrEmpty(mailSession.Message.Bcc))
            {
                message.Cc.Add(MailboxAddress.Parse(mailSession.Message.Bcc));
            }

            message.Subject = mailSession.Message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = mailSession.Message.Body
            };

            if (mailSession.Message.AttachmentFiles.Count > 0)
            {
                foreach (var attachmentFile in mailSession.Message.AttachmentFiles)
                {
                    bodyBuilder.Attachments.Add(attachmentFile);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }
    }
}