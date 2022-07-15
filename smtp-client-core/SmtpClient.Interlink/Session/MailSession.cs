//
//  MailSession.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
namespace SmtpClient.Interlink
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