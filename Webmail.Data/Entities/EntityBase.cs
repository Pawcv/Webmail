using System.ComponentModel.DataAnnotations;

namespace Webmail.Data.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
    }
}
