using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

/* This is a terrible hacky wrapper class I wrote to substitute for the AMAZING Rewired input pluggin.
 * It only supports single player Keyboard input without modification.
 * Purchase the real REWIRED from GUAVAMAN for quality input handling at: https://guavaman.com/projects/rewired/
 * You should be able to import Rewired, delete this, and not have to modify any code, but no guarantees.
 */

namespace Rewired
{
    public static class ReInput
    {
        public static ContollerStatusChanged ControllerConnectedEvent;
        public static ContollerStatusChanged ControllerDisconnectedEvent;
        public static ControllerHelper controllers = new ControllerHelper();
        public static List<Player> players = CreatePlayers(); 
        public static MappingHelper mapping = new MappingHelper();

        private static List<Player> CreatePlayers() 
        {
            List<Player> playerList = new List<Player>();
            for (int i = 0; i < Gamepad.all.Count; i++) 
            {
                Gamepad gp = Gamepad.all[i];
                Player pl = new Player(i, gp); 
                playerList.Add(pl);
            }
            Player keyboardPlayer = new Player(Gamepad.all.Count, UnityEngine.InputSystem.Keyboard.current);
            playerList.Add(keyboardPlayer);
            return playerList;
        }
    }

    public class Player
    {
        public readonly ControllerHelper controllers = new ControllerHelper();

        private int _id;
        private UnityEngine.InputSystem.Keyboard keyboard; 
        private Gamepad gamepad; 

        public Player(int id, UnityEngine.InputSystem.Keyboard keyboard)
        {
            this._id = id;
            this.keyboard = keyboard;
            this.gamepad = null;
        }

        public Player(int id, Gamepad gamepad)
        {
            this._id = id;
            this.gamepad = gamepad;
            this.keyboard = null;
        }

        public int id
        {
            get { return this._id; }
        }

        private List<ButtonControl> GetControlsNullable(string label)
        {
            switch (label)
            {
                case "UISubmit": 
                    return new List<ButtonControl>{keyboard?.enterKey, gamepad?.aButton};
                case "UICancel":
                    return new List<ButtonControl>{keyboard?.escapeKey, gamepad?.bButton, gamepad?.startButton};
                case "UIUp":
                    return new List<ButtonControl>{keyboard?.upArrowKey, gamepad?.dpad?.up, gamepad?.leftStick?.up};
                case "UIDown":
                    return new List<ButtonControl>{keyboard?.downArrowKey, gamepad?.dpad?.down, gamepad?.leftStick?.down};
                case "UILeft":
                    return new List<ButtonControl>{keyboard?.leftArrowKey, gamepad?.dpad?.left, gamepad?.leftStick?.left};
                case "UIRight":
                    return new List<ButtonControl>{keyboard?.rightArrowKey, gamepad?.dpad?.right, gamepad?.leftStick?.right};
                case "Pause":
                    return new List<ButtonControl>{keyboard?.escapeKey, gamepad?.startButton};
                case "ExpandMap":
                    return new List<ButtonControl>{keyboard?.mKey, gamepad?.selectButton};
                case "Jump":
                    return new List<ButtonControl>{keyboard?.spaceKey, gamepad?.aButton};
                case "Attack":
                    return new List<ButtonControl>{keyboard?.jKey, gamepad?.xButton};
                case "AngleUp":
                    return new List<ButtonControl>{keyboard?.iKey, gamepad?.rightTrigger};
                case "AngleDown":
                    return new List<ButtonControl>{keyboard?.kKey, gamepad?.leftTrigger};
                case "SpecialMove":
                    return new List<ButtonControl>{keyboard?.lKey, gamepad?.bButton};
                case "ActivatedItem":
                    return new List<ButtonControl>{keyboard?.uKey, gamepad?.yButton};
                case "PageRight":
                    return new List<ButtonControl>{keyboard?.commaKey, gamepad?.rightShoulder};
                case "PageLeft":
                    return new List<ButtonControl>{keyboard?.periodKey, gamepad?.leftShoulder};
                case "LockPosition":
                    return new List<ButtonControl>{keyboard?.leftCtrlKey, gamepad?.leftStickButton};
                case "WeaponWheel":
                    return new List<ButtonControl>{keyboard?.pKey, gamepad?.rightStickButton};
                case "WeaponsCancel":
                    return new List<ButtonControl>{keyboard?.deleteKey};
                case "CoOpEnterGame":
                    return new List<ButtonControl>{keyboard?.endKey, gamepad?.startButton};
                case "CoOpTeleport":
                    return new List<ButtonControl>{gamepad?.rightStickButton};
                default:
                    Debug.LogError(label + " is not defined");
                    return new List<ButtonControl>();
            }
        }

        private List<ButtonControl> GetControls(string label)
        {
            List<ButtonControl> lstWithNulls = GetControlsNullable(label);
            List<ButtonControl> lst = new List<ButtonControl>();
            foreach (ButtonControl bce in lstWithNulls) 
            {
                if (bce != null) 
                {
                    lst.Add(bce);
                } 
            } 
            return lst;
        }


        public bool GetButtonDown(string label)
        {
            foreach (var key in GetControls(label))
            {
                if (key.wasPressedThisFrame)
                {
                    return true;
                }
            }
            return false;
        }


        public bool GetButtonUp(string label)
        {
                foreach (var key in GetControls(label))
                {
                    if (key.wasReleasedThisFrame)
                    {
                        return true;
                    }
                }
                return false;
        }


        public bool GetButton(string label)
        {
                foreach (var key in GetControls(label))
                {
                    if (key.isPressed)
                    {
                        return true;
                    }
                }
                return false;
        }

        public bool GetAnyButton()
        {
            return GetButton("UISubmit") || GetButton("UICancel");
        }

        public bool GetAnyButtonDown()
        {
            return GetButtonDown("UISubmit") || GetButtonDown("UICancel");
        }

        public float GetAxis(string label)
        {
            if (keyboard != null) 
            {
                switch(label) 
                {
                    case "Vertical":
                        return keyboard.wKey.isPressed ? 1 : (keyboard.sKey.isPressed ? -1 : 0);
                    case "Horizontal":
                        return keyboard.dKey.isPressed ? 1 : (keyboard.aKey.isPressed ? -1 : 0);
                    case "WeaponsVertical":
                        return keyboard.upArrowKey.isPressed ? 1 : (keyboard.downArrowKey.isPressed ? -1 : 0);
                    case "WeaponsHorizontal":
                        return keyboard.rightArrowKey.isPressed ? 1 : (keyboard.leftArrowKey.isPressed ? -1 : 0);
                    default:
                        Debug.LogError(label + " is not defined");
                        return 0; 
                }
            }

            if (gamepad != null) 
            {
                switch(label) 
                {
                    case "Vertical":
                    case "CoOpMoveVertical":
                        return gamepad.dpad.up.isPressed || gamepad.leftStick.up.isPressed ? 1 : (gamepad.dpad.down.isPressed || gamepad.leftStick.down.isPressed ? -1 : 0);
                    case "Horizontal":
                    case "CoOpMoveHorizontal":
                        return gamepad.dpad.right.isPressed || gamepad.leftStick.right.isPressed ? 1 : (gamepad.dpad.left.isPressed || gamepad.leftStick.left.isPressed ? -1 : 0);
                    case "WeaponsVertical":
                        return gamepad.rightStick.up.isPressed ? 1 : (gamepad.rightStick.down.isPressed ? -1 : 0);
                    case "WeaponsHorizontal":
                        return gamepad.rightStick.right.isPressed ? 1 : (gamepad.rightStick.left.isPressed ? -1 : 0);
                    case "CoOpShootVertical":
                        return gamepad.yButton.isPressed ? 1 : (gamepad.aButton.isPressed ? -1 : 0);
                    case "CoOpShootHorizontal":
                        return gamepad.bButton.isPressed ? 1 : (gamepad.xButton.isPressed ? -1 : 0);
                    default:
                        Debug.LogError(label + " is not defined");
                        return 0; 
                }
            }

            Debug.LogError("Axis " + label + " is not defined");
            return 0;
        }
    }

    public class MappingHelper
    {
        private Dictionary<string, InputAction> _map = new Dictionary<string, InputAction>()
        {
            {"Vertical", new InputAction(){ id = 0} },
            {"Horizontal", new InputAction(){ id = 1} },
            {"Attack", new InputAction(){ id = 4} },
            {"Jump", new InputAction(){ id = 5} },
            {"SpecialMove", new InputAction(){ id = 6} },
            {"ActivatedItem", new InputAction(){ id = 7} },
            {"AngleUp", new InputAction(){ id = 8} },
            {"AngleDown", new InputAction(){ id = 9} },
            {"PageRight", new InputAction(){ id = 10} },
            {"PageLeft", new InputAction(){ id = 11} },
            {"ExpandMap", new InputAction(){ id = 12} },
            {"UIHorizontal", new InputAction(){id = 14} },
            {"UIVertical", new InputAction(){id = 15} },
            {"UISubmit", new InputAction(){id = 16} },
            {"UICancel", new InputAction(){id = 17} },
            {"UIUp", new InputAction(){id = 18} },
            {"UIDown", new InputAction(){id = 19} },
            {"UILeft", new InputAction(){id = 20} },
            {"UIRight", new InputAction(){id = 21} },
            {"UIScroll", new InputAction(){id = 31} },
            {"WeaponVertical", new InputAction(){ id = 32} },
            {"WeaponHorizontal", new InputAction(){ id = 33} },
            {"CoinSlot", new InputAction(){ id = 37} },
        };

        public InputAction GetAction(string name)
        {
            if (_map.TryGetValue(name, out InputAction action))
            {
                return action;
            }
            else
            {
                //Debug.LogError("No InputAction defined for " + name);
                return null;
            }
        }
    }

    public delegate void ContollerStatusChanged(ControllerStatusChangedEventArgs args);

    public class ControllerStatusChangedEventArgs
    {
    }

    public class ControllerHelper
    {
        public MapHelper maps = new MapHelper();

        public bool hasKeyboard
        {
            get { return _Keyboard != null; }
        }

        public int joystickCount
        {
            get { return _Joysticks.Count; }
        }

        private List<Controller> _Controllers = new List<Controller>();
        public List<Controller> Controllers
        {
            get { return _Controllers; }
        }

        private List<Joystick> _Joysticks = new List<Joystick>();
        public List<Joystick> Joysticks { get { return _Joysticks; } }

        private Keyboard _Keyboard = new Keyboard();
        public Keyboard Keyboard
        {
            get { return _Keyboard; }
        }

        public bool IsControllerAssignedToPlayer(ControllerType type, int x, int y)
        {
            throw new System.NotImplementedException();
        }

        public void AddController(Controller controller, bool removeFromOtherPlayers)
        {
            if (removeFromOtherPlayers)
            {
                Debug.LogWarning("removeFromOtherPlayers will not function as expect. OPENARNF only supports a single player using keyboard input without modification");
            }

            _Controllers.Add(controller);

            var joystick = controller as Joystick;
            if (joystick != null)
            {
                _Joysticks.Add(joystick);
            }
        }

        public void ClearAllControllers()
        {
            _Controllers.Clear();
            _Joysticks.Clear();
        }

        public Controller GetLastActiveController()
        {
            //throw new System.NotImplementedException();
            //OpenARNF only supports Keyboard input without modification
            return Keyboard;
        }

        public bool IsJoystickAssignedToPlayer(int id, int other)
        {
            throw new System.NotImplementedException();
        }

        public bool GetAnyButton()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Keyboard : Controller
    {
        public override ControllerType type
        {
            get { return ControllerType.Keyboard; }
        }

        public override string name
        {
            get { return "Keyboard"; }
        }

        /*
        public bool GetKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        public bool GetKeyDown(KeyCode keyCode)
        {
            return Input.GetKeyDown(keyCode);            
        }
        */

        public IEnumerable<PollingInfo> PollForAllKeysDown()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PollingInfo
    {
        public KeyCode keyboardKey
        {
            get { throw (new System.NotImplementedException()); }
        }
    }

    public class Joystick : Controller
    {
        public override ControllerType type
        {
            get { return ControllerType.Joystick; }
        }
    }

    public class InputAction
    {
        public int id;
    }

    public class MapHelper
    {
        public void SetAllMapsEnabled(bool someBool)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ActionElementMap> ButtonMapsWithAction(int id, bool someBool)
        {
            throw new System.NotImplementedException();
        }
        public IEnumerable<ActionElementMap> ElementMapsWithAction(int id, bool someBool)
        {
            throw new System.NotImplementedException();
        }

        private Dictionary<int, ActionElementMap> _aems = new Dictionary<int, ActionElementMap>()
        {
            {6, new ActionElementMap()
                {
                    elementIdentifierName = "L",
                }
            },
            {7, new ActionElementMap()
                {
                    elementIdentifierName = "U",
                }
            },
            {16, new ActionElementMap()
                {
                    elementIdentifierName = "Return",
                }
            },
            {17, new ActionElementMap()
                {
                    elementIdentifierName = "Escape",
                }
            },
        };

        public ActionElementMap GetFirstElementMapWithAction(Controller controller, int id, bool skipDisabledMaps)
        {
            if (controller != ReInput.players[0].controllers.Keyboard)
            {
                Debug.LogError("Only the keyboard for the system player is supported by default in OpenARNF. ");
                return null;
            }

            if (_aems.TryGetValue(id, out var aem))
            {
                return aem;
            }
            else
            {
                Debug.LogError("No action element map could be found for id " + id);
                return null;
            }
        }
    }

    public class Controller
    {
        public virtual ControllerType type { get; private set; }

        public int id
        {
            get { throw (new System.NotImplementedException()); }
        }

        public virtual string name { get; }

        public Guid hardwareTypeGuid
        {
            get { throw (new System.NotImplementedException()); }
        }
    }

    public class ControllerMap
    {
        public int controllerId
        {
            get { throw (new System.NotImplementedException()); }
        }
    }

    public class ActionElementMap
    {
        public string elementIdentifierName
        {
            get; set;
        }
        public int elementIdentifierId
        {
            get; private set;
        }
        public AxisRange axisRange
        {
            get; private set;
        }
        public AxisType axisType
        {
            get; private set;
        }
        public Pole axisContribution
        {
            get; private set;
        }
        public ControllerMap controllerMap
        {
            get; private set;
        }
    }

    public class UserDataStore
    {

    }

    public enum AxisType
    {
        None,
    }

    public enum ControllerType
    {
        Joystick,
        Keyboard,
        Mouse,
    }

    public enum AxisRange
    {
        Full,
        Positive,
        Negative,
    }

    public enum Pole
    {
        Positive,
        Negative,
    }
}

namespace Rewired.Data.Mapping
{
    public class HardwareJoystickMap
    {
        public Guid Guid;
    }
}

namespace Rewired.UI.ControlMapper
{
    public partial class ControlMapper
    {
        public ControlMapperButtons references;
        public Action ScreenClosedEvent;
        public bool isOpen;
        public void Open()
        {
            throw (new System.NotImplementedException());
        }

        public void Close(bool someBool)
        {
            throw (new System.NotImplementedException());
        }
    }

    public class ControlMapperButtons
    {
        public UnityEngine.UI.Button removeControllerButton;
        public UnityEngine.UI.Button assignControllerButton;
        public UnityEngine.UI.Button calibrateControllerButton;
    }
}

