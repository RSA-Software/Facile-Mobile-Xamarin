using System;
using SQLite;

namespace Facile.Models
{
    [Table("pagament")]
    public class Pagamenti
    {
        [PrimaryKey]
        public int pag_codice { get; set; }

        [Indexed]
        public string pag_desc { get; set; }

        public int pag_rate { get; set; }
        public int pag_inizio { get; set; }
        public int pag_periodicita { get; set; }
        public string pag_tipo_scadenza { get; set; }
        public string pag_mese1 { get; set; }
        public string pag_mese2 { get; set; }
        public string pag_mese3 { get; set; }
        public int pag_tratta_0 { get; set; }
        public int pag_tratta_1 { get; set; }
        public int pag_tratta_2 { get; set; }
        public int pag_bolli { get; set; }
        public double pag_tot_bolli { get; set; }
        public int pag_banche { get; set; }
        public double pag_tot_banche { get; set; }
        public int pag_tipo_pag { get; set; }
        public string pag_user { get; set; }
        public DateTime pag_last_update { get; set; }
        public bool pag_nota_alimentari { get; set; }
        public string pag_tipo { get; set; }
    }
}
