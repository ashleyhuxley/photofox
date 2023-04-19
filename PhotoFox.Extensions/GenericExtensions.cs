using Azure;
using Moq;

namespace PhotoFox.Extensions
{
    public static class GenericExtensions
    {
        public static AsyncPageable<T> AsAsyncPageable<T>(this T item)
            where T : notnull
        {
            var page = Page<T>.FromValues(new[] { item }, null, Mock.Of<Response>());
            return AsyncPageable<T>.FromPages(new[] { page });
        }
    }
}
