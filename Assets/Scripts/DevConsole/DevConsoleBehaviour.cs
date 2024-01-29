using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DevConsoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private FreeFlyCamera freeFlyCamera;
    [SerializeField]
    private SimulationSpeedController simulationSpeedController;
    private Controls controls;
    private InputAction toggle;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;


    private float pausedTimeScale;
    private static DevConsoleBehaviour instance;

    private DevConsole devConsole;
    private DevConsole DevConsole
    {
        get
        {
            if (devConsole != null) return devConsole;
            return devConsole = new DevConsole(commands);
        }
    }

    private void Awake()
    {
        controls = new Controls();
        toggle = controls.Developer.ToggleConsole;
        toggle.performed += Toggle;
        toggle.Enable();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public void Toggle(CallbackContext ctx)
    {
        if (!ctx.action.triggered) return;
        if (uiCanvas.activeSelf)
        {
            Time.timeScale = pausedTimeScale;
            uiCanvas.SetActive(false);
            SimulationController.instance.paused = false;
            simulationSpeedController.enabled = true;
            freeFlyCamera.enabled = true;
        }
        else
        {
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            SimulationController.instance.paused = true;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
            simulationSpeedController.enabled = false;
            freeFlyCamera.enabled = false;
        }
    }

    public void ProcessCommand(string input)
    {
        inputField.ActivateInputField();
        DevConsole.ProcessCommand(input);
        inputField.text = string.Empty;
    }
}
