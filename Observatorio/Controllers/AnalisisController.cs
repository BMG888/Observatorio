using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;

namespace Observatorio.Controllers
{
    public class AnalisisController : Controller
    {
        private LaudatosiEntities db = new LaudatosiEntities();

        // GET: Analisis
        // Prueba
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string pais, string tanalisis, string idioma, int? page)
        {
            ViewBag.ID_idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion");
            ViewBag.ID_pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            ViewBag.ID_TAnalisis = new SelectList(db.Tipo_Analisis, "ID_TAnalisis", "Descripcion");

            var analisis = db.Analisis.Include(a => a.Idioma).Include(a => a.Pais).Include(a => a.Tipo_Analisis);

            ViewBag.count = analisis.Count();

            if (!string.IsNullOrEmpty(idioma)) // buscador de idioma
            {
                int Id_idioma = Int32.Parse(idioma);
                analisis = analisis.Where(c => c.ID_Idioma == Id_idioma);
                ViewBag.count = analisis.Count();
            }

            if (!string.IsNullOrEmpty(pais)) // buscador de pais
            {
                int Id_pais = Int32.Parse(pais);
                analisis = analisis.Where(c => c.ID_Pais == Id_pais);
                ViewBag.count = analisis.Count();
            }

            if (!string.IsNullOrEmpty(tanalisis)) // buscador de tipo de analisis
            {
                int Id_tanalisis = Int32.Parse(tanalisis);
                analisis = analisis.Where(c => c.ID_TAnalisis == Id_tanalisis);
                ViewBag.count = analisis.Count();
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                analisis = analisis.Where(r => r.Nombre.Contains(searchString));
                ViewBag.count = analisis.Count();
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
            return View(analisis.OrderBy(z => z.Nombre).ToPagedList(pageNumber, pageSize));
        }

        // GET: Analisis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analisis analisis = db.Analisis.Find(id);
            if (analisis == null)
            {
                return HttpNotFound();
            }
            return View(analisis);
        }

        // GET: Analisis/Create
        public ActionResult Create()
        {
            ViewBag.ID_Idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion");
            ViewBag.ID_Pais = new SelectList(db.Pais, "ID_pais", "Descripcion");
            ViewBag.ID_TAnalisis = new SelectList(db.Tipo_Analisis, "ID_TAnalisis", "Descripcion");
            return View();
        }

        // POST: Analisis/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Analisis,Nombre,Descripcion,ID_TAnalisis,ID_Idioma,ID_Pais,URL")] Analisis analisis, HttpPostedFileBase file)
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

                        if (System.IO.File.Exists(Server.MapPath("~/Cargas/Analisis/") + filename))
                        {
                            ViewBag.Message = "El nombre del archivo ya existe en otro archivo, cambia su nombre para poder guardarlo o escoge otro archivo.";
                        }
                        else
                        {
                            file.SaveAs(HttpContext.Server.MapPath("~/Cargas/Analisis/") + filename);
                            analisis.URL = filename;

                            db.Analisis.Add(analisis);
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
            //db.Analisis.Add(analisis);
            //db.SaveChanges();
            //return RedirectToAction("Index");        

            ViewBag.ID_Idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", analisis.ID_Idioma);
            ViewBag.ID_Pais = new SelectList(db.Pais, "ID_pais", "Descripcion", analisis.ID_Pais);
            ViewBag.ID_TAnalisis = new SelectList(db.Tipo_Analisis, "ID_TAnalisis", "Descripcion", analisis.ID_TAnalisis);
            return View(analisis);
        }

        // GET: Analisis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analisis analisis = db.Analisis.Find(id);
            if (analisis == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", analisis.ID_Idioma);
            ViewBag.ID_Pais = new SelectList(db.Pais, "ID_pais", "Descripcion", analisis.ID_Pais);
            ViewBag.ID_TAnalisis = new SelectList(db.Tipo_Analisis, "ID_TAnalisis", "Descripcion", analisis.ID_TAnalisis);
            return View(analisis);
        }

        // POST: Analisis/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Analisis,Nombre,Descripcion,ID_TAnalisis,ID_Idioma,ID_Pais,URL")] Analisis analisis, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file == null)
                {
                    db.Entry(analisis).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var checkextension = Path.GetExtension(file.FileName).ToLower();

                    if (checkextension == ".pdf")
                    {
                        string filename = Path.GetFileName(file.FileName);

                        if (System.IO.File.Exists(Server.MapPath("~/Cargas/Analisis/") + filename))
                        {
                            ViewBag.Message = "El nombre del archivo ya existe en otro archivo, cambia su nombre para poder guardarlo o escoge otro archivo.";
                        }
                        else
                        {
                            System.IO.File.Delete(Server.MapPath("~/Cargas/Analisis/" + analisis.URL)); //elimina archivo
                            file.SaveAs(HttpContext.Server.MapPath("~/Cargas/Analisis/") + filename); //agregar archivo
                            analisis.URL = filename;

                            db.Entry(analisis).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "El archivo debe ser .pdf";
                    }
                }            
            //db.Entry(analisis).State = EntityState.Modified;
            //db.SaveChanges();
            //return RedirectToAction("Index");
        }
            ViewBag.ID_Idioma = new SelectList(db.Idioma, "ID_idioma", "Descripcion", analisis.ID_Idioma);
            ViewBag.ID_Pais = new SelectList(db.Pais, "ID_pais", "Descripcion", analisis.ID_Pais);
            ViewBag.ID_TAnalisis = new SelectList(db.Tipo_Analisis, "ID_TAnalisis", "Descripcion", analisis.ID_TAnalisis);
            return View(analisis);
        }

        // GET: Analisis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analisis analisis = db.Analisis.Find(id);
            if (analisis == null)
            {
                return HttpNotFound();
            }
            return View(analisis);
        }

        // POST: Analisis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Analisis analisis = db.Analisis.Find(id);
            System.IO.File.Delete(Server.MapPath("~/Cargas/Analisis/" + analisis.URL));
            db.Analisis.Remove(analisis);
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
            string fullPath = Path.Combine(Server.MapPath("~/Cargas/Analisis/"), fileName);
            return File(fullPath, "application/octet-stream", fileName);

        }
        [HttpPost]
        public ActionResult ShowPDF(string fileName)
        {//visualizar PDF
            string fullPath = Path.Combine(Server.MapPath("~/Cargas/Analisis/"), fileName);
            FileStream fsSource = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fsSource, "application/pdf");
        }
    }
}
