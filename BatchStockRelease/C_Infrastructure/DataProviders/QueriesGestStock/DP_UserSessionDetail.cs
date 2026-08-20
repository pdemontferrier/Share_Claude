using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;

namespace BatchStockRelease.C_Infrastructure.DataProviders.QueriesGestStock
{
    public class DP_UserSessionDetail : IR_UserSessionDetails
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        public DP_UserSessionDetail(IDbContextFactory<GestStockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DTO_UserSessionDetails>> GetUserSessionDetailsAsync(int appId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await (from us in context.UserSessions
                          join u in context.Users on us.IdUser equals u.Id
                          join a in context.VieApplications on us.IdApplication equals a.Id
                          where us.IdApplication == appId
                          select new DTO_UserSessionDetails
                          {
                              Id = us.Id,
                              IdApplication = us.IdApplication,
                              ApplicationName = a.Nom,
                              Accessible = a.Accessible,
                              IdUser = us.IdUser,
                              DeviceUser = us.DeviceUser ?? string.Empty,
                              FullnameUser = $"{u.Prenom} {u.Nom}",
                              Initial = u.Initial,
                              DeviceId = us.DeviceId ?? string.Empty,
                              DeviceIp = us.DeviceIp ?? string.Empty,
                              Connected = us.Connected,
                              ConnectionDate = us.ConnectionDate,
                              DisconnectionDate = us.DisconnectionDate
                          }).ToListAsync();
        }
    }
}