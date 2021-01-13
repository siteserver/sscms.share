using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Share.Controllers
{
    public partial class WxController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var success = false;
            var errorMessage = string.Empty;
            var appId = string.Empty;
            var timestamp = string.Empty;
            var nonceStr = string.Empty;
            var signature = string.Empty;

            var settings = await _shareManager.GetSettingsAsync(request.SiteId);
            if (settings.IsWxShare)
            {
                string ticket;
                (success, ticket, errorMessage) = await _wxManager.GetJsApiTicketAsync(settings.MpAppId, settings.MpAppSecret);
                if (success)
                {
                    appId = settings.MpAppId;
                    timestamp = _wxManager.GetTimestamp();
                    nonceStr = _wxManager.GetNonceStr();
                    signature = _wxManager.GetJsApiSignature(ticket, nonceStr, timestamp, request.Url);
                }
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                AppId = appId,
                Timestamp = timestamp,
                NonceStr = nonceStr,
                Signature = signature
            };
        }
    }
}
