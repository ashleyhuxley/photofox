using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoFox.Services;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class AlbumPermissionsViewModel : ObservableObject
    {
        private readonly IUserStorage userStorage;

        private readonly IPhotoAlbumService albumService;

        private string selectedUser = string.Empty;

        public AlbumPermissionsViewModel(
            IUserStorage userStorage,
            IPhotoAlbumService albumService)
        {
            this.Users = new ObservableCollection<string>();

            this.AllowedAlbums = new ObservableCollection<AlbumViewModel>();
            this.DisallowedAlbums = new ObservableCollection<AlbumViewModel>();

            this.userStorage = userStorage;
            this.albumService = albumService;

            this.PropertyChanged += AlbumPermissionsViewModel_PropertyChanged;

            this.AllowSelected = new RelayCommand(this.OnAllowSelectedExecute);
            this.RemoveSelected = new RelayCommand(this.OnRemoveSelectedExecute);
        }

        private async void AlbumPermissionsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.SelectedUser))
            {
                await LoadAlbums();
            }
        }

        public ObservableCollection<string> Users { get; }

        public ObservableCollection<AlbumViewModel> AllowedAlbums { get; }

        public ObservableCollection<AlbumViewModel> DisallowedAlbums { get; }

        public ICommand AllowSelected { get; }

        public ICommand RemoveSelected { get; }

        public string SelectedUser
        {
            get => this.selectedUser;
            set
            {
                if (selectedUser == value)
                {
                    return;
                }

                selectedUser = value;
                this.OnPropertyChanged(nameof(this.SelectedUser));
            }
        }

        public async Task Load()
        {
            this.Users.Clear();
            await foreach (var user in this.userStorage.GetUsersAsync())
            {
                this.Users.Add(user.UserName);
            }

            this.SelectedUser = this.Users.First();
        }

        public async Task LoadAlbums()
        {
            this.AllowedAlbums.Clear();
            this.DisallowedAlbums.Clear();

            await foreach (var album in this.albumService.GetAllAlbumsAsync())
            {
                var viewModel = new AlbumViewModel 
                { 
                    AlbumId = album.AlbumId, 
                    Title = album.Title, 
                    Description = album.Description, 
                    Folder = album.Folder,
                    SortOrder = album.SortOrder,
                };

                if (await this.albumService.UserHasPermissionAsync(album.AlbumId, this.SelectedUser))
                {
                    this.AllowedAlbums.Add(viewModel);
                }
                else
                {
                    this.DisallowedAlbums.Add(viewModel);
                }
            }
        }

        private async void OnAllowSelectedExecute()
        {
            var albums = this.DisallowedAlbums.Where(d => d.IsSelected).ToArray();

            foreach (var album in albums)
            {
                await this.albumService.AddPermissionAsync(album.AlbumId, this.SelectedUser);

                this.DisallowedAlbums.Remove(album);
                this.AllowedAlbums.Add(album);
            }
        }

        private async void OnRemoveSelectedExecute()
        {
            var albums = this.AllowedAlbums.Where(d => d.IsSelected).ToArray();

            foreach (var album in albums)
            {
                await this.albumService.RemovePermissionAsync(album.AlbumId, this.SelectedUser);

                this.AllowedAlbums.Remove(album);
                this.DisallowedAlbums.Add(album);
            }
        }
    }
}
