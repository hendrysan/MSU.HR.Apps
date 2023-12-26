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
            Role,
            User,
            Employee,
            GrandAccess,
            WorkDay
        }

        public enum EnumStatusDocumentAttendance
        {
            PENDING,
            CANCELED,
            PROCESSED,
        }
    }
}
