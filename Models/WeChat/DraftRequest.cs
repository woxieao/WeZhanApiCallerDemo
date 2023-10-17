namespace ApiCallerDemo.Models.WeChat
{
    internal class DraftRequest
    {
      
        public string title { get; set; }
        public string author { get; set; }
        public string digest { get; set; }
        public string content { get; set; }
        public string thumb_media_id { get; set; }
        public int need_open_comment { get; set; }
        public int only_fans_can_comment { get; set; }
    }
}