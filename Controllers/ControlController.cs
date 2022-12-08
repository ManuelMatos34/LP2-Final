using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProyectoSC_AE.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Rotativa.AspNetCore;


namespace ProyectoSC_AE.Controllers
{
    public class ControlController : Controller
    {
        private readonly AdmisionCuposContext _DBcontext;
        
        public ControlController(AdmisionCuposContext context)
        {
            _DBcontext = context;
        }
        // Vista Administrador de cupos
        [Authorize(Roles = "Profesor")] // solo pueden entrar los usuarios con este rol
        public IActionResult AdministracionCupos(string buscarAsignatura, int? pageNumber)
        {
            int pageSize = 8;

            if (buscarAsignatura != null)
            {
                List<PosiblesCupo> datos1 = _DBcontext.PosiblesCupos.FromSqlRaw("SELECT * FROM PosiblesCupos WHERE Asignatura like'%" + buscarAsignatura + "%'").ToList();
                return View(PaginatedList<PosiblesCupo>.Create(datos1, pageNumber ?? 1, pageSize));
            }
            else
            {
                List<PosiblesCupo> datos2 = _DBcontext.PosiblesCupos.FromSqlRaw("SELECT * FROM PosiblesCupos order by Id DESC").ToList();
                return View(PaginatedList<PosiblesCupo>.Create(datos2, pageNumber ?? 1, pageSize));
            }
        }

        // Vista administrador de archivos
        [Authorize(Roles = "Administrador")] // solo pueden entrar los usuarios con este rol
        public IActionResult AdministracionNuevoIngreso(string? buscarCorreo, int? pageNumber, string? buscarDesde, string? buscarHasta)
        {
            int pageSize = 8;

            if (buscarCorreo != null)
            {
                List<EstudianteNuevosArchivo> datos = _DBcontext.EstudianteNuevosArchivos.FromSqlRaw("SELECT * FROM EstudianteNuevosArchivos Where EmailEstudiante like'%" + buscarCorreo + "%'").ToList();
                return View(PaginatedList<EstudianteNuevosArchivo>.Create(datos, pageNumber ?? 1, pageSize));
            }
            else if (buscarDesde != null)
            {
                List<EstudianteNuevosArchivo> datos = _DBcontext.EstudianteNuevosArchivos.FromSqlRaw("SELECT * FROM EstudianteNuevosArchivos Where FORMAT (FechaCreacion, 'd', 'fr-CA') BETWEEN '" + buscarDesde+"' AND '"+buscarHasta+"'").ToList();
                return View(PaginatedList<EstudianteNuevosArchivo>.Create(datos, pageNumber ?? 1, pageSize));
            }
            else
            {
                List<EstudianteNuevosArchivo> datos = _DBcontext.EstudianteNuevosArchivos.FromSqlRaw("SELECT * FROM EstudianteNuevosArchivos order by Id DESC").ToList();
                return View(PaginatedList<EstudianteNuevosArchivo>.Create(datos, pageNumber ?? 1, pageSize));
            }
        }

        // enviar correos administrador de archivos
        public async Task<IActionResult> EnviarCorreoAdmisiones(string Para, string asunto, string mensaje)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(Para, ""));
            mail.From = new MailAddress("admisionesunphu@hotmail.com");
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("admisionesunphu@hotmail.com", "1234HOLA");
            smtp.Send(mail);

            TempData["Titulo"] = "Confirmacion";
            TempData["Mensaje"] = "El E-mail se ha enviado correctamente";
            TempData["Tipo"] = "success";

            return RedirectToAction("AdministracionNuevoIngreso", "Control");
        }

        // enviar correos administrador de cupos 
        public async Task<IActionResult> EnviarCorreoCupos(string codigo, string mensaje )
        {

            var datos = _DBcontext.Usuarios.FromSqlRaw("SELect a.Email, a.Id, a.Estatus, a.Tipo, a.Matricula, a.Password from Usuarios a, Estudiantes_Secciones b, PosiblesCupos c where a.Id = b.Id_Estudiante and b.Id_PosiblesCupos = c.Id and c.Codigo ='" + codigo + "'").ToList();

            MailMessage mail = new MailMessage();

            foreach (Usuario destinos in datos)
            {
                mail.To.Add(new MailAddress(destinos.Email, ""));
            }
                mail.From = new MailAddress("cuposUNPHU@hotmail.com");
                mail.Subject = "Estado de la solicidud de cupo";
                mail.Body = mensaje;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("cuposUNPHU@hotmail.com", "1234HOLA");
                smtp.Send(mail);
            
            TempData["Titulo"] = "Confirmacion";
            TempData["Mensaje"] = "El E-mail se ha enviado correctamente";
            TempData["Tipo"] = "success";

            return RedirectToAction("AdministracionCupos", "Control");
        }

        // descargar archivos administrador de archivos
        public FileResult DescargarArchivo(int id)
        {
            EstudianteNuevosArchivo datos = _DBcontext.EstudianteNuevosArchivos.Where(o => o.Id == id).FirstOrDefault(); // query
            string archivoCompleto = datos.EmailEstudiante + datos.Tipo; // nombre archivo
            return File(datos.Archivo,"application/" + datos.Tipo.Replace(".", ""), archivoCompleto); 
        }

        // activar y desactivar archivos administrador de archivos
        public async Task<IActionResult> ActivarDesactivar(int id, string email)
        {
            var ID = new SqlParameter("@varID", id); // parametro
            var Email = new SqlParameter("@VarEmail", email); // parametro
            _DBcontext.EstudianteNuevosArchivos.FromSqlRaw("STPActivoInactivo @varID, @VarEmail", ID, Email).ToList(); // storeprocedure
            await _DBcontext.SaveChangesAsync();

            return RedirectToAction("AdministracionNuevoIngreso", "Control");
        }

        // activar y desactivar cupos administrador de cupos 
        public async Task<IActionResult> ActivarDesactivar2(int id)
        {
            var ID = new SqlParameter("@varID", id); // parametro
            _DBcontext.EstudiantesSecciones.FromSqlRaw("STPactivardesactivar @varID", ID).ToList(); // storeprocedure
            await _DBcontext.SaveChangesAsync();

            return RedirectToAction("AdministracionCupos", "Control");
        }

        // pdf de una seccion especifica, administrador de cupos
        public async Task<IActionResult> GenerarPDF(string id,string codigo, string asignatura)
        {
            var reloj = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            var nombrecompleto = codigo + "-" + asignatura + "-" + reloj;
            List<Usuario> modelo = _DBcontext.Usuarios.FromSqlRaw("Select distinct a.* from Usuarios " +
                "a, Estudiantes_Secciones b, PosiblesCupos c where b.Id_Estudiante = a.Id and " +
                "b.Id_PosiblesCupos = c.Id and b.Id_PosiblesCupos ='" + id + "'").ToList();

            return new ViewAsPdf("GenerarPDF", modelo)
            {

                FileName = $"" + nombrecompleto + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        // pdf de todos los cupos solicitados
        public async Task<IActionResult> GenerarPDF2()
        {
            var reloj = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            //TODO ESTO LO REEMPLAZAS CON TU PROPIA LÓGICA HACIA TU BASE DE DATOS
            List<PosiblesCupo> modelo = _DBcontext.PosiblesCupos.FromSqlRaw("Select * from PosiblesCupos").ToList();

            return new ViewAsPdf("GenerarPDF2", modelo)
            {

                FileName = $"CUPOS SOLICITADOS" + "-" + reloj + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

        public async Task<IActionResult> GenerarPDF3()
        {
            var reloj = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            //TODO ESTO LO REEMPLAZAS CON TU PROPIA LÓGICA HACIA TU BASE DE DATOS
            List<EstudianteNuevosArchivo> modelo = _DBcontext.EstudianteNuevosArchivos.FromSqlRaw("Select * from EstudianteNuevosArchivos").ToList();

            return new ViewAsPdf("GenerarPDF3", modelo)
            {

                FileName = $"ARCHIVOS ESTUDIANTES" + "-" + reloj + ".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
        // cerrar seccion
        public async Task<ActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioAdministracion", "Login");
        }
    }
}
