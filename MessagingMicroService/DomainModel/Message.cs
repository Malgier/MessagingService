using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DomainModel
{
    public class Message
    {
        [Key]
        public virtual int MessageID { get; set; }
        [DisplayName("Message Title")]
        public virtual String Title { get; set; }
        [DisplayName("Message Content")]
        public virtual String MessageContent { get; set; }
        public virtual DateTime DateSent { get; set; }
        [DisplayName("From")]
        public virtual string SenderUserID { get; set; }
        public virtual string ReceiverUserID { get; set; }
    }
}