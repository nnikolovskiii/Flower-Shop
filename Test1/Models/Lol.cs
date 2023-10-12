using System.ComponentModel.DataAnnotations;

namespace Test1.Models
{
    public class Lol
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
