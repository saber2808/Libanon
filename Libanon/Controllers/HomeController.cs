using Libanon.Models;
using Libanon.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Libanon.Controllers
{
    public class HomeController : Controller
    {
        BookContext database = new BookContext();
        readonly IBookRepository _bookRepository;

        public HomeController(IBookRepository bookRepository)
        {
            this._bookRepository = bookRepository;
        }
        public ActionResult Index()
        {
            var book = _bookRepository.GetAll().ToList();
            //ViewBag.Message = msg.Length;
            return View(book);
        }
        public ActionResult MyBook(FormCollection collection)
        {
            string Email = collection.Get("MyBook").ToString();
            string EmailBorrower = collection.Get("MyBorrower").ToString();
            if(Email != "" && EmailBorrower == "")
            {
                return View(_bookRepository.GetMyBook(Email));
            }    
            else if(Email == "" && EmailBorrower != "")
            {
                return View(_bookRepository.GetMyBorrowerBook(EmailBorrower));
            }
            return View(_bookRepository.GetAll());
        }
       
        public ActionResult Create()
        {
            return View(new Book());
        }
        [HttpPost]
        public ActionResult Create(Book book, HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string _filename = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/Images"), _filename);
                    file.SaveAs(_path);
                    book.ImageUrl = _filename;
                }
            }
            if (_bookRepository.Add(book))
            {
                //Gửi mail chứa link xác nhận
                string title = "Xác thực thêm sách mới";
                string mailbody = "Xin chào " + book.CurrentUser.FullName;
                mailbody += "<br /><br />Xin hãy click vào đường dẫn URL để kích hoạt sách lên hàng chờ";
                mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ActiveCreate/{book.Id}") + "'>Click here to activate your book.</a>";
                _bookRepository.SendEmail(title, book.CurrentUser.Email, mailbody);
                TempData["Status"] = "gửi yêu thêm mới sách thành công";
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("Create");

        }
        public ActionResult Booking(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            if(book.EmailBorrower != null)
            {
                TempData["Status"] = "Sách đã có người đặt";
                return RedirectToAction("Index");
            }
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Booking(Book book, int Id)
        {
            if(_bookRepository.GetBookById(Id).EmailBorrower == null)
            {
                _bookRepository.Booking(book);
                //Gửi mail xác nhận đặt sách
                string title = "Xác thực yêu cầu mượn sách";
                string mailbody = "Xin chào " + book.NameBorrower;
                mailbody += "<br /><br />Xin hãy click vào đường dẫn URL để gửi yêu cầu mượn sách";
                mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ActiveBooking/{book.Id}") + "'>Click vào đây để xác nhận yêu cầu mượn sách</a>";
                _bookRepository.SendEmail(title, book.EmailBorrower, mailbody);
                TempData["Status"] = "gửi yêu mượn sách thành công";
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Index");
        }
        public ActionResult ActiveBooking(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            string title = "Xác nhận yêu cầu mượn sách";
            string mailbody = "Xin chào " + book.CurrentUser.FullName;
            mailbody += "<br /><br />Xin hãy click vào đường dẫn URL để chấp nhận hoặc từ chối yêu cầu mượn sách";
            mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/AcceptBooking/{book.Id}") + "'>Click vào đây để chấp nhận yêu cầu mượn sách.</a>";
            mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/RefuseBooking/{book.Id}") + "'>Click vào đây để từ chối yêu cầu mượn sách.</a>";
            _bookRepository.SendEmail(title, book.CurrentUser.Email, mailbody);
            return View(book);
        }
        public ActionResult AcceptBooking(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.AcceptBooking(book);
            //gửi mail cho người mượn
            string title = "Chấp nhận yêu cầu mượn sách";
            string mailbody = "Xin chào " + book.NameBorrower;
            mailbody += "<br /><br />Yêu cầu mượn sách của bạn đã được chấp nhận!";
            mailbody += "<br /><br />Bạn đã yêu cầu mượn thành công sách " + book.Title;
            mailbody += "<br /><br />Nếu đã nhận được sách bạn vui lòng click vào đường link để xác nhận đã nhận được sách ";
            mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ReceivedBorrownerBook/{book.Id}") + "'>Click vào đây nếu bạn đã nhận được sách.</a>";
            _bookRepository.SendEmail(title, book.EmailBorrower, mailbody);
            //gửi mail cho chủ sách
            string title2 = "Xác nhận yêu cầu mượn sách";
            string mailbody2 = "Xin chào " + book.CurrentUser.FullName;
            mailbody2 += "<br /><br />Bạn đã đồng ý yêu cầu mượn sách của " + book.NameBorrower;
            mailbody2 += "<br /><br />Tên sách cho mượn là: " + book.Title;
            mailbody2 += "<br /><br />Nếu bạn đã trao sách đi xin vui lòng click vào đường link để xác nhận ";
            mailbody2 += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ReceivedBook/{book.Id}") + "'>Click vào đây nếu bạn đã trao sách đến người mượn.</a>";
            _bookRepository.SendEmail(title2, book.CurrentUser.Email, mailbody2);
            return View(book);
        }
        public ActionResult ReceivedBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.ReceivedBook(book);
            return View(book);
        }
        public ActionResult ReceivedBorrownerBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            if(book.WasBorrowed != false)
            {
                return View("Chủ sách chưa giao sách!");
            }
            _bookRepository.ReceivedBorrownerBook(book);
            return View(book);
        }
        public ActionResult RefuseBooking(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.RefuseBooking(book);
            string title = "Từ chối yêu cầu mượn sách";
            string mailbody = "Xin chào " + book.NameBorrower;
            mailbody += "<br /><br />Yêu cầu mượn sách " + book.Title + "đã bị từ chối!";
            mailbody += "<br /><br />Vui lòng chọn mượn sách khác!";
            _bookRepository.SendEmail(title, book.EmailBorrower, mailbody);
            string title2 = "Từ chối yêu cầu mượn sách";
            string mailbody2 = "Xin chào " + book.CurrentUser.FullName;
            mailbody2 += "<br /><br />bạn đã từ chối yêu cầu mượn sách " + book.Title;
            mailbody2 += "<br /><br />Từ người dùng" + book.NameBorrower + "Với Email " + book.EmailBorrower;
            _bookRepository.SendEmail(title2, book.CurrentUser.Email, mailbody2);
            return View(book);
        }
        public ActionResult ReturnBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            return View(book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReturnBook(FormCollection collection, int Id)
        {
            string EmailOwner = collection.Get("EmailOwner").ToString();
            string PhoneNumberOwner = collection.Get("PhoneNumberOwner").ToString();
            string FullNameOwner = collection.Get("FullNameOwner").ToString();
            string DescOwner = collection.Get("DescOwner").ToString();
            Book book = _bookRepository.GetBookById(Id);
            if (EmailOwner != book.EmailBorrower || PhoneNumberOwner != book.PhoneBorrower)
            {
                TempData["Status"] = "Email hoặc số điện thoại không đúng!";
                return View("ReturnBook");
            }
            string title = "Xác nhận yêu cầu trả sách";
            string mailbody = "Xin chào " + book.NameBorrower;
            mailbody += "<br /><br />bạn đã trả sách " + book.Title;
            mailbody += "<br /><br />với lý do " + DescOwner;
            mailbody += "<br /><br />bạn vui lòng click vào đường link để xác nhận yêu cầu trả sách ";
            mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ConfirmReturnBook/{book.Id}") + "'>Click vào đây nếu bạn xác nhận trả sách.</a>";
            _bookRepository.SendEmail(title, book.EmailBorrower, mailbody);
            return RedirectToAction("Index");
        }
        public ActionResult ConfirmReturnBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            string title = "Xác nhận yêu cầu trả sách";
            string mailbody = "Xin chào " + book.NameBorrower;
            mailbody += "<br /><br />Yêu cầu trả sách của bạn đã được gửi về hệ thống ";
            mailbody += "<br /><br />Dưới đây là thông tin người sở hữu sách vui lòng liên hệ để trả sách" + book.CurrentUser.FullName + ", Email " + book.CurrentUser.Email;
            mailbody += "<br /><br />Nếu bạn đã giao sách thành công vui lòng click vào đường link dưới đây";
            mailbody += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ReturnedBook/{book.Id}") + "'>Click vào đây nếu bạn đã trả sách.</a>";
            _bookRepository.SendEmail(title, book.EmailBorrower, mailbody);
            string title2 = "Xác nhận yêu cầu trả sách";
            string mailbody2 = "Xin chào " + book.CurrentUser.FullName;
            mailbody2 += "<br /><br />Sách" + book.Title + "của bạn sẽ được trả lại bởi " + book.NameBorrower;
            mailbody2 += "<br /><br />Vui lòng liên hệ email " + book.EmailBorrower + "để thực hiện việc nhận sách";
            mailbody2 += "<br /><br />Nếu bạn đã nhận lại sách thành công vui lòng click vào đường link dưới đây";
            mailbody2 += "<br /><a href = '" + string.Format($"{Request.Url.Scheme}://{Request.Url.Authority}/Home/ReturnedOwnerBook/{book.Id}") + "'>Click vào đây nếu bạn đã nhận được sách.</a>";
            _bookRepository.SendEmail(title2, book.CurrentUser.Email, mailbody2);
            return View(book);
        }
        public ActionResult ReturnedBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            if(book.WasBorrowed != false)
            {
                return View("Người mượn chưa trả sách!");
            }
            _bookRepository.ReturnedBook(book);
            return View(book);
        }
        public ActionResult ReturnedOwnerBook(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.ReturnedOwnerBook(book);
            return View(book);
        }
        public ActionResult ActiveCreate(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.Active(book);
            TempData["Status"] = "Đã kích hoạt sách thành công";
            return View(book);
        }
        public ActionResult Edit(int Id)
        {
            //Khi vừa load form hệ thống sẽ tạo random 1 mã OTP để gửi về mail cho người dùng
            var book = _bookRepository.GetBookById(Id);
            Random rnd = new Random();
            int num = rnd.Next(100000, 999999);
            book.OTP = num.ToString();
            _bookRepository.ActiveUpdate(book);
            string title = "Xác thực thay đổi thông tin sách";
            string mailbody = "Xin chào " + book.CurrentUser.FullName;
            mailbody += "<br /><br />Nhập mã OTP để thay đổi thông tin sản phẩm";
            mailbody += "<br />" + num;
            _bookRepository.SendEmail(title, book.CurrentUser.Email, mailbody);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int Id, Book book, HttpPostedFileBase file, FormCollection collection)
        {
            string oldImgBook = _bookRepository.GetBookById(Id).ImageUrl;
            book.ImageUrl = oldImgBook;
            if (file != null)
            {
                if (file.ContentLength > 0)
                {
                    string _filename = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/Images"), _filename);
                    file.SaveAs(_path);
                    book.ImageUrl = _filename;
                }
            }
            //lấy mã OTP người dùng nhập để so sánh với OTP update
            string inputotp = collection.Get("InputOTP").ToString();
            if (inputotp == _bookRepository.GetBookById(Id).OTP)
            {

                _bookRepository.UpdateBook(book);
                TempData["Status"] = "Đã xác thực thay đổi thông tin sách thành công";
                
            }
            else if(inputotp != _bookRepository.GetBookById(Id).OTP)
            {
                TempData["Error"] = "Mã OTP không chính xác, vui lòng kiểm tra Email để nhận mã OTP mới!";
                return RedirectToAction("Edit");
            }
            return RedirectToAction("Index");
        }
        public ActionResult ActiveUpdate(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            _bookRepository.ActiveUpdate(book);
            return View(book);
        }
        public ActionResult Rating(int Id)
        {
            Book book = _bookRepository.GetBookById(Id);
            return View(book);
        }
        [HttpPost]
        public ActionResult Rating(Book book)
        {
            _bookRepository.UpdateBookRating(book);
            _bookRepository.RatingSameISBN(book);
            TempData["Status"] = "Đánh giá thành công!";
            return RedirectToAction("Index");
        }
    }
}