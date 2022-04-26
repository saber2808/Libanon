﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon.Models
{
    public class ISBN
    {
        public int Id { get; set; }
       
        public string ISBNCode { get; set; }
        public double RatingScore { get; set; } = 0;
        public int AmoutRating { get; set; } = 0;

        public virtual Book Book { get; set; }
    }
    
}