using Models.Entities;

namespace Models.Responses
{
    public class SearchWorkDayResponse : DefaultResponse
    {
        public List<MasterWorkDay>? MasterWorkDays { get; set; }
    }
}
