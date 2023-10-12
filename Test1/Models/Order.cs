using System.ComponentModel.DataAnnotations;
using Test1.Models.Relations;

namespace Test1.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string? UserEmail { get; set; }

        public virtual ICollection<FlowerItemOrder>? FlowerItems { get; set; } = new List<FlowerItemOrder>();

        public string? Street { get; set; }

        public string? City { get; set; }

        public Boolean IsGift { get; set; }

        public Boolean IsDelivery { get; set; }

        public Boolean IsProcessed { get; set; }

        public String? Message { get; set; }

        public double GetPrice()
        {
            return FlowerItems.Sum(Item => Item.FlowerItem.Price
            * Item.FlowerItem.Count);
        }
    }
}
