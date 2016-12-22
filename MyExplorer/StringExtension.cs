namespace MyExplorer
{
    public static class StringExtension
    {
        public static string Truncate(this string s, int size)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return (s.Length <= size) 
                ? s.PadRight(size)
                : (s.Substring(0, size - 3) + "...");
        }
    }
}
