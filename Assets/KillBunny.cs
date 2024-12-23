using UnityEngine;
using UnityEngine.SceneManagement;

// I followed bblakeyyy's video "HOW TO KILL AND RESPAWN PLAYER-Unity Tutorial"
// https://www.youtube.com/watch?v=H69PfxOr6bk 
// Though, the death planes/boxes did turn out a bit different...

public class KillBunny : MonoBehaviour
{
    public int Respawn;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene(Respawn);
        }
    }
}
