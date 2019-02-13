# Lady.Event

Why `Lady` ? No one knows...

### Usage in Unity
```c#
// somewhere in a monobehavior scripts
void Start()
{
    GameObject aChild = transform.GetChild(0).GetChild(0).gameObject;
    
    // listen "Yolo" event on aChild
    Lady.Event.On(aChild, "Yolo", e => {

        Debug.Log("yeah propagation works!");
        Debug.Log(e.currentTarget);

    });
    
    // dispatch an event from the current gameObject to every child (recursively, children and grandchildren)
    Lady.Event.Dispatch(gameObject, "Yolo", propagation: currentTarget => Kit.Utils.Children(currentTarget));
}
```

Ok, but what is `Kit.Utils.Children` ?  
Because Unity do not provide something like `transform.children` or `transform.GetAllChildren`...
```c#
namespace Kit
{
    public static class Utils
    {
        public static GameObject[] Children(GameObject gameObject)
        {
            GameObject[] gameObjects = new GameObject[gameObject.transform.childCount];

            for (int index = gameObject.transform.childCount - 1; index >= 0; index--)
                gameObjects[index] = gameObject.transform.GetChild(index).gameObject;

            return gameObjects;
        }

        public static GameObject[] Children(object @object)
        {
            return Children(@object as GameObject);
        }
    }
}
```
