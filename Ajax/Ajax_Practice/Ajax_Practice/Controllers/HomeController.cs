using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax_Practice.Models;
using System.Web.Helpers;
using System.Threading;
using System.IO;

namespace Ajax_Practice.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarsEntities db = new CarsEntities();
        public ActionResult Index()
        {
            return View(db.Markas.ToList());
        }

        public JsonResult ModelsJson(int id)
        {
            Marka marka = db.Markas.Find(id);
            if (marka == null)
            {
                return Json(new
                {
                    status = 404
                }, JsonRequestBehavior.AllowGet);
            }

            var response = marka.Models.Select(m => new
            {
                m.Id,
                m.Name
            }).ToList();

            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Usercheck(string Email)
        {
            if (db.Users.FirstOrDefault(u=>u.Email== Email)==null)
            {
                return Json(new
                {
                    valid = true,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                valid = false,
                message = "Bu e-poçt ünvanı artıq istifadə edilmişdir"
            }, JsonRequestBehavior.AllowGet);

        }

        string filename;

        public JsonResult Register(User user, List<HttpPostedFileBase> Photos)
        {
            if (string.IsNullOrEmpty(user.Fullname) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email) || Photos == null)
            {
                return Json(new
                {
                    status = 403,
                    message= "Ad Soyad,E-poçt,Şifrə və ya Şəkil boş ola bilməz"
                }, JsonRequestBehavior.AllowGet);
            }

            if (db.Users.FirstOrDefault(u=>u.Email==user.Email)!=null)
            {
                return Json(new
                {
                    status=401,
                    message = "Bu e-poçt ünvanı artıq istifadə edilmişdir"
                }, JsonRequestBehavior.AllowGet);
            }

            foreach (var photos in Photos)
            {
                if ((photos.ContentLength / 1024) / 1024 > 1)
                {
                    return Json(new
                    {
                        status = 406,
                        message = "You can not upload images larger than 1M"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (photos.ContentType != "image/jpeg" && photos.ContentType != "image/png" && photos.ContentType != "image/gif")
                {
                    return Json(new
                    {
                        status = 407,
                        message = "You can only upload images"
                    }, JsonRequestBehavior.AllowGet);
                }

                filename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + photos.FileName;

                string createFile = Path.Combine(Server.MapPath("~/Uploads"), filename);

                photos.SaveAs(createFile);
               
            }
            user.Photo = filename;



            user.Password = Crypto.HashPassword(user.Password);
            db.Users.Add(user);
            db.SaveChanges();

            Thread.Sleep(3000);

            return Json(new
            {
                status=200,
                message="Siz artiq qeydiyyatdan keçdiniz"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}