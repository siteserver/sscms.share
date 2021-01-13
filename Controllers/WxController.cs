using Microsoft.AspNetCore.Mvc;
using SSCMS.Services;
using SSCMS.Share.Abstractions;

namespace SSCMS.Share.Controllers
{
    [Route("api/share/wx")]
    public partial class WxController : ControllerBase
    {
        private const string Route = "";

        private readonly IWxManager _wxManager;
        private readonly IShareManager _shareManager;

        public WxController(IWxManager wxManager, IShareManager shareManager)
        {
            _wxManager = wxManager;
            _shareManager = shareManager;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public string Url { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string AppId { get; set; }
            public string Timestamp { get; set; }
            public string NonceStr { get; set; }
            public string Signature { get; set; }
        }
    }
}