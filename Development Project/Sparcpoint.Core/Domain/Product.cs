using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // EVAL: TODO: crear nueva entidad METADATA
        public Dictionary<string, string> Metadata { get; set; }
        // EVAL: TODO: debe ser un listado de categorias
        public List<int> CategoryIds { get; set; }
    }
}
