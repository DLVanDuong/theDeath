using UnityEngine;

[CreateAssetMenu(fileName = "Character/Th�ng Tin", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    public float maxhealth = 100f;
    public float attack = 10f;
    public float defense = 5f;
    public float speed = 4f;
    public float RunSpeed = 6f;

}
