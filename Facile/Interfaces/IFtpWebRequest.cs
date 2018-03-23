using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facile.Interfaces
{
    public interface IFtpWebRequest
    {
        Task<string> UploadFile(string FtpUrl, string fileName, string userName, string password, string UploadDirectory = "");
        Task<string> DownloadFile(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath);
		Task<List<string>> ListDirectory(string userName, string password, string ftpRemotePath);
    
    }
}
