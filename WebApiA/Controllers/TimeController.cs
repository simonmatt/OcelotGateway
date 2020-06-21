using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApiA.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        [HttpGet]
        public string GetNow()
        {
            return DateTime.Now.ToString("hh:mm:ss");
        }
    }
}