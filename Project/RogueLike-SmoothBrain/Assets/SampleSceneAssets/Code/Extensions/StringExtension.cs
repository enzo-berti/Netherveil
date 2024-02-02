
using static UnityEditor.Progress;

public static class StringExtension
{
    public static string GetCamelCase(this string value)
    {
        string camelCase = string.Empty;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == ' ')
            {
                camelCase += value[i + 1].ToString().ToUpper();
                ++i;
            }
            else
            {
                camelCase += value[i];
            }
        }
        return camelCase;
    }
}
