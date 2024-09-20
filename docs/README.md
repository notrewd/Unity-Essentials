# Overview

A package for the Unity game engine that brings a few features I found handy during game development.

## Key Features

There are various features that focus on different aspects of the game engine. Managing game folders, easy way to create in-game sounds with code, handy editor tools and more.

### Game Directories

Game Directories helps you manage and visualize persistent game folders. A simple editor window that handles all the necessary boilerplate code for creating persistent game folders, so you don't have to touch any code and shows everything in a single place.\
![image](https://github.com/NotRewd/Unity-Essentials/assets/48103943/fa82757c-09b3-4a09-955f-e0aceccf1936)

### Game Sounds

Game Sounds allows you to create game audio during runtime using only a few lines of code. No more messing around with a bunch of audio sources.

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class PlayASound : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    private void Start() => GameSounds.PlaySound(_audioClip);
}
```

### Databases

Essentials Databases allows you to have all of your scriptable objects at one place. No more searching for scriptable objects and making various workarounds for fetching them during runtime. ![Screenshot 2024-08-02 at 13 26 04](https://github.com/user-attachments/assets/160d2e96-8f60-4cc5-9a5f-cbd66a01f051)

### PlayerPrefs Editor

Ever wanted to see your PlayerPrefs or change them through an editor? Well now you can. ![image](https://github.com/NotRewd/Unity-Essentials/assets/48103943/e24d1de9-c434-42aa-a511-414eebc8ace6)
