using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;
//using System.Web.Mvc;
using System.ComponentModel;

namespace PruebaCrudMVC.Models.ViewModels
{
    public class UsersViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no son iguales")]
        [DataType(DataType.Password)]
        public string ConfirmarContraseña { get; set; }





    }

    public class EditUsersViewModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contraseña", ErrorMessage = "Las contraseñas no son iguales")]
        [DataType(DataType.Password)]
        public string ConfirmarContraseña { get; set; }
    }
}