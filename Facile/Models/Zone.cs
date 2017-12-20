using System;
using SQLite;

namespace Facile.Models
{
    public class Zone
    {
        [PrimaryKey]
        public int zon_codice { get; set; }

        [Indexed]
        public string zon_desc { get; set; }

        public string zon_user { get; set; }
        public DateTime zon_last_update { get; set; }
    }
}
