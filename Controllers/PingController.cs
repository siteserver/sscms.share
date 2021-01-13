using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Share.Controllers
{
    [Route("api/share/ping")]
    public class PingController : ControllerBase
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public string Get()
        {
            return "pong";
        }
    }
}
