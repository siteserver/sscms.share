namespace SSCMS.Share.Models
{
    public class Settings
    {
        public bool IsSettings { get; set; }
        public string DefaultTitle { get; set; }
        public string DefaultImageUrl { get; set; }
        public string DefaultDescription { get; set; }
        public bool IsWxShare { get; set; }
        public string MpAppId { get; set; }
        public string MpAppSecret { get; set; }
    }
}