using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Observatorio;

namespace Observatorio.Controllers
{
    public class FormacionsController : Controller
    {
        private LaudatosiEntities db = new LaudatosiEntities();

        // GET: Formacions
        public ActionResult Index()
        {
            return View(db.Formacion.ToList());
        }

        // GET: Formacions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formacion formacion = db.Formacion.Find(id);
            if (formacion == null)
            {
                return HttpNotFound();
            }
            return View(formacion);
        }

        // GET: Formacions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Formacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Formacion,Nombre,Descripcion,Nom_Insti,URL")] Formacion formacion)
        {
            if (ModelState.IsValid)
            {
                db.Formacion.Add(formacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formacion);
        }

        // GET: Formacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formacion formacion = db.Formacion.Find(id);
            if (formacion == null)
            {
                return HttpNotFound();
            }
            return View(formacion);
        }

        // POST: Formacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Formacion,Nombre,Descripcion,Nom_Insti,URL")] Formacion formacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formacion);
        }

        // GET: Formacions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formacion formacion = db.Formacion.Find(id);
            if (formacion == null)
            {
                return HttpNotFound();
            }
            return View(formacion);
        }

        // POST: Formacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Formacion formacion = db.Formacion.Find(id);
            db.Formacion.Remove(formacion);
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
