namespace Sort;

public static class ListReader
{
    public static bool WithError { get; private set;  }

    private static List<string>? GetStringListFromFile(string path)
    {
        WithError = true;
        
        var str = "";
        try
        {
            using (var streamReader = new StreamReader(path))
            {
                str = streamReader.ReadToEnd();
                WithError = false;
            }
        }
        catch
        {
            WithError = true;
        }

        return WithError ? null : str.Split(' ').ToList();
    }

    public static List<int> GetIntListFromFile(string path)
    {
        var strList = GetStringListFromFile(path);

        var intList = new List<int>();
        
        if (WithError)
        {
            return intList;
        }

        foreach (var item in strList)
        {
            if (!int.TryParse(item, out var j))
            {
                WithError = true;
                return intList;
            }
            intList.Add(j);
        }

        return intList;
    }

    private static string GetStringFromIntList(List<int> list)
    {
        return string.Join(' ', list);
    }

    public static void WriteIntListInFile(List<int> list, string path)
    {
        WithError = true;
        try
        {
            using (var streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine(GetStringFromIntList(list));
                WithError = false;
            }
        }
        catch
        {
            WithError = true;
        }
    }
}