using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Camera/Screen Shake Profile", fileName = "Screen Shake Profile"), HelpURL("https://www.youtube.com/watch?v=CgyLIWyDXqo&t=479s")]
public class ScreenShakeProfileSO : ScriptableObject
{
    [Title("Impluse Source Settings")]
    public float ImpulseDuration = 0.2f;
    public float ImpluseForce = 1f;
    public Vector3 DefaultVelocity = new Vector3(0, -1, 0);
    public AnimationCurve ImpluseCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Title("Impulse Listener Settings")]
    public float ListenerAmplitude = 1f;
    public float ListenerFrequency = 1f;
    public float ListenerDuration = 1f;

}
