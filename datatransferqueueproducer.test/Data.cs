using System.IO.Compression;
using System.Text;

namespace DataTransferTest;


public class Test
{
    public string Hersteller { get; set; }
    public string Artikelnummer { get; set; }
    public string Text { get; set; }
}

public class GetItems
{
    public static IEnumerable<Test> GetFirstZipTsvData(string zipfilename)
    {
        using var zip = new ZipArchive(File.OpenRead(zipfilename),ZipArchiveMode.Read);
        using var sourcesting = zip.Entries.First().Open();
        using var reader = new StreamReader(sourcesting, Encoding.Latin1);
        reader.ReadLine();
        while (!reader.EndOfStream)
        {
            var line= reader.ReadLine();
            var split = line.Split('\t');
            if (!string.IsNullOrEmpty(line) && split.Length >= 3)
            {
                    
                yield return new Test { Artikelnummer = split[1], Hersteller = split[0], Text = split[2] };
            }
        }
            
    }

    public static IEnumerable<Test> GetMockData(int count = 2000000)
    {
        int i = 0;
        while (i < count)
        {
            i++;
            yield return new Test
            {
                Artikelnummer = i.ToString(), 
                Hersteller = i.ToString(), 
                Text = "Test "+ i.ToString()
            };
        }
    }
}
