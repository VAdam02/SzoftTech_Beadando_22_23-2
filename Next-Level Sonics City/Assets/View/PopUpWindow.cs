using Model;
using UnityEngine;
using UnityEngine.Events;

public class PopUpWindow : MonoBehaviour
{
    public Tile TileModel
    {
        set
        {
            TileModelChanged?.Invoke(value);
        }
    }
    public UnityEvent<Tile> TileModelChanged = new();
}
