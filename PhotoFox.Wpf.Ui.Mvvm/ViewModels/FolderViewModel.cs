using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class FolderViewModel : ObservableObject
    {
        private bool isSelected;

        public FolderViewModel(string title)
        {
            this.Title = title;
            this.Albums = new ObservableCollection<AlbumViewModel>();
        }

        public string Title { get; }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
