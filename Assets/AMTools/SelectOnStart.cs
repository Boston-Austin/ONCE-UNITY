using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectOnStart : MonoBehaviour
{
    [SerializeField] private GameObject _objectToSelect;
    private bool _isControllerInUse;
    
    private void Start()
    {
        //Debug.Log(_isControllerInUse);
        //Debug.Log(Gamepad.current);

        if(CheckForControllerInput() == true)
        {
            Slider _slider = _objectToSelect.GetComponentInChildren<Slider>();
            Button _button = _objectToSelect.GetComponentInChildren<Button>();
            Dropdown _dropdown = _objectToSelect.GetComponentInChildren<Dropdown>();

            if (_slider != null)
            {
                _slider.Select();
            }

            if (_button != null)
            {
                _button.Select();
            }

            if (_dropdown != null)
            {
                _dropdown.Select();
            }
        }
    }

    private bool CheckForControllerInput()
    {
        return Gamepad.current != null;
    }
}