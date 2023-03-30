using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Table
{
    public interface ILogStorage
    {
        Task Log(LogEntry entry);
        Task Log(
            string message,
            string source,
            string photoId = "",
            string albumId = "",
            string level = "INFO",
            string hash = "");
    }
}
