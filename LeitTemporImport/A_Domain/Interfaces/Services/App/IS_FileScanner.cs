namespace LeitTemporImport.A_Domain.Interfaces.Services.App
{
    public interface IS_FileScanner
    {
        IEnumerable<FileInfo> GetMdbFiles(
            string directoryPath,
            string prefix,
            string extension);
    }
}
