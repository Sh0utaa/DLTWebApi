namespace DLTAPI.models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
    }
}