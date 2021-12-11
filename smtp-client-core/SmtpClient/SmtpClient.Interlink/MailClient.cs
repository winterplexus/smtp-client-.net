//
//  MailClient.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System;
using MimeKit;
using MailKit.Security;

[assembly: CLSCompliant(true)]
namespace SmtpClient.Interlink
{
    public static class MailClient
    {
        private const int timeout = 5000;

        public static void SendSmtpMail(MailData mailData)
        {
            if (mailData == null)
            {
                throw new ArgumentNullException(nameof(mailData));
            }

            var message = CreateMimeMessage(mailData);

            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();

                client.Timeout = timeout;

                if (mailData.UseEnableTls)
                {
                    client.Connect(mailData.ServerName, mailData.PortNumber, SecureSocketOptions.StartTls);
                }
                else
                {
                    client.Connect(mailData.ServerName, mailData.PortNumber);
                }
                if (mailData.UseAuthentication)
                {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(mailData.UserName, mailData.Password);
                }

                client.Send(message);
                client.Disconnect(true);
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

        private static MimeMessage CreateMimeMessage(MailData mailData)
        {
            var message = new MimeMessage();

            if (!string.IsNullOrEmpty(mailData.FromDisplayName))
            {
                message.From.Add(MailboxAddress.Parse(mailData.From));
            }
            else
            {
                message.From.Add(new MailboxAddress(mailData.FromDisplayName, mailData.From));
            }
            if (!string.IsNullOrEmpty(mailData.ToDisplayName))
            {
                message.To.Add(MailboxAddress.Parse(mailData.To));
            }
            else
            {
                message.To.Add(new MailboxAddress(mailData.ToDisplayName, mailData.To));
            }
            if (!string.IsNullOrEmpty(mailData.Cc))
            {
                message.Cc.Add(MailboxAddress.Parse(mailData.Cc));
            }
            if (!string.IsNullOrEmpty(mailData.Bcc))
            {
                message.Cc.Add(MailboxAddress.Parse(mailData.Bcc));
            }

            message.Subject = mailData.Subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = mailData.Body
            };

            if (mailData.AttachmentFiles.Count > 0)
            {
                foreach (var attachmentFile in mailData.AttachmentFiles)
                {
                    bodyBuilder.Attachments.Add(attachmentFile);
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }
    }
}