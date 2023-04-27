using PhotoFox.Model;
using System;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel : ItemViewModelBase<Photo>
    {
        private int? starRatingOverride;

        public PhotoViewModel(Photo photo)
            : base(photo)
        {
        }

        public string Dimensions => $"{this.Item.ImageProperties.Dimensions.Width} x {this.Item.ImageProperties.Dimensions.Height}";

        public int StarRating
        {
            get
            {
                if (starRatingOverride.HasValue)
                {
                    return starRatingOverride.Value;
                }

                return this.Item.ImageProperties.StarRating;
            }
        }

        internal void OverrideStarRating(int newRating)
        {
            starRatingOverride= newRating;
            OnPropertyChanged(nameof(StarRating));
        }
    }
}
