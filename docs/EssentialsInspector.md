# Essentials Inspector

{% hint style="info" %}
**Essentials.Inspector** namespace is needed for this.
{% endhint %}

The Essentials Inspector has a few attributes that can help you customize your inspector properties a little bit better.

## FieldColor

A `FieldColor` attribute changes the color of a property's field.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    [FieldColor(255, 0, 0)] public int _aRedProperty;
    [FieldColor(0, 255, 0)] public int _aGreenProperty;
    [FieldColor(0, 0, 255)] public int _aBlueProperty;
}
```

<img width="346" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/f9f2c140-b8e6-4928-af9a-7a7856287183">

## LabelColor

A `LabelColor` attribute changes the color of a property's label and value.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    [LabelColor(255, 0, 0)] public int _aRedLabel;
    [LabelColor(0, 255, 0)] public int _aGreenLabel;
    [LabelColor(0, 0, 255)] public int _aBlueLabel;
}
```

<img width="346" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/e5a0a5de-fc5f-41a1-ba4b-9518497a5a5d">

## ReadOnly

A `ReadOnly` attribute disables the ability to modify a property and makes it greyed out.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public int normalProperty;
    [ReadOnly] public int readOnlyProperty;
}
```

<img width="346" alt="Screenshot 2024-07-04 at 11 18 35" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/2f8e238e-0d60-4a3f-a2f6-a6bbd579f3dd">

## SetIndentLevel

A `SetIndentLevel` attribute sets the indent level of a property.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public int normalProperty;
    [SetIndentLevel(1)] public int indentedProperty;
}
```

<img width="405" alt="Screenshot 2024-07-04 at 11 21 12" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/703af060-61c6-403f-a02a-fdb987045da5">

## ShowIf

A `ShowIf` attribute shows a property based on a specified condition.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public bool propertyVisible;
    [ShowIf("propertyVisible", true)] public int showIfProperty;
}
```

<img width="405" alt="Screenshot 2024-07-04 at 11 23 41" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/096d8b5b-ebbe-46bc-a888-b21a6bb18338">
<img width="405" alt="Screenshot 2024-07-04 at 11 23 52" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/aaf0881e-b9fb-4bee-9ddb-9839bf7c0ea2">

## HideIf

A `HideIf` attribute hides a property based on a specified condition.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public bool propertyHidden;
    [HideIf("propertyHidden", true)] public int hideIfProperty;
}
```

<img width="405" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/2508e28a-0424-4283-95c0-9e4efb3b86f2">
<img width="405" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/aa422841-c654-4ff7-8180-8d6a280a215d">

## DisableIf

A `DisableIf` attribute makes a property read-only based on a specified condition.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public bool propertyDisabled;
    [DisableIf("propertyDisabled", true)] public int disableIfProperty;
}
```

<img width="405" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/a5aa7cb4-9a53-4068-b5a9-a2c39f3e5877">
<img width="405" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/fab7735f-87a0-4f3e-8866-e0e987aa7177">

## EnableIf

A `EnableIf` attribute makes a property writable based on a specified condition.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public bool propertyEnabled;
    [EnableIf("propertyEnabled", true)] public int enableIfProperty;
}
```

<img width="405" alt="Screenshot 2024-07-06 at 14 43 45" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/443c1dd9-95e8-4339-af95-ed5fbd936fb5">
<img width="405" alt="Screenshot 2024-07-06 at 14 43 53" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/a16fcd3a-418d-4ccc-adcb-03a33709dae0">

## Comparing Values

Attributes like `ShowIf`, `HideIf`, `EnableIf`, `DisableIf`, and so on..., can compare not only boolean values but almost any kind of value. Most notably `float`, `int` and `string`.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public int intCondition;
    [ShowIf("intCondition", 2)] public int showInt;

    public float floatCondition;
    [ShowIf("floatCondition", 1.5f)] public float showFloat;

    public string stringCondition;
    [ShowIf("stringCondition", "MyString")] public string showString;
}
```

### Comparing Int and Float Values With Operators

What if you want the value to be for example less or equal than something? For that you can make it a string and insert operators.
<br />
Valid operators are: `>`, `>=`, `<`, `<=`, `==`, `!=`.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public int lessCondition;
    [ShowIf("lessOrEqualCondition", "<2")] public int showIfLess;

    public int lessOrEqualCondition;
    [ShowIf("lessOrEqualCondition", "<=2")] public int showIfLessOrEqual;

    public int greaterCondition;
    [ShowIf("greaterOrEqualCondition", ">2")] public int showIfGreater;

    public int greaterOrEqualCondition;
    [ShowIf("greaterOrEqualCondition", ">=2")] public int showIfGreaterOrEqual;

    public int equalCondition;
    [ShowIf("equalCondition", "==2")] public int showIfEqual;

    public int notEqualCondition;
    [ShowIf("notEqualCondition", "!=2")] public int showIfNotEqual;
}
```

The same goes with float values.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public float lessCondition;
    [ShowIf("lessCondition", "<2.5f")] public float showIfLess;

    public float lessOrEqualCondition;
    [ShowIf("lessOrEqualCondition", "<=2.5f")] public float showIfLessOrEqual;

    public float greaterCondition;
    [ShowIf("greaterCondition", ">2.5f")] public float showIfGreater;

    public float greaterOrEqualCondition;
    [ShowIf("greaterOrEqualCondition", ">=2.5f")] public float showIfGreaterOrEqual;

    public float equalCondition;
    [ShowIf("equalCondition", "==2.5f")] public float showIfEqual;

    public float notEqualCondition;
    [ShowIf("notEqualCondition", "!=2.5f")] public float showIfNotEqual;
}
```

### Comparing Multiple Values

You are not restricted to comparing only one value. You can compare multiple values. Simply separate them by a comma.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public float multipleValueCondition;
    [ShowIf("multipleValueCondition", ">2f", "<3f")] public float multipleValueProperty;
}
```

### Compare Types

By default, when comparing multiple values, the compare type is set to `CompareType.All` which means that all of the values need to be satisfied. To change this, simply pass `CompareType.Any` to an attribute before passing any values. Now only one value needs to be satisfied.

```cs
using Essentials.Inspector;
using UnityEngine;

public class MyCustomClass : MonoBehaviour
{
    public float multipleValueCondition;
    [ShowIf("multipleValueCondition", CompareType.Any, "<3f", ">10f")] public float multipleValueProperty;
}
```
