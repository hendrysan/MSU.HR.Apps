using System.ComponentModel.DataAnnotations;
using static Models.Entities.EnumEntities;

namespace Models.Entities
{
    public class GrantAccess
    {
        public Guid Id { get; set; }
        public MasterRole? Role { get; set; }
        public EnumSource Source { get; set; }
        public EnumModule Module { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string ActionName { get; set; } = string.Empty;
        public bool IsView { get; set; }
        public bool IsCreate { get; set; } = false;
        public bool IsEdit { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public bool IsExport { get; set; } = false;
    }
}
