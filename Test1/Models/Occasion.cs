using System.ComponentModel.DataAnnotations;
using Test1.Models.Relations;

namespace Test1.Models
{
    public class Occasion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public byte[]? Image { get; set; }

        public virtual IEnumerable<FlowerItemOccasion> FlowerItems { get; set; } = new List<FlowerItemOccasion>();

    }
}
