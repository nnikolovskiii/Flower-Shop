using Test1.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Test1.Models.Relations
{
    public class SizeItem
    {
        [Key]
        public int Id { get; set; }

        public int Count { get; set; }

        public virtual Size Size { get; set; }

        public int FlowerItemId { get; set; }
        public virtual  FlowerItem FlowerItem { get; set; }   

        public double Price { get ; set; }

        public virtual IEnumerable<FlowerItemOrder> Orders { get; set; } = new List<FlowerItemOrder>();
    }
}
