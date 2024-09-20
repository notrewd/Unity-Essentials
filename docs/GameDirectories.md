# Game Directories

{% hint style="info" %}
**Essentials.Core.GameDirectories** namespace is needed for this.
{& endhint %}

Game Directories allows you to create and visualize game folders without the need to write any code.
It can be found in the **Essentials >> Game Directories** menu button.

<img width="606" alt="Game Directories window" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/72d8dea7-9e50-46bd-a7c7-a4d184196093"/>

## Creating Directories

You can create new directories by filling in the name or path and clicking the **Create Directory** button. If the button is greyed out, it means that the name or path is incorrect.

## Nesting Directories

You can nest a directory inside another by specifying the path to that directory, for example **_My Folder/Nested Folder_**. If the **_My Folder_** directory does not exist, a new one will be created.

## Directory References

To understand how to use Game Directories in code, you need to know about **directory references**. Each directory has its own directory reference which is used in code to reference the actual directory. You need to set these references in the **Settings** menu in the bottom left corner in the Game Directories window.

<img width="547" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/ed9b39f5-dea1-4281-98ae-97a8f695c8e5">

_References can't have whitespaces or start with a number._

## Using Directories In Code

Game Directories automatically, upon applying changes, generates all the needed code for the directories to be created and to function. To use them in code all you need to do is include `Essentials.Core.GameDirectories` namespace and reference the target directory using `DirectoriesList.<DIRECTORY_REFERENCE>` by default, which will return the path to the target directory.

## Example Of Referencing MyFolder Directory

```cs
using Essentials.Core.GameDirectories;
using UnityEngine;

public class MyGameDataSaver : MonoBehaviour
{
    private string _pathToDirectory;

    private void Start()
    {
        _pathToDirectory = DirectoriesList.MyFolder;
    }
}
```

It is a good practice to cache the reference when you are sure of that the directory will not get deleted during runtime, since each call to DirectoriesList actually checks whether that directory exists.

```cs
using System.IO;
using Essentials.Core.GameDirectories;
using UnityEngine;

public class MyGameDataSaver : MonoBehaviour
{
    [SerializeField] private string[] _dataArray;

    private void Start()
    {
        SaveData();
    }

    private void SaveData()
    {
        // Good idea to actually cache the value before the loop so the GameDirectories system won't have to recheck the folder every time.
        string directoryPath = DirectoriesList.MyFolder;

        foreach (string data in _dataArray)
        {
            string currentData = File.ReadAllText(directoryPath);
            File.WriteAllText(directoryPath, currentData + data + "\n");
        }
    }
}
```

## Modifying DirectoriesList Class

You can also modify the name of DirectoriesList class or even its location. All of that can be done in the **Settings** menu.
