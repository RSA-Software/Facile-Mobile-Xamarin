using System;
using SQLite;

namespace Facile.Models
{
    public class Cateco
    {
        [PrimaryKey]
        public int eco_codice { get; set; }

        [Indexed]
        public string eco_desc { get; set; }

        public string eco_user { get; set; }
        public DateTime eco_last_update { get; set; }
    }
}
