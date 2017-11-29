using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessagingMicroService.Controllers
{
    [Route("api/[controller]")]
    public class HTMLController : Controller
    {
        [HttpGet]
        public HtmlActionResult Get(string page)
        {
            return new HtmlActionResult("Index",
                new
                {
                    Title = "HTML in Web API",
                    Text = "Now you can easily build any web app with Web API!"
                });
        }
    }
}