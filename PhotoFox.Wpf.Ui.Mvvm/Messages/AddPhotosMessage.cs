using System.Collections.Generic;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class AddPhotosMessage
    { 
        public AddPhotosMessage()
        {
            this.FileNames = new List<string>();
        }

        public List<string> FileNames { get; set; }
    }
}
