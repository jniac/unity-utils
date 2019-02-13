#Lady.Event

Why `Lady` ? No one knows...

### Usage in Unity
```c#
// somewhere in a monobehavior scripts
void Start()
{
    Lady.Event.On(transform.GetChild(0).GetChild(0).gameObject, "Yolo", e => {

        Debug.Log("yeah propagation works!");
        Debug.Log(e.currentTarget);

    });

    Lady.Event.Dispatch(gameObject, "Yolo", propagation: currentTarget => Kit.Utils.Children(currentTarget));
}
```
```c#
namespace Kit
{
    public static class Utils
    {
        public static GameObject[] Children(GameObject gameObject)
        {
            GameObject[] gameObjects = new GameObject[gameObject.transform.childCount];

            int index = 0;
            foreach (Transform transform in gameObject.transform)
                gameObjects[index++] = transform.gameObject;

            return gameObjects;
        }

        public static GameObject[] Children(object @object)
        {
            return Children(@object as GameObject);
        }
    }
}
```