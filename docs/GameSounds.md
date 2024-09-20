# Game Sounds

{% hint style="info" %}
**Essentials.Core.GameSounds** namespace is needed for this.
{& endhint %}

Game Sounds allows you to create in-game audio with just only a few lines of code.

## Playing a GameSound

Playing a GameSound is really easy and a simple process using the `GameSounds.PlaySound()` method. That's all! Nothing less, nothing more.

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        GameSounds.PlaySound(_audioClip);
    }
}
```

_Upon the creation of a GameSound, a new GameObject will be created with an audio source that is attached to it. This GameObject is automatically managed by the GameSounds system. If not specified otherwise, the GameSound will be destroyed once the audio finishes playing._

## Configuring a GameSound

Now, you probably want a little bit more flexibility when it comes to playing sounds. What about volume? Or spatial blending? For those cases, you can **create** a GameSound, configure it, and then play it. You can create a sound by calling the `GameSounds.CreateSound()` method and then you can chain methods on to it to configure it. Remember to always call `Play()` at the end if you want to play your sound afterwards.

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        GameSounds.CreateSound(_audioClip)
            .SetVolume(0.5f) // sets the volume to 0.5
            .SetSpatialBlend(true) // sets the spatial blend to 1
            .SetPosition(new Vector3(5, 0, 3)) // sets the position to 5, 0, 3
            .Play(); // and finally, plays the sound
    }
}
```

## Setting an ID

Each GameSounds has its own unique ID. If not specified, the ID will be generated at random. You can specify the ID using the `SetId()` method. You can use the GameSound's ID to see if it is playing by using the `GameSounds.IsPlaying` method, or to simply stop it using the `GameSounds.StopSound()` method.

```cs
using System.Collections;
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        GameSounds.CreateSound(_audioClip)
            .SetId("MySound") // sets the id
            .Play();

        StartCoroutine(StopSoundAfter(5f));
    }

    private IEnumerator StopSoundAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (GameSounds.IsPlaying("MySound"))
        {
            GameSounds.StopSound("MySound");
        }
    }
}
```

## Configuring GameSounds Default Settings

If you don't like the defaults that a GameSound comes with when it is created, perhaps you want every single GameSound that is created to be in a different audio mixer group, or perhaps they all should have lower volumes, then you can use the **GameSounds Editor** to edit those defaults. It is located in the **Essentials >> Game Sounds** menu. Each GameSound that is created will inherit properties from those defaults unless they are overridden by their own configuration.

<img width="574" alt="GameSounds window" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/67e1d26d-b420-4dbd-ae61-990f8e90c96f">

## Creating Groups

Apart from the default settings which are applied to every single GameSound that is created, you can create your own presets called groups and then assign them to a GameSound. A new group can be created by clicking on the _**New Group**_ button in the **Groups** section.

<img width="574" alt="GameSounds window" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/d846b988-b53d-4236-8493-f305c9f787a1">

## Configuring Groups

Each group can be configured by clicking on the _**Edit**_ button. Upon doing so, a new window appears with the group properties that is very similar to the default properties in the GameSounds window. From there you can configure your newly created group.

<img width="532" alt="GameSound group window" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/b1fa6fc5-940d-492e-81b5-5c5f9f05f9b4">

## Assigning a Group

To use a group, you need to assign it to a GameSound. You can do so by using the `SetGroup()` method. Notice that the `SetGroup()` method accepts the **group name** as its parameter. This name is case sensitive.

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        GameSounds.CreateSound(_audioClip)
            .SetGroup("My Group 1")
            .Play();
    }
}
```

## Overwriting a Group

Perhaps you want to apply a group to a GameSound, but then change one of the settings to something else. You can do this by setting the group and then changing the target setting.

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        GameSounds.CreateSound(_audioClip)
            .SetGroup("My Group 1") // Assign this GameSound to a 'My Group 1' group
            .SetVolume(0.5f) // Overwrite the volume to 0.5f
            .SetSpatialBlend(true) // Overwrite the spatial blend to true
            .Play();
    }
}
```

_Notice that we are setting the group **BEFORE** setting any of the other settings. Changing the settings and then setting a group will make the group overwrite all of the settings set before._

## Advanced Use Cases

For more extreme use cases, you can control a GameSound directly by getting its audio source. **The GameSound needs to be already playing to get the audio source since the audio source is created only when the GameSound starts playing.**

```cs
using Essentials.Core.GameSounds;
using UnityEngine;

public class MySoundCreator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private void Start()
    {
        // Cache the GameSound for later use
        GameSound gameSound = GameSounds.CreateSound(_audioClip);

        gameSound
            .SetId("MySound")
            .SetVolume(0.5f)
            .Play();

        AudioSource audioSource = gameSound.GetAudioSource();
    }
}
```

## Methods

### void GameSounds.CreateSound(AudioClip audioClip)

Creates a GameSound with the specified audio clip.

### void GameSounds.PlaySound(AudioClip audioClip)

Creates and plays a GameSound with the specified audio clip.

### void GameSounds.PlaySound(string id, AudioClip audioClip)

Creates, sets the ID and plays a GameSound with the specified audio clip.

### void GameSounds.StopSound(string id)

Stops all sounds with the specified ID.

### void GameSounds.StopAllSounds()

Stops all sounds regardless of ID.

### bool GameSounds.IsPlaying(string id)

Checks whether a GameSounds is playing or not.

### void GameSound.SetId(string id)

Sets the specified ID for a GameSound.

### void GameSound.SetGroup(string groupName)

Sets the specified group to a GameSound.

### void GameSound.SetClip(AudioClip audioClip)

Sets the specified AudioClip for a GameSound.

### void GameSound.SetParent(Transform parent, bool worldPositionStays = false)

Sets the specified parent for a GameSound.

### void GameSound.SetPosition(Vector 3 position, bool isLocalPosition = false)

Sets the specified position/local position for a GameSound.

### void GameSound.SetVolume(float volume)

Sets the specified volume for a GameSound.

### void GameSound.SetLoop(bool loop)

Sets whether the GameSound should loop.

### void GameSound.SetPriority(int priority)

Sets the specified priority for a GameSound.

### void GameSound.SetSpatialBlend(float spatialBlend)

Sets the specified spatial blend for a GameSound.

### void GameSound.SetSpatialBlend(bool enabled)

Enables or disabled the spatial blend entirely for a GameSound.

### void GameSound.SetSpatialize(bool spatialize)

Sets whether the GameSound should be spatialized.

### void GameSound.SetDopplerLevel(float dopplerLevel)

Sets the specified doppler level for a GameSound.

### void GameSound.SetMinDistance(float minDistance)

Sets the specified minimum distance for a GameSound.

### void GameSound.SetMaxDistance(float maxDistance)

Sets the specified maximum distance for a GameSound.

### void GameSound.SetPanStereo(float panStereo)

Sets the specified pan stereo for a GameSound.

### void GameSound.SetReverbZoneMix(float reverbZoneMix)

Sets the specified reverb zone mix for a GameSound.

### void GameSound.SetAudioMixerGroup(AudioMixerGroup audioMixerGroup)

Sets the specified audio mixer group for a GameSound.

### void GameSound.SetDoNotDestroy(bool doNotDestroy)

Sets whether the GameSound should be destroyed after it is done playing. True by default.

### void GameSound.Play()

Plays the GameSound.

### void GameSound.Stop()

Stops the GameSound.

### string GameSound.GetId()

Gets the GameSound ID.

### GameObject GameSound.GetGameObject()

Gets the GameSound GameObject. Usable only after calling the `Play()` method.

### AudioSource GameSound.GetAudioSource()

Gets the GameSound audio source. Usable only after calling the `Play()` method.
