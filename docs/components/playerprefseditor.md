# PlayerPrefs Editor

The PlayerPrefs Editor allows you to create, see and delete PlayerPrefs in realtime inside Unity Editor without any code. It can be found in the **Essentials >> PlayerPrefs Editor** menu button.

![Screenshot of PlayerPrefs Editor window](https://github.com/NotRewd/Unity-Essentials/assets/48103943/d6bd6612-3849-42ab-92a9-cb0f54b0910e)

## What Are PlayerPrefs

PlayerPrefs is a system created by Unity for easy persistent data saving. It uses key-value pairs to achieve this. Unfortunately, Unity doesn't provide any meaningful way to see all of the created PlayerPrefs. That's where this feature comes in!

## Creating PlayerPrefs

You can create new PlayerPrefs in the PlayerPrefs Editor window by pressing the **New PlayerPref** button. When creating, you'll be asked for a new key, value and a value type. Upon creation, the new PlayerPref will be displayed in the PlayerPrefs Editor window.

![Screenshot of New PlayerPref window](https://github.com/NotRewd/Unity-Essentials/assets/48103943/968fd991-3f2f-4487-9d59-a213be2ee0a5)

## Modifying PlayerPrefs

You can directly modify PlayerPrefs by clicking on any one of the listed PlayerPrefs in the PlayerPrefs Editor window and changing its value. You can also change the type of the said PlayerPref by clicking on the dropdown next to the value.

![Screenshot 2024-07-03 at 17 23 56](https://github.com/NotRewd/Unity-Essentials/assets/48103943/9be71a9c-fff0-4bbb-9a3e-38d8b844bdb3)

## Deleting PlayerPrefs

You can delete a PlayerPref by right clicking on it and choosing **Delete PlayerPref**.

![Screenshot 2024-07-03 at 17 24 57](https://github.com/NotRewd/Unity-Essentials/assets/48103943/ecd28590-d7ca-488d-a963-ee46fbab769b)

## Applying Changes

All of the changes will take effect only when applied, meaning that you must hit the **Apply** button for these changed to take effect. Not applying the changes will result in every single change being reverted, including newly created PlayerPrefs, modified PlayerPrefs, and even deleted PlayerPrefs.

## Filtering PlayerPrefs

You can search for and filter different PlayerPrefs based on the searched key, value or value type. You can do so by clicking on the magnifying glass icon in the search bar.

![image](https://github.com/NotRewd/Unity-Essentials/assets/48103943/b59d228a-7bff-4bf4-a554-e2939b6e0571)

## EditorPrefs

PlayerPrefs Editor has also the capability to show you EditorPrefs. These are internal PlayerPrefs that are used by the Unity Editor, and other third-party plugins and packages. Be very careful when modifying these because even a slight mistake may result in the Unity Editor not behaving correctly. You can also perform the same actions such as creating, modifying, deleting and filtering on EditorPrefs.

![Screenshot 2024-07-03 at 17 27 27](https://github.com/NotRewd/Unity-Essentials/assets/48103943/d6bba6c0-d1dc-400a-a3dc-40994c3e2d86)

## Show Internal PlayerPrefs

When editing PlayerPrefs, there is a checkbox below that says **Show Internal PlayerPrefs**. This shows the PlayerPrefs that the Unity Editor uses internally, and should never be changed manually.
