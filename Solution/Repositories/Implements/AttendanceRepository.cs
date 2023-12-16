using Commons.Loggers;
using Commons.Utilities;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using Models.Responses;
using Repositories.Interfaces;
using System.Net;

namespace Repositories.Implements
{
    public class AttendanceRepository(ConnectionContext context) : IAttendanceRepository
    {
        private readonly string repositoryName = "AttendanceRepository";
        private readonly ConnectionContext _context = context;


        private async Task<int> BulkInsertDocumentAttendanceDetail(List<StagingDocumentAttendanceDetail> documentAttendanceDetails)
        {
            try
            {
                int maxData = 1000;

                if (documentAttendanceDetails.Count > maxData)
                {
                    int total = documentAttendanceDetails.Count;
                    int count = 0;
                    int index = 0;

                    while (count < total)
                    {
                        var data = documentAttendanceDetails.Skip(index).Take(maxData).ToList();
                        _context.StagingDocumentAttendanceDetails.AddRange(data);
                        
                        await _context.SaveChangesAsync();

                        count += maxData;
                        index += maxData;
                    }

                    return count;
                }
                else
                {
                    _context.StagingDocumentAttendanceDetails.AddRange(documentAttendanceDetails);
                    return await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("Attendacne BulkInsertDocumentAttendanceDetail Error : " + ex.Message, ex.InnerException);
            }
        }

        private static async Task<List<StagingDocumentAttendanceDetail>> GetDocumentAttendanceDetails(Stream streamFile, Guid batchId)
        {

            try
            {
                List<StagingDocumentAttendanceDetail> documentAttendanceDetails = [];

                using (var streamReader = new StreamReader(streamFile))
                {
                    string separator = ",";
                    string? line = default;

                    await Task.Run(() =>
                    {
                        while (!string.IsNullOrEmpty(line = streamReader.ReadLine()))
                        {
                            StagingDocumentAttendanceDetail? documentAttendanceDetail = default;

                            string[] values = line.Split(separator);
                            int index = values.Length - 1;

                            if (index > 0)
                            {
                                documentAttendanceDetail = new StagingDocumentAttendanceDetail
                                {
                                    Id = Guid.NewGuid(),
                                    StagingDocumentAttendanceId = batchId,
                                    Separator = separator
                                };

                                if (index >= 0)
                                    documentAttendanceDetail.Column0 = values[0].Trim();

                                if (index >= 1)
                                    documentAttendanceDetail.Column1 = values[1].Trim();

                                if (index >= 2)
                                    documentAttendanceDetail.Column2 = values[2].Trim();

                                if (index >= 3)
                                    documentAttendanceDetail.Column3 = values[3].Trim();

                                if (index >= 4)
                                    documentAttendanceDetail.Column4 = values[4].Trim();

                                if (index >= 5)
                                    documentAttendanceDetail.Column5 = values[5].Trim();

                                if (index >= 6)
                                    documentAttendanceDetail.Column6 = values[6].Trim();

                                if (index >= 7)
                                    documentAttendanceDetail.Column7 = values[7].Trim();

                                if (index >= 8)
                                    documentAttendanceDetail.Column8 = values[8].Trim();

                                if (index >= 9)
                                    documentAttendanceDetail.Column9 = values[9].Trim();

                                if (index >= 10)
                                    documentAttendanceDetail.Column10 = values[10].Trim();
                            }

                            if (documentAttendanceDetail != null)
                                documentAttendanceDetails.Add(documentAttendanceDetail);
                        }
                    });
                }
                return documentAttendanceDetails;
            }
            catch (Exception e)
            {
                throw new NullReferenceException(e.Message, e.InnerException);
            }
        }

        public async Task<DefaultResponse> UploadAsync(MasterUser user, IFormFile file, DateTime documentDate, string? remarks = null)
        {
            DefaultResponse response = new();
            try
            {
                
                Guid BatchId = Guid.NewGuid();

                string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";

                string path = $"attendance/fingerprint/{fileName}";//Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);

                await MinioUtility.SendAsync(path, file.OpenReadStream(), file.ContentType);

                var collections = await GetDocumentAttendanceDetails(file.OpenReadStream(), BatchId);

                StagingDocumentAttendance staging = new()
                {
                    Id = BatchId,
                    DocumentName = file.FileName,
                    Path = path,
                    Size = file.Length.ToString(),
                    Type = file.ContentType,
                    Extension = Path.GetExtension(file.FileName),
                    Status = EnumEntities.EnumStatusDocumentAttendance.PENDING,
                    Remarks = remarks,
                    CreatedByUser = user.Id,
                    CreatedAt = DateTime.Now,
                    DocumentDate = documentDate,
                    TotalRow = collections.Count
                };

                _context.StagingDocumentAttendances.Add(staging);
                await _context.SaveChangesAsync();

                
                var taskDetails = await BulkInsertDocumentAttendanceDetail(collections);

                response.Message = $"Upload document successfuly, total record {taskDetails}";

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }
    }
}
