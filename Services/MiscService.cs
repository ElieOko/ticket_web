using Microsoft.EntityFrameworkCore;
using SCustomers.Data;
using SCustomers.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Services
{
    public interface IMiscService
    {
        Task<MiscModel> GetMiscAsync(int userId, string zone, int userBranchId, CancellationToken token);
    }
    public class MiscService:IMiscService
    {
        public MiscService(AppDbContext context)
        {
            Context = context;
        }

        private AppDbContext Context { get; }
        public async Task<MiscModel> GetMiscAsync(int userId, string zone, int userBranchId, 
            CancellationToken cancellationToken)
        {
            var branches = Context.Branches.AsNoTracking();

            if (!string.IsNullOrEmpty(zone))
            {
               branches = branches.Where(x => x.BranchZone == zone);
            }
               
            var vm = new MiscModel
            {
                TransferTypes = await Context.TransferTypes.AsNoTracking().ToListAsync(cancellationToken),
                Branches = await branches.ToListAsync(cancellationToken),
                Cards = await Context.Cards.AsNoTracking().ToListAsync(cancellationToken),
                Titles = await Context.Titles.AsNoTracking().ToListAsync(cancellationToken),
                Currencies = await Context.Currencies.ToListAsync(cancellationToken),
                TransferStatuses = await Context.TransferStatuses.ToListAsync(cancellationToken),
                Intervals = await Context.Intervals.ToListAsync(cancellationToken),
                Counters = await Context.Counters.Where(x => x.BranchId == userBranchId)
                .ToListAsync(cancellationToken),
                UserLogs = await Context.UserLogs.Where(x => x.UserId == userId).ToListAsync(cancellationToken)
            };

            return vm;
        }
    }
}
