using LeitTemporImport.A_Domain.Interfaces.Services.App;

namespace LeitTemporImport.B_UseCases.Services.App
{
    public class SR_FileScanner : IS_FileScanner
    {
        public IEnumerable<FileInfo> GetMdbFiles(
            string directoryPath,
            string prefix,
            string extension)
        {
            var dir = new DirectoryInfo(directoryPath);

            if (!dir.Exists)
                return Enumerable.Empty<FileInfo>();

            return dir.GetFiles($"*{extension}", SearchOption.TopDirectoryOnly)
                      .Where(f => f.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                      .OrderBy(f => f.Name);
        }
    }
}