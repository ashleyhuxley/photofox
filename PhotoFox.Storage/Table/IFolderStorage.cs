using PhotoFox.Storage.Models;
using System.Collections.Generic;

namespace PhotoFox.Storage.Table
{
    internal interface IFolderStorage
    {
        IAsyncEnumerable<Folder> GetFoldersAsync();
    }
}
