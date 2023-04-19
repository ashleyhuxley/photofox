using CommunityToolkit.Mvvm.ComponentModel;
using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoImportViewModel : ObservableObject
    {
        private UploadStatus status;

        private bool isSelected;

        public PhotoImportViewModel(PhotoImport model)
        {
            this.Model = model;
            this.status = UploadStatus.Ready;
        }

        public PhotoImport Model { get; }

        public UploadStatus Status
        {
            get => this.status;
            set
            {
                this.status = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
