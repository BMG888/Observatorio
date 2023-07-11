using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Observatorio;
using System.IO;
using PagedList;

namespace Observatorio.Controllers
{
    public class InvestigacionsController : Controller
    {
        private LaudatosiEntities db = new LaudatosiEntities();

        // GET: Investigacions
        public ActionResult Index(string sortOrder, string currentFilter, string currentFilter2, string searchString, string nombreautor, string pais, string tinvestigacion, string idioma, int? page)
        {
            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion");
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            ViewBag.ID_TInvestigacion = new SelectList(db.Tipo_Investigacion, "ID_tinv", "Descripcion");            

            var investigacion = db.Investigacion.Include(i => i.Idioma).Include(i => i.Pais).Include(i => i.Tipo_Investigacion);            

            ViewBag.count = investigacion.Count();

            if (!string.IsNullOrEmpty(pais)) // buscador de pais
            {
                int Id_pais = Int32.Parse(pais);
                investigacion = investigacion.Where(c => c.ID_pais == Id_pais);
                ViewBag.count = investigacion.Count();
            }

            if (!string.IsNullOrEmpty(tinvestigacion)) // buscador de tipo de investigacion
            {
                int Id_Tinvestigacion = Int32.Parse(tinvestigacion);
                investigacion = investigacion.Where(c => c.ID_tinv == Id_Tinvestigacion);
                ViewBag.count = investigacion.Count();
            }

            if (!string.IsNullOrEmpty(idioma)) // buscador de idioma
            {
                int Id_idioma = Int32.Parse(idioma);
                investigacion = investigacion.Where(c => c.ID_idioma == Id_idioma);
                ViewBag.count = investigacion.Count();
            }

            if (!string.IsNullOrEmpty(nombreautor)) // buscador nombre autor
            {
                investigacion = investigacion.Where(r => r.Autor.Contains(nombreautor));
                ViewBag.count = investigacion.Count();
            }

            if (!string.IsNullOrEmpty(searchString)) // buscador nombre documento
            {
                investigacion = investigacion.Where(r => r.Nombre.Contains(searchString));
                ViewBag.count = investigacion.Count();
            }            

            ViewBag.CurrentSort = sortOrder; //paginacion

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
                nombreautor = currentFilter2;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentFilter2 = nombreautor;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(investigacion.OrderBy(z => z.Nombre).ToPagedList(pageNumber, pageSize));           
        }

        // GET: Investigacions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investigacion investigacion = db.Investigacion.Find(id);
            if (investigacion == null)
            {
                return HttpNotFound();
            }
            return View(investigacion);
        }

        // GET: Investigacions/Create
        public ActionResult Create()
        {
            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion");
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            ViewBag.ID_tinv = new SelectList(db.Tipo_Investigacion, "ID_tinv", "Descripcion");
            return View();
        }

        // POST: Investigacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_inv,Nombre,Descripcion,ID_tinv,Autor,ID_pais,ID_idioma,URL")] Investigacion investigacion, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {                
                if (file == null)
                {
                    ViewBag.Message = "Debe de adjuntar un archivo .PDF para la creación del artículo.";
                }
                else
                {
                    var checkextension = Path.GetExtension(file.FileName).ToLower();

                    if (checkextension == ".pdf")
                    {
                        string filename = Path.GetFileName(file.FileName);                        

                        if (System.IO.File.Exists(Server.MapPath("~/Cargas/Investigaciones/") + filename))
                        {
                            ViewBag.Message = "El nombre del archivo ya existe en otro archivo, cambia su nombre para poder guardarlo o escoge otro archivo.";
                        }
                        else
                        {
                            file.SaveAs(HttpContext.Server.MapPath("~/Cargas/Investigaciones/") + filename);
                            investigacion.URL = filename;

                            db.Investigacion.Add(investigacion);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "El archivo debe de ser .pdf ";
                    }
                }

            }
            //if (ModelState.IsValid)
            //{
            //    db.Investigacion.Add(investigacion);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", investigacion.ID_idioma);
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", investigacion.ID_pais);
            ViewBag.ID_tinv = new SelectList(db.Tipo_Investigacion, "ID_tinv", "Descripcion", investigacion.ID_tinv);
            return View(investigacion);

        }

        // GET: Investigacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investigacion investigacion = db.Investigacion.Find(id);
            if (investigacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", investigacion.ID_idioma);
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", investigacion.ID_pais);
            ViewBag.ID_tinv = new SelectList(db.Tipo_Investigacion, "ID_tinv", "Descripcion", investigacion.ID_tinv);
            return View(investigacion);
        }

        // POST: Investigacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_inv,Nombre,Descripcion,ID_tinv,Autor,ID_pais,ID_idioma,URL")] Investigacion investigacion, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    db.Entry(investigacion).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var checkextension = Path.GetExtension(file.FileName).ToLower();

                    if (checkextension == ".pdf")
                    {
                        string filename = Path.GetFileName(file.FileName);

                        if (System.IO.File.Exists(Server.MapPath("~/Cargas/Investigaciones/") + filename))
                        {
                            ViewBag.Message = "El nombre del archivo ya existe en otro archivo, cambia su nombre para poder guardarlo o escoge otro archivo.";
                        }
                        else
                        {
                            System.IO.File.Delete(Server.MapPath("~/Cargas/Investigaciones/" + investigacion.URL)); //elimina archivo
                            file.SaveAs(HttpContext.Server.MapPath("~/Cargas/Investigaciones/") + filename); //agregar archivo
                            investigacion.URL = filename;

                            db.Entry(investigacion).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "El archivo debe ser .pdf";
                    }
                }
            }
            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", investigacion.ID_idioma);
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion", investigacion.ID_pais);
            ViewBag.ID_tinv = new SelectList(db.Tipo_Investigacion, "ID_tinv", "Descripcion", investigacion.ID_tinv);
            return View(investigacion);
        }

        // GET: Investigacions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investigacion investigacion = db.Investigacion.Find(id);
            if (investigacion == null)
            {
                return HttpNotFound();
            }
            return View(investigacion);
        }

        // POST: Investigacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Investigacion investigacion = db.Investigacion.Find(id);
            System.IO.File.Delete(Server.MapPath("~/Cargas/Investigaciones/" + investigacion.URL));
            db.Investigacion.Remove(investigacion);
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
        [HttpPost]
        public ActionResult Download(string fileName)
        {//Descarga archivo PDF
            string fullPath = Path.Combine(Server.MapPath("~/Cargas/Investigaciones/"), fileName);
            return File(fullPath, "application/octet-stream", fileName);

        }        
        [HttpPost]
        public ActionResult ShowPDF(string fileName)
        {//visualizar PDF
            string fullPath = Path.Combine(Server.MapPath("~/Cargas/Investigaciones/"), fileName);
            FileStream fsSource = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fsSource, "application/pdf");
        }
    }
}
