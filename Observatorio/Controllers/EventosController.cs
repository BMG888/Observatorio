using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Observatorio;
using PagedList;

namespace Observatorio.Controllers
{
    public class EventosController : Controller
    {
        private LaudatosiEntities db = new LaudatosiEntities();

        // GET: Eventos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string pais, string fecha, int? page)
        {
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            var eventos = db.Eventos.Include(e => e.Pais);

            ViewBag.count = eventos.Count();

            

           

            if (!string.IsNullOrEmpty(searchString)) // buscador nombre del evento
            {
                eventos = eventos.Where(r => r.Nombre.Contains(searchString));
                ViewBag.count = eventos.Count();
            }

            if (!string.IsNullOrEmpty(fecha)) // buscador de fecha
            {
                eventos = eventos.Where(f => f.Fecha == fecha);
                ViewBag.count = eventos.Count();
            }

            if (!string.IsNullOrEmpty(pais)) // buscador de pais
            {
                int Id_pais = Int32.Parse(pais); //cambio string a int
                eventos = eventos.Where(c => c.ID_pais == Id_pais);
                ViewBag.count = eventos.Count();
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
            return View(eventos.OrderBy(z => z.Nombre).ToPagedList(pageNumber, pageSize));
        }
        // GET: Eventos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventos eventos = db.Eventos.Find(id);
            if (eventos == null)
            {
                return HttpNotFound();
            }
            return View(eventos);
        }

        // GET: Eventos/Create
        public ActionResult Create()
        {
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            return View();
        }

        // POST: Eventos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Evento,Nombre,Descripcion,Fecha, Hora,URL,ID_pais")] Eventos eventos)
        {
            if (ModelState.IsValid)
            {
                db.Eventos.Add(eventos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", eventos.ID_pais);
            return View(eventos);
        }

        // GET: Eventos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventos eventos = db.Eventos.Find(id);
            if (eventos == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", eventos.ID_pais);
            return View(eventos);
        }

        // POST: Eventos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Evento,Nombre,Descripcion,Fecha, Hora,URL,ID_pais")] Eventos eventos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", eventos.ID_pais);
            return View(eventos);
        }

        // GET: Eventos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventos eventos = db.Eventos.Find(id);
            if (eventos == null)
            {
                return HttpNotFound();
            }
            return View(eventos);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eventos eventos = db.Eventos.Find(id);
            db.Eventos.Remove(eventos);
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
