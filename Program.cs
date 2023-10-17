using ApiCallerDemo.Logic;
using ApiCallerDemo.Models.WeChat;

namespace ApiCallerDemo
{
    internal class Program
    {
        private static void InitArg(ref string arg, string argName)
        {
            while (string.IsNullOrWhiteSpace(arg))
            {
                Console.WriteLine($"请输入{argName},按回车确认");
                arg = Console.ReadLine() ?? string.Empty;
            }
        }

        static async Task Main(string[] args)
        {

            var weZhanAccessId = "";
            var weZhanAccessKey = "";
            var weZhanDomain = "";
            var weChatAppId = "";
            var weChatSecret = "";
            #region 检查参数是否有填写,如果没有填写则提示输入参数
            InitArg(ref weZhanAccessId, $"微站AccessId(如:{Guid.NewGuid():N})");
            InitArg(ref weZhanAccessKey, $"微站AccessKey(如:{Guid.NewGuid()})");
            InitArg(ref weZhanDomain, "微站域名(如:https://1234abcd.scd.wezhan.cn)");
            InitArg(ref weChatAppId, "微信AppId(如:wxda4bxxxxxxxxxxxx)");
            InitArg(ref weChatSecret, "微信Secret(如:d3ff739088698daxxxxxxxxxxxxxxxxx)");
            #endregion

            //初始化一个微站Api调用实例
            var weZhanApiCaller = new WeZhanApiCaller(weZhanAccessId, weZhanAccessKey, weZhanDomain);
            //初始化一个临时的微信accessToken
            var accessToken = await WeChatApiCaller.InitTokenAsync(weChatAppId, weChatSecret);
            //初始化一个微信Api调用实例
            var weChatApiCaller = new WeChatApiCaller(accessToken);

            //获取一批微站文章ID,并且遍历
            foreach (var articleId in await weZhanApiCaller.GetArticleIdListAsync(1, 10))
            {
                try
                {
                    //获取文章详情
                    var article = await weZhanApiCaller.GetArticleDetailAsync(articleId);
                    //下载文章主图,如果没有主图,则无法发布为微信文章草稿,因为微信需要草稿有一张主图
                    var imgBytes = await weZhanApiCaller.DownloadImgAsync(article.PictureUrl);
                    //上传主图,并且获取素材ID
                    var mediaId = await weChatApiCaller.UploadImage2WeChatAsync(imgBytes);
                    //根据文章内容、主图素材ID,生成一篇文章草稿
                    var draftId = await weChatApiCaller.AddDraftAsync(new DraftRequest()
                    {
                        author = "小编",
                        title = article.Title,
                        digest = article.Summary,
                        content = article.ContentDetail,
                        thumb_media_id = mediaId,
                        need_open_comment = 0,
                        only_fans_can_comment = 0,
                    });

                    //发布文章草稿,大功告成~
                    await weChatApiCaller.PublishDraftAsync(draftId);
                    Console.WriteLine($"文章[{article.Title}]同步到微信成功");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"文章Id[{articleId}]同步到微信失败,{e.Message}\n{e.StackTrace}");
                }
            }

        }
    }
}