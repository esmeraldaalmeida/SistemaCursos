using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaCursos.Models
{
    public class Usuario
    {
        [Key]
        public int IdUser { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11)]
        [Remote("Valida_CPF", "Usuario")]//Objetivo de aplicar a logica de validação na Controller
        public string CPF { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
        public string DataNascimento { get; set; }

        [Display(Name = "Turma")]
        [Required(ErrorMessage = "O campo Turma é obrigatório.")]
        public int IdTurma { get; set; }
    }
}