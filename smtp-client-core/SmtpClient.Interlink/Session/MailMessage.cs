//
//  MailMessage.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System.Collections.Generic;

namespace SmtpClient.Interlink
{
    public class MailMessage
    {
        public MailMessage()
        {
            AttachmentFiles = new List<string>();
        }

        public string From { get; set; }
        public string FromDisplayName { get; set; }
        public string To { get; set; }
        public string ToDisplayName { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ICollection<string> AttachmentFiles { get; }
    }
}