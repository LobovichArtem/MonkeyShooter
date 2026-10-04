using UnityEngine;

[CreateAssetMenu(menuName = "Controller/MouseLookSettings")]
public class MouseLookSettigns : ScriptableObject
{
    [field: SerializeField] public float Sensitivity { get; private set; } = 5f;
    [field: SerializeField] public Vector2 MinMaxX { get; private set; } = new Vector2(-360, 360);
    [field: SerializeField] public Vector2 MinMaxY { get; private set; } = new Vector2(-90, 90);
    [field: SerializeField] public float SmoothSpeed { get; private set; } = 20;

}

