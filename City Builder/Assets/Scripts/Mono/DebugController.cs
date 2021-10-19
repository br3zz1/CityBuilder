using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    
    public static DebugController Instance { get; private set; }

    public bool debugConsoleOpen = false;

    private List<DebugCommand> debugCommandList;

    [SerializeField]
    private InputField debugCommand;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        debugCommandList = new List<DebugCommand>();
        debugCommandList.Add(new DebugCommand("cleartile", Cleartile));
    }

    void Cleartile(string[] args)
    {
        if(args.Length == 2)
        {
            int x;
            int y;
            if(int.TryParse(args[0], out x))
            {
                if(int.TryParse(args[1], out y))
                {
                    WorldManager.Instance.Cleartile(x, y);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (debugConsoleOpen)
            {
                CloseDebugConsole(false);
            }
            else
            {
                OpenDebugConsole();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (debugConsoleOpen)
            {
                CloseDebugConsole(true);
            }
        }
    }

    public void OpenDebugConsole()
    {
        debugConsoleOpen = true;
        debugCommand.gameObject.SetActive(true);
        debugCommand.Select();
        debugCommand.ActivateInputField();
        CameraController.Instance.lockControl = true;
    }

    public void CloseDebugConsole(bool parseCommand)
    {
        debugConsoleOpen = false;
        debugCommand.gameObject.SetActive(false);
        CameraController.Instance.lockControl = false;

        if (parseCommand)
        {
            string command = debugCommand.text;
            debugCommand.text = "";
            ParseCommand(command);
        }
    }

    public void ParseCommand(string command)
    {
        string[] ca = command.Split(' ');

        if (ca.Length == 0) return;

        foreach(DebugCommand dc in debugCommandList)
        {
            if(ca[0].ToLower() == dc.name)
            {
                List<string> cal = new List<string>(ca);
                cal.RemoveAt(0);
                dc.Invoke(cal.ToArray());
            }
        }
    }
}
