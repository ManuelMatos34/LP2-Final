using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class Asignatura
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Asignatura1 { get; set; }
        public int? IdPensum { get; set; }
        public string? Creditos { get; set; }

        public virtual Pensum? IdPensumNavigation { get; set; }
    }
}
