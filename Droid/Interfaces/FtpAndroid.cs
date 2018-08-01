using System;
using System.IO;
using System.Net;
using Facile.Interfaces;
using Facile.Droid.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

[assembly: Xamarin.Forms.Dependency(typeof(FTP))] //You need to put this on iOS/droid class or uwp/etc if you wrote
namespace Facile.Droid.Interfaces
{
	class FTP : IFtpWebRequest
	{
		public FTP() //I saw on Xamarin documentation that it's important to NOT pass any parameter on that constructor
		{
		}

		/// Upload File to Specified FTP Url with username and password and Upload Directory if need to upload in sub folders
		///Base FtpUrl of FTP Server
		///Local Filename to Upload
		///Username of FTP Server
		///Password of FTP Server
		///[Optional]Specify sub Folder if any
		/// Status String from Server
		public async Task<string> UploadFile(string FtpUrl, string fileName, string userName, string password, string UploadDirectory = "")
		{
			return await Task.Run(() =>
			{
				try
				{

					string PureFileName = new FileInfo(fileName).Name;
					String uploadUrl = String.Format("{0}{1}/{2}", FtpUrl, UploadDirectory, PureFileName);
					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uploadUrl);
					req.Proxy = null;
					req.Method = WebRequestMethods.Ftp.UploadFile;
					req.Credentials = new NetworkCredential(userName, password);
					req.UseBinary = true;
					req.UsePassive = true;
					byte[] data = File.ReadAllBytes(fileName);
					req.ContentLength = data.Length;
					Stream stream = req.GetRequestStream();
					stream.Write(data, 0, data.Length);
					stream.Close();
					FtpWebResponse res = (FtpWebResponse)req.GetResponse();
					return res.StatusDescription;

				}
				catch (Exception err)
				{
					return err.ToString();
				}
			});
		}

		public async Task<string> DownloadFile(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath)
		{
			return await Task.Run(() =>
			{
				try
				{
					int bytesRead = 0;
					byte[] buffer = new byte[2048];
					string status = "";

					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpSourceFilePath);
					req.Method = WebRequestMethods.Ftp.DownloadFile;
					req.Proxy = null;
					req.Credentials = new NetworkCredential(userName, password);
					req.UseBinary = true;
					req.UsePassive = true;

					using (Stream reader = req.GetResponse().GetResponseStream())
					{
						using (FileStream fileStream = new FileStream(localDestinationFilePath, FileMode.Create))
						{
							while (true)
							{
								bytesRead = reader.Read(buffer, 0, buffer.Length);

								if (bytesRead == 0)
									break;

								fileStream.Write(buffer, 0, bytesRead);
							}
							reader.Close();
							fileStream.Close();

							using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
							{
								status = res.StatusDescription;
								res.Close();
							}
						}
					}
					return status;
				}
				catch (Exception err)
				{
					return err.ToString();
				}
			});
		}

		public async Task<List<ListingData>> ListDirectory(string userName, string password, string ftpServer)
		{
			return await Task.Run(() =>
			{
				List<ListingData> files = new List<ListingData>();

				try
				{
					//Create FTP request
					FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftpServer);
					//req.Method = WebRequestMethods.Ftp.ListDirectory;
					//req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
					req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
					req.Credentials = new NetworkCredential(userName, password);
					req.UsePassive = true;
					req.UseBinary = true;
					//req.KeepAlive = false;


					using (FtpWebResponse response = (FtpWebResponse)req.GetResponse())
					{
						using (Stream responseStream = response.GetResponseStream())
						{
							using (StreamReader reader = new StreamReader(responseStream))
							{
								string pattern =  @"^([\w-]+)\s+(\d+)\s+(\w+)\s+(\w+)\s+(\d+)\s+" + @"(\w+\s+\d+\s+\d+|\w+\s+\d+\s+\d+:\d+)\s+(.+)$";
								Regex regex = new Regex(pattern);
								IFormatProvider culture = CultureInfo.GetCultureInfo("en-us");
								string[] hourMinFormats = new[] { "MMM dd HH:mm", "MMM dd H:mm", "MMM d HH:mm", "MMM d H:mm" };
								string[] yearFormats = new[] { "MMM dd yyyy", "MMM d yyyy" };


								while (!reader.EndOfStream)
								{
									string line = reader.ReadLine();
									Match match = regex.Match(line);
									string permissions = match.Groups[1].Value;
									int inode = int.Parse(match.Groups[2].Value, culture);
									string owner = match.Groups[3].Value;
									string group = match.Groups[4].Value;
									long size = long.Parse(match.Groups[5].Value, culture);
									DateTime? modified;
									string s = Regex.Replace(match.Groups[6].Value, @"\s+", " ");
									if (s.IndexOf(':') >= 0)
									{
										modified = DateTime.ParseExact(s, hourMinFormats, culture, DateTimeStyles.None);
									}
									else
									{
										modified = DateTime.ParseExact(s, yearFormats, culture, DateTimeStyles.None);
									}
									string name = match.Groups[7].Value;

									var data = new ListingData(name, modified);
									files.Add(data);
								}

								//Clean-up
								reader.Close();
								responseStream.Close(); //redundant
								response.Close();
							}
						}
					}
					return files;
				}
				catch (Exception)
				{
					return null;
				}
			});
		}

	}
}
