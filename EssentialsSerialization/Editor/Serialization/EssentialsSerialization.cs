using UnityEditor;

namespace Essentials.Serialization
{
    public static class EssentialsSerialization
    {
        public static SerializedProperty GetSerializedPropertyFromList(SerializedProperty list, int index)
        {
            if (!list.isArray) return null;

            list.Next(true);
            list.Next(true);

            int listLength = list.intValue;

            for (int i = 0; i < listLength; i++)
            {
                list.Next(false);
                if (i == index) return list;
            }

            return null;
        }

        public static string GetOperationFromString(string s)
        {
            if (s.Length <= 1) return null;
            if (char.IsDigit(s[0]) || s[0] == '.') return null;
            if (s.Length >= 3 && (!char.IsDigit(s[1]) || s[1] == '.'))
            {
                string operation = s.Substring(0, 2);

                if (operation == ">=" || operation == "<=" || operation == "!=" || operation == "==") return operation;
                return null;
            }
            if (s[0] == '>' || s[0] == '<') return s[0].ToString();
            return null;
        }

        public static string GetPureStringFloat(string s)
        {
            return s[^1] == 'f' ? s.Remove(s.Length - 1) : s;
        }
    }
}