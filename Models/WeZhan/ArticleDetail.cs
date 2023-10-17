namespace ApiCallerDemo.Models.WeZhan
{
    internal class ArticleDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ContentDetail { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string PictureUrl { get; set; }
    }
}