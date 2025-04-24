using System;
using System.ComponentModel.DataAnnotations;

namespace BeautySpa.ModelViews.FavoriteModelViews
{
    public class POSTFavoriteModelViews
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid ProviderId { get; set; }
    }
}
