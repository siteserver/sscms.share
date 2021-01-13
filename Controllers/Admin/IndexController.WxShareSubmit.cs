using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Share.Core;

namespace SSCMS.Share.Controllers.Admin
{
    public partial class IndexController
    {

        [HttpPost, Route(RouteWxShare)]
        public async Task<ActionResult<WxShareSubmitResult>> WxShareSubmit([FromBody] WxShareSubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, ShareManager.PermissionsSiteShare))
            {
                return Unauthorized();
            }

            var success = true;
            var errorMessage = string.Empty;
            if (request.IsWxShare)
            {
                (success, _, errorMessage) = await _wxManager.GetAccessTokenAsync(request.MpAppId, request.MpAppSecret);
            }

            if (success)
            {
                var settings = await _shareManager.GetSettingsAsync(request.SiteId);
                settings.IsWxShare = request.IsWxShare;
                settings.MpAppId = request.MpAppId;
                settings.MpAppSecret = request.MpAppSecret;

                await _shareManager.SetSettingsAsync(request.SiteId, settings);
                await _authManager.AddSiteLogAsync(request.SiteId, "修改微信分享设置");
            }

            return new WxShareSubmitResult
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }
    }
}
