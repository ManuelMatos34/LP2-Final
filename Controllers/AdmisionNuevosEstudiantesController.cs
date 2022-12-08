using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoSC_AE.Models;


namespace ProyectoSC_AE.Controllers
{
    public class AdmisionNuevosEstudiantesController : Controller
    {
        private readonly AdmisionCuposContext _context;

        public AdmisionNuevosEstudiantesController(AdmisionCuposContext context)
        {
            _context = context;
        }

        // vista ingresar archivos para estudiantes nacionales
        [Authorize(Roles = "Nacional, Extranjero, Transferido")] // solo pueden entrar los usuarios con este rol
        public IActionResult Index()
        {
            return View();
        }

        // metodo guardar archivos en bd
        [HttpPost]
        public IActionResult Index(IFormFile archivos)
        {
            if (archivos != null)
            {
                if (archivos.Length > 0)
                {
                    var fileName = Path.GetFileName(archivos.FileName); // aqui se obtiene el nombre del archivo
                    var fileExtension = Path.GetExtension(fileName); // aqui se obtienen la extension del archivo
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                    var objArchivos = new EstudianteNuevosArchivo()
                    {
                        Id = 0,
                        Nombre = newFileName, // nombre archivo
                        Tipo = fileExtension, // extension archivo
                        FechaCreacion = DateTime.Now, // fecha actual
                        EmailEstudiante = User.Identity.Name, // usuario actual
                        Estatus = "A"
                    };

                    using (var target = new MemoryStream())
                    {
                        archivos.CopyTo(target);
                        objArchivos.Archivo = target.ToArray();
                    }
                    _context.EstudianteNuevosArchivos.Add(objArchivos);
                    _context.SaveChanges();

                    // mensaje de confirmacion
                    ViewBag.Tittle = "Confirmacion!!";
                    ViewBag.Message = "Sus archivos se han guardado correctamente en la plataforma!";
                    ViewBag.Type = "success";
                }
                else
                {   // mensaje de error
                    ViewBag.Tittle = "Ha ocurrido un error!!";
                    ViewBag.Message = "No hay archivo, Intentelo de nuevo";
                    ViewBag.Type = "error";
                }
                return View();
            }
            else
            {   // mensaje de error
                ViewBag.Tittle = "Ha ocurrido un error!!";
                ViewBag.Message = "No ha mandado nada, Intentelo de nuevo";
                ViewBag.Type = "error";
            }
            return View();
        }
    }
}

