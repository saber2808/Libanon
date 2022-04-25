using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon.Models
{
    public class Book
    {
        public int Id { get; set; }
      
        public string Title { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string Authors { get; set; }
        public string Publisher { get; set; }
        
        public string Category { get; set; }

        public bool? Status { get; set; }
        public bool? WasBorrowed { get; set; }
        public string OTP { get; set; }
        public string EmailBorrower { get; set; }
        public string NameBorrower { get; set; }
        public string PhoneBorrower { get; set; }
        public string DescBorrower { get; set; }


        public virtual User CurrentUser { get; set; }
        public virtual ISBN CurrentISBN { get; set; }
    }
}