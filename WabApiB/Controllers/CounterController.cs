using Microsoft.AspNetCore.Mvc;

namespace WabApiB.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CounterController : ControllerBase
    {
        private static int _count = 0;

        [HttpGet]
        public string Count()
        {
            return $"Count {++_count} from Webapi B";
        }
    }
}