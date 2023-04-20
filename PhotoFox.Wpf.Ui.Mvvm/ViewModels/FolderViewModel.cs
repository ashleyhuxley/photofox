using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class FolderViewModel : ObservableObject
    {
        private bool isSelected;

        public FolderViewModel(string title)
        {
            this.Title = title;
        }

        public string Title { get; }

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
