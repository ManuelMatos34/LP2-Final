using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class Asignatura
    {
        public Asignatura()
        {
            Profesores = new HashSet<Profesore>();
        }

        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Asignatura1 { get; set; }
        public int? IdPensum { get; set; }
        public string? Creditos { get; set; }
        public string? HorasTeoricas { get; set; }
        public string? HorasPracticas { get; set; }

        public virtual Pensum? IdPensumNavigation { get; set; }
        public virtual ICollection<Profesore> Profesores { get; set; }
    }
}
