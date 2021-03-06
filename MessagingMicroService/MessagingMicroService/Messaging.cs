﻿using DomainModel;
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

                if(senderNames != null)
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
                DateSent = DateTime.Now.Date,
                ReceiverUserID = receiverId,
                SenderUserID = userId,
                UserName = user.Name
            };

            return vm;
        }

        public MessageVM GetDetailsVM(int id)
        {
            try
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
            catch (Exception e)
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
                return StatusCodes.Status400BadRequest;
            }
        }
    }
}
