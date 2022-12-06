using Microsoft.AspNetCore.Mvc;
using ProyectoSC_AE.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Mail;

namespace ProyectoSC_AE.Controllers
{
    public class HomeController : Controller
    {
        // vista inicio
        public IActionResult Index()
        {
            return View();
        }

        // Vista de acceso denegado
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> EnviarCorreoContactanos(string mensaje, string telefono, string nombre, string desde)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress("contactanosUNPHU@yopmail.com", ""));
            mail.From = new MailAddress("admisionesunphu@hotmail.com");
            mail.Subject = "Contactanos"+" "+desde;
            mail.Body = mensaje+" "+telefono+" "+nombre;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("admisionesunphu@hotmail.com", "1234HOLA");
            smtp.Send(mail);

            TempData["Titulo"] = "Confirmacion";
            TempData["Mensaje"] = "El E-mail se ha enviado correctamente";
            TempData["Tipo"] = "success";

            return RedirectToAction("Index", "Home");
        }

    }
}