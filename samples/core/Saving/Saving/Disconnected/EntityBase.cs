using System.ComponentModel.DataAnnotations.Schema;

namespace EFSaving.Disconnected
{
    public abstract class EntityBase
    {
        [NotMapped]
        public bool IsNew { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsChanged { get; set; }
    }
}