using System.ComponentModel.DataAnnotations;

namespace Test1.Models.Relations
{
    public class FlowerItemOrder
    {
        [Key]
        public int Id { get; set; }
        public int FlowerItemId { get; set; }
        public virtual  SizeItem? FlowerItem { get; set; }

        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public int Count { get; set; }
    }
}
