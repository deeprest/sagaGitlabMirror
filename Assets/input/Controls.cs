// GENERATED AUTOMATICALLY FROM 'Assets/input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""GlobalActions"",
            ""id"": ""8a6bf452-3efb-41a3-8194-b811718b7ee7"",
            ""actions"": [
                {
                    ""name"": ""Any"",
                    ""type"": ""Button"",
                    ""id"": ""0db8ffb9-5a14-4281-adce-bfaec801389f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""66a096f1-81ee-4428-9710-33581b3aa5d1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""b258d7cd-2c65-48b0-8b9b-cb2857978f71"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Screenshot"",
                    ""type"": ""Button"",
                    ""id"": ""4746dc49-83e3-4a85-92d5-190128af6873"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CursorLockToggle"",
                    ""type"": ""Button"",
                    ""id"": ""06412df7-f38c-45d8-bfc5-13fb38f20fb9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DEV-Respawn"",
                    ""type"": ""Button"",
                    ""id"": ""cab80c60-97b6-4d13-a3a1-b86bc5188194"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dev-Slowmo"",
                    ""type"": ""Button"",
                    ""id"": ""19775fcb-27a4-4c68-8f07-b3b6f0683f97"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""002b3066-1dd4-4976-94aa-e0b637fad3cf"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f34c0a8a-1901-4f83-bce5-ff601f4d7645"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Screenshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eee96763-7ea4-4530-9f3e-84464d7219d7"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CursorLockToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e4a58d9-b12b-4af1-9579-5299f91b0375"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DEV-Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""06f2a596-acf7-4f39-8f9d-c47f8109c794"",
                    ""path"": ""<Gamepad>/*"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.5,max=1)"",
                    ""groups"": """",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e0e33ce-4408-4d79-9de1-a1a189c0b079"",
                    ""path"": ""<Keyboard>/*"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd969cd9-f853-4810-8bef-cb03c55fea89"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dev-Slowmo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MenuActions"",
            ""id"": ""d6ae1441-3532-4728-91ce-ffb7c481ad44"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""093b1a14-9986-43bd-ba26-fdc1003693fa"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Accept"",
                    ""type"": ""Button"",
                    ""id"": ""d80d6c91-51c4-400a-8444-d7405d8110e8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""a328d9b7-e09d-47a2-b2e7-778848eadcc3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""09af329b-034b-4f41-8462-ba48e6d1912f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4622c770-fe84-4d29-9857-93d665d3a99e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f0330c91-df57-476b-912b-1fdb15532724"",
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
                    ""id"": ""65189adc-a478-42fd-a3ca-469c72584040"",
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
                    ""id"": ""89a84f71-023c-4972-b9e9-c2dbcc3a4800"",
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
                    ""id"": ""040f4f64-906f-4b49-961b-1fbc695369b0"",
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
                    ""id"": ""29d3d5a9-5e61-4b84-9fec-49fcb26a42f0"",
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
                    ""id"": ""f1a9682a-e174-4359-9cf7-6e3a8288abef"",
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
                    ""id"": ""a8923858-66a3-48aa-87b7-e22c9fc9b75e"",
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
                    ""id"": ""55218dea-f72d-4693-9484-2bab6f11dd92"",
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
                    ""id"": ""1797878c-0f7e-416a-bb0b-d0d95e268cef"",
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
                    ""id"": ""5cacf1a8-d185-4009-9b9a-1e6cae3367e1"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a1d6d9f8-7b18-4330-84dc-ec5e68e99148"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa97be3f-d7f3-4c1d-a032-6fa59f3e6405"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb12337a-934d-40af-a6e4-d277553a44ab"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b87e8c3f-c650-440b-b49c-0f253e0ba8cf"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1994ef24-627b-44c6-98b7-24acaf39abf1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe93e21c-10fa-442a-9d80-8e85188beb7d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""BipedActions"",
            ""id"": ""079bfb09-e935-42f6-9d7b-a2b980110245"",
            ""actions"": [
                {
                    ""name"": ""MoveRight"",
                    ""type"": ""Button"",
                    ""id"": ""ea778ef3-39b8-4cf4-8ded-d222be21a5ba"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveLeft"",
                    ""type"": ""Button"",
                    ""id"": ""6044f14f-3d3d-4892-8e3d-16d843c8e7c6"",
                    ""expectedControlType"": ""Button"",
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
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""c65feaea-6a5d-449c-9dac-bae3057beaaa"",
                    ""expectedControlType"": """",
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
                    ""name"": ""Interact"",
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
                },
                {
                    ""name"": ""DEV-Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""daad5be2-bb09-4201-939d-730f35d6594c"",
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
                    ""groups"": ""Default"",
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
                    ""groups"": ""Default"",
                    ""action"": ""Interact"",
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
                    ""action"": ""Interact"",
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
                    ""groups"": ""Default"",
                    ""action"": ""Shield"",
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
                    ""id"": ""9034cb0f-33c4-4b97-a8ac-826ca8762534"",
                    ""path"": ""<Gamepad>/rightTrigger"",
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
                    ""id"": ""6bd66471-2006-4a56-90a3-19c7988c7710"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Default"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55094766-01aa-451d-946f-f7db7e95e361"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.5,max=1)"",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0696d703-2cb9-420e-bf32-8409870ca4d2"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DEV-Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd7888e5-dec7-4c73-8ea1-e6a6b38577b2"",
                    ""path"": ""<Gamepad>/leftStick/y"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.3,max=1)"",
                    ""groups"": """",
                    ""action"": ""DEV-Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee9cf94d-4068-47a1-a0af-0837b31bee09"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dc78c54e-77b8-4740-a068-d70957f9fb89"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e9d39cc-c374-4c3f-8e72-1ae36b32c000"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28fb37e3-976b-4178-a397-df39cda9424e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68a1a738-3e90-44b7-ae94-5bd25b6b7c40"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd4d1fdb-69d1-4ba1-9e2c-e2123d1ecae2"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
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
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Default"",
            ""bindingGroup"": ""Default"",
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
        m_GlobalActions_Any = m_GlobalActions.FindAction("Any", throwIfNotFound: true);
        m_GlobalActions_Menu = m_GlobalActions.FindAction("Menu", throwIfNotFound: true);
        m_GlobalActions_Pause = m_GlobalActions.FindAction("Pause", throwIfNotFound: true);
        m_GlobalActions_Screenshot = m_GlobalActions.FindAction("Screenshot", throwIfNotFound: true);
        m_GlobalActions_CursorLockToggle = m_GlobalActions.FindAction("CursorLockToggle", throwIfNotFound: true);
        m_GlobalActions_DEVRespawn = m_GlobalActions.FindAction("DEV-Respawn", throwIfNotFound: true);
        m_GlobalActions_DevSlowmo = m_GlobalActions.FindAction("Dev-Slowmo", throwIfNotFound: true);
        // MenuActions
        m_MenuActions = asset.FindActionMap("MenuActions", throwIfNotFound: true);
        m_MenuActions_Move = m_MenuActions.FindAction("Move", throwIfNotFound: true);
        m_MenuActions_Accept = m_MenuActions.FindAction("Accept", throwIfNotFound: true);
        m_MenuActions_Back = m_MenuActions.FindAction("Back", throwIfNotFound: true);
        m_MenuActions_Point = m_MenuActions.FindAction("Point", throwIfNotFound: true);
        m_MenuActions_Click = m_MenuActions.FindAction("Click", throwIfNotFound: true);
        // BipedActions
        m_BipedActions = asset.FindActionMap("BipedActions", throwIfNotFound: true);
        m_BipedActions_MoveRight = m_BipedActions.FindAction("MoveRight", throwIfNotFound: true);
        m_BipedActions_MoveLeft = m_BipedActions.FindAction("MoveLeft", throwIfNotFound: true);
        m_BipedActions_Jump = m_BipedActions.FindAction("Jump", throwIfNotFound: true);
        m_BipedActions_Dash = m_BipedActions.FindAction("Dash", throwIfNotFound: true);
        m_BipedActions_Aim = m_BipedActions.FindAction("Aim", throwIfNotFound: true);
        m_BipedActions_Fire = m_BipedActions.FindAction("Fire", throwIfNotFound: true);
        m_BipedActions_Graphook = m_BipedActions.FindAction("Graphook", throwIfNotFound: true);
        m_BipedActions_Interact = m_BipedActions.FindAction("Interact", throwIfNotFound: true);
        m_BipedActions_Shield = m_BipedActions.FindAction("Shield", throwIfNotFound: true);
        m_BipedActions_NextWeapon = m_BipedActions.FindAction("NextWeapon", throwIfNotFound: true);
        m_BipedActions_Charge = m_BipedActions.FindAction("Charge", throwIfNotFound: true);
        m_BipedActions_Down = m_BipedActions.FindAction("Down", throwIfNotFound: true);
        m_BipedActions_DEVZoom = m_BipedActions.FindAction("DEV-Zoom", throwIfNotFound: true);
    }

    public void Dispose()
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
    private readonly InputAction m_GlobalActions_Any;
    private readonly InputAction m_GlobalActions_Menu;
    private readonly InputAction m_GlobalActions_Pause;
    private readonly InputAction m_GlobalActions_Screenshot;
    private readonly InputAction m_GlobalActions_CursorLockToggle;
    private readonly InputAction m_GlobalActions_DEVRespawn;
    private readonly InputAction m_GlobalActions_DevSlowmo;
    public struct GlobalActionsActions
    {
        private @Controls m_Wrapper;
        public GlobalActionsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Any => m_Wrapper.m_GlobalActions_Any;
        public InputAction @Menu => m_Wrapper.m_GlobalActions_Menu;
        public InputAction @Pause => m_Wrapper.m_GlobalActions_Pause;
        public InputAction @Screenshot => m_Wrapper.m_GlobalActions_Screenshot;
        public InputAction @CursorLockToggle => m_Wrapper.m_GlobalActions_CursorLockToggle;
        public InputAction @DEVRespawn => m_Wrapper.m_GlobalActions_DEVRespawn;
        public InputAction @DevSlowmo => m_Wrapper.m_GlobalActions_DevSlowmo;
        public InputActionMap Get() { return m_Wrapper.m_GlobalActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActionsActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActionsActions instance)
        {
            if (m_Wrapper.m_GlobalActionsActionsCallbackInterface != null)
            {
                @Any.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnAny;
                @Any.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnAny;
                @Any.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnAny;
                @Menu.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMenu;
                @Pause.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnPause;
                @Screenshot.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @Screenshot.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @Screenshot.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @CursorLockToggle.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnCursorLockToggle;
                @CursorLockToggle.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnCursorLockToggle;
                @CursorLockToggle.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnCursorLockToggle;
                @DEVRespawn.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DEVRespawn.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DEVRespawn.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DevSlowmo.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
                @DevSlowmo.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
                @DevSlowmo.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
            }
            m_Wrapper.m_GlobalActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Any.started += instance.OnAny;
                @Any.performed += instance.OnAny;
                @Any.canceled += instance.OnAny;
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Screenshot.started += instance.OnScreenshot;
                @Screenshot.performed += instance.OnScreenshot;
                @Screenshot.canceled += instance.OnScreenshot;
                @CursorLockToggle.started += instance.OnCursorLockToggle;
                @CursorLockToggle.performed += instance.OnCursorLockToggle;
                @CursorLockToggle.canceled += instance.OnCursorLockToggle;
                @DEVRespawn.started += instance.OnDEVRespawn;
                @DEVRespawn.performed += instance.OnDEVRespawn;
                @DEVRespawn.canceled += instance.OnDEVRespawn;
                @DevSlowmo.started += instance.OnDevSlowmo;
                @DevSlowmo.performed += instance.OnDevSlowmo;
                @DevSlowmo.canceled += instance.OnDevSlowmo;
            }
        }
    }
    public GlobalActionsActions @GlobalActions => new GlobalActionsActions(this);

    // MenuActions
    private readonly InputActionMap m_MenuActions;
    private IMenuActionsActions m_MenuActionsActionsCallbackInterface;
    private readonly InputAction m_MenuActions_Move;
    private readonly InputAction m_MenuActions_Accept;
    private readonly InputAction m_MenuActions_Back;
    private readonly InputAction m_MenuActions_Point;
    private readonly InputAction m_MenuActions_Click;
    public struct MenuActionsActions
    {
        private @Controls m_Wrapper;
        public MenuActionsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MenuActions_Move;
        public InputAction @Accept => m_Wrapper.m_MenuActions_Accept;
        public InputAction @Back => m_Wrapper.m_MenuActions_Back;
        public InputAction @Point => m_Wrapper.m_MenuActions_Point;
        public InputAction @Click => m_Wrapper.m_MenuActions_Click;
        public InputActionMap Get() { return m_Wrapper.m_MenuActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActionsActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActionsActions instance)
        {
            if (m_Wrapper.m_MenuActionsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnMove;
                @Accept.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnAccept;
                @Accept.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnAccept;
                @Accept.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnAccept;
                @Back.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnBack;
                @Point.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPoint;
                @Point.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPoint;
                @Point.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnPoint;
                @Click.started -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_MenuActionsActionsCallbackInterface.OnClick;
            }
            m_Wrapper.m_MenuActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Accept.started += instance.OnAccept;
                @Accept.performed += instance.OnAccept;
                @Accept.canceled += instance.OnAccept;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @Point.started += instance.OnPoint;
                @Point.performed += instance.OnPoint;
                @Point.canceled += instance.OnPoint;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
            }
        }
    }
    public MenuActionsActions @MenuActions => new MenuActionsActions(this);

    // BipedActions
    private readonly InputActionMap m_BipedActions;
    private IBipedActionsActions m_BipedActionsActionsCallbackInterface;
    private readonly InputAction m_BipedActions_MoveRight;
    private readonly InputAction m_BipedActions_MoveLeft;
    private readonly InputAction m_BipedActions_Jump;
    private readonly InputAction m_BipedActions_Dash;
    private readonly InputAction m_BipedActions_Aim;
    private readonly InputAction m_BipedActions_Fire;
    private readonly InputAction m_BipedActions_Graphook;
    private readonly InputAction m_BipedActions_Interact;
    private readonly InputAction m_BipedActions_Shield;
    private readonly InputAction m_BipedActions_NextWeapon;
    private readonly InputAction m_BipedActions_Charge;
    private readonly InputAction m_BipedActions_Down;
    private readonly InputAction m_BipedActions_DEVZoom;
    public struct BipedActionsActions
    {
        private @Controls m_Wrapper;
        public BipedActionsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveRight => m_Wrapper.m_BipedActions_MoveRight;
        public InputAction @MoveLeft => m_Wrapper.m_BipedActions_MoveLeft;
        public InputAction @Jump => m_Wrapper.m_BipedActions_Jump;
        public InputAction @Dash => m_Wrapper.m_BipedActions_Dash;
        public InputAction @Aim => m_Wrapper.m_BipedActions_Aim;
        public InputAction @Fire => m_Wrapper.m_BipedActions_Fire;
        public InputAction @Graphook => m_Wrapper.m_BipedActions_Graphook;
        public InputAction @Interact => m_Wrapper.m_BipedActions_Interact;
        public InputAction @Shield => m_Wrapper.m_BipedActions_Shield;
        public InputAction @NextWeapon => m_Wrapper.m_BipedActions_NextWeapon;
        public InputAction @Charge => m_Wrapper.m_BipedActions_Charge;
        public InputAction @Down => m_Wrapper.m_BipedActions_Down;
        public InputAction @DEVZoom => m_Wrapper.m_BipedActions_DEVZoom;
        public InputActionMap Get() { return m_Wrapper.m_BipedActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BipedActionsActions set) { return set.Get(); }
        public void SetCallbacks(IBipedActionsActions instance)
        {
            if (m_Wrapper.m_BipedActionsActionsCallbackInterface != null)
            {
                @MoveRight.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveRight.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveRight.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveLeft.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @Jump.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnJump;
                @Dash.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDash;
                @Aim.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAim;
                @Fire.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                @Fire.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                @Fire.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnFire;
                @Graphook.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                @Graphook.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                @Graphook.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnGraphook;
                @Interact.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @Shield.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                @Shield.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                @Shield.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnShield;
                @NextWeapon.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @Charge.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Charge.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Charge.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Down.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                @DEVZoom.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDEVZoom;
                @DEVZoom.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDEVZoom;
                @DEVZoom.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDEVZoom;
            }
            m_Wrapper.m_BipedActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveRight.started += instance.OnMoveRight;
                @MoveRight.performed += instance.OnMoveRight;
                @MoveRight.canceled += instance.OnMoveRight;
                @MoveLeft.started += instance.OnMoveLeft;
                @MoveLeft.performed += instance.OnMoveLeft;
                @MoveLeft.canceled += instance.OnMoveLeft;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
                @Graphook.started += instance.OnGraphook;
                @Graphook.performed += instance.OnGraphook;
                @Graphook.canceled += instance.OnGraphook;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Shield.started += instance.OnShield;
                @Shield.performed += instance.OnShield;
                @Shield.canceled += instance.OnShield;
                @NextWeapon.started += instance.OnNextWeapon;
                @NextWeapon.performed += instance.OnNextWeapon;
                @NextWeapon.canceled += instance.OnNextWeapon;
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @DEVZoom.started += instance.OnDEVZoom;
                @DEVZoom.performed += instance.OnDEVZoom;
                @DEVZoom.canceled += instance.OnDEVZoom;
            }
        }
    }
    public BipedActionsActions @BipedActions => new BipedActionsActions(this);
    private int m_DefaultSchemeIndex = -1;
    public InputControlScheme DefaultScheme
    {
        get
        {
            if (m_DefaultSchemeIndex == -1) m_DefaultSchemeIndex = asset.FindControlSchemeIndex("Default");
            return asset.controlSchemes[m_DefaultSchemeIndex];
        }
    }
    public interface IGlobalActionsActions
    {
        void OnAny(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnScreenshot(InputAction.CallbackContext context);
        void OnCursorLockToggle(InputAction.CallbackContext context);
        void OnDEVRespawn(InputAction.CallbackContext context);
        void OnDevSlowmo(InputAction.CallbackContext context);
    }
    public interface IMenuActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAccept(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
    }
    public interface IBipedActionsActions
    {
        void OnMoveRight(InputAction.CallbackContext context);
        void OnMoveLeft(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnGraphook(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnShield(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnDEVZoom(InputAction.CallbackContext context);
    }
}
