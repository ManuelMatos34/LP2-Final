using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSC_AE.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net.Mail;

namespace ProyectoSC_AE.Controllers
{
    public class SolicitudCuposController : Controller
    {
        private readonly AdmisionCuposContext _DBcontext;
        public SolicitudCuposController(AdmisionCuposContext context)
        {
            _DBcontext = context;
        }

        [Authorize(Roles = "Estudiante")]
        public IActionResult Index(string buscarAsignatura, int? pageNumber)
        {
            // aqui se pesa mediante un viewbag una lista de tipo asignatura con todas las asignaturas a la vista
            List<Asignatura> datos = _DBcontext.Asignaturas.FromSqlRaw("SELECT * FROM Asignaturas").ToList();
            this.ViewBag.Asignaturas = datos;

            int pageSize = 8;

            if (buscarAsignatura != null)
            {
                // si la variable buscarasignatura llega llena me buscara todas las asignaturas igual a la variable
                List<PosiblesCupo> datos1 = _DBcontext.PosiblesCupos.FromSqlRaw("SELECT * FROM PosiblesCupos WHERE Estatus = 'A' and Asignatura like'%" + buscarAsignatura + "%' order by Id DESC").ToList();
                return View(PaginatedList<PosiblesCupo>.Create(datos1, pageNumber?? 1, pageSize));
            }
            else
            {
                // aqui vendran todos los usuarios activos 
                List<PosiblesCupo> datos2 = _DBcontext.PosiblesCupos.FromSqlRaw("SELECT * FROM PosiblesCupos WHERE Estatus = 'A' order by Id DESC").ToList();
                return View(PaginatedList<PosiblesCupo>.Create(datos2, pageNumber ?? 1, pageSize));
            }
        }

        public async Task<IActionResult> Unirse(int id, int cantidad)
        {
            if(id != 0)
            {
                if (cantidad != 20)
                {
                    SqlConnection con = new SqlConnection("Data Source=DESKTOP-G4NOF27\\SQLEXPRESS;" +
                        "Initial Catalog=Admision-Cupos;Integrated Security=True");
                    con.Open();
                    SqlCommand cmd = new SqlCommand("STPUnirse", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@matricula", User.Identity.Name);
                    cmd.Parameters.AddWithValue("@materia", id);
                    cmd.Parameters.Add("@ERROR", SqlDbType.VarChar, 500);
                    cmd.Parameters["@ERROR"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    var message = (string)cmd.Parameters["@ERROR"].Value;
                    con.Close();

                    if (message == "E")
                    {
                        TempData["Titulo"] = "Ha ocurrido un error";
                        TempData["Mensaje"] = "Ya estas en esta seccion";
                        TempData["Tipo"] = "error";

                    }
                    else
                    {
                        TempData["Titulo"] = "Confirmacion";
                        TempData["Mensaje"] = "Te has unido correctamente";
                        TempData["Tipo"] = "success";
                    }


                    return RedirectToAction("Index", "SolicitudCupos");
                }
                // mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error";
                TempData["Mensaje"] = "Esta sesion ya esta llena!!";
                TempData["Tipo"] = "error";

                return RedirectToAction("Index", "SolicitudCupos");

            }
            else
            {
                //// mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error";
                TempData["Mensaje"] = "Tu cupo no pudo ser procesado";
                TempData["Tipo"] = "error";

                return RedirectToAction("Index", "SolicitudCupos");
            }
            
        }

        public async Task<IActionResult> CrearSesion(string asignatura, string dia, string hora1, string hora2, string? dia_2, string? hora1_2, string hora2_2, string dia_3, string hora1_3, string hora2_3)
        {
            // aqui se obtiene el horario que selecciono el estudiante
            var horario = dia + " " + hora1 + " " + hora2 + " " + dia_2 + " " + hora1_2 + " " + hora2_2 + " " + dia_3 + " " + hora1_3 + " " + hora2_3;

            try {

                SqlConnection con = new SqlConnection("Data Source=DESKTOP-G4NOF27\\SQLEXPRESS;" +
                "Initial Catalog=Admision-Cupos;Integrated Security=True");
                con.Open();
                SqlCommand cmd = new SqlCommand("STPcrearsesion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@varAsignatura", asignatura);
                cmd.Parameters.AddWithValue("@varHorario", horario);
                cmd.Parameters.AddWithValue("@matricula", User.Identity.Name);
                cmd.Parameters.Add("@ERROR", SqlDbType.VarChar, 500);
                cmd.Parameters["@ERROR"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                var message = (string)cmd.Parameters["@ERROR"].Value;
                con.Close();

                var datos = _DBcontext.Profesores.FromSqlRaw("Select a.* From Profesores a, Asignaturas b where a.Asignatura = b.Id and b.Asignatura = '" + asignatura+"'").ToList();

                var mensaje = "Buenas Maestro/a estaria dispuesto de dar la asigantura " + message + " " + asignatura + " en el horaio " + horario + " en caso de confirmar responda este mismo mensaje, si no ignorelo, se mantendra informado por este medio";

                MailMessage mail = new MailMessage();

                foreach (Profesore destinos in datos)
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

                // mensaje correcto
                TempData["Titulo"] = "Confirmacion";
                TempData["Mensaje"] = "La sesion se ha creado correctamente";
                TempData["Tipo"] = "success";

                return RedirectToAction("Index", "SolicitudCupos");
            }
            catch
            {
                // mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error";
                TempData["Mensaje"] = "No se ha podido crear la sesion";
                TempData["Tipo"] = "error";

                return RedirectToAction("Index", "SolicitudCupos");
            }
        }

        public async Task<ActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioSesionCupos", "Login");
        }
    }
}
