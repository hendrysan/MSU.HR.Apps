using Commons.Loggers;
using Commons.Utilities;
using Discord;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Responses;
using Repositories.Interfaces;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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

                Guid batchId = Guid.NewGuid();


                var collections = await GetDocumentAttendanceDetails(file.OpenReadStream(), batchId);

                var dates = collections.Select(i => i.Column1).Distinct().ToList();

                if (dates.Count > 2)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "More date on column 1";
                    return response;
                }

                string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";
                string path = $"attendance/fingerprint/{fileName}";//Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", file.FileName);
                await MinioUtility.SendAsync(path, file.OpenReadStream(), file.ContentType);


                StagingDocumentAttendance staging = new()
                {
                    Id = batchId,
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

        public async Task<DefaultResponse> ProocessDocumentAsync(MasterUser user, Guid documentId)
        {
            DefaultResponse response = new();
            try
            {
                var staging = await _context.StagingDocumentAttendances
                    .Where(i => i.Id == documentId)
                    .Include(i => i.Details)
                    .FirstOrDefaultAsync();

                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Data not found";
                    return response;
                }

                var presents = staging.Details?.Select(i => new Present()
                {
                    IdNumber = Convert.ToInt32(Regex.Match(i.Column5 ?? string.Empty, @"\d+", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Value).ToString(),
                    DateTimeWork = DateTime.ParseExact($"{i.Column1} {i.Column2}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                }).Distinct().ToList();

                if (presents == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Invalid convertion data, please call administrator";
                    return response;
                }

                List<MasterAttendance> entities = [];
                MasterAttendance? master = default;

                foreach (var id in presents.Select(i => i.IdNumber).Distinct().ToList())
                {
                    var workIn = presents.Where(i => i.IdNumber == id).Select(i => i.DateTimeWork).Min();
                    var workOut = presents.Where(i => i.IdNumber == id).Select(i => i.DateTimeWork).Max();
                    long ticks = 0;

                    if (workIn != null && workOut != null)
                        ticks = workOut.Value.Ticks - workIn.Value.Ticks;

                    master = new MasterAttendance()
                    {
                        Id = Guid.NewGuid(),
                        IdNumber = id ?? string.Empty,
                        SourceData = "Staging",
                        SourceId = documentId.ToString(),
                        CreatedAt = DateTime.Now,
                        TotalWorkHours = Convert.ToDouble(TimeSpan.FromTicks(ticks).TotalHours),
                        PresentIn = workIn.HasValue ? workIn.Value : null,
                        PresentOut = workOut.HasValue ? workOut.Value : null
                    };

                    entities.Add(master);
                }

                if (entities.Count > 0)
                {
                    _context.MasterAttendances.AddRange(entities);
                    await _context.SaveChangesAsync();
                }

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

        private class Present
        {
            public string? IdNumber { get; set; }
            public DateTime? DateTimeWork { get; set; }
        }
    }


}
