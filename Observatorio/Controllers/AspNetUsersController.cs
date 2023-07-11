using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;
using Observatorio;
using Observatorio.Models;
using PagedList;
using Persistence;

namespace Observatorio.Controllers
{
    public class AspNetUsersController : Controller
    {
        private LaudatosiEntities db = new LaudatosiEntities();
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: AspNetUsers
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var usuariorol = (from user in context.Users
                              from userrole in user.Roles
                              join role in context.Roles on userrole.RoleId equals role.Id
                              select new Usuarios() {
                                  UserID = user.Id,                                  
                                  Email = user.Email,
                                  Rol = role.Name,
                                  NombreCompleto = user.Nombre + " " + user.Apellido
                              });

            if (!string.IsNullOrEmpty(searchString))
            {
                usuariorol = usuariorol.Where(r => r.NombreCompleto.Contains(searchString));
                ViewBag.count = usuariorol.Count();
            }

            ViewBag.CurrentSort = sortOrder; //paginacion

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;                
            }

            ViewBag.CurrentFilter = searchString;            
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(usuariorol.OrderBy(z => z.NombreCompleto).ToPagedList(pageNumber, pageSize));
        }
          
        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            var user = (from u in context.Users
                        where u.Id == id
                        select u).Single();

            Usuarios model = new Usuarios();

            model.UserID = user.Id;

            model.Nombre = user.Nombre;

            model.Apellido = user.Apellido;

            model.Email = user.Email;

            model.Rol = (from usr in context.Users
                         from userrol in usr.Roles
                         join role in context.Roles on userrol.RoleId equals role.Id
                         where (usr.Id == id)
                         select role.Id).Single();

            ViewBag.Rol = new SelectList(context.Roles, "Id", "Name");

            return View(model);
        }

        // POST: AspNetUsers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Usuarios usr, string viejorol)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appusr = new ApplicationUser();
                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(store);

                var currentUser = manager.FindById(usr.UserID);

                currentUser.Nombre = usr.Nombre;
                currentUser.Apellido = usr.Apellido;
                currentUser.Email = usr.Email;
                currentUser.UserName = usr.Email;

                if (usr.Password != null)
                {
                    Passhash sr = new Passhash();
                    string nuevapassword = sr.hash(usr.Password);
                    currentUser.PasswordHash = nuevapassword;
                }

                await manager.UpdateAsync(currentUser);
                var stx = store.Context;
                stx.SaveChanges();

                return RedirectToAction("Index", "AspNetUsers");
            }
            ViewBag.Rol = new SelectList(context.Roles, "Id", "Name");
            return View(usr);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            if (aspNetUsers == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUsers);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUsers aspNetUsers = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUsers);
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
