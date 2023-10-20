using Microsoft.AspNetCore.Mvc;
using SCustomers.Dtos;
using SCustomers.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SCustomers.Controllers
{
    public class CountersController : BaseController
    {
        public CountersController(ICounterService service)
        {
            CounterService = service;
        }
        public ICounterService CounterService { get; }

        public async Task<IActionResult> OnGetCounters(CancellationToken cancellationToken)
        {
            return Ok(await CounterService.GetCounters(UserBranchId, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCounter(CounterCreateDto request, CancellationToken cancellationToken)
        {
            await CounterService.CreateCounter(UserBranchId, request, cancellationToken);
            return  NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> OnPutCounter(CounterUpdateDto request, CancellationToken cancellationToken)
        {
            await CounterService.UpdateCounter(request, cancellationToken);
            return NoContent();
        }
        [HttpDelete("{counterId}")]
        public async Task<IActionResult> OnDeleteCounter(int counterId, CancellationToken cancellationToken)
        {
            await CounterService.DeleteCounter(counterId, cancellationToken);
            return NoContent();
        }
    }
}
