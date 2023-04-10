using PhotoFox.Storage.Models;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Queue
{
    public interface IUploadQueue
    {
        Task QueueUploadMessage(UploadMessage message);
    }
}
