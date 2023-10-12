

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Test1.Models.Enums;
using Test1.Models.Relations;

namespace Test1.Models
{
    public class FlowerItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public virtual ICollection<FlowerItemFlowerType>? FlowerTypes { get; set; } = new List<FlowerItemFlowerType>();

        [AllowNull]
        public byte[]? Image { get; set; }


        public virtual ICollection<ColorItem>? ColorItems { get; set; } = new List<ColorItem>();

        public virtual ICollection<SizeItem>? SizeItems { get; set; } = new List<SizeItem>();

        public virtual ICollection<FlowerItemOccasion> Occasions { get; set; } = new List<FlowerItemOccasion>();
    }
}
