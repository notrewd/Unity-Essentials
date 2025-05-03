using System;
using UnityEngine;

namespace Essentials.Inspector
{
    /// <summary>
    /// Attribute to make a field read-only in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}