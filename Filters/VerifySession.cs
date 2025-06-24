using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PruebaCrudMVC.Controllers;
using PruebaCrudMVC.Models;

namespace PruebaCrudMVC.Filters
{
    public class VerifySession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Ouser = (Users)HttpContext.Current.Session["User"];

            if (Ouser == null)
            {
                if (filterContext.Controller is AccessController == false)
                {
                    filterContext.HttpContext.Response.Redirect("~/Access /Index");
                }
            }
            else
            {
                if (filterContext.Controller is AccessController == true)
                {
                    filterContext.HttpContext.Response.Redirect("~/Clientes/Index");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}