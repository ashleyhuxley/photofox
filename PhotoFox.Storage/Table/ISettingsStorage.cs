using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface ISettingsStorage
    {
        Task<string> GetSetting(string name);
    }
}
