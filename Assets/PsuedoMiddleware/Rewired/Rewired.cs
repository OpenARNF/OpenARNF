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
        public static PlayerHelper players = new PlayerHelper();
        public static Player SystemPlayer
        {
            get { return players.SystemPlayer; }

        }
        public static MappingHelper mapping = new MappingHelper();
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
                Debug.LogError("No InputAction defined for " + name);
                return null;
            }
        }
    }

    public delegate void ContollerStatusChanged(ControllerStatusChangedEventArgs args);

    public class ControllerStatusChangedEventArgs
    {
    }

    public class PlayerHelper
    {
        public Player SystemPlayer = new Player();

        public Player GetSystemPlayer()
        {
            return SystemPlayer;
        }

        public int playerCount
        {
            //Only a single player is supported with this class. BUY REWIRED!
            get { return 1; }
        }

        public Player GetPlayer(int id)
        {
            if (id == 0)
            {
                return SystemPlayer;
            }
            else
            {
                return null;
            }
        }

        public List<Player> GetActivePlayers()
        {
            return new List<Player>{SystemPlayer};
        }
    }

    public class Player
    {
        public readonly ControllerHelper controllers = new ControllerHelper();
        private static UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
        private static UnityEngine.InputSystem.Gamepad gamepad = UnityEngine.InputSystem.Gamepad.current;

        public int id
        {
            get { return 0; }
        }

        public string name
        {
            get { return "OPEN ARNF DEBUG"; }
        }


        private Dictionary<string, List<ButtonControl>> _keyValues = new Dictionary<string, List<ButtonControl>>()
        {
            {"UISubmit", new List<ButtonControl>{keyboard.enterKey, gamepad.aButton} },
            {"UICancel", new List<ButtonControl>{keyboard.escapeKey, gamepad.bButton, gamepad.startButton} },
            {"UIUp", new List<ButtonControl>{keyboard.upArrowKey, gamepad.dpad.up, gamepad.leftStick.up} },
            {"UIDown", new List<ButtonControl>{keyboard.downArrowKey, gamepad.dpad.down, gamepad.leftStick.down} },
            {"UILeft", new List<ButtonControl>{keyboard.leftArrowKey, gamepad.dpad.left, gamepad.leftStick.left} },
            {"UIRight", new List<ButtonControl>{keyboard.rightArrowKey, gamepad.dpad.right, gamepad.leftStick.right} },
            {"Pause", new List<ButtonControl>{keyboard.escapeKey, gamepad.startButton} },
            {"ExpandMap", new List<ButtonControl>{keyboard.mKey, gamepad.selectButton} },
            /*
            {"Up", new List<ButtonControl>{keyboard.wKey, gamepad.dpad.up} },
            {"Down", new List<ButtonControl>{keyboard.sKey, gamepad.dpad.down} },
            {"Left", new List<ButtonControl>{keyboard.aKey, gamepad.dpad.left} },
            {"Right", new List<ButtonControl>{keyboard.dKey, gamepad.dpad.right} },
            */
            {"Jump", new List<ButtonControl>{keyboard.spaceKey, gamepad.aButton} },
            {"Attack", new List<ButtonControl>{keyboard.jKey, gamepad.xButton} },
            {"AngleUp", new List<ButtonControl>{keyboard.iKey, gamepad.rightTrigger} },
            {"AngleDown", new List<ButtonControl>{keyboard.kKey, gamepad.leftTrigger} },
            {"SpecialMove", new List<ButtonControl>{keyboard.lKey, gamepad.bButton} },
            {"ActivatedItem", new List<ButtonControl>{keyboard.uKey, gamepad.yButton} },
            {"PageRight", new List<ButtonControl>{keyboard.commaKey, gamepad.rightShoulder} },
            {"PageLeft", new List<ButtonControl>{keyboard.periodKey, gamepad.leftShoulder} },
            {"LockPosition", new List<ButtonControl>{keyboard.leftCtrlKey, gamepad.leftStickButton} },
            {"WeaponWheel", new List<ButtonControl>{keyboard.pKey, gamepad.rightStickButton} },
            /*
            {"CoinSlot", KeyCode.Insert },
            {"WeaponCancel", KeyCode.Delete },
            */
        };

        public bool GetButtonDown(string label)
        {
            if (_keyValues.TryGetValue(label, out var keyList))
            {
                foreach (var key in keyList)
                {
                    if (key.wasPressedThisFrame)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Debug.LogError(label + " is not defined");
                return false;
            }
        }


        public bool GetButtonUp(string label)
        {
            if (_keyValues.TryGetValue(label, out var keyList))
            {
                foreach (var key in keyList)
                {
                    if (key.wasReleasedThisFrame)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Debug.LogError(label + " is not defined");
                return false;
            }
        }


        public bool GetButton(string label)
        {
            if (_keyValues.TryGetValue(label, out var keyList))
            {
                foreach (var key in keyList)
                {
                    if (key.isPressed)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                Debug.LogError(label + " is not defined");
                return false;
            }
        }

        public bool GetAnyButton()
        {
            foreach (var bcList in _keyValues.Values)
            {
                foreach (var bc in bcList)
                {
                    if (bc.isPressed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetAnyButtonDown()
        {
            foreach (var bcList in _keyValues.Values)
            {
                foreach (var bc in bcList)
                {
                    if (bc.wasPressedThisFrame)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public float GetAxis(string label)
        {
            if (label == "Vertical")
            {
                if (keyboard.wKey.isPressed || gamepad.dpad.up.isPressed || gamepad.leftStick.up.isPressed)
                {
                    return 1;
                }
                else if (keyboard.sKey.isPressed || gamepad.dpad.down.isPressed || gamepad.leftStick.down.isPressed)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if (label == "Horizontal")
            {
                if (keyboard.dKey.isPressed || gamepad.dpad.right.isPressed || gamepad.leftStick.right.isPressed)
                {
                    return 1;
                }
                else if (keyboard.aKey.isPressed || gamepad.dpad.left.isPressed || gamepad.leftStick.left.isPressed)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if (label == "WeaponsVertical")
            {
                if (keyboard.upArrowKey.isPressed || gamepad.rightStick.up.isPressed)
                {
                    return 1;
                }
                else if (keyboard.downArrowKey.isPressed || gamepad.rightStick.down.isPressed)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if (label == "WeaponsHorizontal")
            {
                if (keyboard.rightArrowKey.isPressed || gamepad.rightStick.right.isPressed)
                {
                    return 1;
                }
                else if (keyboard.leftArrowKey.isPressed || gamepad.rightStick.left.isPressed)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            Debug.LogError("Axis " + label + " is not defined");
            return 0;
        }
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
            if (controller != ReInput.SystemPlayer.controllers.Keyboard)
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

