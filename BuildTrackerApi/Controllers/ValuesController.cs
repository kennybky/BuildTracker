﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildTrackerApi.Helpers;
using BuildTrackerApi.Models;
using BuildTrackerApi.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildTrackerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return "value";
        }

        [HttpGet("test2")]
        [Authorize(Roles = "ADMIN, DEVELOPER")]
        [AllowNotConfirmed]
        public ActionResult<string> Test2()
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
