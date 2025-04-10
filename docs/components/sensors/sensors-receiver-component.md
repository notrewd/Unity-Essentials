---
layout:
  title:
    visible: true
  description:
    visible: false
  tableOfContents:
    visible: true
  outline:
    visible: true
  pagination:
    visible: true
---

# Sensors Receiver Component

{% hint style="info" %}
**Essentials.Core.Sensors** namespace is needed for this.
{% endhint %}

The Sensors Receiver component is responsible for reacting in a certain way when the sensors cast from the Sensors component are detected.

It works together with the Sensors component, and is required for the Sensors system to work.

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-03-06 at 10.09.31.png" alt="" width="373"><figcaption><p>Sensors Receiver component properties</p></figcaption></figure></div>

## Configuring the callback type

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-03-20 at 09.09.13.png" alt="" width="348"><figcaption><p>Callback type property</p></figcaption></figure></div>

The callback type determines what happens, when the sensors are detected.

By default, the callback type is set to **Disable Renderer**, which automatically disables the renderer when there are no sensors detected, and only enables it when it is being detected by the sensors.

### Assigning custom callback type

If you want to have custom behavior instead of the default one, which is disabling the renderer, you can set the callback type to **Custom**. By switching to the custom option, you can subscribe to two UnityEvent properties: **OnSensorsReceived** and **OnSensorsLost**.

**OnSensorsReceived** event is fired once when the sensors are first detected.\
**OnSensorsLost** event is fired once when the sensors are no longer being detected.

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-03-06 at 10.20.16.png" alt="" width="357"><figcaption><p>Sensors Receiver custom callback type</p></figcaption></figure></div>

## Changing the check interval

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-03-20 at 09.03.46 (1).png" alt="" width="348"><figcaption><p>Check interval property</p></figcaption></figure></div>

{% hint style="info" %}
Check interval property is defined in **seconds**.
{% endhint %}

By default, the receiver checks **every second** for any sensors being present at the moment. To optimize the performance, you can change the check interval to your needs.

{% hint style="warning" %}
Keep in mind that more frequent checks are **more** computationally heavy.
{% endhint %}

## Configuring the Sensors ID

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-03-20 at 09.15.11.png" alt="" width="364"><figcaption><p>Sensors Id property</p></figcaption></figure></div>

The Sensors Id property can be configured to detect sensors fired **only** from a certain Sensors component that **match** by the ID. By default, this property is set to **0**.
