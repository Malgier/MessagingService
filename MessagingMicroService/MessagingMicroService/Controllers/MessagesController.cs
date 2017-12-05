using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DomainModel;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Authorization;

namespace MessagingMicroService.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private MessageContext context;
        private Messaging messaging;

        public MessagesController(MessageContext cxt)
        {
            context = cxt;
            messaging = new Messaging(context);
            //cxt.Database.EnsureCreated();
        }
        // GET api/messages
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Message>))]
        public IActionResult Get()
        {
            return Ok(context.Messages);
        }

        // GET api/messages/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(context.Messages.SingleOrDefault(x => x.MessageID == id));
        }

        // GET api/messages/5
        [HttpGet("{userId}")]
        public IActionResult GetByUser(string userId)
        {
            return Ok(context.Messages.Where(x => x.ReceiverUserID == userId));
        }

        // POST api/messages
        [Route("SaveMessage")]
        [HttpPost]
        public IActionResult SaveMessage([FromBody]Message message)
        {
            return StatusCode(messaging.ApiSaveMessage(message));
        }

        // PUT api/messages/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Message message)
        {
            return StatusCode(messaging.ApiPut(id, message));
        }

        // DELETE api/messages/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var message = context.Messages.SingleOrDefault(x => x.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            context.Messages.Remove(message);
            context.SaveChanges();
            return new OkResult();
        }
    }
}
