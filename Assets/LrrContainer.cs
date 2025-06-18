using UnityEngine;

public class LrrContainer : MonoBehaviour
{
    public LrrDisplay[] LrrDisplays = new LrrDisplay[4];

    public void EnableDisplayCount(int playerCount)
    {
        for (int x = 0; x < LrrDisplays.Length; x++)
        {
            LrrDisplays[x].gameObject.SetActive(x < playerCount);
        }
    }
}
