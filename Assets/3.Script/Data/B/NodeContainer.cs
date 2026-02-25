using UnityEngine;
public class NodeContainer : MonoBehaviour
{
    public Floors[] floors;
#if UNITY_EDITOR
    [ContextMenu("NodeContainer Create")]
    public void FloorCount()
    {
        floors = new Floors[6];
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i] = new Floors();
        }
        floors[0].Floor = new GameObject[1];
        floors[1].Floor = new GameObject[2];
        floors[2].Floor = new GameObject[3];
        floors[3].Floor = new GameObject[4];
        floors[4].Floor = new GameObject[4];
        floors[5].Floor = new GameObject[1];
    }
#endif
}
[System.Serializable]
public class Floors
{
    public GameObject[] Floor;
}
