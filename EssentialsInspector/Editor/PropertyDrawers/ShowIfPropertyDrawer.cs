using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Essentials.Serialization;
using UnityEditor;
using UnityEngine;

namespace Essentials.Inspector
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private bool isShown;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIfAttribute = (ShowIfAttribute)attribute;
            SerializedProperty condition = property.serializedObject.FindProperty(showIfAttribute.conditionName);

            if (condition == null)
            {
                DisplayError(position, ErrorType.InvalidConditionName);
                return;
            }
            if (showIfAttribute.compareValue == null && showIfAttribute.compareValues == null)
            {
                DisplayError(position, ErrorType.InvalidCompareValue);
                return;
            }

            switch (condition.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (showIfAttribute.compareValue is int) isShown = (int)showIfAttribute.compareValue == condition.intValue;
                    else if (showIfAttribute.compareValue is string)
                    {
                        if (string.IsNullOrWhiteSpace(showIfAttribute.compareValue.ToString()) || !showIfAttribute.compareValue.ToString().Any(char.IsDigit))
                        {
                            DisplayError(position, ErrorType.InvalidInt);
                            return;
                        }

                        string operation = EssentialsSerialization.GetOperationFromString(showIfAttribute.compareValue.ToString());

                        if (operation == null && int.TryParse(showIfAttribute.compareValue.ToString(), out int value))
                        {
                            isShown = value == condition.intValue;
                        }
                        else if (operation != null && int.TryParse(showIfAttribute.compareValue.ToString().Substring(operation.Length), out value))
                        {
                            isShown = operation switch
                            {
                                ">=" => condition.intValue >= value,
                                "<=" => condition.intValue <= value,
                                ">" => condition.intValue > value,
                                "<" => condition.intValue < value,
                                "!=" => condition.intValue != value,
                                "==" => condition.intValue == value,
                                _ => false
                            };
                        }
                        else
                        {
                            DisplayError(position, ErrorType.InvalidInt);
                            return;
                        }
                    }
                    else if (showIfAttribute.compareValues != null)
                    {
                        string[] values = showIfAttribute.compareValues.Select(x => x.ToString()).ToArray();

                        if (!values.All(x => x.All(char.IsDigit)))
                        {
                            DisplayError(position, ErrorType.InvalidInt);
                            return;
                        }

                        isShown = values.Any(x => int.Parse(x) == condition.intValue);
                    }
                    break;
                case SerializedPropertyType.Boolean:
                    if (showIfAttribute.compareValue is bool) isShown = (bool)showIfAttribute.compareValue == condition.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    if (showIfAttribute.compareValue is float) isShown = (float)showIfAttribute.compareValue == condition.floatValue;
                    else if (showIfAttribute.compareValue is int) isShown = (int)showIfAttribute.compareValue == condition.floatValue;
                    else if (showIfAttribute.compareValue is string)
                    {
                        if (string.IsNullOrWhiteSpace(showIfAttribute.compareValue.ToString()) || !showIfAttribute.compareValue.ToString().Any(char.IsDigit))
                        {
                            DisplayError(position, ErrorType.InvalidFloat);
                            return;
                        }

                        string operation = EssentialsSerialization.GetOperationFromString(showIfAttribute.compareValue.ToString());
                        string pureFloat = EssentialsSerialization.GetPureStringFloat(showIfAttribute.compareValue.ToString());

                        if (operation == null && float.TryParse(pureFloat, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            isShown = value == condition.floatValue;
                        }
                        else if (operation != null && float.TryParse(pureFloat.Substring(operation.Length), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                        {
                            isShown = operation switch
                            {
                                ">=" => condition.floatValue >= value,
                                "<=" => condition.floatValue <= value,
                                ">" => condition.floatValue > value,
                                "<" => condition.floatValue < value,
                                "!=" => condition.floatValue != value,
                                "==" => condition.floatValue == value,
                                _ => false
                            };
                        }
                        else
                        {
                            DisplayError(position, ErrorType.InvalidFloat);
                            return;
                        }
                    }
                    else if (showIfAttribute.compareValues != null)
                    {
                        List<float> values = new List<float>();

                        foreach (object valueObj in showIfAttribute.compareValues)
                        {
                            switch (valueObj)
                            {
                                case float obj:
                                    values.Add(obj);
                                    break;
                                case int obj:
                                    values.Add(obj);
                                    break;
                                case string _ when float.TryParse(EssentialsSerialization.GetPureStringFloat(valueObj.ToString()), NumberStyles.Float, CultureInfo.InvariantCulture, out float value):
                                    values.Add(value);
                                    break;
                                default:
                                    DisplayError(position, ErrorType.InvalidFloat);
                                    return;
                            }
                        }

                        isShown = values.Contains(condition.floatValue);
                    }
                    break;
                case SerializedPropertyType.String:
                    if (showIfAttribute.compareValue is string) isShown = showIfAttribute.compareValue.ToString() == condition.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    if (showIfAttribute.compareValue is Color) isShown = (Color)showIfAttribute.compareValue == condition.colorValue;
                    break;
                case SerializedPropertyType.Enum:
                    if (showIfAttribute.compareValue is Enum) isShown = Enum.GetValues(showIfAttribute.compareValue.GetType()).GetValue(condition.enumValueIndex).ToString() == showIfAttribute.compareValue.ToString();
                    else if (showIfAttribute.compareValues != null) isShown = showIfAttribute.compareValues.Select(x => x.ToString()).Contains(Enum.GetValues(showIfAttribute.compareValues[0].GetType()).GetValue(condition.enumValueIndex).ToString());
                    break;
                case SerializedPropertyType.Vector2:
                    if (showIfAttribute.compareValue is Vector2) isShown = (Vector2)showIfAttribute.compareValue == condition.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    if (showIfAttribute.compareValue is Vector3) isShown = (Vector3)showIfAttribute.compareValue == condition.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    if (showIfAttribute.compareValue is Vector4) isShown = (Vector4)showIfAttribute.compareValue == condition.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    if (showIfAttribute.compareValue is Rect) isShown = (Rect)showIfAttribute.compareValue == condition.rectValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    if (showIfAttribute.compareValue is AnimationCurve) isShown = ((AnimationCurve)showIfAttribute.compareValue).Equals(condition.animationCurveValue);
                    break;
                case SerializedPropertyType.Bounds:
                    if (showIfAttribute.compareValue is Bounds) isShown = (Bounds)showIfAttribute.compareValue == condition.boundsValue;
                    break;
                case SerializedPropertyType.Quaternion:
                    if (showIfAttribute.compareValue is Quaternion) isShown = (Quaternion)showIfAttribute.compareValue == condition.quaternionValue;
                    break;
                case SerializedPropertyType.Vector2Int:
                    if (showIfAttribute.compareValue is Vector2Int) isShown = (Vector2Int)showIfAttribute.compareValue == condition.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    if (showIfAttribute.compareValue is Vector3Int) isShown = (Vector3Int)showIfAttribute.compareValue == condition.vector3IntValue;
                    break;
                case SerializedPropertyType.BoundsInt:
                    if (showIfAttribute.compareValue is BoundsInt) isShown = (BoundsInt)showIfAttribute.compareValue == condition.boundsIntValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    if (showIfAttribute.compareValue is string)
                    {
                        string value = showIfAttribute.compareValue.ToString();

                        if (string.IsNullOrWhiteSpace(value) || value != "!=null")
                        {
                            DisplayError(position, ErrorType.InvalidCompareValue);
                            return;
                        }

                        isShown = condition.objectReferenceValue != null;
                    }
                    break;
                default:
                    DisplayError(position, ErrorType.InvalidCompareType);
                    break;
            }

            if (isShown) EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (isShown) return EditorGUI.GetPropertyHeight(property);
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private void DisplayError(Rect position, ErrorType errorType)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            GUI.Label(position, "Error", style);

            const string prefix = "Essentials Inspector:";

            switch (errorType)
            {
                case ErrorType.InvalidInt:
                    Debug.LogError($"{prefix} ShowIf attribute has invalid int value.");
                    break;
                case ErrorType.InvalidFloat:
                    Debug.LogError($"{prefix} ShowIf attribute has invalid float value.");
                    break;
                case ErrorType.InvalidCompareType:
                    Debug.LogError($"{prefix} ShowIf attribute has invalid compare type.");
                    break;
                case ErrorType.InvalidCompareValue:
                    Debug.LogError($"{prefix} ShowIf attribute has invalid compare value.");
                    break;
                case ErrorType.InvalidConditionName:
                    Debug.LogError($"{prefix} ShowIf attribute has invalid condition name.");
                    break;
                default:
                    Debug.LogError($"{prefix} An unknown error has occured.");
                    break;
            }
        }

        private enum ErrorType { InvalidInt, InvalidCompareType, InvalidCompareValue, InvalidConditionName, InvalidFloat }
    }
}