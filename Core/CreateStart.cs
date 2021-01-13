using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Share.Abstractions;
using SSCMS.Utils;

namespace SSCMS.Share.Core
{
    public class CreateStart : IPluginCreateStartAsync
    {
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IShareManager _shareManager;

        public CreateStart(IPathManager pathManager, ISiteRepository siteRepository, IContentRepository contentRepository, IShareManager shareManager)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _shareManager = shareManager;
        }

        public async Task ParseAsync(IParseContext context)
        {
            var settings = await _shareManager.GetSettingsAsync(context.SiteId);
            if (!settings.IsWxShare) return;

            var site = await _siteRepository.GetAsync(context.SiteId);

            var title = site.SiteName;
            var description = string.Empty;
            var imageUrl = string.Empty;
            if (context.ContentId > 0)
            {
                var content = await _contentRepository.GetAsync(site, context.ChannelId, context.ContentId);
                if (content != null)
                {
                    title = content.Title;
                    description = content.Summary;
                    if (string.IsNullOrEmpty(description))
                    {
                        description = StringUtils.MaxLengthText(StringUtils.StripTags(content.Body), 50);
                    }
                    if (!string.IsNullOrEmpty(content.ImageUrl))
                    {
                        imageUrl = await _pathManager.ParseSiteUrlAsync(site, content.ImageUrl, false);
                        imageUrl = PageUtils.AddProtocolToUrl(imageUrl);
                    }
                }
            }
            if (string.IsNullOrEmpty(title))
            {
                title = settings.DefaultTitle;
            }
            if (string.IsNullOrEmpty(imageUrl) && !string.IsNullOrEmpty(settings.DefaultImageUrl))
            {
                imageUrl = await _pathManager.ParseSiteUrlAsync(site, settings.DefaultImageUrl, false);
                imageUrl = PageUtils.AddProtocolToUrl(imageUrl);
            }
            if (string.IsNullOrEmpty(description))
            {
                description = settings.DefaultDescription;
            }

            var axiosJsUrl = _pathManager.GetSiteFilesUrl(site, Libraries.AxiosJs);
            var wxShareJsUrl = _pathManager.GetApiHostUrl(site, $"assets/share/js/wxShare.js");
            var apiUrl = _pathManager.GetApiHostUrl(site, "api/share/wx");

            context.FootCodes.TryAdd($"{ShareManager.PluginId}_weixinShare", $@"
<script>
var wxShare = {{title: '{StringUtils.ToJsString(title)}', desc: '{StringUtils.ToJsString(description)}', imgUrl: '{StringUtils.ToJsString(imageUrl)}', siteId: {context.SiteId}, apiUrl: '{StringUtils.ToJsString(apiUrl)}'
}};
</script>
<script src=""{axiosJsUrl}""></script>
<script src=""//res.wx.qq.com/open/js/jweixin-1.6.0.js""></script>
<script src=""{wxShareJsUrl}""></script>
");
        }
    }
}
