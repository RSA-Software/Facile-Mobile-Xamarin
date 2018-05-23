using System;
using System.Collections.Generic;
using Facile.Interfaces;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Facile.Models;
using Facile.ExportModels;
using Facile.Utils;
using PCLStorage;
using Newtonsoft.Json;
using Syncfusion.SfBusyIndicator.XForms;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IncassiInvia : ContentPage
	{
		private readonly SQLiteAsyncConnection _dbcon;
		private int _n_start;
		private int _n_stop;
		private DateTime _d_start;
		private DateTime _d_stop;

		public IncassiInvia()
		{
			_n_start = 0;
			_n_stop = 0;
			_d_start = DateTime.Now;
			_d_stop = DateTime.Now;

			InitializeComponent();

			_dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			try
			{
				_n_stop = await _dbcon.ExecuteScalarAsync<int>("SELECT MAX(dsp_codice) FROM scapaghe");
				_n_start = await _dbcon.ExecuteScalarAsync<int>("SELECT MIN(dsp_codice) FROM scapaghe");


				_d_start = await _dbcon.ExecuteScalarAsync<DateTime>("SELECT MIN(dsp_data) FROM scapaghe");
				_d_stop = await _dbcon.ExecuteScalarAsync<DateTime>("SELECT MAX(dsp_data) FROM scapaghe");

				m_n_stop.Value = _n_stop;
				m_n_start.Value = _n_start;

				m_d_stop.Date = _d_stop;
				m_d_start.Date = _d_start;
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore!", "Impossibile leggere dal database : " + ex.Message, "OK");
				await Navigation.PopModalAsync();
			}
		}

		async void OnInviaClicked(object sender, System.EventArgs e)
		{
			_n_start = Convert.ToInt32(m_n_start.Value);
			_n_stop = Convert.ToInt32(m_n_stop.Value);

			TimeSpan t = new TimeSpan(1, 0, 0, 0);

			_d_start = m_d_start.Date;
			_d_stop = m_d_stop.Date + t;

			if (_n_stop < _n_start || _n_stop == 0)
			{
				m_n_stop.Focus();
				return;
			}
			if (_d_start > _d_stop)
			{
				m_d_stop.Focus();
				return;
			}

			LocalImpo lim;

			//
			// Leggiamo le impostazioni
			//
			try
			{
				lim = await _dbcon.GetAsync<LocalImpo>(1);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impostazioni locali non trovate!\nRiavviare l'App.", "OK");
				await Navigation.PopModalAsync();
				return;
			}

			if (string.IsNullOrWhiteSpace(lim.ftpServer))
			{
				await DisplayAlert("Attenzione!", "Server non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}
			if (string.IsNullOrWhiteSpace(lim.user))
			{
				await DisplayAlert("Attenzione!", "Utente non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}
			if (lim.age == 0)
			{
				await DisplayAlert("Attenzione!", "Agente non impostato o non valido.", "OK");
				await Navigation.PopModalAsync();
				return;
			}

			busyIndicator.IsBusy = true;
			busyIndicator.AnimationType = AnimationTypes.Ball;
			busyIndicator.Title = "Estrazione Incassi";

			//
			// Estraiamo i dati selezionati
			//

			List<ScaPagHead> dspList = null;
			try
			{
				dspList = await _dbcon.QueryAsync<ScaPagHead>("SELECT * from scapaghe WHERE (dsp_codice >= ? AND dsp_codice <= ?) AND (dsp_data >= ? AND dsp_data < ?)", _n_start, _n_stop, _d_start.Ticks, _d_stop.Ticks);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore!", "Impossibile leggere dal database : " + ex.Message, "OK");
				return;
			}
			if (dspList == null || dspList.Count == 0)
			{
				await DisplayAlert("Errore!", "Non ci sono dati da inviare!", "OK");
				return;
			}

			var incList = new List<Incasso>();

			foreach (var dsp in dspList)
			{
				var inc = new Incasso();
				inc.head = dsp;

				//
				// Inseriamo i dati del cliente
				//
				try
				{
					inc.cliente = await _dbcon.GetAsync<Clienti>(dsp.dsp_clifor);
				}
				catch
				{
					await DisplayAlert("Attenzione!", "Dati cliente non presenti in archivio", "OK");
					return;
				}

				//
				// Inseriamo i dati del destinatario
				//
				if (dsp.dsp_dst != 0)
				{
					try
					{
						inc.destinazione = await _dbcon.GetAsync<Destinazioni>(dsp.dsp_dst);
					}
					catch
					{
						await DisplayAlert("Attenzione!", "Dati destinazione non presenti in archivio", "OK");
						return;
					}
				}

				//
				// Inseriamo le righe
				//
				var sql = string.Format("SELECT * FROM scapagro WHERE dsr_codice = {0} ORDER BY dsr_idx", dsp.dsp_codice);
				inc.rows = await _dbcon.QueryAsync<ScaPagRow>(sql);

				//
				// Aggiungiamo il documento alla list
				//
				incList.Add(inc);
			}

			//
			// Creaiamo il file
			//
			var fname = string.Format("INC{0:00000}", ((App)Application.Current).facile_db_impo.dit_codice);
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localJson = rootFolder.Path + "/" + fname + ".JSON";

			IFile json_file = await rootFolder.CreateFileAsync(localJson, CreationCollisionOption.ReplaceExisting);
			await json_file.WriteAllTextAsync(JsonConvert.SerializeObject(incList, Formatting.Indented));

			//
			// Trasmettiamo il file
			// 
			busyIndicator.AnimationType = AnimationTypes.Globe;
			busyIndicator.Title = "Trasmissione";

			string password = "";
			string remoteServer = "";

			if (lim.ftpServer == "Facile - 01")
				remoteServer = "ftp://www.facile2013.it";

			if (lim.ftpServer == "Facile - 02")
				remoteServer = "ftp://www.rsaweb.com";

			if (lim.ftpServer == "Facile - 03")
				remoteServer = "ftp://www.facilecloud.com";

			if (remoteServer == "")
				throw new Exception("Server non impostato o non valido");

			if (lim.user != "demo2017")
				password = $"$_{lim.user}_$";
			else
				password = lim.user;

			var remotePath = $"/{lim.age}/out";


			var ftp = DependencyService.Get<IFtpWebRequest>();
			string result = await ftp.UploadFile(remoteServer, localJson, lim.user, password, remotePath);
			busyIndicator.IsBusy = false;
			if (result.StartsWith("221", StringComparison.CurrentCulture))
			{
				//
				// Marchiamo i documenti come non più editabili
				//
				foreach (var dsp in dspList)
				{
					dsp.dsp_inviato = true;
				}
				await _dbcon.UpdateAllAsync(dspList);
				await DisplayAlert("Facile", "Invio incassi concluso con successo!", "OK");
			}
			else if (result.StartsWith("530", StringComparison.CurrentCulture))
			{
				await DisplayAlert("Facile", "Parametri di Login non validi!\nVerificare il nome utente configurato.", "OK");
			}
			else if (result.StartsWith("System.Net.WebException", StringComparison.CurrentCulture))
			{
				await DisplayAlert("Facile", result, "OK");
			}
			else
			{
				await DisplayAlert("Facile", "Impossibile caricare il file sul server!", "OK");
			}

			//
			// Rimuoviamo il file
			//
			await json_file.DeleteAsync();
			await Navigation.PopModalAsync();
		}
	}
}
