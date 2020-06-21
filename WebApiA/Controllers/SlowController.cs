using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApiA.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SlowController : ControllerBase
    {
        [HttpGet]
        public async Task<string> GetName()
        {
            await Task.Delay(6000);
            return "matt";
        }
    }
}