using Microsoft.EntityFrameworkCore;
using SCustomers.Data;
using SCustomers.Dtos;
using SCustomers.Entities;
using SCustomers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Services
{
    public interface ICounterService
    {
        Task<List<Counter>> GetCounters(int branchId, CancellationToken cancellationToken);
        Task CreateCounter(int branchId, CounterCreateDto request, CancellationToken cancellationToken);
        Task UpdateCounter(CounterUpdateDto request, CancellationToken cancellationToken);
        Task DeleteCounter(int counterId, CancellationToken cancellationToken);
    }
    public class CounterService:ICounterService
    {
        public CounterService(AppDbContext context)
        {
            DbContext = context;
        }

        public AppDbContext DbContext { get; }
        public async Task<List<Counter>> GetCounters(int branchId, CancellationToken cancellationToken)
        {
            return await DbContext.Counters.Where(x => x.BranchId == branchId).ToListAsync(cancellationToken);
        }
        public async Task CreateCounter(int branchId, CounterCreateDto request, CancellationToken cancellationToken)
        {
            var counter = new Counter { BranchId = branchId, Name = request.Name };

            DbContext.Counters.Add(counter);

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateCounter(CounterUpdateDto request, CancellationToken cancellationToken)
        {
            var counter = await DbContext.Counters.Where(x => x.CounterId == request.CounterId)
                                    .FirstOrDefaultAsync(cancellationToken);

            if (counter == null)
                throw new NotFoundException($"Aucun guichet avec l'id {request.CounterId}");
            counter.Name = request.Name;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteCounter(int counterId, CancellationToken cancellationToken)
        {
            var counter = await DbContext.Counters.Where(x => x.CounterId == counterId)
                                    .FirstOrDefaultAsync(cancellationToken);

            if (counter == null)
                throw new NotFoundException($"Aucun guichet avec l'id {counterId}");

            DbContext.Counters.Remove(counter);

            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
