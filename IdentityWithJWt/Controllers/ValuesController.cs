using IdentityWithJWt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityWithJWt.Controllers
{ 
   
    [Route("api/[controller]")] 
    [ApiController]
    [Authorize]  
    public class ValuesController : ControllerBase
    {
        private AppIdentityDbContext context;
        public ValuesController (AppIdentityDbContext context)
        {
            this.context = context;
        }

        //GET api/values
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<IEnumerable<string>> Get() 
        {
            return context.Users.Select(x => x.UserName).ToArray();
        }
    }

}
