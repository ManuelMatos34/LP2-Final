using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoSC_AE.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ProyectoSC_AE.Controllers
{
    public class LoginController : Controller
    {
        private readonly AdmisionCuposContext _context;

        public LoginController(AdmisionCuposContext context)
        {
            _context = context;
        }

        // vista iniciar sesion nuevos estudiantes
        public IActionResult InicioSesion()
        {
            return View();
        }

        // vista iniciar sesion cupos
        public IActionResult InicioSesionCupos()
        {
            return View();
        }

        public IActionResult InicioAdministracion()
        {
            return View();
        }

        // metodo obtener estudiantesNuevos
        public List<EstudiantesNuevo> ObtenerEstudiantes()
        {
            List<EstudiantesNuevo> Estudiantes = _context.EstudiantesNuevos.FromSqlRaw("SELECT * FROM EstudiantesNuevos").ToList(); // aqii se obtienen todos los estudiantes de nuevo ingreso
            return Estudiantes;
        }

        // metodo validar estudiantesNuevos
        public EstudiantesNuevo ValidarEstudiante(string correo)
        {
            return ObtenerEstudiantes().Where(item => item.Email == correo).FirstOrDefault(); // aqui se valida si el correo introducido existe en la base de datos
        }

        // metodo validar inicio sesion
        [HttpPost]
        public async Task<IActionResult>ValidarInicio(EstudiantesNuevo _estudiante)
        {
            var estudiante = ValidarEstudiante(_estudiante.Email);

            if (estudiante != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, estudiante.Email)
                };

                string[] estudianterol = { estudiante.Tipo};

                // aqui se aplican los roles
                foreach (string rol in estudianterol)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // si el administrador desactiva el usuario pasara esto
                if (estudiante.Estatus == "I")
                {
                    TempData["Titulo"] = "Atencion!!";
                    TempData["Mensaje"] = "Este correo ya ha subido todo sus documentos!!";
                    TempData["Tipo"] = "info";

                    return RedirectToAction("InicioSesion", "Login");

                }

                // aqui si el estudiante esta activo
                return RedirectToAction("Index", "AdmisionNuevosEstudiantes");

            }
            else
            {
                // mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error!!";
                TempData["Mensaje"] = "Asegurese de haber llenado el formulario de admision!!";
                TempData["Tipo"] = "error";

                return RedirectToAction("InicioSesion", "Login");
            }
        }

        // aqui se obtienen todos los estudiantes inscritos
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> Usuarios = _context.Usuarios.FromSqlRaw("SELECT * FROM Usuarios").ToList(); // aqui se obtienen todos los estudiantes inscritos
            return Usuarios;
        }

        // aqui se validan si los datos introducidos estan correctos
        public Usuario ValidarUsuario(string usuario, string password)
        {
            return ObtenerUsuarios().Where(item => item.Matricula == usuario && item.Password == password).FirstOrDefault();
        }

        // aqui se validan los estudiantes inscritos
        [HttpPost]
        public async Task<IActionResult> ValidarInicioUsuario(Usuario _usuario)
        {
            var usuario = ValidarUsuario(_usuario.Matricula, _usuario.Password);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _usuario.Matricula)
                };

                string[] usuariorol = { usuario.Tipo };

                foreach (string rol in usuariorol)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // condiciones dependiendo el rol del usuario ingresado
                if (usuario.Tipo == "Profesor")
                {

                    return RedirectToAction("AdministracionCupos", "Control");
                }
                else if(usuario.Tipo == "Administrador")
                {

                    return RedirectToAction("AdministracionNuevoIngreso", "Control");
                }
                else if (usuario.Tipo == "Estudiante")
                {

                    return RedirectToAction("Index", "SolicitudCupos");
                }

                // mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error!!";
                TempData["Mensaje"] = "Asegurese de haber introducido los datos correctos!!";
                TempData["Tipo"] = "error";

                return RedirectToAction("InicioSesionCupos", "Login");
            }
            else
            {
                // mensaje de error
                TempData["Titulo"] = "Ha ocurrido un error!!";
                TempData["Mensaje"] = "Asegurese de haber introducido los datos correctos!!";
                TempData["Tipo"] = "error";

                return RedirectToAction("InicioSesionCupos", "Login");
            }
        }

        // cerrar sesion
        public async Task<ActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("InicioSesion", "Login");
        }
    }
}
