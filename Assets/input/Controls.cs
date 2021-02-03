// GENERATED AUTOMATICALLY FROM 'Assets/input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
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
                    ""type"": ""Value"",
                    ""id"": ""66a096f1-81ee-4428-9710-33581b3aa5d1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""b258d7cd-2c65-48b0-8b9b-cb2857978f71"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""4746dc49-83e3-4a85-92d5-190128af6873"",
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
                    ""name"": ""DEV-Clone"",
                    ""type"": ""Button"",
                    ""id"": ""e7af10e7-d798-4de6-a720-df64e8970763"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Screenshot"",
                    ""type"": ""Button"",
                    ""id"": ""b44b7429-04b5-48ab-9faa-297e2529bd3f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RecordToggle"",
                    ""type"": ""Button"",
                    ""id"": ""354e88f1-933e-4ba7-8642-7ad82e28fb66"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RecordPlayback"",
                    ""type"": ""Button"",
                    ""id"": ""7edecc6a-dba8-4135-b668-4f76a8f9b73a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dev-Slowmo"",
                    ""type"": ""Button"",
                    ""id"": ""93472218-24d2-4f44-b8fb-a02f293bcd50"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Minimap"",
                    ""type"": ""Button"",
                    ""id"": ""342939eb-b668-4681-9361-6f8fc5c9ac1a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9c82a3ba-164b-45d0-b741-c11a9ef6f940"",
                    ""path"": ""*/{Menu}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
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
                    ""id"": ""fe6572f3-0ae9-454f-8661-cb7048125008"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DEV-Respawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b387d4b-f92d-4ad8-949c-b8a3f9c59ec1"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c4fa5526-fcbe-4fba-b7ee-0df572b68995"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Mouse+Keyboard"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86b6b44e-93c3-40df-9f08-7137f2dac6a4"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b5de7bb-05d3-4992-a4ff-0887472392e8"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bdccc75f-7e28-4cce-ac38-4b2deb34e8be"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87158277-8853-4327-b395-f83b048dc15d"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""963a790a-c4a6-4c84-910c-33721318b3af"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1a9c887-189e-4361-8186-220907292107"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard;Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a074b853-d3bf-4dd0-b823-6c5d5ceae530"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df864519-b60e-437b-944b-9f141382b34a"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f5a9896-59a0-4c25-8a92-ed6d8663c297"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.5)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""370548f0-cff7-46f7-8e5f-821fb1402922"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone(min=0.5)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Any"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8fff7fd-3999-4672-a4b4-313f556bc93e"",
                    ""path"": ""<Keyboard>/f6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""RecordToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ea916033-f984-4091-84b2-89185b83aa0b"",
                    ""path"": ""<Keyboard>/f7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""RecordPlayback"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f158069e-88d2-4b22-9550-89da2c3b06a1"",
                    ""path"": ""<Keyboard>/f2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DEV-Clone"",
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
                    ""id"": ""6b67b0b9-40ed-43f6-97c1-457b2629bf4a"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Dev-Slowmo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d9f5a648-b68f-4c16-ae88-a27713822cf8"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Minimap"",
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
                    ""expectedControlType"": ""Button"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a1d6d9f8-7b18-4330-84dc-ec5e68e99148"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Accept"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb12337a-934d-40af-a6e4-d277553a44ab"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""464f9819-b75d-428d-a479-d0c21b8832ed"",
                    ""expectedControlType"": ""Dpad"",
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
                    ""name"": ""Ability"",
                    ""type"": ""Button"",
                    ""id"": ""6dbe8df2-50f6-4b00-bc6e-c66715b6a2ae"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Value"",
                    ""id"": ""24499f08-18e1-495f-80fa-640349852f96"",
                    ""expectedControlType"": ""Button"",
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
                    ""name"": ""NextAbility"",
                    ""type"": ""Button"",
                    ""id"": ""ba7b8c1f-eb10-4471-8c3a-c965a98b989f"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f8a413ad-0f47-424c-988c-02c8e67b95e4"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Ability"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c431a4e6-c214-4e6c-b089-ad979431500b"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Ability"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8156ca34-3bd2-4e66-a3d4-b1e7bf7997ca"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55094766-01aa-451d-946f-f7db7e95e361"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b664b3c6-cd12-4fda-a334-22f604dae9ae"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee9cf94d-4068-47a1-a0af-0837b31bee09"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e9d39cc-c374-4c3f-8e72-1ae36b32c000"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd4d1fdb-69d1-4ba1-9e2c-e2123d1ecae2"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
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
                    ""groups"": ""Mouse+Keyboard"",
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
                    ""groups"": ""Gamepad"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c5cd233-8222-45f3-8779-3e3705c9c346"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""NextAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""928334a5-83a8-4277-a82f-8e4f4dbe22a9"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""NextAbility"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""FourKeys"",
                    ""id"": ""7e77fdb2-bf83-4e00-81a2-85e67497ec07"",
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
                    ""id"": ""7a971386-2866-45f3-b5d6-8211fc8f7b0b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9493bb9b-f2d2-4a17-b7d1-ee7c66cbffea"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f8a7078-1a53-4662-8a17-6338d5599210"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""70a1a97d-2080-4e4b-a0f6-a9b8d1f5d4c7"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse+Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""FourKeys"",
                    ""id"": ""f6cf03a2-3130-4e74-8e9e-9e745420a783"",
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
                    ""id"": ""a0244e89-1f9d-4624-9380-fd37a254fff7"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3dc20b4b-bb3a-4057-bc98-d84f758c76ce"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e375bf47-8f6a-4c53-a197-7e0846859530"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""076adcca-2f64-4b9f-a3b8-691892819329"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse+Keyboard"",
            ""bindingGroup"": ""Mouse+Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
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
        m_GlobalActions_DEVRespawn = m_GlobalActions.FindAction("DEV-Respawn", throwIfNotFound: true);
        m_GlobalActions_DEVClone = m_GlobalActions.FindAction("DEV-Clone", throwIfNotFound: true);
        m_GlobalActions_Screenshot = m_GlobalActions.FindAction("Screenshot", throwIfNotFound: true);
        m_GlobalActions_RecordToggle = m_GlobalActions.FindAction("RecordToggle", throwIfNotFound: true);
        m_GlobalActions_RecordPlayback = m_GlobalActions.FindAction("RecordPlayback", throwIfNotFound: true);
        m_GlobalActions_DevSlowmo = m_GlobalActions.FindAction("Dev-Slowmo", throwIfNotFound: true);
        m_GlobalActions_Minimap = m_GlobalActions.FindAction("Minimap", throwIfNotFound: true);
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
        m_BipedActions_Move = m_BipedActions.FindAction("Move", throwIfNotFound: true);
        m_BipedActions_Jump = m_BipedActions.FindAction("Jump", throwIfNotFound: true);
        m_BipedActions_Dash = m_BipedActions.FindAction("Dash", throwIfNotFound: true);
        m_BipedActions_Aim = m_BipedActions.FindAction("Aim", throwIfNotFound: true);
        m_BipedActions_Fire = m_BipedActions.FindAction("Fire", throwIfNotFound: true);
        m_BipedActions_Ability = m_BipedActions.FindAction("Ability", throwIfNotFound: true);
        m_BipedActions_Interact = m_BipedActions.FindAction("Interact", throwIfNotFound: true);
        m_BipedActions_NextWeapon = m_BipedActions.FindAction("NextWeapon", throwIfNotFound: true);
        m_BipedActions_NextAbility = m_BipedActions.FindAction("NextAbility", throwIfNotFound: true);
        m_BipedActions_Charge = m_BipedActions.FindAction("Charge", throwIfNotFound: true);
        m_BipedActions_Down = m_BipedActions.FindAction("Down", throwIfNotFound: true);
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
    private readonly InputAction m_GlobalActions_DEVRespawn;
    private readonly InputAction m_GlobalActions_DEVClone;
    private readonly InputAction m_GlobalActions_Screenshot;
    private readonly InputAction m_GlobalActions_RecordToggle;
    private readonly InputAction m_GlobalActions_RecordPlayback;
    private readonly InputAction m_GlobalActions_DevSlowmo;
    private readonly InputAction m_GlobalActions_Minimap;
    public struct GlobalActionsActions
    {
        private @Controls m_Wrapper;
        public GlobalActionsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Any => m_Wrapper.m_GlobalActions_Any;
        public InputAction @Menu => m_Wrapper.m_GlobalActions_Menu;
        public InputAction @Pause => m_Wrapper.m_GlobalActions_Pause;
        public InputAction @DEVRespawn => m_Wrapper.m_GlobalActions_DEVRespawn;
        public InputAction @DEVClone => m_Wrapper.m_GlobalActions_DEVClone;
        public InputAction @Screenshot => m_Wrapper.m_GlobalActions_Screenshot;
        public InputAction @RecordToggle => m_Wrapper.m_GlobalActions_RecordToggle;
        public InputAction @RecordPlayback => m_Wrapper.m_GlobalActions_RecordPlayback;
        public InputAction @DevSlowmo => m_Wrapper.m_GlobalActions_DevSlowmo;
        public InputAction @Minimap => m_Wrapper.m_GlobalActions_Minimap;
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
                @DEVRespawn.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DEVRespawn.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DEVRespawn.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVRespawn;
                @DEVClone.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVClone;
                @DEVClone.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVClone;
                @DEVClone.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDEVClone;
                @Screenshot.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @Screenshot.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @Screenshot.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnScreenshot;
                @RecordToggle.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordToggle;
                @RecordToggle.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordToggle;
                @RecordToggle.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordToggle;
                @RecordPlayback.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordPlayback;
                @RecordPlayback.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordPlayback;
                @RecordPlayback.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnRecordPlayback;
                @DevSlowmo.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
                @DevSlowmo.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
                @DevSlowmo.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnDevSlowmo;
                @Minimap.started -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMinimap;
                @Minimap.performed -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMinimap;
                @Minimap.canceled -= m_Wrapper.m_GlobalActionsActionsCallbackInterface.OnMinimap;
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
                @DEVRespawn.started += instance.OnDEVRespawn;
                @DEVRespawn.performed += instance.OnDEVRespawn;
                @DEVRespawn.canceled += instance.OnDEVRespawn;
                @DEVClone.started += instance.OnDEVClone;
                @DEVClone.performed += instance.OnDEVClone;
                @DEVClone.canceled += instance.OnDEVClone;
                @Screenshot.started += instance.OnScreenshot;
                @Screenshot.performed += instance.OnScreenshot;
                @Screenshot.canceled += instance.OnScreenshot;
                @RecordToggle.started += instance.OnRecordToggle;
                @RecordToggle.performed += instance.OnRecordToggle;
                @RecordToggle.canceled += instance.OnRecordToggle;
                @RecordPlayback.started += instance.OnRecordPlayback;
                @RecordPlayback.performed += instance.OnRecordPlayback;
                @RecordPlayback.canceled += instance.OnRecordPlayback;
                @DevSlowmo.started += instance.OnDevSlowmo;
                @DevSlowmo.performed += instance.OnDevSlowmo;
                @DevSlowmo.canceled += instance.OnDevSlowmo;
                @Minimap.started += instance.OnMinimap;
                @Minimap.performed += instance.OnMinimap;
                @Minimap.canceled += instance.OnMinimap;
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
    private readonly InputAction m_BipedActions_Move;
    private readonly InputAction m_BipedActions_Jump;
    private readonly InputAction m_BipedActions_Dash;
    private readonly InputAction m_BipedActions_Aim;
    private readonly InputAction m_BipedActions_Fire;
    private readonly InputAction m_BipedActions_Ability;
    private readonly InputAction m_BipedActions_Interact;
    private readonly InputAction m_BipedActions_NextWeapon;
    private readonly InputAction m_BipedActions_NextAbility;
    private readonly InputAction m_BipedActions_Charge;
    private readonly InputAction m_BipedActions_Down;
    public struct BipedActionsActions
    {
        private @Controls m_Wrapper;
        public BipedActionsActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveRight => m_Wrapper.m_BipedActions_MoveRight;
        public InputAction @MoveLeft => m_Wrapper.m_BipedActions_MoveLeft;
        public InputAction @Move => m_Wrapper.m_BipedActions_Move;
        public InputAction @Jump => m_Wrapper.m_BipedActions_Jump;
        public InputAction @Dash => m_Wrapper.m_BipedActions_Dash;
        public InputAction @Aim => m_Wrapper.m_BipedActions_Aim;
        public InputAction @Fire => m_Wrapper.m_BipedActions_Fire;
        public InputAction @Ability => m_Wrapper.m_BipedActions_Ability;
        public InputAction @Interact => m_Wrapper.m_BipedActions_Interact;
        public InputAction @NextWeapon => m_Wrapper.m_BipedActions_NextWeapon;
        public InputAction @NextAbility => m_Wrapper.m_BipedActions_NextAbility;
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
                @MoveRight.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveRight.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveRight.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveRight;
                @MoveLeft.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMoveLeft;
                @Move.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnMove;
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
                @Ability.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAbility;
                @Ability.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAbility;
                @Ability.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnAbility;
                @Interact.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnInteract;
                @NextWeapon.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @NextWeapon.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextWeapon;
                @NextAbility.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextAbility;
                @NextAbility.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextAbility;
                @NextAbility.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnNextAbility;
                @Charge.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Charge.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Charge.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnCharge;
                @Down.started -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_BipedActionsActionsCallbackInterface.OnDown;
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
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
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
                @Ability.started += instance.OnAbility;
                @Ability.performed += instance.OnAbility;
                @Ability.canceled += instance.OnAbility;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @NextWeapon.started += instance.OnNextWeapon;
                @NextWeapon.performed += instance.OnNextWeapon;
                @NextWeapon.canceled += instance.OnNextWeapon;
                @NextAbility.started += instance.OnNextAbility;
                @NextAbility.performed += instance.OnNextAbility;
                @NextAbility.canceled += instance.OnNextAbility;
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
            }
        }
    }
    public BipedActionsActions @BipedActions => new BipedActionsActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse+Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IGlobalActionsActions
    {
        void OnAny(InputAction.CallbackContext context);
        void OnMenu(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnDEVRespawn(InputAction.CallbackContext context);
        void OnDEVClone(InputAction.CallbackContext context);
        void OnScreenshot(InputAction.CallbackContext context);
        void OnRecordToggle(InputAction.CallbackContext context);
        void OnRecordPlayback(InputAction.CallbackContext context);
        void OnDevSlowmo(InputAction.CallbackContext context);
        void OnMinimap(InputAction.CallbackContext context);
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
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnFire(InputAction.CallbackContext context);
        void OnAbility(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnNextWeapon(InputAction.CallbackContext context);
        void OnNextAbility(InputAction.CallbackContext context);
        void OnCharge(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
    }
}
