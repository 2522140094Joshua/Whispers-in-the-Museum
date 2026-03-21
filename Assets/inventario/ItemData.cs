using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Informacion del Item")]
    public string itemName = "Item";
    public Sprite icon; // Arrastra una imagen en el Inspector

    [Header("Descripcion (opcional)")]
    [TextArea]
    public string description = "";
}