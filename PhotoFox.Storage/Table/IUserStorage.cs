using PhotoFox.Storage.Models;
using System.Collections.Generic;

namespace PhotoFox.Storage.Table
{
    public interface IUserStorage
    {
        IAsyncEnumerable<User> GetUsersAsync();
    }
}
