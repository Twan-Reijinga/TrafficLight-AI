using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Info Command", menuName = "Utilities/DevConsole/Commands/Info")]
public class InfoCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        CarInfoVisualizer info = GameObject.Find("Main Camera").GetComponent<CarInfoVisualizer>(); // use find camera instead

        if (info == null)
        {
            return false;
        }

        info.enabled = !info.enabled;
        info.infoParent.gameObject.SetActive(info.enabled);

        return true;
    }
}