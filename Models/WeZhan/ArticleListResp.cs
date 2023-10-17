namespace ApiCallerDemo.Models.WeZhan
{
    internal class ArticleListResp
    {
        public List<ArticleListItem> List { get; set; } = new();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPageCount { get; set; }
    }
}