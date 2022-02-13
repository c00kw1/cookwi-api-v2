using System;

namespace Cookwi.Api.Helpers
{
    public class AppSettings
    {
        public SecuritySettings Security { get; set; }

        public S3Settings S3 { get; set; }

        public MailSettings Mail { get; set; }

    }

    public class SecuritySettings
    {
        public string JwtSecret
        {
            get => Environment.GetEnvironmentVariable("API_JWT_SECRET") ?? _jwtSecret;
            set { _jwtSecret = Environment.GetEnvironmentVariable("API_JWT_SECRET") ?? value; }
        }
        public string _jwtSecret;
        public int JwtTTL { get; set; }
    }

    public class S3Settings
    {
        public string Bucket { get; set; }
        public string Region { get; set; }
        public string Endpoint { get; set; }
    }

    public class MailSettings
    {
        public string EmailFrom { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass
        {
            get => Environment.GetEnvironmentVariable("MAIL_SMTP_PWD") ?? _smtpPass;
            set { _smtpPass = Environment.GetEnvironmentVariable("MAIL_SMTP_PWD") ?? value; }
        }

        private string _smtpPass;
    }
}