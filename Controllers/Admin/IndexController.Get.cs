using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Share.Core;
using SSCMS.Utils;

namespace SSCMS.Share.Controllers.Admin
{
    public partial class IndexController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, ShareManager.PermissionsSiteShare))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
            var ipAddress = PageUtils.GetIpAddress(Request);
            var settings = await _shareManager.GetSettingsAsync(request.SiteId);

            return new GetResult
            {
                SiteUrl = siteUrl,
                IpAddress = ipAddress,
                Settings = settings
            };
        }
    }
}
