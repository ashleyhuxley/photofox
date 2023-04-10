using System.Collections.Generic;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands.Parameters
{
    public class AddPhotoCommandParameters
    {
        public AddPhotoCommandParameters(IEnumerable<string> files, string albumId)
        {
            this.Files = files;
            this.AlbumId = albumId;
        }

        public IEnumerable<string> Files { get; }
        public string AlbumId { get; }
    }
}
