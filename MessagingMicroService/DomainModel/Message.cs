using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }
        [DisplayName("Message Title")]
        public String Title { get; set; }
        [DisplayName("Message Content")]
        public String MessageContent { get; set; }
        public DateTime DateSent { get; set; }
        [DisplayName("From")]
        public int SenderUserID { get; set; }
        public int ReceiverUserID { get; set; }
    }
}