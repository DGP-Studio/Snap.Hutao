using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public partial class UnUsedResourceStringTest
{
    [TestMethod]
    public void GetAllUnUsedResourceStrings()
    {
        string path = AppContext.BaseDirectory;
        for(int i = 0; i < 5; i++)
        {
            path = Path.GetDirectoryName(path)!;
        }

        string resxPath = Path.Combine(path, @"Snap.Hutao\Resource\Localization\SH.resx");
        MatchCollection matches = DataRegex.Matches(File.ReadAllText(resxPath));
        HashSet<string> strings = matches.Select(m => m.Groups[1].Value).ToHashSet();


        foreach (ref string file in Directory.GetFiles(path,"*", SearchOption.AllDirectories).AsSpan())
        {
            if (Path.GetExtension(file) is not (".cs" or ".xaml"))
            {
                continue;
            }

            if (file.Contains("Snap.Hutao.Test") || file.Contains("Snap.Hutao\\obj") || file.Contains("Snap.Hutao\\bin"))
            {
                continue;
            }

            string content = File.ReadAllText(file);
            foreach (string str in strings)
            {
                if (content.Contains(str, StringComparison.Ordinal))
                {
                    strings.Remove(str);
                }
            }
        }

        foreach (string str in strings)
        {
            Console.WriteLine(str);
        }
    }

    [GeneratedRegex("""
        data name="(.*?)" xml:space="preserve"
        """)]
    private partial Regex DataRegex { get; }
}