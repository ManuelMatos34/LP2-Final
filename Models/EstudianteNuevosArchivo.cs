using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class EstudianteNuevosArchivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public byte[] Archivo { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public string EmailEstudiante { get; set; } = null!;
        public string? Estatus { get; set; }
    }
}
