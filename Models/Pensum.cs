using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class Pensum
    {
        public Pensum()
        {
            Asignaturas = new HashSet<Asignatura>();
        }

        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Pensum1 { get; set; }
        public string? Estatus { get; set; }

        public virtual ICollection<Asignatura> Asignaturas { get; set; }
    }
}
