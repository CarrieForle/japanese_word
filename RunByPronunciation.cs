using System.Text.RegularExpressions;

namespace lol.Run;

public class RunByPronunciation : IRun
{
    private string? _filePath;
    private static RunByPronunciation _instance = new();

    private RunByPronunciation() { }

    public static RunByPronunciation GetInstance()
    {
        return _instance;
    }

    public static void InitializeInstance(string path) {
        _instance._filePath = path;
    }

    public void Run()
    {
        try
        {
            string[] contents = File.ReadAllLines(_filePath!);

            List<Vocabulary> vocabularies = [];
            Random random = new();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var query = from line in contents
                        where !string.IsNullOrEmpty(line) && !new string[] { "#", "!", ">" }.Where(line.StartsWith).Select(str => str).Any()
                        select line;

            Console.WriteLine($"{query.Count()} queries has been loaded!\n");

            foreach (string str in query)
            {
                string line = str;
                Vocabulary vocab = new();

                var matches = Regex.Matches(line, @"\[(.+?)\]\(.*?\)");

                foreach (Match match in matches)
                {
                    line = line.Replace(match.Value, match.Groups[1].Value);
                }

                try
                {
                    int first_bracket = line.IndexOf("（");
                    int second_bracket = line.IndexOf("） ");

                    vocab.word = line[..first_bracket];
                    vocab.pronunciation = line[(1 + first_bracket)..second_bracket];
                    vocab.description = line[(2 + second_bracket)..];
                }
                catch
                {
                    string[] a = line.Split(' ', 2);
                    vocab.word = a[0];
                    vocab.pronunciation = string.Empty;
                    vocab.description = a[1];
                }

                vocabularies.Add(vocab);
            }

            while (true)
            {
                vocabularies.Sort((x, y) => random.Next(-1, 2));

                for (int i = 0; i < vocabularies.Count; i++)
                {
                    var vo = vocabularies[i];
                    vo.description = vo.description.Trim();

                    if (vo.pronunciation != string.Empty)
                    {
                        Console.WriteLine(vo.pronunciation);
                    }
                    else
                    {
                        Console.WriteLine(vo.word);
                    }

                    if (Console.ReadLine() == "q") {
                        return;
                    }
                    
                    Console.Write($"\t{vo.word}");

                    if (vo.pronunciation.Trim() != string.Empty)
                    {
                        Console.Write($" ({vo.pronunciation})");
                    }
                    if (vo.description.Trim() != string.Empty)
                    {
                        Console.Write($" {vo.description}");
                    }

                    Console.WriteLine("\n");
                }

                Console.WriteLine($"You have reviewed all {query.Count()} queries! Do you want to do it again in random order? (y or n)");

                while (Console.ReadLine() is string v)
                {
                    if (v == "y")
                    {
                        break;
                    }
                    else if (v == "n")
                    {
                        return;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}\n{e.StackTrace}");
        }
    }
}