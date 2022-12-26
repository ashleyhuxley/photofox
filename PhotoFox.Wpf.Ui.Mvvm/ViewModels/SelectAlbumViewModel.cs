using CommunityToolkit.Mvvm.ComponentModel;
using PhotoFox.Core.Extensions;
using PhotoFox.Services;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class SelectAlbumViewModel : ObservableObject
    {
        private readonly IPhotoAlbumService photoAlbumService;

        private AlbumViewModel emptyAlbum = new AlbumViewModel { Title = "New Album...", AlbumId = string.Empty };

        private AlbumViewModel selectedAlbum;

        private string newAlbumName;

        public SelectAlbumViewModel(IPhotoAlbumService photoAlbumService)
        {
            AlbumList = new ObservableCollection<AlbumViewModel>();
            this.photoAlbumService = photoAlbumService;

            selectedAlbum = emptyAlbum;
            newAlbumName = string.Empty;
        }

        public ObservableCollection<AlbumViewModel> AlbumList { get; }

        public bool NewAlbumTextIsEnabled
        {
            get => this.SelectedAlbum == emptyAlbum;
        }

        public AlbumViewModel SelectedAlbum
        {
            get => this.selectedAlbum;
            set
            {
                this.selectedAlbum = value;
                this.OnPropertyChanged(nameof(this.SelectedAlbum));
                this.OnPropertyChanged(nameof(this.NewAlbumTextIsEnabled));

                if (!this.NewAlbumTextIsEnabled)
                {
                    this.NewAlbumName = string.Empty;
                }
            }
        }

        public string NewAlbumName
        {
            get => this.newAlbumName;
            set
            {
                this.newAlbumName = value;
                OnPropertyChanged(nameof(this.NewAlbumName));
            }
        }

        public async Task Load()
        {
            var unsortedList = new List<AlbumViewModel>();

            AlbumList.Add(emptyAlbum);

            await foreach (var album in this.photoAlbumService.GetAllAlbumsAsync())
            {
                var viewModel = new AlbumViewModel
                {
                    AlbumId = album.AlbumId,
                    Title = album.Title
                };

                unsortedList.Add(viewModel);
            }

            unsortedList.OrderBy(a => a.Title).ForEach(AlbumList.Add);
        }
    }
}