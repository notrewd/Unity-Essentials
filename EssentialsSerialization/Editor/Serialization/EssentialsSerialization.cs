using System;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

        public static bool CompareValues(SerializedProperty property, object compareValue)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (compareValue is int) return (int)compareValue == property.intValue;

                    if (compareValue is string)
                    {
                        if (string.IsNullOrWhiteSpace(compareValue.ToString()) || !compareValue.ToString().Any(char.IsDigit))
                        {
                            Debug.LogWarning("Essentials Serialization: Invalid compare value: " + compareValue);
                            return false;
                        }

                        string operation = GetOperationFromString(compareValue.ToString());

                        if (operation == null && int.TryParse(compareValue.ToString(), out int value)) return value == property.intValue;

                        if (operation != null && int.TryParse(compareValue.ToString().Substring(operation.Length), out value))
                        {
                            return operation switch
                            {
                                ">=" => property.intValue >= value,
                                "<=" => property.intValue <= value,
                                ">" => property.intValue > value,
                                "<" => property.intValue < value,
                                "!=" => property.intValue != value,
                                "==" => property.intValue == value,
                                _ => false
                            };
                        }

                        Debug.LogWarning("Essentials Serialization: Invalid compare value: " + compareValue);
                        return false;
                    }
                    break;

                case SerializedPropertyType.Boolean:
                    if (compareValue is bool boolValue) return boolValue == property.boolValue;
                    break;

                case SerializedPropertyType.Float:
                    if (compareValue is float floatValue) return floatValue == property.floatValue;
                    if (compareValue is int intValue) return intValue == property.floatValue;

                    if (compareValue is string)
                    {
                        if (string.IsNullOrWhiteSpace(compareValue.ToString()) || !compareValue.ToString().Any(char.IsDigit))
                        {
                            Debug.LogWarning("Essentials Serialization: Invalid compare value: " + compareValue);
                            return false;
                        }

                        string operation = GetOperationFromString(compareValue.ToString());
                        string pureFloat = GetPureStringFloat(compareValue.ToString());

                        if (operation == null && float.TryParse(pureFloat, NumberStyles.Float, CultureInfo.InvariantCulture, out float value)) return value == property.floatValue;

                        if (operation != null && float.TryParse(pureFloat.Substring(operation.Length), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                        {
                            return operation switch
                            {
                                ">=" => property.floatValue >= value,
                                "<=" => property.floatValue <= value,
                                ">" => property.floatValue > value,
                                "<" => property.floatValue < value,
                                "!=" => property.floatValue != value,
                                "==" => property.floatValue == value,
                                _ => false
                            };
                        }

                        Debug.LogWarning("Essentials Serialization: Invalid compare value: " + compareValue);
                        return false;
                    }

                    break;

                case SerializedPropertyType.String:
                    if (compareValue is string) return compareValue.ToString() == property.stringValue;
                    break;

                case SerializedPropertyType.Color:
                    if (compareValue is Color colorValue) return colorValue == property.colorValue;
                    break;

                case SerializedPropertyType.Enum:
                    if (compareValue is Enum) return Enum.GetValues(compareValue.GetType()).GetValue(property.enumValueIndex).ToString() == compareValue.ToString();
                    break;

                case SerializedPropertyType.Vector2:
                    if (compareValue is Vector2 vector2Value) return vector2Value == property.vector2Value;
                    break;

                case SerializedPropertyType.Vector3:
                    if (compareValue is Vector3 vector3Value) return vector3Value == property.vector3Value;
                    break;

                case SerializedPropertyType.Vector4:
                    if (compareValue is Vector4 vector4Value) return vector4Value == property.vector4Value;
                    break;

                case SerializedPropertyType.Rect:
                    if (compareValue is Rect rectValue) return rectValue == property.rectValue;
                    break;

                case SerializedPropertyType.AnimationCurve:
                    if (compareValue is AnimationCurve animationCurveValue) return animationCurveValue.Equals(property.animationCurveValue);
                    break;

                case SerializedPropertyType.Bounds:
                    if (compareValue is Bounds boundsValue) return boundsValue == property.boundsValue;
                    break;

                case SerializedPropertyType.Quaternion:
                    if (compareValue is Quaternion quaternionValue) return quaternionValue == property.quaternionValue;
                    break;

                case SerializedPropertyType.Vector2Int:
                    if (compareValue is Vector2Int vector2IntValue) return vector2IntValue == property.vector2IntValue;
                    break;

                case SerializedPropertyType.Vector3Int:
                    if (compareValue is Vector3Int vector3IntValue) return vector3IntValue == property.vector3IntValue;
                    break;

                case SerializedPropertyType.BoundsInt:
                    if (compareValue is BoundsInt boundsIntValue) return boundsIntValue == property.boundsIntValue;
                    break;

                case SerializedPropertyType.ObjectReference:
                    if (compareValue is string)
                    {
                        string value = compareValue.ToString();

                        if (string.IsNullOrWhiteSpace(value) || value != "!=null")
                        {
                            Debug.LogWarning("Essentials Serialization: Invalid compare value: " + compareValue);
                            return false;
                        }

                        return property.objectReferenceValue != null;
                    }
                    break;

                default:
                    Debug.LogWarning("Essentials Serialization: Unsupported compare type: " + compareValue.GetType());
                    return false;
            }

            return false;
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

        public static bool CompareValues(SerializedProperty property, object[] compareValues, CompareType compareType = CompareType.All)
        {
            if (property == null) return false;
            if (compareValues == null || compareValues.Length == 0) return false;

            foreach (object compareValue in compareValues)
            {
                bool result = CompareValues(property, compareValue);

                if (compareType == CompareType.Any && result) return true;
                if (compareType == CompareType.All && !result) return false;
            }

            if (compareType == CompareType.All) return true;

            return false;
        }
    }

    public enum CompareType
    {
        All,
        Any
    }
}