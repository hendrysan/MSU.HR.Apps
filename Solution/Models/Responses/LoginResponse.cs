using Models.Entities;

namespace Models.Responses
{
    public class LoginResponse : DefaultResponse
    {
        public MasterUser? MasterUser { get; set; }
        public MasterEmployee? MasterEmployee { get; set; }
        public List<GrantAccess>? Grants { get; set; }
    }
}
