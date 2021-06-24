﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookkeeping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookkeepingController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }
    }
}
