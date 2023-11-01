using Commons.Loggers;
using System.Text.Json;

namespace UnitTest.CommonTest
{
    public class DiscordLoggerUnitTest
    {
        [Fact]
        public async Task SendTextWithFileAsync()
        {
            // Arrange
            var serviceName = "UnitTest";
            var message = "UnitTest with file";
            var query = JsonSerializer.Deserialize<object>(Body());

            // Act
            var status = await DiscordLogger.SendAsync(serviceName, message, query);

            // Assert
            Assert.True(status == System.Net.HttpStatusCode.OK || status == System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task SendTextOnlyAsync()
        {
            // Arrange
            var serviceName = "UnitTest";
            var message = "UnitTest without file";

            // Act
            var status = await DiscordLogger.SendAsync(serviceName, message);

            // Assert
            Assert.True(status == System.Net.HttpStatusCode.OK || status == System.Net.HttpStatusCode.NoContent);
        }

        string Body()
        {

            return $"{{\r\n  \"organizationId\": 2,\r\n  \"admissionId\": 2000006344702,\r\n  \"encounterTicketId\": \"45027b2d-97d6-48ff-9002-7469a0b4f7c4\",\r\n  \"patientId\": 2000002053312,\r\n  \"admissionDate\": \"2023-08-30T00:00:00\",\r\n  \"statusId\": \"3\",\r\n  \"doctorId\": 2000000025604,\r\n  \"userId\": 2000000025604,\r\n  \"userName\": \"siloam.therapist3\",\r\n  \"encounterProcedures\": [\r\n    {{\r\n      \"procedureItemId\": 41205,\r\n      \"procedureItemTypeId\": 5,\r\n      \"isFutureOrder\": false,\r\n      \"futureOrderDate\": null,\r\n      \"packageName\": null\r\n    }},\r\n     {{\r\n      \"procedureItemId\": 41210,\r\n      \"procedureItemTypeId\": 5,\r\n      \"isFutureOrder\": false,\r\n      \"futureOrderDate\": null,\r\n      \"packageName\": null\r\n    }}\r\n  ],\r\n  \"reminders\": [\r\n    {{\r\n      \"notification\": \"reminder test\",\r\n      \"therapistName\": \"Ivanna Therapist\",\r\n      \"therapistId\": 2000000025604,\r\n      \"isMyself\": true,\r\n      \"isActive\": true\r\n    }},\r\n    {{\r\n      \"notification\": \"test 2\",\r\n      \"therapistName\": \"Ivanna Therapist\",\r\n      \"therapistId\": 2000000025604,\r\n      \"isMyself\": true,\r\n      \"isActive\": true\r\n    }},\r\n    {{\r\n      \"notification\": \"test 1\",\r\n      \"therapistName\": \"Ivanna Therapist\",\r\n      \"therapistId\": 2000000025604,\r\n      \"isMyself\": true,\r\n      \"isActive\": true\r\n    }}\r\n  ],\r\n  \"subjective\": [\r\n    {{\r\n      \"soapMappingId\": \"2874a832-8503-4cad-b5dd-535775e94ac0\",\r\n      \"remarks\": \"anamnesis from copy-soap\",\r\n      \"value\": \"anamnesis from copy-soap\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"e851f782-8210-49eb-a074-f26c104f5ddf\",\r\n      \"remarks\": \"keluhan from copy-soap edit lagi\",\r\n      \"value\": \"keluhan from copy-soap edit lagi\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"078147ba-9e11-4da0-86fa-8bd901d82923\",\r\n      \"remarks\": \"\",\r\n      \"value\": \"\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"cf87f125-f2f9-4689-aa5e-91eb26571937\",\r\n      \"remarks\": \"\",\r\n      \"value\": \"\",\r\n      \"score\": \"0\"\r\n    }}\r\n  ],\r\n  \"objective\": [\r\n    {{\r\n      \"soapMappingId\": \"e5efd220-b68e-4652-ad03-d56ef29fcebb\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"ae3ca8c2-eab0-41b6-9e3e-3ecb8071a9d0\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"52ce9350-bfb2-4072-8893-d0c6cf8b3634\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"e903246c-df95-4fe0-96d2-cf90c036d3d7\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"e6ae2ea9-b321-4756-bf96-2dc232e4a7ba\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"78cbb61f-4a11-470a-b770-1a44eb04357f\",\r\n      \"remarks\": \"90\",\r\n      \"value\": \"90\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"2eeca752-a2ea-4426-b3cf-c1ea3833bf30\",\r\n      \"remarks\": \"36\",\r\n      \"value\": \"36\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"2a8dbddb-edfe-4704-876e-5a2d735bb541\",\r\n      \"remarks\": \"150\",\r\n      \"value\": \"150\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"a8e0013b-0443-4e7a-b670-4db9362b40e4\",\r\n      \"remarks\": \"60\",\r\n      \"value\": \"60\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"19516be9-9d54-4e30-8e90-0a8b41f5ba66\",\r\n      \"remarks\": \"30\",\r\n      \"value\": \"30\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"bf3050a2-046b-4053-9367-140be53e5d28\",\r\n      \"remarks\": \"4\",\r\n      \"value\": \"4\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"3aae8dc2-484f-4f3c-a01b-1b0c3f107176\",\r\n      \"remarks\": \"6\",\r\n      \"value\": \"6\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"dc2b9915-0e92-44c2-b66c-2c3eff5a489c\",\r\n      \"remarks\": \"Pasien yang akan menjalani prosedur yang menggunakan sedasi,Pasien dengan keterbatasan fisik\",\r\n      \"value\": \"[\\\"Pasien yang akan menjalani prosedur yang menggunakan sedasi\\\",\\\"Pasien dengan keterbatasan fisik\\\"]\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"b0c9d8e0-7533-43cd-bfcf-e8c0ac1d4b7e\",\r\n      \"remarks\": \"Edukasi Pencegahan,Pastikan rem terkunci, tempat tidur posisi rendah\",\r\n      \"value\": \"[\\\"Edukasi Pencegahan\\\",\\\"Pastikan rem terkunci, tempat tidur posisi rendah\\\"]\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"7218971c-e89f-4172-ae3c-b7fb855c1d6d\",\r\n      \"remarks\": \"Pemeriksaan Fisik Lain Lain\",\r\n      \"value\": \"Pemeriksaan Fisik Lain Lain\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"309f236a-52c0-4c49-b22b-904b819b48c4\",\r\n      \"remarks\": \"Objective\",\r\n      \"value\": \"Objective\",\r\n      \"score\": \"0\"\r\n    }}\r\n  ],\r\n  \"assessment\": [\r\n    {{\r\n      \"soapMappingId\": \"d24d0881-7c06-4563-bf75-3a20b843dc47\",\r\n      \"remarks\": \"Assesment\",\r\n      \"value\": \"Assesment\",\r\n      \"score\": \"0\"\r\n    }}\r\n  ],\r\n  \"plan\": [\r\n    {{\r\n      \"soapMappingId\": \"337a371f-baf5-424a-bdc5-c320c277cac6\",\r\n      \"remarks\": \"[\\\"3201000001\\\",\\\"3201000005\\\",\\\"3201000005\\\"]\",\r\n      \"value\": \"[\\\"3201000001\\\",\\\"3201000005\\\",\\\"3201000005\\\"]\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"05fc5c00-f6e8-4138-aef0-37c2dcd26159\",\r\n      \"remarks\": \"test edukasi\",\r\n      \"value\": \"test edukasi\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"f2302359-9b72-404e-a4a2-d037414e2b2c\",\r\n      \"remarks\": \"test proc\",\r\n      \"value\": \"test proc\",\r\n      \"score\": \"0\"\r\n    }},\r\n    {{\r\n      \"soapMappingId\": \"2df0294d-f94e-4ba4-8ba1-f017bfb55d92\",\r\n      \"remarks\": \"test plan\",\r\n      \"value\": \"test plan\",\r\n      \"score\": \"0\"\r\n    }}\r\n  ],\r\n  \"medicalAssessmentFunction\": [],\r\n  \"medicalFirstAssessment\": []\r\n}}\r\n";
        }
    }
}
