using SistemaCursos.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SistemaCursos.Context
{
    public class Context : DbContext
    {
        public DbSet<Turma> tbTurma { get; set; }
        public DbSet<Curso> tbCurso { get; set; }

        public DbSet<Usuario> tbUsuario { get; set; }
    }
}