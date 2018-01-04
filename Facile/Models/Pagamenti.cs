using System;
using SQLite;

namespace Facile.Models
{
	public enum TipoPagamento
	{
		PAG_RIMESSA_DIRETTA = 1,
		PAG_RIBA = 2,
		PAG_RID = 3,
		PAG_TRATTA = 4,
		PAG_ASSEGNO = 5,
		PAG_CONTRASSEGNO = 6,
		PAG_CONTANTI = 7,
		PAG_ASSEGNO_CIRCOLARE = 8,
		PAG_CONTANTI_PRESSO_TESORERIA = 9,
		PAG_BONIFICO = 10, 
		PAG_VAGLIA_CAMBIARIO = 11,
		PAG_BOLLETTINO_BANCARIO = 12,
		PAG_CARTA_DI_PAGAMENTO = 13,
		PAG_RID_UTENZE = 14,
		PAG_RID_VELOCE = 15,
		PAG_MAV = 16,
		PAG_QUIETANZA_ERARIO = 17,
		PAG_GIROCONTO_CONTI_SPECIALI = 18,
		PAG_DOMICILIAZIONE_BANCARIA = 19,
		PAG_DOMICILIAZIONE_POSTALE = 20,
		PAG_BOLLETTINO_CC_POSTALE = 21,
		PAG_SEPA_DIRECT_DEBIT = 22,
		PAG_SEPA_DIRECT_DEBIT_CORE = 23,
		PAG_SEPA_DIRECT_DEBIT_B2B = 24,
		PAG_TRATTENUTA_SOMME_RISCOSSE = 25,
	}

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
