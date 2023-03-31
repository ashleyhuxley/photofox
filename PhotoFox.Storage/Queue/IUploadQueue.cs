using System.Threading.Tasks;

namespace PhotoFox.Storage.Queue
{
    public interface IUploadQueue
    {
        Task QueueUploadMessage(string photoId, string albumId, string title);
    }
}
