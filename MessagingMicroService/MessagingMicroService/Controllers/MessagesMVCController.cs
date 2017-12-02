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


        public MessagesMVCController(MessageContext context)
        {
            _context = context;
            messageCont = new MessagesController(_context);
            client = new Client();

        }

        // GET: MessagesMVC/MyMesages/5
        [HttpGet]
        [Route("MyMessages/{id}")]
        public IActionResult MyMessages(int id)
        {
            IActionResult HttpResult = messageCont.GetByUser(id);

            if (HttpResult is OkObjectResult)
            {
                var result = HttpResult as OkObjectResult;
                IEnumerable<Message> content = result.Value as IEnumerable<Message>;
                var myMessages = content.Where(x => x.ReceiverUserID == id).ToList();

                Client client = new Client();
                User user = client.GetUser("http://localhost:57520/", "api/User/" + id);
                List<User> senderNames = client.GetUsers("http://localhost:57520/", "api/User");
                List<string> senderFullNames = new List<string>();

                foreach (User senderUser in senderNames)
                {
                    senderFullNames.Add(senderUser.Name);
                }

                MyMessageVM vm = new MyMessageVM()
                {
                    MyMessages = myMessages,
                    ReveiverName = user.Name,
                    SenderNames = senderFullNames
                };
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("Send/{receiverId}")]
        [HttpGet]
        // GET: MessagesMVC/Send
        public IActionResult Send(int receiverId = 1)
        {
            User user = client.GetUser("http://localhost:57520/", "api/User/" + receiverId);

            int userID = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            MessageVM vm = new MessageVM()
            {
                DateSent = DateTime.Now,
                ReceiverUserID = receiverId,
                SenderUserID = userID,
                UserName = user.Name
            };

            return View(vm);
        }

        [Route("SaveMessage")]
        [HttpPost]
        // POST: MessagesMVC/Send
        public IActionResult SaveMessage([FromBody]MessageVM vm)
        {
            if (ModelState.IsValid)
            {
                Message message = new Message()
                {
                    Title = vm.Title,
                    MessageContent = vm.MessageContent,
                    DateSent = vm.DateSent,
                    SenderUserID = vm.SenderUserID,
                    ReceiverUserID = vm.ReceiverUserID
                };

                messageCont.SaveMessage(message);
                return RedirectToAction(nameof(MyMessages));
            }
            return View(vm);
        }

        [Route("Details/{id}")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            IActionResult HttpResult = messageCont.Get(id);

            if (HttpResult is OkObjectResult)
            {
                var result = HttpResult as OkObjectResult;
                Message content = result.Value as Message;
                User user = client.GetUser("http://localhost:57520/", "api/User/" + content.SenderUserID);
                MessageVM vm = new MessageVM()
                {
                    DateSent = content.DateSent,
                    MessageContent = content.MessageContent,
                    Title = content.Title,
                    UserName = user.Name

                };

                return View(vm);
            }
            else
            {
                return NotFound();
            }
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
