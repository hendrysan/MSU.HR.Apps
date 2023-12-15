namespace Models.Entities
{
    public class EnumEntities
    {
        public enum EnumSource
        {
            WebApi,
            WebClient
        }

        public enum EnumModule
        {
            User,
            Employee
        }

        public enum EnumStatusDocumentAttendance
        {
            PENDING,
            CANCELED,
            PROCESSED,
        }
    }
}
