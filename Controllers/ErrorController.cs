using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WildlifeMVC.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult GenericError()
        {
            ViewBag.ErrorDetails = "Generic";
            return View();
        }

        public ActionResult ErrorCode400()
        {
            ViewBag.ErrorDetails = "400";
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult ErrorCode404()
        {
            ViewBag.ErrorDetails = "404";
            Response.StatusCode = 404;
            return View();
        }
    }
}