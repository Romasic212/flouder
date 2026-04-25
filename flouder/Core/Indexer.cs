using flouder.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Indexer
{
    public List<FileMeta> Scan(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        var result = new List<FileMeta>();

        foreach (var file in files)
        {
            var info = new FileInfo(file);

            result.Add(new FileMeta
            {
                Path = file,
                Size = info.Length,
                LastModified = info.LastWriteTimeUtc,
                Hash = ComputeHash(file)
            });
        }

        return result;
    }

    private string ComputeHash(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);

        var hash = sha.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}   