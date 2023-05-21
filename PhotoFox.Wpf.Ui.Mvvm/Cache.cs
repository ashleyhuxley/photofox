using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Collections.Generic;

namespace PhotoFox.Wpf.Ui.Mvvm
{
    internal static class Cache
    {
        internal static List<AlbumViewModel> Albums { get; } = new List<AlbumViewModel>();

        internal static List<PhotoViewModel> Photos { get; } = new List<PhotoViewModel>();
    }
}
