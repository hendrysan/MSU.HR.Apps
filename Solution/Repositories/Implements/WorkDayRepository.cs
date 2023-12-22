using ClosedXML.Excel;
using Commons.Loggers;
using Commons.Utilities;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Models.Entities;
using Models.Requests;
using Models.Responses;
using Repositories.Interfaces;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace Repositories.Implements
{
    public class WorkDayRepository(ConnectionContext context) : IWorkDayRepository
    {
        private readonly string repositoryName = "WorkDayRepository";
        private readonly ConnectionContext _context = context;

        public async Task<FileTemplateResponse> GenerateTemplateAsync()
        {
            FileTemplateResponse response = new();
            try
            {
                string templateName = "template/TemplateWorkDay.xlsx";
                string urlTemplate = await MinioUtility.GetAsync(templateName);

                using WebClient wc = new();
                var fileDownload = wc.DownloadData(urlTemplate);

                response.FileBytes = fileDownload;
                response.FileName = Path.GetFileName(urlTemplate);
                response.FileType = MimeMapping.MimeUtility.GetMimeMapping(urlTemplate);

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

        private static List<MasterWorkDay> ReadDocumentDetail(Stream streamFile, Guid batchId, Guid userId)
        {
            List<MasterWorkDay> masters = new();

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
                int cellIndex = 1;


                master.Year = Convert.ToInt32(row.Cell(cellIndex).GetString());
                master.Month = ("0" + row.Cell(++cellIndex).GetString())[^2..];
                master.Day01 = row.Cell(++cellIndex).GetString();
                master.Day02 = row.Cell(++cellIndex).GetString();
                master.Day03 = row.Cell(++cellIndex).GetString();
                master.Day04 = row.Cell(++cellIndex).GetString();
                master.Day05 = row.Cell(++cellIndex).GetString();
                master.Day06 = row.Cell(++cellIndex).GetString();
                master.Day07 = row.Cell(++cellIndex).GetString();
                master.Day08 = row.Cell(++cellIndex).GetString();
                master.Day09 = row.Cell(++cellIndex).GetString();
                master.Day10 = row.Cell(++cellIndex).GetString();
                master.Day11 = row.Cell(++cellIndex).GetString();
                master.Day12 = row.Cell(++cellIndex).GetString();
                master.Day13 = row.Cell(++cellIndex).GetString();
                master.Day14 = row.Cell(++cellIndex).GetString();
                master.Day15 = row.Cell(++cellIndex).GetString();
                master.Day16 = row.Cell(++cellIndex).GetString();
                master.Day17 = row.Cell(++cellIndex).GetString();
                master.Day18 = row.Cell(++cellIndex).GetString();
                master.Day19 = row.Cell(++cellIndex).GetString();
                master.Day20 = row.Cell(++cellIndex).GetString();
                master.Day21 = row.Cell(++cellIndex).GetString();
                master.Day22 = row.Cell(++cellIndex).GetString();
                master.Day23 = row.Cell(++cellIndex).GetString();
                master.Day24 = row.Cell(++cellIndex).GetString();
                master.Day25 = row.Cell(++cellIndex).GetString();
                master.Day26 = row.Cell(++cellIndex).GetString();
                master.Day27 = row.Cell(++cellIndex).GetString();
                master.Day28 = row.Cell(++cellIndex).GetString();
                master.Day29 = row.Cell(++cellIndex).GetString();
                master.Day30 = row.Cell(++cellIndex).GetString();
                master.Day31 = row.Cell(++cellIndex).GetString();

                masters.Add(master);
            }

            return masters;
        }


        public async Task<DefaultResponse> UploadAsync(MasterUser masterUser, IFormFile file, string? remarks = null)
        {
            DefaultResponse response = new();

            try
            {
                Guid batchId = Guid.NewGuid();

                var collections = ReadDocumentDetail(file.OpenReadStream(), batchId, masterUser.Id);

                List<MasterWorkDay> dataInsert = new();

                foreach (var data in collections)
                {
                    var exist = await _context.MasterWorkDays.Where(i => i.Year == data.Year && i.Month == data.Month).FirstOrDefaultAsync();
                    if (exist == null)
                    {
                        dataInsert.Add(data);
                    }
                    else
                    {
                        exist.UpdatedAt = DateTime.Now;
                        exist.UpdatedByUser = masterUser.Id;
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
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }

        public Task<DataTableResponse> DataTableAsync(DataTableRequest request)
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

    }
}
