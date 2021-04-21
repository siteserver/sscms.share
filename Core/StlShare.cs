using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Share.Abstractions;
using SSCMS.Utils;

namespace SSCMS.Share.Core
{
    public class StlShare : IPluginParseAsync
    {
        private const string AttributeUrl = "url";      // 网址，默认使用 window.location.href
        private const string AttributeSource = "source";// 来源（QQ空间会用到）, 默认读取head标签：<meta name="site" content="" />
        private const string AttributeTitle = "title";  // 标题，默认读取 document.title 或者 <meta name="title" content="" />
        private const string AttributeOrigin = "origin";    // 分享 @ 相关 twitter 账号
        private const string AttributeDescription = "description";  // 描述, 默认读取head标签：<meta name="description" content="" />
        private const string AttributeImage = "image";              // 图片, 默认取网页中第一个img标签
        private const string AttributeSites = "sites";              //               : ['qzone', 'qq', 'weibo','wechat', 'douban'], // 启用的站点
        private const string AttributeDisabled = "disabled";        //            : ['google', 'facebook', 'twitter'], // 禁用的站点
        private const string AttributeWechatQrcodeTitle = "wechatQrcodeTitle";      //微信扫一扫：分享，微信二维码提示文字
        private const string AttributeWechatQrcodeHelper = "wechatQrcodeHelper";    //微信里点“发现”，扫一下二维码便可将本文分享至朋友圈。'

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IShareManager _shareManager;

        public StlShare(IPathManager pathManager, ISiteRepository siteRepository, IContentRepository contentRepository, IShareManager shareManager)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _shareManager = shareManager;
        }

        public string ElementName => "stl:share";

        public async Task<string> ParseAsync(IParseStlContext context)
        {
            var url = string.Empty;
            var source = string.Empty;
            var title = string.Empty;
            var origin = string.Empty;
            var description = string.Empty;
            var image = string.Empty;
            var sites = "weibo, qq, qzone, douban";
            var disabled = string.Empty;
            var wechatQrcodeTitle = string.Empty;
            var wechatQrcodeHelper = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in context.StlAttributes.AllKeys)
            {
                var value = context.StlAttributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeUrl))
                {
                    url = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSource))
                {
                    source = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTitle))
                {
                    title = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeOrigin))
                {
                    origin = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeDescription))
                {
                    description = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeImage))
                {
                    image = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSites))
                {
                    sites = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeDisabled))
                {
                    disabled = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWechatQrcodeTitle))
                {
                    wechatQrcodeTitle = await context.ParseAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWechatQrcodeHelper))
                {
                    wechatQrcodeHelper = await context.ParseAsync(value);
                }
                else
                {
                    attributes[name] = await context.ParseAsync(value);
                }
            }

            var settings = await _shareManager.GetSettingsAsync(context.SiteId);
            var site = await _siteRepository.GetAsync(context.SiteId);

            if (settings.IsWxShare)
            {
                sites += ",wechat";
            }

            var cssUrl = _pathManager.GetApiHostUrl(site, "/assets/share/css/share.min.css");
            var jsUrl = _pathManager.GetApiHostUrl(site, "/assets/share/js/social-share.min.js");

            context.HeadCodes[ShareManager.PluginId] = @$"<link rel=""stylesheet"" href=""{cssUrl}"">";
            context.FootCodes[ShareManager.PluginId] = @$"<script type=""text/javascript"" src=""{jsUrl}""></script>";

            Content content = null;
            if (context.ContentId > 0)
            {
                if (string.IsNullOrEmpty(title))
                {
                    if (content == null)
                    {
                        content = await _contentRepository.GetAsync(site, context.ChannelId, context.ContentId);
                    }

                    if (content != null)
                    {
                        title = content.Title;
                    }
                }
                if (string.IsNullOrEmpty(image))
                {
                    if (content == null)
                    {
                        content = await _contentRepository.GetAsync(site, context.ChannelId, context.ContentId);
                    }

                    if (content != null && !string.IsNullOrEmpty(content.ImageUrl))
                    {
                        image = await _pathManager.ParseSiteUrlAsync(site, content.ImageUrl, false);
                        image = PageUtils.AddProtocolToUrl(image);
                    }
                }
                if (string.IsNullOrEmpty(description))
                {
                    if (content == null)
                    {
                        content = await _contentRepository.GetAsync(site, context.ChannelId, context.ContentId);
                    }

                    if (content != null)
                    {
                        description = content.Summary;
                        if (string.IsNullOrEmpty(description))
                        {
                            description = StringUtils.MaxLengthText(StringUtils.StripTags(content.Body), 50);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(title))
            {
                title = settings.DefaultTitle;
            }
            if (string.IsNullOrEmpty(image) && !string.IsNullOrEmpty(settings.DefaultImageUrl))
            {
                image = await _pathManager.ParseSiteUrlAsync(site, settings.DefaultImageUrl, false);
                image = PageUtils.AddProtocolToUrl(image);
            }
            if (string.IsNullOrEmpty(description))
            {
                description = settings.DefaultDescription;
            }

            if (!string.IsNullOrEmpty(url))
            {
                attributes["data-url"] = url;
            }
            if (!string.IsNullOrEmpty(source))
            {
                attributes["data-source"] = source;
            }
            if (!string.IsNullOrEmpty(title))
            {
                attributes["data-title"] = title;
            }
            if (!string.IsNullOrEmpty(origin))
            {
                attributes["data-origin"] = origin;
            }
            if (!string.IsNullOrEmpty(description))
            {
                attributes["data-description"] = description;
            }
            if (!string.IsNullOrEmpty(image))
            {
                attributes["data-image"] = image;
            }
            if (!string.IsNullOrEmpty(sites))
            {
                attributes["data-sites"] = sites;
            }
            if (!string.IsNullOrEmpty(disabled))
            {
                attributes["data-disabled"] = disabled;
            }
            if (!string.IsNullOrEmpty(wechatQrcodeTitle))
            {
                attributes["data-wechat-qrcode-title"] = wechatQrcodeTitle;
            }
            if (!string.IsNullOrEmpty(wechatQrcodeHelper))
            {
                attributes["data-data-wechat-qrcode-helper"] = wechatQrcodeHelper;
            }

            return $@"<div class=""social-share"" {TranslateUtils.ToAttributesString(attributes)}></div>";
        }
    }
}
