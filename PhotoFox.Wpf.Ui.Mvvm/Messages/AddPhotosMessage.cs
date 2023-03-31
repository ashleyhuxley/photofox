using System;
using System.Collections.Generic;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class AddPhotosMessage
    { 
        public AddPhotosMessage()
        {
            this.FileNames = new List<string>();
            this.AlbumId = Guid.Empty.ToString();
        }

        public List<string> FileNames { get; set; }

        public string AlbumId { get; set; }
    }
}
