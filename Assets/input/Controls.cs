// GENERATED AUTOMATICALLY FROM 'Assets/input/Controls.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Controls : IInputActionCollection
{
    private InputActionAsset asset;
    public Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""GlobalActions"",
            ""id"": ""8a6bf452-3efb-41a3-8194-b811718b7ee7"",
            ""actions"": [
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""66a096f1-81ee-4428-9710-33581b3aa5d1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f76ed92c-c61d-417e-9270-e5239ed52fdb"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c82a3ba-164b-45d0-b741-c11a9ef6f940"",
                    ""path"": ""*/{Menu}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""BipedActions"",
            ""id"": ""d6ae1441-3532-4728-91ce-ffb7c481ad44"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""ea778ef3-39b8-4cf4-8ded-d222be21a5ba"",
                    ""expectedControlType"": ""Dpad"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""9c7ce899-70d3-4d23-bada-d4c9ae6dcd70"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""0f8d3974-ed11-452e-8a05-04f2e72ad7a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Graphook"",
                    ""type"": ""Button"",
                    ""id"": ""6dbe8df2-50f6-4b00-bc6e-c66715b6a2ae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WorldSelect"",
                    ""type"": ""Button"",
                    ""id"": ""24499f08-18e1-495f-80fa-640349852f96"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shield"",
                    ""type"": ""Button"",
                    ""id"": ""d52c1089-e44d-413e-81b6-a1a4c2b3d425"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""c65feaea-6a5d-449c-9dac-bae3057beaaa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""db260e37-2539-4250-933c-7940bbfe0a77"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextWeapon"",
                    ""type"": ""Button"",
                    ""id"": ""27765ef2-3b6d-4042-9473-0b48f0484ad8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Charge"",
                    ""type"": ""Button"",
                    ""id"": ""0de15a2d-4eb1-4c7f-88b8-4ad5c15ee7b1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""e0eb1488-3fe2-4bc1-9139-5ce1cd2e5c7e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""183a8e71-261e-4e84-8f4b-41fb87dbf152"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DefaultControlScheme"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0363b3e5-c2f0-4973-a672-74c8a5327745"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e34fd6ec-e277-4c8e-9685-a84cc8d376e0"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WorldSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5522710d-c697-4bb9-8a37-b5735381ae90"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WorldSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f92359fc-9e18-4be4-bf50-4a61fb36ab8b"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f7c4e10-bc56-489b-a7f3-113a1d228899"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shield"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c33449f1-9335-4213-908c-6d1182446ad2"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0a7a2a72-f465-4804-b4ea-0167400c9146"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a064992-593a-4449-abcf-b99f6e210f04"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a1816db8-1011-4465-831c-9f094d661dcf"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8a413ad-0f47-424c-988c-02c8e67b95e4"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50d5685e-afe5-46d0-9647-3b62dcabb456"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextWeapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6709029a-2603-4009-ba2e-0ba7ee55ddb6"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Graphook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8156ca34-3bd2-4e66-a3d4-b1e7bf7997ca"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f024a6aa-b0ca-46a1-8772-95f99be6d559"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8a3a97d3-ba40-4722-91f4-6774c5f2a55d"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55094766-01aa-451d-946f-f7db7e95e361"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DefaultControlScheme"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6bd66471-2006-4a56-90a3-19c7988c7710"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""DefaultControlScheme"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b3ac1316-608f-41bb-9f2b-d41b09c1058a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d3f07979-0e2c-416b-a38f-c9fb5b3c1939"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2f3c9569-b29c-4373-a73a-5b9ed21e0224"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1acc43be-678d-409b-bb1d-4fd06c2f2c99"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d61d60b6-9486-4b0f-b4fa-115c33ebf196"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""65ed0b02-90bc-4c06-a3c8-a34a9d32efe4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f1cff8fd-ce5f-489f-aba5-170b2cafcc8b"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5e6c9c67-5c49-441e-8f51-616f9d5826e0"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8a9544ee-efb0-477a-a7c3-7e2cc532b74c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3efd785c-980e-4dc0-9ec2-25bc92deff2b"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""DefaultControlScheme"",
            ""bindingGroup"": ""DefaultControlScheme"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // GlobalActions
        m_GlobalActions = asset.FindActionMap("GlobalActions", throwIfNotFound: true);
        m_GlobalActions_Menu = m_GlobalActions.FindAction("Menu", throwIfNotFound: true);
        // BipedActions
        m_BipedActions = asset.FindActionMap("BipedActions", throwIfNotFound: true);
        m_BipedActions_Move = m_BipedActions.FindAction("Move", throwIfNotFound: true);
        m_BipedActions_Aim = m_BipedActions.FindAction("Aim", throwIfNotFound: true);
        m_BipedActions_Fire = m_BipedActions.FindAction("Fire", throwIfNotFound: true);
        m_BipedActions_Graphook = m_BipedActions.FindAction("Graphook", throwIfNotFound: true);
        m_BipedActions_WorldSelect = m_BipedActions.FindAction("WorldSelect", throwIfNotFound: true);
        m_BipedActions_Shield = m_BipedActions.FindAction("Shield", throwIfNotFound: true);
        m_BipedActions_Dash = m_BipedActions.FindAction("Dash", throwIfNotFound: true);
        m_BipedActions_Jump = m_BipedActions.FindAction("Jump", throwIfNotFound: true);
        m_BipedActions_NextWeapon = m_BipedActions.FindAction("NextWeapon", throwIfNotFound: true);
        m_BipedActions_Charge = m_BipedActions.FindAction("Charge", throwIfNotFound: true);
        m_BipedActions_Down = m_BipedActions.FindAction("Down", throwIfNotFound: true);
    }

    ~Controls()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // GlobalActions
    private readonly InputActionMap m_GlobalActions;
    private IGlobalActionsActions m_GlobalActionsActionsCallbackInterface;
    private readonly InputAction m_GlobalActions_Menu;
    public struct GlobalActionsActions
    {
        private Controls m_Wrapper;
        public GlobalActionsActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menu => m_Wrapper.m_GlobalActions_Menu;
        public InputActionMap Get() { return m_Wrapper.m_GlobalActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActionsActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActionsActions instance)
        {
            if (m_Wrapper.m_GlobalActionsActionsCallbackInterface != null)
            {
                Menu.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
                Menu.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
                Menu.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
            }
            m_Wrapper.m_GlobalActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                Menu.started += instance.OnMenu;
                Menu.performed += instance.OnMenu;
                Menu.canceled += instance.OnMenu;
            }
        }
    }
    public GlobalActionsActions @GlobalActions => new GlobalActionsActions(this);

    // BipedActions
    private readonly InputActionMap m_BipedActions;
    private IBipedActionsActions m_BipedActionsActionsCallbackInterface;
    private readonly InputAction m_BipedActions_Move;
    private readonly InputAction m_BipedActions_Aim;
    private readonly InputAction m_BipedActions_Fire;
    private readonly InputAction m_BipedActions_Graphook;
    private readonly InputAction m_BipedActions_WorldSelect;
    private readonly InputAction m_BipedActions_Shield;
    private readonly InputAction m_BipedActions_Dash;
    private readonly InputAction m_BipedActions_Jump;
    private readonly InputAction m_BipedActions_NextWeapon;
    private readonly InputAction m_BipedActions_Charge;
    private readonly InputAction m_BipedActions_Down;
    public struct BipedActionsActions
    {
        private Controls m_Wrapper;
        public BipedActionsActions(Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_BipedActions_Move;
        public InputAction @Aim => m_Wrapper.m_BipedActions_Aim;
        public InputAction @Fire => m_Wrapper.m_BipedActions_Fire;
        public InputAction @Graphook => m_Wrapper.m_BipedActions_Graphook;
        public InputAction @WorldSelect => m_Wrapper.m_BipedActions_WorldSelect;
        public InputAction @Shield => m_Wrapper.m_BipedActions_Shield;
        public InputAction @Dash => m_Wrapper.m_BipedActions_Dash;
        public InputAction @Jump => m_Wrapper.m_BipedActions_Jump;
        public InputAction @NextWeapon => m_Wrapper.m_BipedActions_NextWeapon;
        public InputAction @Charge => m_Wrapper.m_BipedActions_Charge;
        public InputAction @Down => m_Wrapper.m_BipedActions_Down;
        public InputActionMap Get() { return m_Wrapper.m_BipedActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BipedActionsActions set) { return set.Get(); }
        public void SetCallbacks(IBipedActionsActions instance)
        {
            if (m_Wrapper.m_BipedActionsActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
                Aim.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                Aim.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                Aim.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                Fire.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                Fire.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                Fire.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                Graphook.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                Graphook.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                Graphook.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                WorldSelect.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnWorldSelect;
                WorldSelect.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnWorldSelect;
                WorldSelect.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnWorldSelect;
                Shield.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                Shield.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                Shield.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                Dash.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                Dash.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                Dash.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                Jump.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                Jump.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                Jump.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                NextWeapon.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                NextWeapon.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                NextWeapon.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                Charge.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                Charge.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                Charge.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                Down.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                Down.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                Down.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
            }
            m_Wrapper.m_BipedActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Aim.started += instance.OnAim;
                Aim.performed += instance.OnAim;
                Aim.canceled += instance.OnAim;
                Fire.started += instance.OnFire;
                Fire.performed += instance.OnFire;
                Fire.canceled += instance.OnFire;
                Graphook.started += instance.OnGraphook;
                Graphook.performed += instance.OnGraphook;
                Graphook.canceled += instance.OnGraphook;
                WorldSelect.started += instance.OnWorldSelect;
                WorldSelect.performed += instance.OnWorldSelect;
                WorldSelect.canceled += instance.OnWorldSelect;
                Shield.started += instance.OnShield;
                Shield.performed += instance.OnShield;
                Shield.canceled += instance.OnShield;
                Dash.started += instance.OnDash;
                Dash.performed += instance.OnDash;
                Dash.canceled += instance.OnDash;
                Jump.started += instance.OnJump;
                Jump.performed += instance.OnJump;
                Jump.canceled += instance.OnJump;
                NextWeapon.started += instance.OnNextWeapon;
                NextWeapon.performed += instance.OnNextWeapon;
                NextWeapon.canceled += instance.OnNextWeapon;
                Charge.started += instance.OnCharge;
                Charge.performed += instance.OnCharge;
                Charge.canceled += instance.OnCharge;
                Down.started += instance.OnDown;
                Down.performed += instance.OnDown;
                Down.canceled += instance.OnDown;
            }
        }
    }
    public BipedActionsActions @BipedActions => new BipedActionsActions(this);
    private int m_DefaultControlSchemeSchemeIndex = -1;
    public InputControlScheme DefaultControlSchemeScheme
    {
        get
        {
            if (m_DefaultControlSchemeSchemeIndex == -1) m_DefaultControlSchemeSchemeIndex = asset.FindControlSchemeIndex("DefaultControlScheme");
            return asset.controlSchemes[m_DefaultControlSchemeSchemeIndex];
        }
    }
    public interface IGlobalActionsActions
    {
        void OnMenu(InputAction.CallbackContext context);
    }
    public interface IBipedActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnGraphook(InputAction.CallbackContext context);
        void OnWorldSelect(InputAction.CallbackContext context);
        void OnShield(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
    }
}
