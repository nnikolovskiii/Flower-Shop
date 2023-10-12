using Test1.Models.Relations;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class FlowerType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public byte[]? Image { get; set; }

        public virtual IEnumerable<FlowerItemFlowerType>? FlowerItems { get; set; } = new List<FlowerItemFlowerType>();
    }
}
