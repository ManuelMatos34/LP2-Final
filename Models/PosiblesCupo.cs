using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class PosiblesCupo
    {
        public PosiblesCupo()
        {
            EstudiantesSecciones = new HashSet<EstudiantesSeccione>();
        }

        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Asignatura { get; set; }
        public string? Horario { get; set; }
        public string? Estatus { get; set; }
        public string? Creditos { get; set; }
        public int? Cantidad { get; set; }

        public virtual ICollection<EstudiantesSeccione> EstudiantesSecciones { get; set; }
    }
}
