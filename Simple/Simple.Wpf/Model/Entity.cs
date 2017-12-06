using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Simple.Wpf.Model
{
    [Table("ENTITIES")]
    public class Entity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}