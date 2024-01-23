using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Info Command", menuName = "Utilities/DevConsole/Commands/Info")]
public class InfoCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        GameObject obj = GameObject.Find("CarInfoHolder"); // use find camera instead

        Debug.Log("a");
        if (obj == null)
        {
            return false;
        }
        Debug.Log("a");

        obj.SetActive(!obj.activeSelf);

        return true;
    }
}