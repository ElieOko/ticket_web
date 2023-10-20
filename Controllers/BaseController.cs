using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace SCustomers.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected int UserId
        {
            get
            {
                return int.Parse(HttpContext.User.Identity.Name);
            }
        }
        protected int UserBranchId
        {
            get
            {
                return int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type =="bid").Value);
            }
        }
    }
}
