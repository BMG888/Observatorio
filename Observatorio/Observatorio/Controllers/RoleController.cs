using Microsoft.AspNet.Identity.EntityFramework;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Observatorio.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role
        ApplicationDbContext context = new ApplicationDbContext();
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            var roles = context.Roles.ToList();
            return View(roles);
        }
        public ActionResult Create()
        {
            var rol = new IdentityRole();
            return View();
        }
        [HttpPost]
        public ActionResult Create(IdentityRole rol)
        {
            context.Roles.Add(rol);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}