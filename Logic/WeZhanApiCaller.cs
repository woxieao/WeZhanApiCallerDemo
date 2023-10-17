using ApiCallerDemo.Models.WeZhan;
using System.Net.Http.Json;

namespace ApiCallerDemo.Logic
{
    internal class WeZhanApiCaller
    {

        private readonly HttpClient _httpClient;

        private readonly Uri _weZhanDomain;
        private const string GetArticleListUrl = "/OpenNews/List";
        private const string GetArticleDetailUrl = "/OpenNews/Detail";
        /// <summary>
        /// 微站Api调用类,文档详见 https://open-api.clouddream.net/
        /// </summary>
        /// <param name="accessId">微站accessId</param>
        /// <param name="accessKey">微站accessKey</param>
        /// <param name="weZhanDomain">微站二级域名</param>
        public WeZhanApiCaller(string accessId, string accessKey, string weZhanDomain)
        {
            _weZhanDomain = new Uri(weZhanDomain);
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _weZhanDomain;
            _httpClient.DefaultRequestHeaders.Add(nameof(accessId), accessId);
            _httpClient.DefaultRequestHeaders.Add(nameof(accessKey), accessKey);
        }

        /// <summary>
        /// 获取文章ID列表
        /// </summary>
        /// <param name="pageIndex">分页下标</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<int>> GetArticleIdListAsync(int pageIndex = 1, int pageSize = 10)
        {
            var resp = (await _httpClient.GetFromJsonAsync<WeZhanResp<ArticleListResp>>($"{GetArticleListUrl}?pageIndex={pageIndex}&pageSize={pageSize}"))!;
            return resp.IsSuccess ? resp.Data.List.Select(i => i.Id).ToList() : throw new Exception(resp.Msg);

        }
        /// <summary>
        /// 获取微站文章详情
        /// </summary>
        /// <param name="articleId">文章ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ArticleDetail> GetArticleDetailAsync(int articleId)
        {
            var resp = (await _httpClient.GetFromJsonAsync<WeZhanResp<ArticleDetail>>($"{GetArticleDetailUrl}?id={articleId}"))!;
            return resp.IsSuccess ? resp.Data : throw new Exception(resp.Msg);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="imgUrl">图片地址</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> DownloadImgAsync(string imgUrl)
        {
            if (string.IsNullOrWhiteSpace(imgUrl))
            {
                throw new Exception("图片地址为空,无法继续操作");
            }
            else
            {
                if (imgUrl.StartsWith("//"))
                {
                    imgUrl = $"http:{imgUrl}";
                }
                _httpClient.DefaultRequestHeaders.Referrer = _weZhanDomain;
                return await (await _httpClient.GetAsync(imgUrl)).Content.ReadAsByteArrayAsync();
            }
        }
    }
}