using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public ActionResult Index()
        {
            List<CancionesTableViewModel> lst = null;
            using(PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                int usuarioID = ((Users)Session["User"]).ID;
                var canciones = db.Canciones.ToList();
                lst = (from d in db.Canciones
                      select new CancionesTableViewModel
                      {
                          ID = d.ID,
                          Nombre = d.Nombre_Cancion,
                          Artista = d.Artista,
                          Genero = d.Genero,
                          CalificaionUsuario = db.Calificacion
                                .Where(cal => cal.IdCancion == d.ID && cal.IdUsuario == usuarioID)
                                .Select(cal => (int?)cal.Puntuacion)
                                .FirstOrDefault(),

                          PromedioCalificacion = db.Calificacion
                                .Where(cal => cal.IdCancion == d.ID)
                                .Select(cal => (double?)cal.Puntuacion)
                                .Average() ?? 0,
                          TotalCalificaciones = db.Calificacion.Count(cal => cal.IdCancion == d.ID)
                      }).ToList();

                // Obtener géneros únicos
                var generos = db.Canciones
                                .Select(c => c.Genero)
                                .Distinct()
                                .OrderBy(g => g)
                                .ToList();

                ViewBag.Generos = new SelectList(generos); // Pasar a la vista
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
                    .GroupBy(c => c.ID)
                    .Select(g => g.FirstOrDefault())
                    .Select(c => new {
                        c.ID,
                        c.Nombre_Cancion,
                        c.Artista,
                        c.Genero,
                        c.Favorito
                }).ToList();

                return Json(canciones, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult BuscarFav(string termino)
        {
            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                var canciones = db.Canciones
                    .Where(c => c.Nombre_Cancion.Contains(termino) || c.Artista.Contains(termino) && c.Favorito == 1)
                    .GroupBy(c => c.ID)
                    .Select(g => g.FirstOrDefault())
                    .Select(c => new {
                        c.ID,
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

            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
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

            using(PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
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

            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
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
            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
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
                int usuarioID = ((Users)Session["User"]).ID;
                var canciones = db.Canciones.ToList();
                lst = (from d in db.Canciones
                       where d.Favorito == 1
                       select new CancionesTableViewModel
                       {
                           ID = d.ID,
                           Nombre = d.Nombre_Cancion,
                           Artista = d.Artista,
                           Genero = d.Genero,
                           CalificaionUsuario = db.Calificacion
                                .Where(cal => cal.IdCancion == d.ID && cal.IdUsuario == usuarioID)
                                .Select(cal => (int?)cal.Puntuacion)
                                .FirstOrDefault(),

                           PromedioCalificacion = db.Calificacion
                                .Where(cal => cal.IdCancion == d.ID)
                                .Select(cal => (double?)cal.Puntuacion)
                                .Average() ?? 0,
                           TotalCalificaciones = db.Calificacion.Count(cal => cal.IdCancion == d.ID)
                       }).ToList();
                var generos = db.Canciones
                                .Select(c => c.Genero)
                                .Distinct()
                                .OrderBy(g => g)
                                .ToList();

                ViewBag.Generos = new SelectList(generos); // Pasar a la vista
            }

            return View(lst);
        }

        public ActionResult NoFavorito(int ID)
        {
            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                var oCanciones = db.Canciones.Find(ID);
                oCanciones.Favorito = 0; // se elimina de Favoritos
                db.Entry(oCanciones).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Content("1");
        }

        [HttpPost]
        public ActionResult Calificar(int cancionId, int puntuacion)
        {
            int usuarioId = ((Users)Session["User"]).ID; // Asegúrate de tener esto en sesión

            using (PruebasCrudMVCEntities db = new PruebasCrudMVCEntities())
            {
                var calificacion = db.Calificacion.FirstOrDefault(c => c.IdUsuario == usuarioId && c.IdCancion == cancionId);

                if (calificacion == null)
                {
                    calificacion = new Calificacion
                    {
                        IdUsuario = usuarioId,
                        IdCancion = cancionId,
                        Puntuacion = puntuacion
                    };
                    db.Calificacion.Add(calificacion);
                }
                else
                {
                    calificacion.Puntuacion = puntuacion;
                    db.Entry(calificacion).State = EntityState.Modified;
                }

                db.SaveChanges();

                // Obtener nuevo promedio
                var nuevoPromedio = db.Calificacion
                                      .Where(c => c.IdCancion == cancionId)
                                      .Average(c => (double?)c.Puntuacion) ?? 0;

                return Json(new { promedio = Math.Round(nuevoPromedio, 1) });
            }
        }
    }
}