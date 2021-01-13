using System.Threading.Tasks;
using SSCMS.Repositories;
using SSCMS.Share.Abstractions;
using SSCMS.Share.Models;

namespace SSCMS.Share.Core
{
    public class ShareManager : IShareManager
    {
        public const string PluginId = "sscms.share";
        public const string PermissionsSiteShare = "site_share";

        private readonly ISiteRepository _siteRepository;
        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IPluginConfigRepository _pluginConfigRepository;

        public ShareManager(ISiteRepository siteRepository, IWxAccountRepository wxAccountRepository, IPluginConfigRepository pluginConfigRepository)
        {
            _siteRepository = siteRepository;
            _wxAccountRepository = wxAccountRepository;
            _pluginConfigRepository = pluginConfigRepository;
        }

        public async Task<Settings> GetSettingsAsync(int siteId)
        {
            var settings = new Settings
            {
                IsSettings = 
                    await _pluginConfigRepository.GetAsync<bool>(PluginId, siteId,
                        nameof(Settings.IsSettings)),
                IsWxShare = await _pluginConfigRepository.GetAsync<bool>(PluginId, siteId,
                    nameof(Settings.IsWxShare)),
            };
            if (settings.IsSettings)
            {
                settings.DefaultTitle =
                    await _pluginConfigRepository.GetAsync<string>(PluginId, siteId,
                        nameof(Settings.DefaultTitle));
                settings.DefaultImageUrl =
                    await _pluginConfigRepository.GetAsync<string>(PluginId, siteId,
                        nameof(Settings.DefaultImageUrl));
                settings.DefaultDescription =
                    await _pluginConfigRepository.GetAsync<string>(PluginId, siteId,
                        nameof(Settings.DefaultDescription));
            }
            else
            {
                var site = await _siteRepository.GetAsync(siteId);
                settings.DefaultTitle = site.SiteName;
                settings.DefaultImageUrl = site.ImageUrl;
                settings.DefaultDescription = site.Description;
            }

            if (settings.IsWxShare)
            {
                settings.MpAppId =
                    await _pluginConfigRepository.GetAsync<string>(PluginId, siteId,
                        nameof(Settings.MpAppId));
                settings.MpAppSecret =
                    await _pluginConfigRepository.GetAsync<string>(PluginId, siteId,
                        nameof(Settings.MpAppSecret));
            }
            else
            {
                var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);
                account.MpAppId = account.MpAppId;
                account.MpAppSecret = account.MpAppSecret;
            }

            return settings;
        }

        public async Task SetSettingsAsync(int siteId, Settings settings)
        {
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.IsSettings), true);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.DefaultTitle), settings.DefaultTitle);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.DefaultImageUrl), settings.DefaultImageUrl);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.DefaultDescription), settings.DefaultDescription);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.IsWxShare), settings.IsWxShare);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.MpAppId), settings.MpAppId);
            await _pluginConfigRepository.SetAsync(PluginId, siteId, nameof(Settings.MpAppSecret), settings.MpAppSecret);
        }
    }
}
