using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCustomers.Services;

namespace SCustomers.Controllers
{
    public class MiscController : BaseController
    {
        public MiscController(IMiscService service)
        {
            MiscService = service;
        }

        private IMiscService MiscService { get; }
        public async Task<IActionResult> OnGetMisc(string zone, CancellationToken cancellationToken)
        {
            return Ok(await MiscService.GetMiscAsync(UserId, zone, UserBranchId, cancellationToken));
        }
    }
}
