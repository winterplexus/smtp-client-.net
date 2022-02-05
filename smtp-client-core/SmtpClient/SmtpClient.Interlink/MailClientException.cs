//
//  MailClientException.cs
//
//  Copyright (c) Wiregrass Code Technology 2019-2022
//
using System;
using System.Runtime.Serialization;

namespace SmtpClient.Interlink
{
    [Serializable]
    public class MailClientException : Exception
    {
        public MailClientException()
        {
        }

        protected MailClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MailClientException(string message) : base(message)
        {
        }

        public MailClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
