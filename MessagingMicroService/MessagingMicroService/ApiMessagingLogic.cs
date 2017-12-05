using DomainModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingMicroService
{
    public class ApiMessagingLogic
    {
        private readonly MessageContext _context;

        public ApiMessagingLogic(MessageContext context)
        {
            _context = context;
        }

        public int ApiSaveMessage(Message message)
        {
            try
            {
                if (message == null)
                {
                    return StatusCodes.Status404NotFound;
                }

                _context.Messages.Add(message);
                _context.SaveChanges();

                return StatusCodes.Status200OK;
            }
            catch (Exception e)
            {
                return StatusCodes.Status400BadRequest;
            }
        }

        public int ApiPut(int id, Message message)
        {
            try
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
            catch (Exception e)
            {
                return StatusCodes.Status400BadRequest;
            }
        }

        public int ApiDelete(int id)
        {
            try
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
            catch (Exception e)
            {
                return StatusCodes.Status400BadRequest;
            }
        }
    }
}
