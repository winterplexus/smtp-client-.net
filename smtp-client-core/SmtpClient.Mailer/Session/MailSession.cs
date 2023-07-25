//
//  MailSession.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2023
//
namespace SmtpClient.Mailer
{
    public class MailSession
    {
        public MailSession()
        {
            Parameters = new MailParameters();
            Message = new MailMessage();
        }

        public MailParameters Parameters { get; set; }
        public MailMessage Message { get; set; }
    }
}