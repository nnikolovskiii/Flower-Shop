using Test1.Models.Enums;
using Test1.Models.Relations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models.ViewModels
{
    public class FlowerItemImageView
    {
        public FlowerItem FlowerItem{ get; set; }

        public IFormFile ImageFile { get; set; }

        public IEnumerable<Size> Sizes { get; set; }
        public IEnumerable<Color> Colors { get; set; }

        public string[] SelectedColors { get; set; }

        public string[] SelectedSizes { get; set; }
        public string PricesJson { get; set; }

        public List<SizeItem> SelectedSizeItems { get; set; }
        public List<FlowerType> SelectedFlowerTypes { get; set; }
        public List<Occasion> SelectedOccasions { get; set; }

        public List<Color> selectedColors { get; set; }

    }
}
