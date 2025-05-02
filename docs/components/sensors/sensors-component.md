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

# Sensors Component

{% hint style="info" %}
**Essentials.Core.Sensors** namespace is needed for this.
{% endhint %}

The Sensors component is used to sense game objects by casting a bunch of raycasts in parallel. It utilizes the Jobs system and Burst compilation for the best efficiency possible.

<div align="left"><figure><img src="../../.gitbook/assets/image.png" alt="" width="362"><figcaption><p>Sensors component properties</p></figcaption></figure></div>

{% hint style="danger" %}
Due to how the sensors count is implemented in code, you **SHOULD** **NOT** change the sensors count property during runtime. Doing so will result in a bunch of errors.
{% endhint %}

## Configuring the Scanning

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 12.53.31.png" alt="" width="356"><figcaption><p>Static scan method properties</p></figcaption></figure></div>

The scanning category has a bunch of configuration options that control how the sensors should behave, and whether they are static or dynamic.

### Scan Methods

Scan method is the main option responsible for the behavior of the sensors. There are two categories of scan methods: **static** (which is the static scan method) and **dynamic** (which are the rest of the methods).

### Static Scan Method

The default one is the **static** **scan method**, which means that the sensors cast are stationary and there is no dynamic movement involved.

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 10.08.35.png" alt="" width="375"><figcaption><p>Static horizontal sensors</p></figcaption></figure></div>

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 10.09.53.png" alt="" width="375"><figcaption><p>Static vertical sensors</p></figcaption></figure></div>

### Vertical Scan Method

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 12.55.33.png" alt="" width="356"><figcaption><p>Vertical scan method properties</p></figcaption></figure></div>

The **vertical** **scan method** is used to dynamically move the sensors during runtime in a vertical motion. Useful for when you want to cover large vertical areas with just a few sensors.

The **scan angle amplitude** property tells the system how much the sensors move up and down.

The **scan angle frequency** property tells the system how fast it moves up and down.

<div align="left"><figure><img src="../../.gitbook/assets/Screen Recording 2025-01-16 at 13.04.30.gif" alt="" width="375"><figcaption><p>Vertical scan method with horizontal sensors orientation</p></figcaption></figure></div>

### Horizontal Scan Method

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 13.10.51.png" alt="" width="356"><figcaption><p>Horizontal scan method properties</p></figcaption></figure></div>

The **horizontal** **scan method** is used to dynamically move the sensors during runtime in a horizontal motion. Useful for when you want to cover large horizontal areas with just a few sensors.

The **scan angle amplitude** property tells the system how much the sensors move left and right.

The **scan angle frequency** property tells the system how fast it moves left and right.

<div align="left"><figure><img src="../../.gitbook/assets/Screen Recording 2025-01-16 at 13.13.10.gif" alt="" width="375"><figcaption><p>Horizontal scan method with vertical sensors orientation</p></figcaption></figure></div>

### Random Scan Method

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-16 at 13.32.28.png" alt="" width="356"><figcaption><p>Random scan method properties</p></figcaption></figure></div>

The **random** **scan method** moves the sensors by a random value up, down, left or right, depending on the configuration.

The **random scan type** property defines whether the random movement is **horizontal** – left and right, or **vertical** – up and down.

The **horizontal or vertical randomization** property (depends on the random scan type) defines what is the maximum amount that the sensors can move randomly in the configured direction.

<div align="left"><figure><img src="../../.gitbook/assets/Screen Recording 2025-01-16 at 13.30.09.gif" alt="" width="375"><figcaption><p>Random horizontal scan method with vertical random scan type</p></figcaption></figure></div>

### Sensors Orientation

The **sensors orientation** property controls whether the sensors should be laid out vertically, horizontally or in both directions, depending on the use case. This property is available for every scan method.

## Configuring the Sensors

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-19 at 09.50.14.png" alt="" width="356"><figcaption><p>Sensors category</p></figcaption></figure></div>

The sensors can be configured in the Sensors category, which contains the sensors count, sensors angle and sensors range properties.

* **Sensors Count** – Sets the amount of sensors that should be cast from the game object.
* **Sensors Angle** – Sets the spread angle of the sensors in degrees that should be cast from the game object.
* **Sensors Range** – Sets the maximum allowed range in meters that the sensors can travel.

{% hint style="info" %}
The **Sensors Angle** property is split into two – vertical and horizontal – when the sensors orientation is set to **Both**.
{% endhint %}

## Configuring the Sensors ID

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-19 at 10.03.33.png" alt="" width="351"><figcaption><p>Sensors ID property</p></figcaption></figure></div>

The Sensors component detects only those receivers that have the same ID as the Sensors Receiver component. By default, every Sensors and Sensors Receiver component has the same ID, which is **0**. This means that **every** **Sensors component** detects **every** **Sensors Receiver component**. To change this, you can modify the Sensors ID property so that only **certain sensors detect certain receivers**.

## Debugging the Sensors

<div align="left"><figure><img src="../../.gitbook/assets/Screenshot 2025-01-19 at 10.12.05.png" alt="" width="357"><figcaption><p>Debug category</p></figcaption></figure></div>

There are two options available for debugging that provide a visualization of the sensors in action.

* **Show Sensors** – Shows the sensors being cast from the game object during runtime.
* **Show Sensor Hits** – Turns on color coding. Sensors that do not detect any game objects are shown in <mark style="color:green;">**green**</mark>, indicating a clear path, while sensors that register a hit, meaning they have detected a game object, are displayed in <mark style="color:red;">**red**</mark>.
