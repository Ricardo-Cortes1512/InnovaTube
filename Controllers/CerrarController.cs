using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PruebaCrudMVC.Controllers
{
    public class CerrarController : Controller
    {
        public ActionResult CerrarSesion()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Access");
        }
    }
}