using DomainModel;
using MessagingMicroService.Controllers;
using MessagingMicroService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MessagingMicroService
{
    public class Messaging
    {
        private readonly MessageContext _context;
        private MessagesController messageCont;
        private Client client;

        public Messaging(MessageContext context)
        {
            _context = context;
            messageCont = new MessagesController(_context);
            client = new Client();
        }

        public MyMessageVM GetMyMessageVM(string userId)
        {
            IActionResult HttpResult = messageCont.GetByUser(userId);

            if (HttpResult is OkObjectResult)
            {
                var result = HttpResult as OkObjectResult;
                IEnumerable<Message> content = result.Value as IEnumerable<Message>;

                Client client = new Client();
                User user = client.GetUser("http://localhost:51520/", "api/User/" + userId);
                List<User> senderNames = client.GetUsers("http://localhost:51520/", "api/User");
                List<string> senderFullNames = new List<string>();

                foreach (User senderUser in senderNames)
                {
                    senderFullNames.Add(senderUser.Name);
                }

                MyMessageVM vm = new MyMessageVM()
                {
                    MyMessages = content.ToList(),
                    ReveiverName = user.Name,
                    SenderNames = senderFullNames
                };
                return vm;
            }
            else
            {
                return new MyMessageVM();
            }
        }

        public MessageVM GetSendVM(string receiverId, string userId)
        {
            User user = client.GetUser("http://localhost:51520/", "api/User/" + receiverId);

            MessageVM vm = new MessageVM()
            {
                DateSent = DateTime.Now,
                ReceiverUserID = receiverId,
                SenderUserID = userId,
                UserName = user.Name
            };

            return vm;
        }

        public MessageVM GetDetailsVM(int id)
        {
            IActionResult HttpResult = messageCont.Get(id);

            if (HttpResult is OkObjectResult)
            {
                var result = HttpResult as OkObjectResult;
                Message content = result.Value as Message;
                User user = client.GetUser("http://localhost:51520/", "api/User/" + content.SenderUserID);
                MessageVM vm = new MessageVM()
                {
                    DateSent = content.DateSent,
                    MessageContent = content.MessageContent,
                    Title = content.Title,
                    UserName = user.Name

                };

                return vm;
            }
            else
            {
                return new MessageVM();
            }
        }

        public int MVCSend(MessageVM vm)
        {
            try
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
                return StatusCodes.Status200OK;
            }
            catch (Exception e)
            {
                return StatusCodes.Status404NotFound;
            }
        }

        public int ApiSaveMessage(Message message)
        {
            if (message == null)
            {
                return StatusCodes.Status404NotFound;
            }

            _context.Messages.Add(message);
            _context.SaveChanges();

            return StatusCodes.Status200OK;
        }

        public int ApiPut(int id, Message message)
        {
            if (message == null || message.MessageID != id)
            {
                return StatusCodes.Status400BadRequest;
            }

            var mess = _context.Messages.SingleOrDefault(x => x.MessageID == id);
            if (mess == null)
            {
                return StatusCodes.Status404NotFound;
            }

            mess.Title = message.Title;
            mess.MessageContent = message.MessageContent;
            mess.DateSent = message.DateSent;

            _context.Messages.Update(mess);
            _context.SaveChanges();
            return StatusCodes.Status200OK;
        }

        public int ApiDelete(int id)
        {
            var message = _context.Messages.SingleOrDefault(x => x.MessageID == id);
            if (message == null)
            {
                return StatusCodes.Status404NotFound;
            }

            _context.Messages.Remove(message);
            _context.SaveChanges();
            return StatusCodes.Status200OK;
        }
    }
}
