﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessagingMicroService.Model
{
    public class User
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Authorised { get; set; }
        public bool Active { get; set; }
    }
}