using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingMicroService.Model
{
    public class MessageVM
    {
        public int MessageID { get; set; }
        public String Title { get; set; }
        public String MessageContent { get; set; }
        public DateTime DateSent { get; set; }
        public string SenderUserID { get; set; }
        public string ReceiverUserID { get; set; }

        public string UserName { get; set; }
    }
}
