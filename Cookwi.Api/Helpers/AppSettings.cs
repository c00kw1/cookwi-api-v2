namespace Cookwi.Api.Helpers
{
    public class AppSettings
    {
        #region Security

        public string Secret { get; set; }
        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }

        #endregion

        #region Mails

        public string EmailFrom { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }

        #endregion

        public string S3Bucket { get; set; }
        public string S3Region { get; set; }
    }
}