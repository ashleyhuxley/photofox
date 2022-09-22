using PhotoFox.Core.Extensions;
using System.IO;
using System.Security.Cryptography;

namespace PhotoFox.Core.Hashing
{
    public class StreamHashMD5 : IStreamHash
    {
        public string ComputeHash(Stream input)
        {
            byte[] bytes = new byte[input.Length];

            int numBytesToRead = (int)input.Length;
            int numBytesRead = 0;

            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = input.Read(bytes, numBytesRead, numBytesToRead);

                // The end of the file is reached.
                if (n == 0)
                {
                    break;
                }

                numBytesRead += n;
                numBytesToRead -= n;
            }

            // Start at 0, hash bytesToHash elements
            var md5 = MD5.Create();

            var result = md5.ComputeHash(bytes, 0, numBytesRead);
            return result.ToHex(true);
        }
    }
}
