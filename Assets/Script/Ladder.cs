using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetNearLadder(this); // Gán thang hiện tại cho nhân vật
                Debug.Log("Player entered ladder zone.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetNearLadder(null); // Xóa trạng thái thang khi rời khỏi
                Debug.Log("Player exited ladder zone.");
            }
        }
    }
}
