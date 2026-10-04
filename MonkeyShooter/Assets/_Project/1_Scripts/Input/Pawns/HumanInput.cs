using UnityEngine;

public sealed class HumanInput : BaseInput<HumanInputData, HumanPacker>
{
    public class Place
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public Place(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
    public override string RequiredActionMap => "Human";

    private Place _placePosition = null;
    public Transform CurrentTransform { get; private set; }

    public void SetCurrentTransform(Transform tr) => CurrentTransform = tr;
    public void SetPlace(Vector3 position, Vector3 rotation)
    {
        _placePosition = new Place(position, rotation);
    }

    public Place GetPlace() 
    {
        if (_placePosition == null)
            return null;
        var p = _placePosition;
        _placePosition = null;
        return p; 
    }
}