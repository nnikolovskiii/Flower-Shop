using System.ComponentModel.DataAnnotations;
using Test1.Models.Enums;

namespace Test1.Models.Relations
{
    public class ColorItem
    {
        [Key]
        public int Id { get; set; }

        public Color Color { get; set; }

        public int FlowerItemId { get; set; }
        public virtual FlowerItem FlowerItem { get; set; }
    }
}
