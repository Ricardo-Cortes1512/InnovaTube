using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PruebaCrudMVC.Models;
using PruebaCrudMVC.Models.ViewModels;

namespace PruebaCrudMVC.Controllers
{
    public class AccessController : Controller
    {
        // GET: Access
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Enter(string user, string password)
        {
            try
            {
                using (PruebasCrudMVCEntities db= new PruebasCrudMVCEntities())
                {
                    var lst = from d in db.Users
                              where d.Nombre == user || d.Usuario == user && d.Contraseña == password
                              select d;
                    if (lst.Count()>0)
                    {
                        Session["User"] = lst.First();
                        Session["NombreUsuario"] = user;
                        return Content("1");
                    }
                    else
                    {
                        return Content("Usuario invalido");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Ocurrio un error en el inicio de sesion" + ex.Message);
            }
        }

        public ActionResult RecuperarContraseña(EditUsersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new PruebasCrudMVCEntities())
            {
                var oUser = db.Users.Find(model.ID);
                oUser.Contraseña = model.Contraseña;

                db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect(Url.Content("~/Access/Index"));
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Add(UsersViewModel model)
        {
            var captchaResponse = Request.Form["g-recaptcha-response"];
            var isCaptchaValid = await ValidarCaptcha(captchaResponse);
            if (!isCaptchaValid)
            {
                ModelState.AddModelError("", "Por favor verifica que no eres un robot.");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new PruebasCrudMVCEntities())
            {
                //var clientes = db.tbl_Clientes.ToList();
                //ViewBag.Clientes = new SelectList(clientes, "id", "Cliente");
                Users oUser = new Users();
                oUser.Nombre = model.Nombre;
                oUser.Apellido = model.Apellido;
                oUser.Usuario = model.Usuario;
                oUser.Contraseña = model.Contraseña;

                db.Users.Add(oUser);

                db.SaveChanges();

            }
            return Redirect(Url.Content("~/Users/"));
        }

        public async Task<bool> ValidarCaptcha(string response)
        {
            var secretKey = "6LdqJ2wrAAAAAAjpAA-IzHMtx_jU2ysoMqdM-l5s";
            var httpClient = new HttpClient();

            var values = new Dictionary<string, string>{
                { "secret", secretKey },
                { "response", response }
                                                        };

            var content = new FormUrlEncodedContent(values);
            var verificationResponse = await httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var verificationResult = await verificationResponse.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(verificationResult);
            return result.success == "true";
        }
    }
}