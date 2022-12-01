using UnityEngine;

public class EventConsumer : MonoBehaviour
{
    public void Step(string FootName)
    {
        Debug.LogFormat("Step with foot " + FootName);
    }

    public void PunchSound1(AnimationEvent animationEvent)
    {
        string l_StringParameter = animationEvent.stringParameter;
        float l_FloatParameter = animationEvent.floatParameter;
        int l_IntParameter = animationEvent.intParameter;
        Object l_ObjectParameter = animationEvent.objectReferenceParameter;
    }
}