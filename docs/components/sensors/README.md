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

# Sensors

{% hint style="info" %}
**Essentials.Core.Sensors** namespace is needed for this.
{% endhint %}

Sensors allows you to sense different objects in the game world using a bunch of configurable raycasts. It utilizes the Jobs system and Burst compilation for the best efficiency.

## Getting Started

To get started, you'll need two components: **Sensors** and **Sensors Receiver**.

**Sensors** component is the one which is the source of the raycasts. It is typically placed on a camera to allow the camera to detect game objects that are in view. By default, the sensors are cast horizontally, and are static. You can change the default behavior by modifying the sensors component's settings. More on that in the [Sensors Component](sensors-component.md) section.

**Sensors Receiver**, on the other hand, is the one used to receive the raycasts cast from the Sensors component. It is typically attached to any game object that requires to be detected by the Sensors component. By default, the sensors receiver hides the renderer of the game object until it is sensed by the Sensors component. You can hange the default behavior by modifying the sensors receiver component's settings. More on that in the [Sensors Receiver Component](sensors-receiver-component.md) section.
