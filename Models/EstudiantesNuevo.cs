using System;
using System.Collections.Generic;

namespace ProyectoSC_AE.Models
{
    public partial class EstudiantesNuevo
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string? Estatus { get; set; }
    }
}
