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
            //Lấy thông tin các sách của 1 người sở hữu thông qua Email
            var mybook  = (from s in database.Books
                           where s.CurrentUser.Email.ToUpper() == Email.ToUpper()
                           select s).ToList();
            return mybook;
            //return database.Books.Where(s => s.CurrentUser.Email == Email).ToList();
        }
        public IEnumerable<Book> GetMyBorrowerBook(string EmailBorrower)
        {
            //Lấy thông tin các sách đã mượn thông qua email
            var myborrowerbook = (from s in database.Books
                                  where s.EmailBorrower.ToUpper() == EmailBorrower.ToUpper()
                                  select s).ToList();
            return myborrowerbook;
            //return database.Books.Where(s => s.EmailBorrower == EmailBorrower).ToList();
        }
        public bool Booking(Book book)
        {
            
            //database.Entry<Book>(book).State = EntityState.Modified;
            //Lưu thông tin người dùng đặt sách
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
            //Nếu chủ sở hữu chấp nhận đặt sách => thay đổi tình trạng
            var item = database.Books.Find(book.Id);
            item.Status = true;
            database.SaveChanges();
            return true;
        }
        public bool RefuseBooking(Book book)
        {
            //database.Entry<Book>(book).State = EntityState.Modified;
            //Nếu người dùng từ chối sẽ xóa các thông tin và trả sách lại trên kệ
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
            //Xác nhận người chủ sách đã giao sách chưa?
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
            //Xác nhận người mượn đã nhận được sách chưa
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
            //Người dùng trả sách
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
            //Xác nhận người mượn đã giao lại sách chưa
            var item = database.Books.Find(book.Id);
            if (item.WasBorrowed == true)
            {
                item.WasBorrowed = false;
                database.SaveChanges();
            }
            return true;
        }
        public bool ReturnedOwnerBook(Book book)
        {
            //Xác nhận chủ sách đã nhận lại sách chưa
            var item = database.Books.Find(book.Id);
            if (item.WasBorrowed == false)
            {
                item.Status = false;
                item.WasBorrowed = null;
                database.SaveChanges();
            }
            return true;
        }
        public bool UpdateBook(Book book)
        {
            //Cập nhật thông tin sách
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
            //Kích hoạt sách lên kệ
            var item = database.Books.Find(book.Id);
            item.Status = false;
            database.SaveChanges();
            return true;
        }
        public bool ActiveUpdate(Book book)
        {
            //Xác nhận update sách
            var item = database.Books.Find(book.Id);
            item.OTP = book.OTP;
            database.SaveChanges();
            return true;
        }
        public void UpdateBookRating(Book book)
        {
            //Tính điểm đánh giá trung bình cho sách
            Book item = database.Books.Find(book.Id);
            double ratingScore = (book.CurrentISBN.RatingScore 
                + (item.CurrentISBN.RatingScore * item.CurrentISBN.AmoutRating)) / (item.CurrentISBN.AmoutRating + 1);
            item.CurrentISBN.RatingScore = Math.Round(ratingScore, 2);
            item.CurrentISBN.AmoutRating++;
            database.SaveChanges();
        }

        public void RatingSameISBN(Book book)
        {
            //Các sách có ISBN giống nhau sẽ có điểm đánh giá giống nhau
            IEnumerable<Book> lstbooks = database.Books.Where(b => b.CurrentISBN.ISBNCode == book.CurrentISBN.ISBNCode);
            foreach (var item in lstbooks)
            {
                item.CurrentISBN.RatingScore = book.CurrentISBN.RatingScore;
            }
            database.SaveChanges();
        }
        public void SendEmail(string MailTitle, string ToEmail, string MailContent)
        {
            //gửi mail
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
