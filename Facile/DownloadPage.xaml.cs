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

namespace Facile
{
	public partial class DownloadPage : ContentPage
	{
		private async void Close_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		private async void Download_Clicked(object sender, System.EventArgs e)
		{
			var test = await ImportTableAsync<Zone>("zone");
			test = await ImportTableAsync<Cateco>("cateco");
			test = await ImportTableAsync<Pagamenti>("pagament");
			test = await ImportTableAsync<Tabelle>("tabelle");
			test = await ImportTableAsync<Agenti>("agenti1");
			test = await ImportTableAsync<Misure>("misure");
			test = await ImportTableAsync<Clienti>("clienti1");
			test = await ImportTableAsync<Destinazioni>("destina1");
			test = await ImportTableAsync<Scadenze>("scadenze");
			test = await ImportTableAsync<Codiva>("codiva");
			test = await ImportTableAsync<Reparti>("reparti");
			test = await ImportTableAsync<Catmerc>("catmerc1");
			test = await ImportTableAsync<Fornitori>("fornito1");
			test = await ImportTableAsync<Depositi>("depositi");
			test = await ImportTableAsync<Lotti>("lotti1");
			test = await ImportTableAsync<Artanag>("artanag");
			test = await ImportTableAsync<Listini>("listini1");
			test = await ImportTableAsync<Fatture>("fatture2");
			test = await ImportTableAsync<FatRow>("fatrow2");
			test = await ImportTableAsync<Vettori>("vettori1");
			test = await ImportTableAsync<Banche>("banche1");
			test = await ImportTableAsync<Canali>("canali");
			test = await ImportTableAsync<Stagioni>("stagioni");
			test = await ImportTableAsync<Marchi>("marchi");
			test = await ImportTableAsync<Associazioni>("associaz");

			await Navigation.PopModalAsync();
		}

		public async Task<bool> ImportTableAsync<T>(string tblName)
		{
			String remotePath = "ftp://www.facile2013.it/testzip" + "/" + tblName.ToUpper() + ".zip";
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localZip = rootFolder.Path + "/" + tblName.ToUpper() + ".ZIP";
			String localPath = rootFolder.Path + "/" + tblName.ToUpper() + ".JSON";

			Desc.Text = "Downloading " + tblName.ToLower() + ".zip";
			var ftp = DependencyService.Get<IFtpWebRequest>();
			string result = await ftp.DownloadFile("demo2014", "demo2014", remotePath, localZip);
			if (result.StartsWith("221"))
			{

				Desc.Text = "Unzip " + tblName.ToLower() + ".zip";
				IFile zip_file = await FileSystem.Current.GetFileFromPathAsync(localZip);
				var zip_stream = await zip_file.OpenAsync(FileAccess.Read); 
				ZipInputStream  zip = new ZipInputStream(zip_stream);

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

				var file = await FileSystem.Current.GetFileFromPathAsync(localPath);
				if (file != null)
				{
					Desc.Text = "Deserializing " + tblName.ToLower() + ".json";
					string qry = "DELETE FROM " + tblName.ToLower();
					string str = await file.ReadAllTextAsync();

					var settings = new JsonSerializerSettings();
					settings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
					settings.NullValueHandling = NullValueHandling.Ignore;
					FacileJson<T> json = JsonConvert.DeserializeObject<FacileJson<T>>(str, settings);

					Desc.Text = "Importing " + tblName.ToLower() + " " + json.Records;
					var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
					await dbcon.ExecuteAsync(qry);
					await dbcon.InsertAllAsync(json.Data);



					//var rows = await dbcon.Table<Fatture>().CountAsync();


					await file.DeleteAsync();
				}
			}
			else
			{
				await DisplayAlert("Result", result, "ok");
				return (false);
			}

			return (true);
		}
		/*
		public async Task<bool> ImportTableAsync<T>(string tblName)
		{
			String remotePath = "ftp://www.facile2013.it/test" + "/" + tblName.ToUpper() + ".JSON";
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			String localPath = rootFolder.Path + "/" + tblName.ToUpper() + ".JSON";

			Desc.Text = "Downloading " + tblName.ToLower() + ".json";
			var ftp = DependencyService.Get<IFtpWebRequest>();
			string result = await ftp.DownloadFile("demo2014", "demo2014", remotePath, localPath);
			if (result.StartsWith("221"))
			{
				var file = await FileSystem.Current.GetFileFromPathAsync(localPath);
				if (file != null)
				{
					Desc.Text = "Deserializing " + tblName.ToLower() + ".json";
					string qry = "DELETE FROM " + tblName.ToLower();
					string str = await file.ReadAllTextAsync();

					var settings = new JsonSerializerSettings();
					settings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
					settings.NullValueHandling = NullValueHandling.Ignore;
					FacileJson<T> json = JsonConvert.DeserializeObject<FacileJson<T>>(str, settings);

					Desc.Text = "Importing " + tblName.ToLower() + " " + json.Records;
					var dbcon = DependencyService.Get<ISQLiteDb>().GetConnection();
					await dbcon.ExecuteAsync(qry);
					await dbcon.InsertAllAsync(json.Data);



					//var rows = await dbcon.Table<Fatture>().CountAsync();


					await file.DeleteAsync();
				}
			}
			else
			{
				await DisplayAlert("Result", result, "ok");
				return (false);
			}

			return (true);
		}
*/
		public DownloadPage()
		{
			InitializeComponent();


			//HttpWebRequest httpRequest;
			//            FtpWebRequest ftpRequest;

			//          FtpWebResponse ftpResponse;

			//            try
			//           {

			//Settings required to establish a connection with the server
			// httpRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri("ftp://facile2013.it/age00001/FileName.txt"));


			//httpRequest.Method = WebRequestMethods..Ftp.UploadFile;
			/*
			this.ftpRequest.Proxy = null;
			this.ftpRequest.UseBinary = true;
			this.ftpRequest.Credentials = new NetworkCredential("UserName", "Password");

			/*
			//Selection of file to be uploaded
			FileInfo ff = new FileInfo("File Local Path With File Name");//e.g.: c:\\Test.txt
			byte[] fileContents = new byte[ff.Length];

			//will destroy the object immediately after being used
			using (FileStream fr = ff.OpenRead())
			{
				fr.Read(fileContents, 0, Convert.ToInt32(ff.Length));
			}

			using (Stream writer = ftpRequest.GetRequestStream())
			{
				writer.Write(fileContents, 0, fileContents.Length);
			}
			//Gets the FtpWebResponse of the uploading operation
			this.ftpResponse = (FtpWebResponse)this.ftpRequest.GetResponse();
			Response.Write(this.ftpResponse.StatusDescription); //Display response
			*/
			//         }
			//         catch (WebException webex)
			//         {
			//             DisplayAlert("Attenzione",webex.ToString(),"ok");
			//         }


		}
	}
}
