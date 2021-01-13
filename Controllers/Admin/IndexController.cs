using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Share.Abstractions;
using SSCMS.Share.Models;

namespace SSCMS.Share.Controllers.Admin
{
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "share";
        private const string RouteSettings = "share/settings";
        private const string RouteWxShare = "share/wxShare";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _wxManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IShareManager _shareManager;

        public IndexController(IAuthManager authManager, IPathManager pathManager, IWxManager wxManager, ISiteRepository siteRepository, IShareManager shareManager)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
            _shareManager = shareManager;
        }

        public class GetResult
        {
            public string SiteUrl { get; set; }
            public string IpAddress { get; set; }
            public Settings Settings { get; set; }
        }

        public class SettingsSubmitRequest
        {
            public int SiteId { get; set; }
            public string DefaultTitle { get; set; }
            public string DefaultImageUrl { get; set; }
            public string DefaultDescription { get; set; }
        }

        public class WxShareSubmitRequest
        {
            public int SiteId { get; set; }
            public bool IsWxShare { get; set; }
            public string MpAppId { get; set; }
            public string MpAppSecret { get; set; }
        }

        public class WxShareSubmitResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
