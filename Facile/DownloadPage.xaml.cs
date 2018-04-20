using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Forms;
using Facile.Interfaces;
using Facile.Imports;
using Facile.Models;
using PCLStorage;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Xamarin.Forms.Xaml;
using Syncfusion.XForms.PopupLayout;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DownloadPage : ContentPage
	{
		protected bool first;
		public DownloadPage()
		{
			first = true;
			InitializeComponent();
		}

		async protected override void OnAppearing()
		{
			base.OnAppearing();
			if (first)
			{
				var response = await DisplayAlert("Facile", "La ricezione dati potrebbe richiede una connessione internet e potrebbero essere necessari diversi minuti.\n\nVuoi proseguire?", "Si", "No");
				if (response)
					await Download();
				else
					await Navigation.PopModalAsync();
			}
			first = false;
		}

		private async Task Download()
		{
			try
			{
				var test = await ImportTableAsync<Ditte>("impostazioni", m_dit_download, m_dit_unzip, m_dit_json, m_dit_load, m_dit, m_dit_rec);
				test = await ImportTableAsync<Zone>("zone", m_zon_download, m_zon_unzip, m_zon_json, m_zon_load, m_zon, m_zon_rec);
				test = await ImportTableAsync<Cateco>("cateco", m_eco_download, m_eco_unzip, m_eco_json, m_eco_load, m_eco, m_eco_rec);
				test = await ImportTableAsync<Pagamenti>("pagament", m_pag_download, m_pag_unzip, m_pag_json, m_pag_load, m_pag, m_pag_rec);
				test = await ImportTableAsync<Tabelle>("tabelle", m_tab_download, m_tab_unzip, m_tab_json, m_tab_load, m_tab, m_tab_rec);
				test = await ImportTableAsync<Agenti>("agenti1", m_age_download, m_age_unzip, m_age_json, m_age_load, m_age, m_age_rec);
				test = await ImportTableAsync<Misure>("misure", m_mis_download, m_mis_unzip, m_mis_json, m_mis_load, m_mis, m_mis_rec);
				test = await ImportTableAsync<Clienti>("clienti1", m_cli_download, m_cli_unzip, m_cli_json, m_cli_load, m_cli, m_cli_rec);
				test = await ImportTableAsync<Destinazioni>("destina1", m_dst_download, m_dst_unzip, m_dst_json, m_dst_load, m_dst, m_dst_rec);
				test = await ImportTableAsync<Scadenze>("scadenze", m_sca_download, m_sca_unzip, m_sca_json, m_sca_load, m_sca, m_sca_rec);
				test = await ImportTableAsync<Codiva>("codiva", m_iva_download, m_iva_unzip, m_iva_json, m_iva_load, m_iva, m_iva_rec);
				test = await ImportTableAsync<Reparti>("reparti", m_rep_download, m_rep_unzip, m_rep_json, m_rep_load, m_rep, m_rep_rec);
				test = await ImportTableAsync<Catmerc>("catmerc1", m_mer_download, m_mer_unzip, m_mer_json, m_mer_load, m_mer, m_mer_rec);
				test = await ImportTableAsync<Fornitori>("fornito1", m_for_download, m_for_unzip, m_for_json, m_for_load, m_for, m_for_rec);
				test = await ImportTableAsync<Depositi>("depositi", m_dep_download, m_dep_unzip, m_dep_json, m_dep_load, m_dep, m_dep_rec);
				test = await ImportTableAsync<Lotti>("lotti1", m_lot_download, m_lot_unzip, m_lot_json, m_lot_load, m_lot, m_lot_rec);
				test = await ImportTableAsync<Artanag>("artanag", m_ana_download, m_ana_unzip, m_ana_json, m_ana_load, m_ana, m_ana_rec);
				test = await ImportTableAsync<Listini>("listini1", m_lis_download, m_lis_unzip, m_lis_json, m_lis_load, m_lis, m_lis_rec);
				test = await ImportTableAsync<Fatture>("fatture2", m_fat_download, m_fat_unzip, m_fat_json, m_fat_load, m_fat, m_fat_rec);
				test = await ImportTableAsync<FatRow>("fatrow2", m_row_download, m_row_unzip, m_row_json, m_row_load, m_row, m_row_rec);
				test = await ImportTableAsync<Vettori>("vettori1", m_vet_download, m_vet_unzip, m_vet_json, m_vet_load, m_vet, m_vet_rec);
				test = await ImportTableAsync<Banche>("banche1", m_ban_download, m_ban_unzip, m_ban_json, m_ban_load, m_ban, m_ban_rec);
				test = await ImportTableAsync<Canali>("canali", m_can_download, m_can_unzip, m_can_json, m_can_load, m_can, m_can_rec);
				test = await ImportTableAsync<Stagioni>("stagioni", m_sta_download, m_sta_unzip, m_sta_json, m_sta_load, m_sta, m_sta_rec);
				test = await ImportTableAsync<Marchi>("marchi", m_mar_download, m_mar_unzip, m_mar_json, m_mar_load, m_mar, m_mar_rec);
				test = await ImportTableAsync<Associazioni>("associaz", m_asg_download, m_asg_unzip, m_asg_json, m_asg_load, m_asg, m_asg_rec);
				test = await ImportTableAsync<Barcode>("barcode", m_bar_download, m_bar_unzip, m_bar_json, m_bar_load, m_bar, m_bar_rec);
				test = await ImportTableAsync<Trasporti>("trasport", m_tra_download, m_tra_unzip, m_tra_json, m_tra_load, m_tra, m_tra_rec);
				test = await ImportTableAsync<Agganci>("agganci1", m_agg_download, m_agg_unzip, m_agg_json, m_agg_load, m_agg, m_agg_rec);
				test = await ImportTableAsync<Descrizioni>("descriz1", m_des_download, m_des_unzip, m_des_json, m_des_load, m_des, m_des_rec);

				await DisplayAlert("Facile", "Importazione dati conclusa regolarmente!", "Ok");
			}
			catch (Exception ex)
			{
				await DisplayAlert("Download Error", ex.Message, "OK");
			}
			await Navigation.PopModalAsync();
		}

		public async Task<bool> ImportTableAsync<T>(string tblName, Image m_down, Image m_unzip, Image m_json, Image m_load, Label m_label, Label m_rec)
		{
			String remotePath = "ftp://www.facile2013.it/1/in" + "/" + tblName.ToUpper() + ".ZIP";
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localZip = rootFolder.Path + "/" + tblName.ToUpper() + ".ZIP";
			String localPath = rootFolder.Path + "/" + tblName.ToUpper() + ".JSON";

			m_label.FontSize = m_label.FontSize + 3;
			m_label.TextColor = Color.Red;
			m_label.FontAttributes = FontAttributes.Italic | FontAttributes.Bold;
			m_down.Source = "ic_hourglass_full_white.png";

			m_desc.Text = "Downloading " + tblName.ToLower() + ".zip";
			var ftp = DependencyService.Get<IFtpWebRequest>();
			string result = await ftp.DownloadFile("demo2017", "demo2017", remotePath, localZip);
			if (result.StartsWith("221", StringComparison.CurrentCulture))
			{
				m_down.Source = "ic_file_download_black.png";
				m_desc.Text = "Unzip " + tblName.ToLower() + ".zip";
				IFile zip_file = await FileSystem.Current.GetFileFromPathAsync(localZip);
				var zip_stream = await zip_file.OpenAsync(FileAccess.Read); 
				ZipInputStream  zip = new ZipInputStream(zip_stream);

				m_unzip.Source = "ic_hourglass_full_white.png";

				ZipEntry theEntry;
				string fileName = string.Empty;
				string fileExtension = string.Empty;
				string fileSize = string.Empty;

				while ((theEntry = zip.GetNextEntry()) != null)
				{
					fileName = Path.GetFileName(theEntry.Name);
					fileExtension = Path.GetExtension(fileName);

					if (!string.IsNullOrEmpty(fileName))
					{
						try
						{
							//FileStream streamWriter = File.Create(Server.MapPath(virtualPath + fileName));
							IFolder folder = FileSystem.Current.LocalStorage;
							IFile json_file = await folder.CreateFileAsync(fileName,CreationCollisionOption.ReplaceExisting);
							Stream json_stream = await json_file.OpenAsync(FileAccess.ReadAndWrite); 

							int size = 2048;
							byte[] data = new byte[2048];

							do
							{
								size = zip.Read(data, 0, data.Length);
								json_stream.Write(data, 0, size);
							} while (size > 0);

							//fileSize = Convert.ToDecimal(streamWriter.Length / 1024).ToString() + ” KB”;

							json_stream.Dispose();

						}
						catch (Exception ex)
						{
							await DisplayAlert("Result", ex.ToString(), "ok");
						}
					}
				}
				zip.Dispose();
				m_unzip.Source = "ic_build_black.png";

				var file = await FileSystem.Current.GetFileFromPathAsync(localPath);
				if (file != null)
				{
					m_json.Source = "ic_hourglass_full_white.png";
					m_desc.Text = "Deserializing " + tblName.ToLower() + ".json";
					string qry = "DELETE FROM " + tblName.ToLower();
					string str = await file.ReadAllTextAsync();

					var settings = new JsonSerializerSettings();
					settings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
					settings.NullValueHandling = NullValueHandling.Ignore;
					FacileJson<T> json = JsonConvert.DeserializeObject<FacileJson<T>>(str, settings);

					m_json.Source = "ic_code_black.png";

					m_load.Source = "ic_hourglass_full_white.png";
					m_desc.Text = "Importing " + tblName.ToLower() + " " + json.Records;
					var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
					await dbcon.ExecuteAsync(qry);
					await dbcon.InsertAllAsync(json.Data);

					m_load.Source = "ic_storage_black.png";
					m_rec.TextColor = Color.Black;
					m_rec.Text = json.Records.ToString();

						
					//var rows = await dbcon.Table<Fatture>().CountAsync();


					await file.DeleteAsync();
				}

				m_label.FontSize = m_label.FontSize - 3;
				m_label.TextColor = Color.Black;
				m_label.FontAttributes = FontAttributes.Bold;
			}
			else
			{
				await DisplayAlert("Result", result, "ok");
				return (false);
			}

			return (true);
		}

		/*
		async void DownLoadImagesClicked(object sender, System.EventArgs e)
		{
			String remoteServer = "ftp://www.facile2013.it/images/";
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

			Desc.Text = "Otteniamo la lista dei files... ";
			var ftp = DependencyService.Get<IFtpWebRequest>();
			var files = await ftp.ListDirectory("demo2017", "demo2017", remoteServer);

			if (files != null && files.Count > 0)
			{
				int idx = 0;
				foreach (var file in files)
				{
					String remoteFile = remoteServer + file;
					String localFile = imagesFolder.Path + "/" + file;

					idx++;
					Desc.Text = string.Format("Downloading file {0} di {1} - {2}", idx ,files.Count, file);
					var result = await ftp.DownloadFile("demo2017", "demo2017", remoteFile, localFile);
					if (!result.StartsWith("221", StringComparison.CurrentCulture))
					{
						break;
					}
				}
			}
			Desc.Text = "";
		}
*/

	}
}
