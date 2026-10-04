using Unity.Cinemachine;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    //private HumanInput _controller;

    //private CinemachineCamera _camera;
    

    //public void Start()
    //{        
    //    _camera = GetComponentInChildren<CinemachineCamera>();
    //    _controller = ServiceLocator.Get<HumanInput>();
    //    _controller.OnEnableControl += Enable;
    //    _controller.OnDisableControl += Disable;
    //    _controller.SetCurrentTransform(transform);
    //    Disable();
    //}

    //private void OnDestroy()
    //{
    //    _controller.OnEnableControl -= Enable;
    //    _controller.OnDisableControl -= Disable;
    //}

    //private void Update()
    //{
    //    if (_controller == null || _controller.IsActive == false)
    //        return;
    //    var input = _controller.GetInput();
    //    if (input.IsInteract)
    //        _useInteractive.Interact();
    //}

    //private void Enable()
    //{
    //    var place = _controller.GetPlace();
    //    if(place != null)
    //    {
    //        transform.position = place.Position;
    //        transform.eulerAngles = place.Rotation;
    //    }
    //    gameObject.SetActive(true);
    //    ToggleCamera(100);
    //    _useInteractive.Enable();

        
    //}
    //private void Disable()
    //{
    //    ToggleCamera(0);
    //    _useInteractive.Disable();
    //    gameObject.SetActive(false);
    //}

    //private void ToggleCamera(int priority)
    //{
    //    if (_camera != null)
    //        _camera.Priority = priority;
    //}
}
