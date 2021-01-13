using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Share.Core;

namespace SSCMS.Share.Controllers.Admin
{
    public partial class IndexController
    {

        [HttpPost, Route(RouteSettings)]
        public async Task<ActionResult<BoolResult>> SettingsSubmit([FromBody] SettingsSubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, ShareManager.PermissionsSiteShare))
            {
                return Unauthorized();
            }

            var settings = await _shareManager.GetSettingsAsync(request.SiteId);

            settings.IsSettings = true;
            settings.DefaultTitle = request.DefaultTitle;
            settings.DefaultImageUrl = request.DefaultImageUrl;
            settings.DefaultDescription = request.DefaultDescription;
            await _shareManager.SetSettingsAsync(request.SiteId, settings);
            await _authManager.AddSiteLogAsync(request.SiteId, "修改页面分享设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
