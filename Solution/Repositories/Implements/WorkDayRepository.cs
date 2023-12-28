using ClosedXML.Excel;
using Commons.Loggers;
using Commons.Utilities;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Requests;
using Models.Responses;
using Repositories.Interfaces;
using System.Globalization;
using System.Net;

namespace Repositories.Implements
{
    public class WorkDayRepository(ConnectionContext context) : IWorkDayRepository
    {
        private readonly string repositoryName = "WorkDayRepository";
        private readonly ConnectionContext _context = context;

        public Task<DataTableResponse> DataTableAsync(DataTableRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultResponse> GenerateDate(MasterUser masterUser, int year)
        {
            throw new NotImplementedException();
        }

        public Task<SearchWorkDayResponse> SearchAsync(string period)
        {
            throw new NotImplementedException();
        }

        public Task<SearchWorkDayResponse> SearchAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<SearchWorkDayResponse> SearchAsync(List<string> periods)
        {
            throw new NotImplementedException();
        }

        public Task<SearchWorkDayResponse> SearchAsync(List<DateTime> dates)
        {
            throw new NotImplementedException();
        }

        public Task<SearchWorkDayResponse> SearchAsync(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Task<DefaultResponse> UpdateDate(MasterUser masterUser, DateTime date, string value)
        {
            throw new NotImplementedException();
        }



        /*
        public async Task<FileTemplateResponse> GenerateTemplateAsync()
        {
            FileTemplateResponse response = new();
            try
            {
                string templateName = "template/TemplateWorkDay.xlsx";
                string urlTemplate = await MinioUtility.GetAsync(templateName);

                using HttpClient httpClient = new();
                using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlTemplate);
                using Stream streamToReadFrom = await httpResponse.Content.ReadAsStreamAsync();
                using (MemoryStream ms = new())
                {
                    streamToReadFrom.CopyTo(ms);
                    response.FileBytes = ms.ToArray();
                }


                response.FileName = Path.GetFileName(urlTemplate);
                response.FileType = MimeMapping.MimeUtility.GetMimeMapping(urlTemplate);



            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);

            }

            return response;
        }

        private static List<MasterWorkDay> ReadDocumentDetail(Stream streamFile, Guid batchId, Guid userId)
        {
            List<MasterWorkDay> masters = [];

            List<DayOfWeek> isWeekEnd =
            [
                DayOfWeek.Sunday,
                DayOfWeek.Saturday
            ];

            using XLWorkbook workbook = new(streamFile);
            var ws = workbook.Worksheet(1).RangeUsed().RowsUsed().Skip(1);

            MasterWorkDay master;
            foreach (var row in ws)
            {
                master = new MasterWorkDay()
                {
                    Month = string.Empty,
                    BatchId = batchId,
                    CreatedAt = DateTime.Now,
                    CreatedByUser = userId
                };

                var rowNumber = row.RowNumber();
                int cellIndex = 2;

                master.Year = Convert.ToInt32(row.Cell(1).GetString());
                master.Month = ("0" + row.Cell(2).GetString())[^2..];

                string period = $"{master.Year}{master.Month}";
                DateTime startDate = DateTime.ParseExact($"{period}01", "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                var diff = (endDate - startDate).TotalDays + 1;

                for (int i = 1; i <= diff; i++)
                {
                    var buildDate = startDate.AddDays(i - 1);
                    if (buildDate <= endDate)
                    {
                        var _day = buildDate.ToString("dd");

                        string valueDate = isWeekEnd.Contains(buildDate.DayOfWeek) ? "H" : row.Cell(i + cellIndex).GetString();

                        master.GetType()?.GetProperty($"Day{_day}")?.SetValue(master, valueDate);
                    }
                }
                masters.Add(master);
            }

            return masters;
        }

        private async Task BulkInsertUpdate(List<MasterWorkDay> workDays, Guid userUpdate)
        {
            List<MasterWorkDay> dataInsert = [];

            foreach (var data in workDays)
            {
                var exist = await _context.MasterWorkDays.Where(i => i.Year == data.Year && i.Month == data.Month).FirstOrDefaultAsync();
                if (exist == null)
                {
                    dataInsert.Add(data);
                }
                else if (exist != data)
                {
                    exist.UpdatedAt = DateTime.Now;
                    exist.UpdatedByUser = userUpdate;
                    exist.Day01 = data.Day01;
                    exist.Day02 = data.Day02;
                    exist.Day03 = data.Day03;
                    exist.Day04 = data.Day04;
                    exist.Day05 = data.Day05;
                    exist.Day06 = data.Day06;
                    exist.Day07 = data.Day07;
                    exist.Day08 = data.Day08;
                    exist.Day09 = data.Day09;
                    exist.Day10 = data.Day10;
                    exist.Day11 = data.Day11;
                    exist.Day12 = data.Day12;
                    exist.Day13 = data.Day13;
                    exist.Day14 = data.Day14;
                    exist.Day15 = data.Day15;
                    exist.Day16 = data.Day16;
                    exist.Day17 = data.Day17;
                    exist.Day18 = data.Day18;
                    exist.Day19 = data.Day19;
                    exist.Day20 = data.Day20;
                    exist.Day21 = data.Day21;
                    exist.Day22 = data.Day22;
                    exist.Day23 = data.Day23;
                    exist.Day24 = data.Day24;
                    exist.Day25 = data.Day25;
                    exist.Day26 = data.Day26;
                    exist.Day27 = data.Day27;
                    exist.Day28 = data.Day28;
                    exist.Day29 = data.Day29;
                    exist.Day30 = data.Day30;
                    exist.Day31 = data.Day31;

                    _context.MasterWorkDays.Update(exist);
                    await _context.SaveChangesAsync();
                }
            }


            if (dataInsert.Count > 0)
            {
                _context.MasterWorkDays.AddRange(dataInsert);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DefaultResponse> UploadAsync(MasterUser masterUser, IFormFile file, string? remarks = null)
        {
            DefaultResponse response = new();

            try
            {
                Guid batchId = Guid.NewGuid();

                var collections = ReadDocumentDetail(file.OpenReadStream(), batchId, masterUser.Id);

                await BulkInsertUpdate(collections, masterUser.Id);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);

            }

            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(string period)
        {
            SearchWorkDayResponse response = new()
            {
                DetailWorkDays = []
            };
            try
            {

                DateTime startDate = DateTime.ParseExact($"{period}01", "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                string year = period[..4];
                string month = period.Substring(4, 2);

                var workDay = await _context.MasterWorkDays.Where(i => i.Year.ToString() == year && i.Month == month).FirstOrDefaultAsync();

                if (workDay == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Period not found";
                    return response;
                }

                var diff = (endDate - startDate).TotalDays + 1;

                DetailWorkDay detailWorkDay = new();

                for (int i = 1; i <= diff; i++)
                {
                    DateTime buildDate = startDate.AddDays(i - 1);
                    if (buildDate <= endDate)
                    {
                        string _day = buildDate.ToString("dd");
                        detailWorkDay = new()
                        {
                            Day = buildDate,
                            Value = workDay?.GetType()?.GetProperty($"Day{_day}")?.GetValue(workDay)?.ToString()
                        };
                        response.DetailWorkDays.Add(detailWorkDay);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, period);


            }

            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(DateTime date)
        {

            SearchWorkDayResponse response = new()
            {
                DetailWorkDays = []
            };

            try
            {

                var workDay = await _context.MasterWorkDays.Where(i => i.Year == date.Year && i.Month == date.ToString("MM")).FirstOrDefaultAsync();
                if (workDay == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Period not found";
                    return response;
                }

                DetailWorkDay detailWorkDay = new()
                {
                    Day = date,
                    Value = workDay?.GetType()?.GetProperty($"Day{date:dd}")?.GetValue(workDay, null)?.ToString()
                };
                response.DetailWorkDays.Add(detailWorkDay);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, date);


            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(List<string> periods)
        {
            SearchWorkDayResponse response = new()
            {
                DetailWorkDays = []
            };
            try
            {
                foreach (var period in periods)
                {
                    var data = await this.SearchAsync(period);

                    if (data.StatusCode != HttpStatusCode.OK)
                    {
                        response.StatusCode = data.StatusCode;
                        response.Message = data.Message;
                        return response;
                    }

                    if (data.DetailWorkDays?.Count > 0)
                    {
                        response.DetailWorkDays.AddRange(data.DetailWorkDays);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, periods);

            }

            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(List<DateTime> dates)
        {
            SearchWorkDayResponse response = new()
            {
                DetailWorkDays = []
            };
            try
            {
                foreach (var date in dates)
                {
                    var data = await this.SearchAsync(date);

                    if (data.StatusCode != HttpStatusCode.OK)
                    {
                        response.StatusCode = data.StatusCode;
                        response.Message = data.Message;
                        return response;
                    }

                    if (data.DetailWorkDays?.Count > 0)
                    {
                        response.DetailWorkDays.AddRange(data.DetailWorkDays);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, dates);

            }

            return response;
        }

        public Task<DataTableResponse> DataTableAsync(DataTableRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<DefaultResponse> GenerateDate(MasterUser masterUser, int year)
        {
            DefaultResponse response = new();
            try
            {
                Guid batchId = Guid.NewGuid();
                DateTime startDate = new(year: year, month: 1, day: 1);
                DateTime endDate = new(year: year, month: 12, day: 31);
                List<MasterWorkDay> workDays = [];

                MasterWorkDay? master;

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    string _year = date.ToString("yyyy");
                    string _month = date.ToString("MM");
                    string _day = date.ToString("dd");

                    master = workDays.Where(i => i.Year.ToString() == _year && i.Month.ToString() == _month).FirstOrDefault();

                    if (master == null)
                    {
                        master = new MasterWorkDay()
                        {
                            Year = Convert.ToInt32(_year),
                            Month = _month,
                            BatchId = batchId,
                            CreatedAt = DateTime.Now,
                            CreatedByUser = masterUser.Id
                        };
                    }
                    else
                        workDays.Remove(master);


                    if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                        master.GetType()?.GetProperty($"Day{_day}")?.SetValue(master, "H");
                    else
                        master.GetType()?.GetProperty($"Day{_day}")?.SetValue(master, string.Empty);

                    workDays.Add(master);
                }

                await BulkInsertUpdate(workDays, masterUser.Id);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, year);

            }

            return response;
        }

        public async Task<DefaultResponse> UpdateDate(MasterUser masterUser, DateTime date, string value)
        {
            DefaultResponse response = new();
            try
            {
                var master = await _context.MasterWorkDays.Where(i => i.Year == date.Year && i.Month == date.ToString("MM")).FirstOrDefaultAsync();

                if (master == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = $"Period not genereted on date = {date.ToShortDateString()}";
                    return response;
                }
                master.GetType()?.GetProperty($"Day{date:dd}")?.SetValue(master, value);
                master.UpdatedAt = DateTime.Now;
                master.UpdatedByUser = masterUser.Id;


            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, new { date, value });

            }

            return response;
        }
        
        */
    }
}
