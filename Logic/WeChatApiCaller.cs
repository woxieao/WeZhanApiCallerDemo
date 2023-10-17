using ApiCallerDemo.Models.WeChat;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ApiCallerDemo.Logic
{
    internal class WeChatApiCaller
    {
        private readonly string _token;
        private static readonly HttpClient HttpClient = new()
        {
            BaseAddress = new Uri(WeChatApiDomain)
        };

        #region 接口地址
        private const string WeChatApiDomain = "https://api.weixin.qq.com/";
        private const string GetTokenUrl = "/cgi-bin/token";
        private const string UploadImgUrl = "/cgi-bin/material/add_material";
        private const string AddDraftUrl = "/cgi-bin/draft/add";
        private const string PublishDraftUrl = "/cgi-bin/freepublish/submit";
        #endregion
        /// <summary>
        /// 微信Api调用类
        /// </summary>
        /// <param name="token"></param>
        public WeChatApiCaller(string token)
        {
            _token = token;
        }

        /// <summary>
        /// 获取临时access_token,文档详见 https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Get_access_token.html
        /// </summary>
        /// <param name="appId">微信appId</param>
        /// <param name="secret">微信secret</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> InitTokenAsync(string appId, string secret)
        {
            var resp = await HttpClient.GetFromJsonAsync<GetAccessTokenResp>($"{GetTokenUrl}?grant_type=client_credential&appid={appId}&secret={secret}");
            return resp?.access_token ?? throw new Exception("获取微信access_token失败");
        }

        /// <summary>
        /// 新增永久图片素材,文档详见 https://developers.weixin.qq.com/doc/offiaccount/Asset_Management/Adding_Permanent_Assets.html
        /// </summary>
        /// <param name="imgBytes">图片二进制流</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UploadImage2WeChatAsync(byte[] imgBytes)
        {
            var imageContent = new ByteArrayContent(imgBytes);

            imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"media\"",
                FileName = $"\"{Guid.NewGuid():N}.png\"",
            };
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var requestContent = new MultipartFormDataContent($"{Guid.NewGuid()}")
            {
                imageContent
            };
            var boundary = requestContent.Headers?.ContentType?.Parameters.First(o => o.Name == "boundary")!;
            boundary.Value = boundary.Value?.Replace("\"", string.Empty);
            var resp = await (await HttpClient.PostAsync($"{UploadImgUrl}?access_token={_token}&type=image", requestContent)).Content.ReadFromJsonAsync<MediaResp>();
            return resp?.media_id ?? throw new Exception("上传素材失败");
        }

        /// <summary>
        /// 新增文章草稿,文档详见 https://developers.weixin.qq.com/doc/offiaccount/Draft_Box/Add_draft.html
        /// </summary>
        /// <param name="draftRequest"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> AddDraftAsync(DraftRequest draftRequest)
        {
            var resp = await (await HttpClient.PostAsJsonAsync($"{AddDraftUrl}?access_token={_token}", new { articles = new[] { draftRequest } }, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            })).Content.ReadFromJsonAsync<MediaResp>();
            return resp?.media_id ?? throw new Exception("新建文章草稿失败");
        }

        /// <summary>
        /// 发布草稿为文章,文档详见 https://developers.weixin.qq.com/doc/offiaccount/Publish/Publish.html
        /// </summary>
        /// <param name="draftId">草稿ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> PublishDraftAsync(string draftId)
        {
            var resp = await (await HttpClient.PostAsJsonAsync($"{PublishDraftUrl}?access_token={_token}", new { media_id = draftId })).Content.ReadFromJsonAsync<PublishResp>();
            return resp?.publish_id ?? throw new Exception("发布文章草稿失败");
        }

    }
}