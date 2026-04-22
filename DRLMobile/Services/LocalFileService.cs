

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace DRLMobile.Services
{
    public class LocalFileService : ILocalFileService
    {

        private readonly IStorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        public LocalFileService()
        {

        }



        public string GetLocalFilePathByFileType(SrcZipFileType type, string urlOrFileName)
        {
            var fileName = HelperMethods.GetNameFromURL(urlOrFileName);
            bool doesFileExist = false;
            switch (type)
            {
                case SrcZipFileType.Product:
                    var productFolderPath = Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipProductFolder, fileName);
                    doesFileExist = File.Exists(productFolderPath);
                    return doesFileExist ? productFolderPath : string.Empty;
                case SrcZipFileType.SalesDocs:
                    var salesDocsFolderPath = Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipSalesDocs, fileName);
                    doesFileExist = File.Exists(salesDocsFolderPath);
                    return doesFileExist ? salesDocsFolderPath : string.Empty;

                default:
                    return "";
            }
        }

        public bool IsSrcZipFolderExist(SrcZipFileType type)
        {
            switch (type)
            {
                case SrcZipFileType.Product:
                    var productFolder = Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipProductFolder);
                    return Directory.Exists(productFolder);
                case SrcZipFileType.SalesDocs:
                    var salesDocFolder = Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipSalesDocs);
                    return Directory.Exists(salesDocFolder);
                default:
                    return false;
            }

        }
    }
}
