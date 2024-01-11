using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class DevConsoleBehaviour : MonoBehaviour
{
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
            if (devConsole != null) return DevConsole;
            return devConsole = new DevConsole(commands);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void Toggle(CallbackContext ctx)
    {
        if (!ctx.action.triggered) return;
        if (uiCanvas.activeSelf)
        {
            Time.timeScale = pausedTimeScale;
            uiCanvas.SetActive(false);
        }
        else
        {
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string input)
    {
        DevConsole.ProcessCommand(input);
        inputField.text = string.Empty;
    }
}
