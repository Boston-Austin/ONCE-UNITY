using UnityEngine;
using UnityEngine.InputSystem;
using Button = UnityEngine.UI.Button;

namespace AMTools.AMToolsController
{
    public static class AMToolsController
    {
        private static bool _isControllerInUse = false;

        public static void AMToolsSelect(this Button _buttonToSelect)
        {
            _isControllerInUse = CheckForControllerInput();

            if(_isControllerInUse)
            {
                _buttonToSelect.Select();
            }
        }

        public static void EnableCursor()
        {
            _isControllerInUse = CheckForControllerInput();

            if(_isControllerInUse == false)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        
        public static void DisableCursor()
        {
            //_isControllerInUse = CheckForControllerInput();

            //if(_isControllerInUse == false)
            //{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            //}
        }

        private static bool CheckForControllerInput()
        {
            return Gamepad.current != null;
        }
    }
}