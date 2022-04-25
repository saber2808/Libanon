﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public virtual Book Book { get; set; }
    }
}