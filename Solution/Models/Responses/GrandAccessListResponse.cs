using Models.Entities;

namespace Models.Responses
{
    public class GrandAccessListResponse : DefaultResponse
    {
        public List<GrantAccess>? List { get; set; }
    }
}
