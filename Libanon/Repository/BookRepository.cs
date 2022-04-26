using Libanon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Libanon.Repository
{
    public class BookRepository : IBookRepository
    {
        BookContext database = new BookContext();
        readonly IBookRepository _bookRepository;

        public BookRepository(BookContext bookContext)
        {
            this.database = bookContext;
        }
        public IEnumerable<Book> GetAll()
        {
            var mybook = (from s in database.Books
                          where s.Status != null && s.WasBorrowed == null
                          select s).ToList();
            return mybook;
        }
        public bool Add(Book book)
        {
            if(book == null)
            {
                throw new NotImplementedException();
            }
            book.Status = null;
            database.Books.Add(book);
            database.SaveChanges();
            return true;
        }
        public Book GetBookById(int Id)
        {
            return database.Books.Find(Id);
        }
        public IEnumerable<Book> GetMyBook(string Email)
        {
            var mybook  = (from s in database.Books
                           where s.CurrentUser.Email.ToUpper() == Email.ToUpper()
                           select s).ToList();
            return mybook;
            //return database.Books.Where(s => s.CurrentUser.Email == Email).ToList();
        }
        public IEnumerable<Book> GetMyBorrowerBook(string EmailBorrower)
        {
            var myborrowerbook = (from s in database.Books
                                  where s.EmailBorrower.ToUpper() == EmailBorrower.ToUpper()
                                  select s).ToList();
            return myborrowerbook;
            //return database.Books.Where(s => s.EmailBorrower == EmailBorrower).ToList();
        }
        public bool Booking(Book book)
        {
            //database.Entry<Book>(book).State = EntityState.Modified;
            var item = database.Books.Find(book.Id);
            item.EmailBorrower = book.EmailBorrower;
            item.NameBorrower = book.NameBorrower;
            item.PhoneBorrower = book.PhoneBorrower;
            item.DescBorrower = book.DescBorrower;
            database.SaveChanges();
            return true;
        }
        public bool AcceptBooking(Book book)
        {
            //database.Entry<Book>(book).State = EntityState.Modified;
            var item = database.Books.Find(book.Id);
            item.Status = true;
            database.SaveChanges();
            return true;
        }
        public bool RefuseBooking(Book book)
        {
            //database.Entry<Book>(book).State = EntityState.Modified;
            var item = database.Books.Find(book.Id);
            item.EmailBorrower = null;
            item.NameBorrower = null;
            item.PhoneBorrower = null;
            item.DescBorrower = null;
            item.Status = false;
            database.SaveChanges();
            return true;
        }
        public bool ReceivedBook(Book book)
        {
            var item = database.Books.Find(book.Id);
            if(item.WasBorrowed == null)
            {
                item.WasBorrowed = false;
                database.SaveChanges();
            }
            return true;
        }
        public bool ReceivedBorrownerBook(Book book)
        {
            var item = database.Books.Find(book.Id);
            if (item.WasBorrowed == false)
            {
                item.WasBorrowed = true;
                database.SaveChanges();
            }
            return true;
        }
        public bool ReturnBook(Book book)
        {
            var item = database.Books.Find(book.Id);
            item.Status = false;
            item.EmailBorrower = null;
            item.NameBorrower = null;
            item.PhoneBorrower = null;
            item.DescBorrower = null;
            database.SaveChanges();
            return true;
        }
        public bool ReturnedBook(Book book)
        {
            var item = database.Books.Find(book.Id);
            if (item.WasBorrowed == true)
            {
                item.WasBorrowed = false;
                database.SaveChanges();
            }
            else if (item.WasBorrowed == false)
            {
                item.Status = false;
                item.WasBorrowed = null;
                database.SaveChanges();
            }
            return true;
        }
        public bool UpdateBook(Book book)
        {
            var item = database.Books.Find(book.Id);
            item.Title = book.Title;
            item.Authors = book.Authors;
            item.ImageUrl = book.ImageUrl;
            item.Status = book.Status;
            item.Category = book.Category;
            item.CurrentISBN.ISBNCode = book.CurrentISBN.ISBNCode;
            item.CurrentUser.FullName = book.CurrentUser.FullName;
            item.CurrentUser.Email = book.CurrentUser.Email;
            database.SaveChanges();
            return true;
        }
        public bool Active(Book book)
        {
            var item = database.Books.Find(book.Id);
            item.Status = false;
            database.SaveChanges();
            return true;
        }
        public bool ActiveUpdate(Book book)
        {
            var item = database.Books.Find(book.Id);
            database.SaveChanges();
            return true;
        }
        public void UpdateBookRating(Book book)
        {
            Book item = database.Books.Find(book.Id);
            double ratingScore = (book.CurrentISBN.RatingScore 
                + (item.CurrentISBN.RatingScore * item.CurrentISBN.AmoutRating)) / (item.CurrentISBN.AmoutRating + 1);
            item.CurrentISBN.RatingScore = Math.Round(ratingScore, 2);
            item.CurrentISBN.AmoutRating++;
            database.SaveChanges();
        }

        public void RatingSameISBN(Book book)
        {
            IEnumerable<Book> lstbooks = database.Books.Where(b => b.CurrentISBN.ISBNCode == book.CurrentISBN.ISBNCode);
            foreach (var item in lstbooks)
            {
                item.CurrentISBN.RatingScore = book.CurrentISBN.RatingScore;
            }
            database.SaveChanges();
        }
        public void SendEmail(string MailTitle, string ToEmail, string MailContent)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(ToEmail);
            mail.From = new MailAddress(ToEmail);
            mail.Subject = MailTitle;
            mail.Body = MailContent;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("devtest.libanon@gmail.com", "Libanon@123");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
