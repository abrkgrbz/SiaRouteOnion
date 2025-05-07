namespace SiaRoute.WebApp.Helpers
{
    public class CensoringHelper
    {
        public static string NameCensoring(string name)
        {
            string[] words = name.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 2)
                {
                    words[i] = words[i].Substring(0, 2) + new string('*', words[i].Length - 2);
                }
                else
                {
                    words[i] = new string('*', words[i].Length);
                }
            }

            return string.Join(' ', words);
        }
    }
}
