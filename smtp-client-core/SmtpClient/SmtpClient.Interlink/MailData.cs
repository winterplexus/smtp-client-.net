//
//  MailData.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System.Collections.Generic;

namespace SmtpClient.Interlink
{
    public class MailData
    {
        public MailData()
        {
            AttachmentFiles = new List<string>();
        }

        public string ServerName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseAuthentication { get; set; }
        public bool UseEnableTls { get; set; }
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