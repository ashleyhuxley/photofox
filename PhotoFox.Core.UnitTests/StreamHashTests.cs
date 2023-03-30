using PhotoFox.Core.Hashing;

namespace PhotoFox.Core.UnitTests
{
    public class StreamHashTests
    {
        [Test]
        public void ComputeHash_ValidInput_ReturnsCorrectHash()
        {
            var hash = new StreamHashMD5();

            byte[] input = new byte[] { 1, 2, 3, 4, 5, 6 };
            using (var stream = new MemoryStream(input))
            {
                Assert.That(hash.ComputeHash(stream), Is.EqualTo("6AC1E56BC78F031059BE7BE854522C4C"));
            }
        }

        [Test]
        public void ComputeHash_EmptyInput_DoesNotThrow()
        {
            var hash = new StreamHashMD5();

            byte[] input = Array.Empty<byte>();
            using (var stream = new MemoryStream(input))
            {
                Assert.DoesNotThrow(() => hash.ComputeHash(stream));
            }
        }
    }
}
