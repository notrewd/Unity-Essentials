using System;
using UnityEngine;

namespace Essentials.Inspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute {}
}