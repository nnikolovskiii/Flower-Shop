using System.ComponentModel.DataAnnotations;

namespace Test1.Models.Relations
{
    public class FlowerItemFlowerType
    {
        [Key]
        public int Id { get; set; }
        public int FlowerItemId { get; set; }
        public virtual FlowerItem? FlowerItem { get; set; }

        public int FlowerTypeId { get; set; }
        public virtual FlowerType? FlowerType { get; set; }
    }
}
