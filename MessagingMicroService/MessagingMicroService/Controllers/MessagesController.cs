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

        public MessagesController(MessageContext cxt)
        {
            context = cxt;
            cxt.Database.EnsureCreated();
        }
        // GET api/messages
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Message>))]
        public IActionResult Get()
        {
            var messages = context.Messages;

            return Ok(messages);
        }

        // GET api/messages/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var message = context.Messages.SingleOrDefault(x => x.MessageID == id);
            return Ok(message);
        }

        // GET api/messages/5
        [HttpGet("{userId}")]
        public IActionResult GetByUser(string userId)
        {
            var message = context.Messages.Where(x => x.ReceiverUserID == userId);
            return Ok(message);
        }

        // POST api/messages
        [Route("SaveMessage")]
        [HttpPost]
        public IActionResult SaveMessage([FromBody]Message message)
        {
            if (message == null)
            {
                return BadRequest();
            }

            context.Messages.Add(message);
            context.SaveChanges();

            return CreatedAtRoute("", new { id = message.MessageID }, message);
        }

        // PUT api/messages/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Message message)
        {
            if (message == null || message.MessageID != id)
            {
                return BadRequest();
            }

            var mess = context.Messages.SingleOrDefault(x => x.MessageID == id);
            if (mess == null)
            {
                return NotFound();
            }

            mess.Title = message.Title;
            mess.MessageContent = message.MessageContent;
            mess.DateSent = message.DateSent;

            context.Messages.Update(mess);
            context.SaveChanges();
            return new NoContentResult();
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
            return new NoContentResult();
        }
    }
}
