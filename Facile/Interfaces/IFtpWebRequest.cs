using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facile.Interfaces
{
	public class ListingData
	{
		private string _name;
		private DateTime? _last_update;

		public ListingData(string name, DateTime? data)
		{
			Name = name;
			LastUpdate = data;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public DateTime? LastUpdate
		{
			get { return _last_update; }
			set { _last_update = value; }
		}
	}

    public interface IFtpWebRequest
    {
        Task<string> UploadFile(string FtpUrl, string fileName, string userName, string password, string UploadDirectory = "");
        Task<string> DownloadFile(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath);
		Task<List<ListingData>> ListDirectory(string userName, string password, string ftpRemotePath);
    }
}
