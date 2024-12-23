using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class DoggyRoom : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Doggy");
        }
    }
}
