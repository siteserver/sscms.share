using System.Threading.Tasks;
using SSCMS.Share.Models;

namespace SSCMS.Share.Abstractions
{
    public interface IShareManager
    {
        Task<Settings> GetSettingsAsync(int siteId);

        Task SetSettingsAsync(int siteId, Settings settings);
    }
}