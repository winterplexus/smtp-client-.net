//
//  MailParameters.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2023
//
namespace SmtpClient.Mailer
{
    public class MailParameters
    {
        public string ServerName { get; set; }
        public int PortNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseAuthentication { get; set; }
        public bool EnableTls { get; set; }
        public int Timeout { get; set; }
        public string ProtocolLogFilePath { get; set; }
    }
}