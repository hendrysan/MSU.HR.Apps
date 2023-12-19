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

                //var idNumbers = staging.Details?.Select(i => new
                //{
                //    IdNumber = Convert.ToInt32(Regex.Match(i.Column5 ?? string.Empty, @"\d+").Value)
                //}).Distinct().ToList();

                List<Present>? presents = staging.Details?.Select(i => new Present()
                {
                    IdNumber = Convert.ToInt32(Regex.Match(i.Column5 ?? string.Empty, @"\d+").Value).ToString(),
                    DateTimeWork = DateTime.ParseExact($"{i.Column1} {i.Column2}", "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                }).Distinct().ToList();


                List<DateTime> dateAttendance = staging.Details.Select(i => DateTime.ParseExact(i.Column1 ?? string.Empty, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    .Distinct().ToList();

                List<MasterAttendance> entities = [];
                MasterAttendance? master = default;

                foreach (var present in presents ?? [])
                {
                    bool isExists = entities
                        .Where(i => i.IdNumber == present.IdNumber
                        && i.PresentIn == present.DateTimeWork
                    ).Any();


                    if (!isExists)
                    {
                        master = new MasterAttendance()
                        {
                            Id = Guid.NewGuid(),
                            IdNumber = present.IdNumber ?? string.Empty,
                            SourceData = "Staging",
                            SourceId = documentId.ToString(),
                            CreatedAt = DateTime.Now
                        };

                        master.PresentIn = present.DateTimeWork;
                        Present? checkDataOut = presents
                            .Where(i => i.IdNumber == present.IdNumber && i.DateTimeWork > master.PresentIn.Value.AddHours(4))
                            .OrderBy(i => i.DateTimeWork)
                            .FirstOrDefault();
                        if (checkDataOut != null)
                        {
                            master.PresentOut = checkDataOut.DateTimeWork;
                            long ticks = master.PresentOut.Value.Ticks - master.PresentIn.Value.Ticks;
                            master.TotalWorkHours = Convert.ToDouble(TimeSpan.FromTicks(ticks).TotalHours);
                        }
                        else
                        {
                            var doubleInsert = entities.Where(i => i.IdNumber == master.IdNumber && i.PresentOut == master.PresentIn).FirstOrDefault();
                            continue;
                        }

                        //if (!entities.Where(i => i.IdNumber == master.IdNumber && i.PresentIn == master.PresentOut).Any())
                        entities.Add(master);

                    }
                }

                if (entities.Count > 0)
                {
                    //var duplicates = entities.Where(i => i.PresentIn == i.PresentOut).ToList();
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
