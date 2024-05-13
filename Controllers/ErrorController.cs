using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WildlifeMVC.Controllers
{
    public class ErrorController : Controller
    {
        //handle the the custom error views

        public ActionResult GenericError()
        {
            return View();
        }

        public ActionResult ErrorCode400()
        {
            Response.StatusCode = 400;
            return View();
        }

        public ActionResult ErrorCode404()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}