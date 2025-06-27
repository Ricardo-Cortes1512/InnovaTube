using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PruebaCrudMVC.Models.TableViewModels
{
    public class CancionesTableViewModel
    {
        public int ID { get; set; } 
        public string Nombre { get; set; }
        public string Artista { get; set; }
        public string  Genero { get; set; }
        public int favorito { get; set; }
        public int? CalificaionUsuario { get; set; }
        public double PromedioCalificacion { get; set; }
        public int TotalCalificaciones { get; set; }

    }
}