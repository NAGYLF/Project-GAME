using EphemeralCourage_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EphemeralCourage_API.Controllers
{
    [Route("AdminsController")]
    [ApiController]
    public class AdminsDataController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Admin>> Get()
        {
            using (var context = new DatasDbContext())
            {
                return StatusCode(201, context.Admins.ToList());
            }
        }

    }
}
