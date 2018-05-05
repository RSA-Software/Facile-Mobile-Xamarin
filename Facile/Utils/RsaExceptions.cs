using System;
namespace Facile.Utils
{
	public class RsaException : Exception
	{
		public static readonly string LockedMsg = "Il record è bloccato da un altro nodo della rete.";
		public static readonly string DeletedMsg = "Il record è stato eliminato da un altro nodo della rete.";
		public static readonly string DuplicateMsg = "In archivio esiste già un record con lo stesso codice.";
		public static readonly string ModifiedMsg = "Il record è stato modificato da un altro nodo della rete.";
		public static readonly string AbortedMsg = "Operazione Annullata dall'Utente.";
		public static readonly string CancelMsg = "Non è possibile eliminare il record.";
		public static readonly string NotFoundMsg = "Record non trovato in archivio.";

		public static readonly string ArticoloMsg = "Il Codice dell'Articolo non è valido o disponibile.";
		public static readonly string CodivaMsg = "Il Codice Iva non è valido o disponibile.";
		public static readonly string TagliaMsg = "Il Codice Taglia non è valido o disponibile.";
		public static readonly string AssEanMsg = "Il Codice dell'Articolo Associato non è valido o disponibile.";
		public static readonly string CatMercMsg = "Il codice della Categoria Merceologica non è valido o disponibile.";
		public static readonly string MisuraMsg = "Il codice dell' Unità di Misura non è valido o disponibile.";
		public static readonly string MisuraMolMsg = "Il codice dell' Unità di Misura Moltiplicativo non è valido o disponibile.";
		public static readonly string FornitoriMsg = "Il codice del Fornitore non è valido o disponibile.";
		public static readonly string Fornit1Msg = "Il codice del Fornitore Alternativo non è valido o disponibile.";
		public static readonly string RepartoMsg = "Il codice del Reparto non è valido o disponibile.";
		public static readonly string StagioneMsg = "Il codice della Stagione non è valido o disponibile.";
		public static readonly string MarchioMsg = "Il codice del Marchio non è valido o disponibile.";
		public static readonly string GruppoMixMsg = "Il Codice del Gruppo Mix non è valido o disponibile.";
		public static readonly string ContoMsg = "Il codice del Conto non è valido o disponibile.";
		public static readonly string TabCofMsg = "Il Codice del Colore Fornitore richiesto non è disponibile.";
		public static readonly string TabTomMsg = "Il Codice della Tomaia richiesta non è disponibile.";
		public static readonly string TabFonMsg = "Il Codice del Fondo richiesto non è disponibile.";
		public static readonly string TabColMsg = "Il Codice del Colore richiesto non è disponibile.";
		public static readonly string VuotoMsg = "Il Codice del Vuoto richiesto non è valido o disponibile.";
		public static readonly string ProdEneMsg = "Il codice del prodotto energetico non è valido o disponibile.";
		public static readonly string AssEanUgMsg = "Il codice dell'Articolo è uguale al codice associato.";
		public static readonly string AssEanAssMsg = "Associazione Articolo a Catena.";
		public static readonly string CaumagaMsg = "Il codice della Causale di Magazzino richiesto non è valido o disponibile.";
		public static readonly string CauCarForMsg = "La Causale deve essere di Carico e in Relazione con Fornitori.";
		public static readonly string SezioneMsg = "Il codice della Sezione non è valido o disponibile.";
		public static readonly string OperatoreMsg = "Il codice dell' Operatore non è valido o disponibile.";
		public static readonly string CommessaMsg = "Il codice della Commessa non è valido o disponibile.";
		public static readonly string CenCostoMsg = "Il codice del centro di Costo non è valido o disponibile.";
		public static readonly string RelazMsg = "La Causale di Magazzino non è in Relazione corretta con i Clienti o i Fornitori.";
		public static readonly string ClientiMsg = "Il Codice del Cliente richiesto non è valido o disponibile.";
		public static readonly string DestinMsg = "Il Codice del destinatario non valido o disponibile.";
		public static readonly string AgentiMsg = "Il codice dell' Agente non è valido o disponibile.";
		public static readonly string DepositoMsg = "Il codice del Deposito non è valido o disponibile.";
		public static readonly string LottoMsg = "Il lotto non è valido o disponibile.";
		public static readonly string NoTagliaMsg = "Non è stata specificata la Taglia dell' articolo.";
		public static readonly string NoColoreMsg = "Non è stato specificato il Colore dell' Articolo.";
		public static readonly string ConcessMsg = "Il codice del Concessionario non è valido o disponibile";
		public static readonly string IntestMsg = "Codice Intestatario non Valido o Disponibile.";
		public static readonly string PagamentiMsg = "Il codice del Tipo di Pagamento non è valido o disponibile.";
		public static readonly string BancheMsg = "Il codice della Banca non è valido o disponibile.";
		public static readonly string ImballoMsg = "Il codice del tipo Aspetto Merci non è valido o disponibile.";
		public static readonly string TrasportoMsg = "Il codice del Motivo di Trasporto non è valido o disponibile.";
		public static readonly string Vettore1Msg = "Il codice del vettore non è valido o disponibile.";
		public static readonly string Vettore2Msg = "Il codice del vettore non è valido o disponibile.";
		public static readonly string VettoriMsg = "Il codice del Trasportatore non è valido o disponibile.";
		public static readonly string Codiva1Msg = "Il codice dell' aliquota iva non è valido o disponibile.";
		public static readonly string Codiva2Msg = "Il codice dell' aliquota iva non è valido o disponibile.";
		public static readonly string Codiva3Msg = "Il codice dell' aliquota iva non è valido o disponibile.";
		public static readonly string Codiva4Msg = "Il codice dell' aliquota iva non è valido o disponibile.";
		public static readonly string CommittenteMsg = "Il codice del Committente  non è valido o disponibile.";
		public static readonly string ProprietarioMsg = "Il codice del Proprietario non è valido o disponibile,";
		public static readonly string CaricatoreMsg = "Il codice del Caricatore non è valido o disponibile.";
		public static readonly string LuogoCaricoMsg = "Il codice del Luogo di Carico non è valido o disponibile.";
		public static readonly string SettoreMsg = "Per il settore della risorsa selezionato non è ammessa questa operazione.";
		public static readonly string PeriodoMsg = "Il codice del periodo non è valido o disponibile.";
		public static readonly string GruppoMsg = "Il Codice del Gruppo richiesto non è valido o disponibile.";
		public static readonly string AgenziaMsg = "Il codice dell' Agenzia richiesta non è valido o disponibile.";
		public static readonly string OspiteMsg = "Il codice dell' Ospite richiesto non è valido o disponibile.";
		public static readonly string RifatturazioneMsg = "Codice Causale Rifatturazione non valido o disponibile.";
		public static readonly string CauContMsg = "Il codice della Causale Contabile non è valido o disponibile.";
		public static readonly string MovConMsg = "Il registro della Causale Contabile non è Valido";
		public static readonly string ImpoMasForMsg = "Mastro Fornitori non impostato.";
		public static readonly string ImpoMasCliMsg = "Mastro Clienti non impostato.";
		public static readonly string ImpoMasUgualeMsg = "Il Mastro Fornitori non puo' essere uguale al mastro Clienti.";
		public static readonly string CapoAreaMsg = "Il codice del Capo Area non è valido o disponibile.";
		public static readonly string ForGiroMsg = "Il codice del Fornitore per il giro non è valido o disponibile.";
		public static readonly string CambioCliMsg = "Il cambio dell' intestatario del documento non è consentito nel caso in cui ci siano scadenze pagate.\nPer poter cambiare l' intestatario è necessario rimuovere le registrazioni di prima nota e le scadenze interessate.";
		public static readonly string CambioDstMsg = "Il cambio del destinatario del documento non è consentito nel caso in cui ci siano scadenze pagate.\nPer poter cambiare il destinatario è necessario rimuovere le registrazioni di prima nota e le scadenze interessate.";
		public static readonly string StatusMsg = "I Dati del documento non possono essere modificati.";
		public static readonly string TabellaMsg = "Codice Tabella Collegata non Valido o Disponibile.";
		public static readonly string MixedMsg = "Nella documento sono stati inserti articoli con icva inclusa\ne articoli con iva esclusa.\n\n Impossibile calcolare  i totali.";
		public static readonly string IvaZeroMsg = "Rigo Documento con Codice IVA a Zero.";
		public static readonly string TroppiMsg = "Nel documento ci sono troppe aliquote Iva.\nE' possibile usare al massimo 4 aliquote diverse.";

		public static readonly int NoErr = 0;
		public static readonly int LockedErr = -10;
		public static readonly int DeletedErr = -11;
		public static readonly int DuplicateErr = -12;
		public static readonly int ModifiedErr = -13;
		public static readonly int AbortedErr = -14;
		public static readonly int CancelErr = -15;
		public static readonly int NotFoundErr = -16;
		public static readonly int ArticoloErr = -20;
		public static readonly int CodivaErr = -21;
		public static readonly int TagliaErr = -22;
		public static readonly int AssEanErr = -23;
		public static readonly int CatMercErr = -24;
		public static readonly int MisuraErr = -25;
		public static readonly int MisuraMolErr = -26;
		public static readonly int FornitoriErr = -27;
		public static readonly int Fornit1Err = -28;
		public static readonly int RepartoErr = -29;
		public static readonly int StagioneErr = -30;
		public static readonly int MarchioErr = -31;
		public static readonly int GruppoMixErr = -32;
		public static readonly int ContoErr = -33;
		public static readonly int TabCofErr = -34;
		public static readonly int TabTomErr = -35;
		public static readonly int TabFonErr = -36;
		public static readonly int TabColErr = -37;
		public static readonly int VuotoErr = -38;
		public static readonly int ProdEndErr = -39;
		public static readonly int AssEanUgErr = -40;
		public static readonly int AssEanAssErr = -41;
		public static readonly int CaumagaErr = -42;
		public static readonly int CauCarForErr = -43;
		public static readonly int SezioneErr = -44;
		public static readonly int OperatoreErr = -45;
		public static readonly int CommessaErr = -46;
		public static readonly int CenCostoErr = -47;
		public static readonly int RelazErr = -48;
		public static readonly int ClientiErr = -49;
		public static readonly int DestinErr = -50;
		public static readonly int AgentiErr = -51;
		public static readonly int DepositoErr = -52;
		public static readonly int LottoErr = -53;
		public static readonly int NoTagliaErr = -54;
		public static readonly int NoColoreErr = -55;
		public static readonly int ConcessErr = -56;
		public static readonly int IntestErr = -57;
		public static readonly int PagamentiErr = -58;
		public static readonly int BancheErr = -59;
		public static readonly int ImballoErr = -60;
		public static readonly int TrasportoErr = -61;
		public static readonly int Vettore1Err = -62;
		public static readonly int Vettore2Err = -63;
		public static readonly int VettoriErr = -64;
		public static readonly int Codiva1Err = -65;
		public static readonly int Codiva2Err = -66;
		public static readonly int Codiva3Err = -67;
		public static readonly int Codiva4Err = -68;
		public static readonly int CommittenteErr = -69;
		public static readonly int ProprietarioErr = -70;
		public static readonly int CaricatoreErr = -71;
		public static readonly int LuogoCaricoErr = -72;
		public static readonly int SettoreErr = -73;
		public static readonly int PeriodoErr = -74;
		public static readonly int GruppoErr = -75;
		public static readonly int AgenziaErr = -76;
		public static readonly int OspiteErr = -77;
		public static readonly int RifatturazioneErr = -78;
		public static readonly int CauContErr = -79;
		public static readonly int MovConErr = -80;
		public static readonly int ImpoMasForErr = -81;
		public static readonly int ImpoMasCliErr = -82;
		public static readonly int ImpoMasUgualeErr = -83;
		public static readonly int CapoAreaErr = -84;
		public static readonly int ForGiroErr = -85;
		public static readonly int CambioCliErr = -86;
		public static readonly int CambioDstErr = -86;
		public static readonly int StatusErr = -87;
		public static readonly int TabellaErr = -88;
		public static readonly int MixedErr = -89;
		public static readonly int IvaZeroErr = -90;
		public static readonly int TroppiErr = -91;

		private readonly int _error;
		public RsaException()
		{
			_error = 0;
		}

		public RsaException(string message, int err)
			: base(message)
		{
			_error = err;
		}

		public RsaException(string message, int err, Exception inner)
			: base(message, inner)
		{
			_error = err;
		}

		public int GetError()
		{
			return _error;
		}
	}
}
