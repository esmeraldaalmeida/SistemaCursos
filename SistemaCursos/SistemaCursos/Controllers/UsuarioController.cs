using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaCursos.Context;
using SistemaCursos.Models;

namespace SistemaCursos.Controllers
{
    public class UsuarioController : Controller
    {
        private SistemaCursos.Context.Context db = new SistemaCursos.Context.Context();

        // GET: Usuario
        public ActionResult Index()
        {
            return View(db.tbUsuario.ToList());
        }

        public ActionResult ListaAlunosTurmas(int? idTurma)
        {
            List<Usuario> listaAlunos = db.tbUsuario.Where(a => a.IdTurma == idTurma).ToList();
            return View(listaAlunos);
        }

        public JsonResult Valida_CPF(string cpf)
        {
            // Obs.: para que funcione é preciso que no seu localhost não tenha 2 ou mais users
            // com um mesmo cpf cadastrados previamente
            Usuario c = db.tbUsuario.SingleOrDefault(s => s.CPF == cpf);
            bool retorno = false;

            try
            {
                if (ValidacaoInternaCPF(cpf))
                {
                    /*if (c == default)
                    {
                        retorno = true;
                        return Json(retorno, JsonRequestBehavior.AllowGet);
                    }*/
                    return Json(true, JsonRequestBehavior.AllowGet);

                }
                else
                    return Json(retorno, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(retorno, JsonRequestBehavior.AllowGet);
            }
        }

        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Remove caracteres não numéricos
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemovePontos(string text)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"[^0-9]");
            string ret = reg.Replace(text, string.Empty);
            return ret;
        }

        /// <summary>
        /// Valida se um cpf é válido
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static bool ValidacaoInternaCPF(string cpf)
        {
            //Remove formatação do número, ex: "123.456.789-01" vira: "12345678901"
            cpf = RemovePontos(cpf);

            if (cpf.Length > 11)
                return false;

            while (cpf.Length != 11)
                cpf = '0' + cpf;

            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (cpf[i] != cpf[0])
                    igual = false;

            if (igual || cpf == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(cpf[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                return false;

            return true;
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.tbUsuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            List<Turma> listaTurma = db.tbTurma.Where(x => x.Ativo == true).ToList();

            ViewBag.ListaTurma = listaTurma;

            return View();
        }

        // POST: Usuario/Create
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUser,Nome,CPF,DataNascimento,IdTurma")] Usuario usuario)
        {
            Usuario userCpf = db.tbUsuario.FirstOrDefault(x => x.CPF == usuario.CPF);
            List<Turma> listaTurma = db.tbTurma.Where(x => x.Ativo == true).ToList();
            List<Usuario> listaUsuario = db.tbUsuario.Where(s => s.IdTurma == usuario.IdTurma).ToList();
            Turma turmaUser = db.tbTurma.FirstOrDefault(x => x.IdTurma == usuario.IdTurma);
            ViewBag.ListaTurma = listaTurma;

            if (ModelState.IsValid)
            {
                if (userCpf != default)
                {
                    usuario = userCpf;
                    ViewBag.UserCPF = userCpf;
                    return View("Create", userCpf);//RedirectToAction("Details", "Usuario", new { id = userCpf.IdUser});
                }

                else
                {
                    if (listaUsuario.Count == turmaUser.QtdeAlunos)
                    {
                        ModelState.AddModelError("", "A turma já está cheia. Tente uma outra.");
                    }
                    else
                    {
                        //return View(usuario);
                        db.tbUsuario.Add(usuario);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

            }

            return View(userCpf);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.tbUsuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            List<Turma> listaTurma = db.tbTurma.Where(x => x.Ativo == true).ToList();
            ViewBag.ListaTurma = listaTurma;
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // Para proteger-se contra ataques de excesso de postagem, ative as propriedades específicas às quais deseja se associar. 
        // Para obter mais detalhes, confira https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUser,Nome,CPF,DataNascimento,IdTurma")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<Turma> listaTurma = db.tbTurma.Where(x => x.Ativo == true).ToList();
            ViewBag.ListaTurma = listaTurma;
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.tbUsuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.tbUsuario.Find(id);
            db.tbUsuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
