namespace BPT_Service.Common.Dtos
{
    public class EmailConfigModel
    {
        public string FromUserEmail { get; set; }
        public string FullUserName { get; set; }
        public string SendGridKey { get; set; }
    }
}