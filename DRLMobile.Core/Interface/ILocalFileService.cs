namespace DRLMobile.Core.Interface
{
    public interface ILocalFileService
    {
        bool IsSrcZipFolderExist(SrcZipFileType type);
        string GetLocalFilePathByFileType(SrcZipFileType type, string urlOrFileName);
        string GetLocalFilePathByFileTypeForCustomerDocument(string urlOrFileName);
    }
}
