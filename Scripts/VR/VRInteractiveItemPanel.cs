public class VRInteractiveItemPanel : VRInteractiveItemBaseclass
{
    public override void FocusIn()
    {
//        Debug.Log("VRInteractiveItemPanel::focusIn: " + gameObject.name);
        bHasFocus = true;
    }

    public override void FocusOut()
    {
//        Debug.Log("VRInteractiveItemPanel::focusOut: " + gameObject.name);
        bHasFocus = false;
    }
}