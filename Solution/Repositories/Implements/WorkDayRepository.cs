using Commons.Loggers;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Requests;
using Models.Responses;
using Repositories.Interfaces;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace Repositories.Implements
{
    public class WorkDayRepository(ConnectionContext context) : IWorkDayRepository
    {
        private readonly string repositoryName = "WorkDayRepository";
        private readonly ConnectionContext _context = context;

        public async Task<DataTableResponse> DataTableAsync(DataTableRequest request)
        {
            DataTableResponse response = new();
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;

                string whereClause = string.Empty;
                string orderClause = string.Empty;

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    string search = request.SearchValue;
                    whereClause = $"WHERE \"Remarks\" LIKE '%{search}%' OR to_char(\"DateWork\", 'YYYY-MM-DD') LIKE '%{search}%'";
                }
                else
                {
                    whereClause = $"WHERE \"DateWork\" >= '%{new DateTime(year: DateTime.Today.Year, month: 1, day: 1)}%'";
                }

                if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortColumnDirection))
                {
                    string column = request.SortColumn;
                    string direction = request.SortColumnDirection;

                    orderClause = $"ORDER BY \"{column}\" {direction}";
                }
                else
                {
                    orderClause = $"ORDER BY \"Id\" ASC";
                }

                string baseQuery = $"SELECT * FROM \"MasterWorkDays\"";

                totalRecord = await _context.MasterWorkDays.FromSqlRaw(baseQuery).CountAsync();
                filterRecord = await _context.MasterWorkDays.FromSqlRaw($"{baseQuery} {whereClause}").CountAsync();

                var _data = await _context.MasterWorkDays.FromSqlRaw($"{baseQuery} {whereClause} {orderClause}").Skip(request.Skip).Take(request.PageSize).ToListAsync();



                /*
                var data = _context.Set<MasterWorkDay>()
                                   .AsQueryable();

                totalRecord = data.Count();

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    data = data.Where(x =>
                        x.Remarks.ToLower().Contains(request.SearchValue.ToLower()) ||
                        request.SearchValue.Contains(x.DateWork.ToString("yyyy-MM-dd"))
                    ).AsQueryable();
                }
                filterRecord = data.Count();

                if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortColumnDirection))
                {
                    switch (request.SortColumn)
                    {
                        case nameof(MasterWorkDay.DateWork):
                            data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.DateWork) : data.OrderBy(x => x.DateWork);
                            break;

                        case nameof(MasterWorkDay.Remarks):
                            data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Remarks) : data.OrderBy(x => x.Remarks);
                            break;
                        default:
                            data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Id) : data.OrderBy(x => x.Id);
                            break;
                    }
                }
                else
                {
                    data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Id) : data.OrderBy(x => x.Id);
                }


                var list = await data.Skip(request.Skip).Take(request.PageSize).ToListAsync();

                */

                response.Draw = request.Draw;
                response.RecordsTotal = totalRecord;
                response.RecordsFiltered = filterRecord;
                response.Data = _data;


            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        private async Task BulkInsertUpdate(List<MasterWorkDay> workDays, MasterUser masterUser)
        {
            List<MasterWorkDay> masters = new List<MasterWorkDay>();
            foreach (var workDay in workDays)
            {
                var exists = await _context.MasterWorkDays.FirstOrDefaultAsync(i => i.DateWork == workDay.DateWork);
                if (exists != null)
                {
                    exists.UpdatedAt = DateTime.Now;
                    exists.UpdatedByUser = masterUser.Id;
                    exists.IsHoliday = workDay.IsHoliday;
                    exists.Remarks = workDay.Remarks;

                    _context.MasterWorkDays.Update(exists);
                    await _context.SaveChangesAsync();
                }
                else
                    masters.Add(workDay);
            }

            if (masters.Count > 0)
            {
                _context.MasterWorkDays.AddRange(masters);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DefaultResponse> GenerateDate(MasterUser masterUser, int year)
        {
            DefaultResponse response = new();
            try
            {
                DateTime startDate = new(year: year, month: 1, day: 1);
                DateTime endDate = new(year: year, month: 12, day: 31);
                List<MasterWorkDay> workDays = [];

                MasterWorkDay? workDay;

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {

                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        workDay = new MasterWorkDay()
                        {
                            DateWork = date,
                            IsHoliday = true,
                            CreatedAt = DateTime.Now,
                            Remarks = date.DayOfWeek.ToString(),
                            CreatedByUser = masterUser.Id,
                            UpdatedAt = DateTime.Now,
                            UpdatedByUser = masterUser.Id,
                        };

                        workDays.Add(workDay);
                    }
                }

                await BulkInsertUpdate(workDays, masterUser);

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(string period)
        {
            SearchWorkDayResponse response = new();
            response.MasterWorkDays = new List<MasterWorkDay>();
            try
            {
                DateTime startDate = DateTime.ParseExact($"{period}01", "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                var datas = await _context.MasterWorkDays.Where(i => i.DateWork >= startDate && i.DateWork <= endDate).ToListAsync();
                response.MasterWorkDays.AddRange(datas);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(DateTime date)
        {
            SearchWorkDayResponse response = new();
            response.MasterWorkDays = new List<MasterWorkDay>();
            try
            {
                var datas = await _context.MasterWorkDays.Where(i => i.DateWork == date).ToListAsync();
                response.MasterWorkDays.AddRange(datas);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(List<string> periods)
        {
            SearchWorkDayResponse response = new();
            response.MasterWorkDays = new List<MasterWorkDay>();
            try
            {
                foreach (var period in periods)
                {
                    DateTime startDate = DateTime.ParseExact($"{period}01", "yyyyMMdd", CultureInfo.InvariantCulture);
                    DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                    var datas = await _context.MasterWorkDays.Where(i => i.DateWork >= startDate && i.DateWork <= endDate).ToListAsync();
                    response.MasterWorkDays.AddRange(datas);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(List<DateTime> dates)
        {
            SearchWorkDayResponse response = new();
            response.MasterWorkDays = new List<MasterWorkDay>();
            try
            {
                var datas = await _context.MasterWorkDays.Where(i => dates.Contains(i.DateWork)).ToListAsync();
                response.MasterWorkDays.AddRange(datas);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<SearchWorkDayResponse> SearchAsync(DateTime start, DateTime end)
        {
            SearchWorkDayResponse response = new();
            response.MasterWorkDays = new List<MasterWorkDay>();
            try
            {
                var datas = await _context.MasterWorkDays.Where(i => i.DateWork >= start && i.DateWork <= end).ToListAsync();
                response.MasterWorkDays.AddRange(datas);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                object obj = new { start, end };
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<DefaultResponse> UpdateDate(MasterUser masterUser, DateTime date, string remarks)
        {
            DefaultResponse response = new();
            try
            {
                List<MasterWorkDay> masters = new List<MasterWorkDay>()
                {
                    new MasterWorkDay() {DateWork = date, IsHoliday = true, Remarks = remarks},
                };

                await BulkInsertUpdate(masters, masterUser);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
        }

        public async Task<DefaultResponse> DeleteDate(MasterUser masterUser, int id)
        {
            DefaultResponse response = new();
            try
            {
                var deleted = await _context.MasterWorkDays.FirstOrDefaultAsync(i => i.Id == id);

                if (deleted != null)
                {
                    _context.MasterWorkDays.Remove(deleted);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message);
            }
            return response;
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
