using System;
using System.IO;
using System.Text;
using System.IO.Compression;

public static class CompressedFileHelper
{
    public static byte[] CompressText(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        
        var output = new MemoryStream();
        using (var gZipStream = new GZipStream(output, CompressionMode.Compress))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        return output.ToArray();

    }

    public static string DecompressText(byte[] bytes)
    {

        var input = new MemoryStream(bytes);
        var output = new MemoryStream();

        using (var gZipStream = new GZipStream(input, CompressionMode.Decompress))
        {
            gZipStream.CopyTo(output);
            var outputBytes = output.ToArray();
            return Encoding.UTF8.GetString(outputBytes);
        }
    }

    public static void CompressToFile(string fileName, string text)
    {
        var bytes = CompressText(text);
        File.WriteAllBytes(fileName, bytes);
    }

    public static string DecompressFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File not found: {fileName}");
        }

        var bytes = File.ReadAllBytes(fileName);
        var text = DecompressText(bytes);
        return text;
    }
}

