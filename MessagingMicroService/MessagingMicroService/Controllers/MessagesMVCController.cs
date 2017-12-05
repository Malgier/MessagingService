using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DomainModel;
using System.Security.Claims;
using MessagingMicroService.Model;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace MessagingMicroService.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MessagesMVCController : Controller
    {
        private readonly MessageContext _context;
        private MessagesController messageCont;
        private Client client;
        private Messaging messaging;

        public MessagesMVCController(MessageContext context)
        {
            _context = context;
            messageCont = new MessagesController(_context);
            client = new Client();
            messaging = new Messaging(_context);
        }

        // GET: MessagesMVC/MyMesages/5
        [HttpGet]
        [Route("MyMessages/{userId}")]
        public IActionResult MyMessages(string userId)
        {
            return View(messaging.GetMyMessageVM(userId));
        }

        [Route("Send/{receiverId}")]
        [HttpGet]
        // GET: MessagesMVC/Send
        public IActionResult Send(string receiverId = "")
        {
            return View(messaging.GetSendVM(receiverId, User.FindFirst(ClaimTypes.NameIdentifier).Value));
        }

        [Route("SaveMessage")]
        [HttpPost]
        // POST: MessagesMVC/Send
        public IActionResult SaveMessage([FromBody]MessageVM vm)
        {
            if (ModelState.IsValid)
            {
                return StatusCode(messaging.MVCSend(vm));
            }
            return View(vm);
        }

        [Route("Details/{id}")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            return View(messaging.GetDetailsVM(id));
        }

        // GET: MessagesMVC/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .SingleOrDefaultAsync(m => m.MessageID == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: MessagesMVC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.SingleOrDefaultAsync(m => m.MessageID == id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyMessages));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageID == id);
        }
    }
}
