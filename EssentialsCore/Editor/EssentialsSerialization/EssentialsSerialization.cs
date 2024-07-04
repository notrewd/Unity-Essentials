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
    }
}