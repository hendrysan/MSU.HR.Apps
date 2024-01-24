using Commons.Loggers;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Requests;
using Models.Responses;
using Repositories.Interfaces;
using System.Net;
using static Models.Entities.EnumEntities;

namespace Repositories.Implements
{
    public class GrantAccessRepository(ConnectionContext context) : IGrantAccessRepository
    {
        private readonly string repositoryName = "GrandAccessRepository";
        private readonly ConnectionContext _context = context;

        public async Task<DefaultResponse> CheckAccess(EnumSource source, string roleCode, EnumModule module, EnumAction action)
        {
            DataTableResponse response = new();
            try
            {
                var data = await _context.GrantAccesses.Where(i => i.Source == source && i.Role.Code == roleCode && i.Module == module).FirstOrDefaultAsync();

                if (data == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                switch (action)
                {
                    case EnumAction.View:
                        response.StatusCode = data.IsView ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
                        break;
                    case EnumAction.Delete:
                        response.StatusCode = data.IsDelete ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
                        break;
                    case EnumAction.Create:
                        response.StatusCode = data.IsCreate ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
                        break;
                    case EnumAction.Edit:
                        response.StatusCode = data.IsEdit ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
                        break;
                    case EnumAction.Export:
                        response.StatusCode = data.IsExport ? HttpStatusCode.OK : HttpStatusCode.Unauthorized;
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.BadRequest;
                        break;
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

        public async Task<DataTableResponse> DataTableAsync(DataTableRequest request)
        {
            DataTableResponse response = new();
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;
                var data = _context.Set<GrantAccess>()
                                   .Include(i => i.Role)
                                   .AsQueryable();

                totalRecord = data.Count();

                if (!string.IsNullOrEmpty(request.SearchValue))
                {
                    //data = data.Where(x =>
                    //x.Code.ToLower().Contains(request.searchValue.ToLower()) ||
                    //x.Name.ToLower().Contains(request.searchValue.ToLower())
                    //);

                    //data = data.Where(x => x.Department != null && x.Department.Name.ToLower().Contains(request.searchValue.ToLower()));
                    //data = data.Where(x => x.Section != null && x.Section.Name.ToLower().Contains(request.searchValue.ToLower()));
                }

                filterRecord = data.Count();

                if (!string.IsNullOrEmpty(request.SortColumn) && !string.IsNullOrEmpty(request.SortColumnDirection))
                {
                    switch (request.SortColumn)
                    {
                        case nameof(GrantAccess.Module):
                            data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Module) : data.OrderBy(x => x.Module);
                            break;

                        default:
                            data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Source) : data.OrderBy(x => x.Source);
                            break;
                    }
                }
                else
                {
                    data = request.SortColumnDirection == "desc" ? data.OrderByDescending(x => x.Source) : data.OrderBy(x => x.Source);
                }

                var list = await data.Skip(request.Skip).Take(request.PageSize).ToListAsync();

                response.Draw = request.Draw;
                response.RecordsTotal = totalRecord;
                response.RecordsFiltered = filterRecord;
                response.Data = list;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex.Message, request);
            }
            return response;
        }

        public async Task<DefaultResponse> InititalAdminAsync()
        {
            DefaultResponse response = new();
            try
            {
                Guid roleAdmin = Guid.Parse("2b66adff-0fe3-46c7-ad05-aaf941fc5938");
                MasterRole masterRole = new();


                if (!(await _context.MasterRoles.AnyAsync(i => i.Code == "Administrator")))
                {
                    masterRole.Id = roleAdmin;
                    masterRole.Code = "Administrator";
                    masterRole.Name = "Administrator";
                    _context.MasterRoles.Add(masterRole);
                    await _context.SaveChangesAsync();
                }

                masterRole = await _context.MasterRoles.FirstOrDefaultAsync(i => i.Id == roleAdmin) ?? new();

                List<string> moduls = Enum.GetNames(typeof(EnumModule)).ToList();

                foreach (var modul in moduls)
                {
                    GrantAccess grantAccess = new GrantAccess()
                    {
                        Id = Guid.NewGuid(),
                        Role = masterRole,
                        Source = EnumSource.WebClient,
                        Module = Enum.Parse<EnumModule>(modul),
                        ActionName = "read, write",
                        IsCreate = true,
                        IsDelete = true,
                        IsEdit = true,
                        IsExport = true,
                        IsView = true,
                    };

                    var exists = await _context.GrantAccesses
                        .Where(i => i.Role == grantAccess.Role && i.Source == grantAccess.Source && i.Module == grantAccess.Module)
                        .FirstOrDefaultAsync();

                    if (exists == null)
                    {
                        _context.GrantAccesses.Add(grantAccess);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
            }

            return response;
        }

        public async Task<GrandAccessListResponse> ListAccess(string roleCode, EnumSource source)
        {
            GrandAccessListResponse response = new();
            try
            {
                var role = await _context.MasterRoles.FirstOrDefaultAsync(i => i.Code == roleCode);

                var access = await _context.GrantAccesses.Where(i => i.Role == role && i.Source == source)
                    .ToListAsync();

                if (access.Count > 0)
                {
                    response.List = access.ToList();
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
            }

            return response;
        }

        public async Task<GrandAccessListResponse> ListAccess(EnumSource source)
        {
            GrandAccessListResponse response = new();
            try
            {
                var access = await _context.GrantAccesses.Where(i => i.Source == source)
                    .ToListAsync();

                if (access.Count > 0)
                {
                    response.List = access.ToList();
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
            }

            return response;
        }
    }
}
