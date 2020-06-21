using Microsoft.AspNetCore.Mvc;

namespace WabApiB.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        //[Route("sex")]
        public string GetSex(string name)
        {
            if (name == "Jonathan")
                return "Man";
            return null;
        }

        [HttpGet]
        //[Route("age")]
        public int? GetAge(string name)
        {
            if (name == "Jonathan")
                return 24;
            return null;
        }
    }
}