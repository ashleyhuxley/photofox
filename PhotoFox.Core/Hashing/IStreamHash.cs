using System.IO;

namespace PhotoFox.Core.Hashing
{
    public interface IStreamHash
    {
        string ComputeHash(Stream input);
    }
}
