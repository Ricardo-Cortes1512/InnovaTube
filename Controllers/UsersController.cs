using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PruebaCrudMVC.Models;
using PruebaCrudMVC.Models.TableViewModels;
using PruebaCrudMVC.Models.ViewModels;

namespace PruebaCrudMVC.Controllers
{
    public class UsersController : Controller
    {

        // GET: Clientes
        public ActionResult Index()
        {
            List<CancionesTableViewModel> lst = null;
            using(PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                lst = (from d in db.Canciones
                      select new CancionesTableViewModel
                      {
                          Nombre = d.Nombre_Cancion,
                          Artista = d.Artista,
                          Genero = d.Genero,
                      }).ToList();
            }

            return View(lst);
        }

        [HttpGet]
        public JsonResult Buscar(string termino)
        {
            using(PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                var canciones = db.Canciones
                .Where(c => c.Nombre_Cancion.Contains(termino) || c.Artista.Contains(termino))
                .Select(c => new {
                    c.Nombre_Cancion,
                    c.Artista,
                    c.Genero,
                    c.Favorito
                }).ToList();

                return Json(canciones, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult Edit(int id)
        {
            EditUsersViewModel model = new EditUsersViewModel();

            using(var db = new PruebasCrudMVCEntities())
            {
                var oUser = db.Users.Find(id);
                model.ID = oUser.ID;
                model.Contraseña = oUser.Contraseña;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditUsersViewModel model)
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
            return Redirect(Url.Content("~/Users/"));
        }

        [HttpPost]
        public ActionResult Favorito(int ID)
        {
            using (var db = new PruebasCrudMVCEntities())
            {
                var oCanciones = db.Canciones.Find(ID);
                oCanciones.Favorito = 1; // se agrega a Favoritos
                db.Entry(oCanciones).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Content("1");
        }

        public async Task<bool> ValidarCaptcha(string response)
        {
            var secretKey = "6Lfs82crAAAAAHhC5F0k_vRvDPmf50PY9qd9HOor";
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

        public ActionResult Favoritos()
        {
            List<CancionesTableViewModel> lst = null;
            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                lst = (from d in db.Canciones
                       where d.Favorito == 1
                       select new CancionesTableViewModel
                       {
                           Nombre = d.Nombre_Cancion,
                           Artista = d.Artista,
                           Genero = d.Genero,
                       }).ToList();
            }

            return View(lst);
        }

        public ActionResult NoFavorito(int ID)
        {
            using (var db = new PruebasCrudMVCEntities())
            {
                var oCanciones = db.Canciones.Find(ID);
                oCanciones.Favorito = 0; // se elimina de Favoritos
                db.Entry(oCanciones).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Content("1");
        }

        //public JsonResult ObtenerClientes()
        //{
        //    using (var db = new PruebasCrudMVCEntities())
        //    {
        //        var clientes = db.tbl_Clientes
        //                       .Select(c => new { c.id, c.Cliente }).ToList();
        //        return Json(clientes, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //public ActionResult Crear()
        //{
        //    using (var db = new PruebasCrudMVCEntities())
        //    {
        //        var clientes = db.tbl_Clientes.ToList();
        //        var model = new UsersViewModel
        //        {
        //            Clientes = new SelectList(clientes, "id", "Cliente")
        //        };
        //        return View(model);
        //    }

        //}
    }
}