using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace Input
{
    public class Input : MonoBehaviour, InputActions.IGameplayActions
    {
        [SerializeField]
        private Camera _camera = default;

        [SerializeField]
        private float _maxTapTime = 0.4f;
        
        [SerializeField]
        private float _maxTapDistance = 10f;
        
        private InputActions _inputActions;
        private Vector2 _pressedPosition;
        private bool _wasPressed;
        private double _tapTime;

        private void OnEnable()
        {
            _inputActions = new InputActions();
            _inputActions.Gameplay.SetCallbacks(this);
            _inputActions.Gameplay.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Gameplay.Disable();
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            var pointer = context.ReadValue<PointerInput>();

            if (pointer.Contact && !_wasPressed)
            {
                _wasPressed = true;
                _tapTime = context.time;
                _pressedPosition = pointer.Position;
            }
            else if (!pointer.Contact && _wasPressed)
            {
                if((pointer.Position - _pressedPosition).SqrMagnitude() < _maxTapDistance * _maxTapDistance && _tapTime - context.time < _maxTapTime)
                    Tap(pointer.Position);
                _wasPressed = false;
            }
        }

        private void Tap(Vector2 position)
        {
            var ray = _camera.ScreenPointToRay(position);
            if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity, int.MaxValue))
                return;
            
            
            var inputHandler = hit.collider.GetComponent<RayCastDispatcher>();
            if (inputHandler != null) 
                inputHandler.DispatchRayCast();
        }
    }

    public struct PointerInput
    {
        public bool Contact;
        public Vector2 Position;
    }
    
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public class PointerInputComposite : InputBindingComposite<PointerInput>
    {
        [InputControl(layout = "Button")]
        public int contact;

        [InputControl(layout = "Vector2")]
        public int position;

        public override PointerInput ReadValue(ref InputBindingCompositeContext context)
        {
            var contact = context.ReadValueAsButton(this.contact);
            var position = context.ReadValue<Vector2, Vector2MagnitudeComparer>(this.position);

            return new PointerInput
            {
                Contact = contact,
                Position = position,
            };
        }

        #if UNITY_EDITOR
        static PointerInputComposite()
        {
            Register();
        }
        #endif

        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            InputSystem.RegisterBindingComposite<PointerInputComposite>();
        }
    }
}