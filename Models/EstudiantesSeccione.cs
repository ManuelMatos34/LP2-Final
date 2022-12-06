using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class EstudiantesSeccione
    {
        public int Id { get; set; }
        public int? IdEstudiante { get; set; }
        public int? IdPosiblesCupos { get; set; }
        public string? Estatus { get; set; }

        public virtual Usuario? IdEstudianteNavigation { get; set; }
        public virtual PosiblesCupo? IdPosiblesCuposNavigation { get; set; }
    }
}
