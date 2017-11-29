using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingMicroService.Model
{
    public class MyMessageVM
    {
        public List<Message> MyMessages { get; set; }
        public string ReveiverName { get; set; }
        public List<string> SenderNames { get; set; }
    }
}
