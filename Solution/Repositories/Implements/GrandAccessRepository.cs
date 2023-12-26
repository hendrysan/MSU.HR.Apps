using Commons.Loggers;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Responses;
using Repositories.Interfaces;
using System.Net;
using static Models.Entities.EnumEntities;

namespace Repositories.Implements
{
    public class GrandAccessRepository(ConnectionContext context) : IGrandAccessRepository
    {
        private readonly string repositoryName = "GrandAccessRepository";
        private readonly ConnectionContext _context = context;
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

                masterRole = await _context.MasterRoles.FindAsync(roleAdmin);

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
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }

        public async Task<GrandAccessListResponse> ListAccess(string roleCode, EnumSource source)
        {
            GrandAccessListResponse response = new();
            try
            {
                var access = await _context.GrantAccesses.Where(i => i.Role.Code == roleCode && i.Source == source)
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
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }
    }
}
