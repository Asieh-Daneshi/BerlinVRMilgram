using UnityEngine;

public class VRInteractiveItemBaseclass : MonoBehaviour
{
    protected bool bHasFocus;

    public virtual void FocusIn()
    {
        Debug.Log("VRInteractiveItem::focusIn: " + gameObject.name);
        bHasFocus = true;
    }

    public virtual void FocusOut()
    {
        Debug.Log("VRInteractiveItem::focusOut: " + gameObject.name);
        bHasFocus = false;
    }

    public string getName()
    {
        return gameObject.name;
    }

    public bool hasFocus()
    {
        return bHasFocus;
    }
}