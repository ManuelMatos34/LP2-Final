using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            EstudiantesSecciones = new HashSet<EstudiantesSeccione>();
        }

        public int Id { get; set; }
        public string? Matricula { get; set; }
        public string? Password { get; set; }
        public string? Estatus { get; set; }
        public string? Tipo { get; set; }
        public string? Email { get; set; }

        public virtual ICollection<EstudiantesSeccione> EstudiantesSecciones { get; set; }
    }
}
