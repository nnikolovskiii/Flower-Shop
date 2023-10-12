using System.ComponentModel.DataAnnotations;

namespace Test1.Models.Relations
{
    public class FlowerItemOccasion
    {
        [Key]
        public int Id { get; set; }
        public int FlowerItemId { get; set; }
        public virtual FlowerItem? FlowerItem { get; set; }  

        public int OccasionId { get; set; }
        public virtual Occasion? Occasion { get; set; } 


    }
}
