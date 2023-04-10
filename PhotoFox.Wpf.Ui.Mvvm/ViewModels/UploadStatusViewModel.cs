using CommunityToolkit.Mvvm.ComponentModel;
using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class UploadStatusViewModel : ObservableObject
    {
        private string error;

        private UploadStatus status;

        public UploadStatusViewModel(string filename)
        {
            this.Filename = filename;
            this.error = string.Empty;
            this.status = UploadStatus.Ready;
        }

        public string Filename { get; }

        public string Error
        {
            get => this.error;
            set
            {
                this.error = value;
                this.OnPropertyChanged(nameof(Error));
            }
        }

        public UploadStatus Status
        {
            get => this.status;
            set
            {
                this.status = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }
    }
}
