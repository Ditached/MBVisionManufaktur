using UnityEngine;

public class QuickMaterialSwitcher : MonoBehaviour
{
    public Material[] materials;


    public void ChangeToMaterial(int index)
    {
        this.gameObject.GetComponent<Renderer>().material = materials[index];
    }
}
