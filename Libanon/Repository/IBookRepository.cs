using Libanon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libanon.Repository
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();
        bool Add(Book book);
        Book GetBookById(int Id);
        IEnumerable<Book> GetMyBook(string Email);
        IEnumerable<Book> GetMyBorrowerBook(string EmailBorrower);
        bool Booking(Book book);
        bool AcceptBooking(Book book);
        bool RefuseBooking(Book book);
        bool ReceivedBook(Book book);
        bool ReceivedBorrownerBook(Book book);
        void UpdateBookRating(Book book);
        void RatingSameISBN(Book book);
        bool ReturnBook(Book book);
        bool ReturnedBook(Book book);
        bool UpdateBook(Book book);
        bool ActiveUpdate(Book book);
        bool Active(Book book);
        void SendEmail(string Email, string ToEmail, string MailContent);
    }
}
