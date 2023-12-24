using Commons.Loggers;
using Models.Entities;
using Models.Responses;
using Repositories.Interfaces;
using System.Net;

namespace Repositories.Implements
{
    public class PayrollRepository : IPayrollRepository
    {

        private readonly string repositoryName = "UserRepository";
        //private readonly ConnectionContext _context = context;
        //private readonly IMailRepository _mailRepository = mailRepository;


        public async Task<DefaultResponse> GeneratePayslip(MasterUser masterUser, int yearMonth, Guid payslipId)
        {
            DefaultResponse response = new();

            try
            {
                //string output = "D:\\export excel.xlsx";
                //var fileInfo = new FileInfo(output);

                //if (fileInfo != null)
                //    File.Delete(fileInfo.FullName);

                //string template = "template/TemplatePayslip.xlsx";
                //string urlTemplate = await MinioUtility.GetAsync(template);

                //using WebClient wc = new WebClient();
                //var fileDownload = wc.DownloadData(urlTemplate);
                //using MemoryStream stream = new MemoryStream(fileDownload);
                //stream.Seek(0, SeekOrigin.Begin);
                //using XLWorkbook workbook = new XLWorkbook(stream);
                //IXLWorksheet ws = workbook.Worksheet(1);
                //ws.Cell("A1").Value = "Nama Perusahaan";

                //workbook.SaveAs(output);





            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, masterUser);
                throw new NullReferenceException(ex.Message, ex.InnerException);

            }
            return response;
        }

        public Task<DefaultResponse> PayrollInfo(MasterUser masterUser)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultResponse> PayslipPeriod(MasterUser masterUser, int year)
        {
            throw new NotImplementedException();
        }
    }
}
