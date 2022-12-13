using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class Profesore
    {
        public Profesore()
        {
            EstudiantesSecciones = new HashSet<EstudiantesSeccione>();
        }

        public int Id { get; set; }
        public string? Email { get; set; }
        public int? Asignatura { get; set; }
        public string? Nombre { get; set; }

        public virtual Asignatura? AsignaturaNavigation { get; set; }
        public virtual ICollection<EstudiantesSeccione> EstudiantesSecciones { get; set; }
    }
}
