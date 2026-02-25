using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void RequestEffect(int index)
    {
        transform.root.BroadcastMessage("AnimationEventEffect", index, SendMessageOptions.DontRequireReceiver);
    }
}