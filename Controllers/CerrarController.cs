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
            Session.Clear();        // Limpia todas las variables de sesión
            Session.Abandon();      // Finaliza la sesión actual

            return RedirectToAction("Index", "Access");
        }
    }
}