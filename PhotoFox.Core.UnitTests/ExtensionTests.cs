using PhotoFox.Extensions;

namespace PhotoFox.Core.UnitTests
{
    public class ExtensionTests
    {
        [SetUp]
        public void Setup()
        {
            // Method intentionally left empty.
        }

        [Test]
        public void ByteArrayToHex_EmptyArray_DoesNotThrow()
        {
            byte[] bytes = Array.Empty<byte>();
            Assert.DoesNotThrow(() => { bytes.ToHex(true); });
        }

        [Test]
        public void ByteArrayToHex_UpperCaseIsTrue_ResultIsUpper()
        {
            byte[] bytes = new byte[] { 0b10101011, 0b11001101 };
            var result = bytes.ToHex(true);
            Assert.That(result, Is.EqualTo("ABCD"));
        }

        [Test]
        public void ByteArrayToHex_UpperCaseIsFalse_ResultIsLower()
        {
            byte[] bytes = new byte[] { 0b10101011, 0b11001101 };
            var result = bytes.ToHex(false);
            Assert.That(result, Is.EqualTo("abcd"));
        }

        [Test]
        public void ToPartitionKey_IsLocalDate_ThrowsArgumentException()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Local);
            Assert.Throws<ArgumentException>(() => { date.ToPartitionKey(); });
        }

        [Test]
        public void ToPartitionKey_IsUnspecifiedDate_ThrowsArgumentException()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Unspecified);
            Assert.Throws<ArgumentException>(() => { date.ToPartitionKey(); });
        }

        [Test]
        public void ToPartitionKey_IsUtcDate_ReturnsCorrectFormat()
        {
            var date = DateTime.SpecifyKind(new DateTime(2000, 7, 23), DateTimeKind.Utc);
            Assert.That(date.ToPartitionKey(), Is.EqualTo("20000723"));
        }

        [Test]
        public void ToBatchId_ValidInt_ReturnCorrectFormat()
        {
            var batch = 123.ToBatchId();
            Assert.That(batch, Is.EqualTo("000000000123"));
        }

        [Test]
        public void ToRotationDegrees_LessThanZero_ThrowsOutOfRangeException()
        {
            int? input = -1;
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { _ = input.ToRotationDegrees(); });
        }

        [Test]
        public void ToRotationDegrees_HigherThanEight_ThrowsOutOfRangeException()
        {
            int? input = -9;
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { _ = input.ToRotationDegrees(); });
        }

        [Test]
        public void ToRotationDegrees_IsNull_ReturnsZero()
        {
            int? input = null;
            Assert.That(input.ToRotationDegrees(), Is.EqualTo(0));
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(3, 180)]
        [TestCase(6, 90)]
        [TestCase(8, 270)]
        public void ToRotationDegrees_IsValidValue_ReturnsCorrectValue(int? input, int expected)
        {
            Assert.That(input.ToRotationDegrees(), Is.EqualTo(expected));
        }

        [Test]
        public void ToFileSize_NegativeInput_DoesNotThrow()
        {
            long input = -1;
            Assert.DoesNotThrow(() => { _ = input.ToFileSize(); });
        }

        [Test]
        [TestCase(1, "1 B")]
        [TestCase(10, "10 B")]
        [TestCase(1024, "1 KB")]
        [TestCase(1048576, "1 MB")]
        [TestCase(1572864, "1.5 MB")]
        [TestCase(1073741824, "1 GB")]
        [TestCase(1610612736, "1.5 GB")]
        public void ToFileSize_ValidInput_ReturnsCorrectValue(long input, string expected)
        {
            Assert.That(input.ToFileSize(), Is.EqualTo(expected));
        }
    }
}