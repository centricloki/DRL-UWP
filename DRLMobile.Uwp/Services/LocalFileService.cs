using DRLMobile.Core.Helpers;
using DRLMobile.Core.Interface;

using System.IO;

using Windows.Storage;

namespace DRLMobile.Uwp.Services
{
    public class LocalFileService : ILocalFileService
    {
        private readonly IStorageFolder LocalFolder = ApplicationData.Current.LocalFolder;

        private string CheckFileExists(string productFolderPath, string fileName)
        {
            string result = "";
            bool doesFileExist = File.Exists(productFolderPath);
            if (doesFileExist)
            {
               // MemoryCacheService.Instance.Create<string>(fileName, () => productFolderPath);
                result = productFolderPath;
            }
            return result;
        }

        private string IsFileExists(string productFolderPath, string fileName)
        {
            string result = "";
            bool doesFileExist = File.Exists(productFolderPath);
            if (doesFileExist)
            {
                //MemoryCacheService.Instance.Create<string>(fileName, () => productFolderPath);
                result = productFolderPath;
            }
            return result;
        }

        public string GetLocalFilePathByFileType(SrcZipFileType type, string urlOrFileName)
        {
            var fileName = HelperMethods.GetNameFromURL(urlOrFileName);
            //string result = MemoryCacheService.Instance.Get<string>(fileName);
            //if (string.IsNullOrWhiteSpace(result))
            //{
            string result = "";
            switch (type)
            {
                case SrcZipFileType.Product:
                    result = CheckFileExists(Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipProductFolder, fileName), fileName);
                    break;
                case SrcZipFileType.SalesDocs:
                    result = CheckFileExists(Path.Combine(LocalFolder.Path, ApplicationConstants.SrzFileName, ApplicationConstants.SRCZipSalesDocs, fileName), fileName);
                    break;
                case SrcZipFileType.Signature:
                    result = CheckFileExists(Path.Combine(LocalFolder.Path, fileName), fileName);
                    break;
                default:
                    result = "no_product.png";
                    break;
            }
            //MemoryCacheService.Instance.GetOrCreate(fileName, () => result);
            //}            
            return result;
        }

        public string GetLocalFilePathByFileTypeForCustomerDocument(string urlOrFileName)
        {
            var fileName = HelperMethods.GetNameFromURL(urlOrFileName);

            bool doesFileExist;

            var productFolderPath = Path.Combine(LocalFolder.Path, ApplicationConstants.CustomerDocumentsSubFolder, fileName);

            doesFileExist = File.Exists(productFolderPath);

            return doesFileExist ? productFolderPath : string.Empty;
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