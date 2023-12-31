namespace Models.Entities
{
    public class EnumEntities
    {
        public enum EnumSource
        {
            WebApi,
            WebClient
        }

        public enum EnumAction
        {
            View,
            Create,
            Edit,
            Delete,
            Export
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
