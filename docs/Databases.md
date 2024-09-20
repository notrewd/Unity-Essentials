# Databases

{% hint style="info" %}
**Essentials.Core.Databases** namespace is needed for this.
{& endhint %}

Databases allows you to organize and manage all of your scriptable objects in one place.
<img width="843" alt="Screenshot 2024-08-02 at 13 36 08" src="https://github.com/user-attachments/assets/3971a5e9-f140-4231-bc53-88dd1b15ff56">

## Creating a Database Item

Before creating a database, we first need to create a database item that will be used to populate the database itself. Creating a database item is like creating a scriptable object, but it derives from the `DatabaseItem` class instead of the `ScriptableObject` one. In this example, we are creating a database item with two properties: name and description.

```cs
using Essentials.Core.Databases;

public class MyItem : DatabaseItem
{
    public string name;
    public string description;
}
```

_Every database item has an ID property created by default. This ID property is to help you uniquely identify a database item._

## Creating a Database

Now that we have a database item created and ready to go, we can move on to creating the database itself. To create a database, we need to create an empty class that derives from `DatabaseObject` along with the `Database` attribute that accepts the type of the database item that we want to have in our database. We also need to be able to create it from the asset menu and that's where the [`CreateAssetMenu`](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute comes in.

```cs
using UnityEngine;
using Essentials.Core.Databases;

[Database(typeof(MyItem))]
[CreateAssetMenu(fileName = "My Database", menuName = "Databases/My Database", order = 1)]
public class MyDatabase : DatabaseObject { }
```

Now we should be able to see and create a database in our Project window.

<img width="398" alt="image" src="https://github.com/user-attachments/assets/367d2f8f-a6df-45a1-9e26-6af39a8b9257">

## Working with a Database

Now that we have a database created, it's time to go and open it. We can do so by double clicking on the database or by clicking on the **Open Database** button in the inspector when the database is selected. Upon opening the database for the first time, it will look something like this. A blank database that has got nothing in it. And that's because we need to create an item.  
<img width="843" alt="image" src="https://github.com/user-attachments/assets/be701fe0-e183-4a37-ba01-24c19123074d">

## Creating an Item in a Database

To create an item, click on the **New Item** button _(by default)_ in the upper right corner of the database.  
<img width="843" alt="Untitled" src="https://github.com/user-attachments/assets/1a2b6a4c-96e5-476e-ab57-dcc94d079ada">  
Upon doing so, a new window will appear, asking for the item's name. Choose whatever name suits your newly created item best and hit **Create**.  
<img width="412" alt="Screenshot 2024-08-02 at 14 03 37" src="https://github.com/user-attachments/assets/326e1073-8e12-486e-a884-b98760367788">  
The item now appears in the database and when clicking on it, it will display all of its properties that we have defined earlier. Notice that we have not defined an **ID** property and that's because the ID property is added by default to every database item. It's also automatically generated and filled out from the item's name.  
<img width="843" alt="image" src="https://github.com/user-attachments/assets/7b148749-0573-4e69-bccc-c31eee4a95f6">

## Renaming an Item in a Database

Renaming an item can be done by right clicking on an item and choosing **Rename \<Item Name\>**. Upon doing so, a new window will appear asking for a new name.  
<img width="262" alt="image" src="https://github.com/user-attachments/assets/e422dbda-e745-4e8e-9411-6a1c23e812cb">

## Deleting an Item in a Database

Deleting an item can be done by selecting an item and clicking on the **Delete Item** button in the upper right corner of the database. It can also be done by right clicking an item and choosing **Delete \<Item Name\>**.  
<img width="843" alt="Untitled" src="https://github.com/user-attachments/assets/00a54e10-23f4-4f08-a1df-48d75935e36d">

## Item IDs

Each created item has a unique ID that can be used when fetching an item. Apart from that, IDs can be used in whatever way you'd like, for example inventory system. The database system automatically generates an ID for an item. This ID can then be changed to your liking. **Duplicate IDs shouldn't be used.**

### Database Warnings

If there is a discrepancy found when it comes to IDs, warnings will appear in the inspector when a database is selected. The warnings also include an option to automatically fix the issue, which will change the affected IDs in a way that there will be no discrepancies.  
<img width="375" alt="image" src="https://github.com/user-attachments/assets/4d6b9e0f-b13c-44b3-b831-5a468cbbdf03">

## Fetching Items from a Database

### Fetching All Items

We can fetch all of the items by having a reference to a database and calling the `GetAllItems` method. Since `GetAllItems` method returns an array of DatabaseItems, there is a friendlier, generic version of the method that casts all of the items to a specified type. Here in this example, we are using a generic method to fetch all of the items from a database and print them out.

```cs
using UnityEngine;

public class GetDatabaseItems : MonoBehaviour
{
    [SerializeField] private MyDatabase _myDatabase;

    private void Start()
    {
        MyItem[] myItems = _myDatabase.GetAllItems<MyItem>();

        foreach (MyItem myItem in myItems)
        {
            string name = myItem.name;
            string description = myItem.description;

            Debug.Log($"{name} : {description}");
        }
    }
}
```

### Fetching Items by an ID

If we want to get a specific item, we can call the `GetItem` method and pass its ID as an argument. Again, there is a generic version of this method available that is used in this example.

```cs
using UnityEngine;

public class GetDatabaseItems : MonoBehaviour
{
    [SerializeField] private MyDatabase _myDatabase;

    private void Start()
    {
        MyItem myItem = _myDatabase.GetItem<MyItem>("myItem");

        string name = myItem.name;
        string description = myItem.description;

        Debug.Log($"{name} : {description}");
    }
}
```

_Remember that IDs are **case-sensitive**. If there is more than one item with the same ID (which shouldn't happen), the first one found will be returned._

## Methods

### DatabaseItem[] DatabaseObject.GetAllItems()

Returns an array of all the DatabaseItems stored in the DatabaseObject.

### T[] DatabaseObject.GetAllItems\<T\>()

Returns an array of all the DatabaseItems casted as T in the DatabaseObject.

### DatabaseItem DatabaseObject.GetItem(string id)

Returns a DatabaseItem with the specified ID. Null is returned if none is found.

### T DatabaseObject.GetItem\<T\>(string id)

Returns a DatabaseItem casted as T with the specified ID. Null is returned if none is found.
