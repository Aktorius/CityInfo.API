﻿using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = "test@test.com";
        private string _mailFrom = "from@test.com";

        public void Send(string subject, string message)
        {            
            //just log to debug for demo purpose
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo} using CloudMailService");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}