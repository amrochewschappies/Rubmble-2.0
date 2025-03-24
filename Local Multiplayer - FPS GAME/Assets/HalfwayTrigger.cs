using UnityEngine;

public class HalfwayTrigger : MonoBehaviour
{
    public GameManager _gameManager; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject hitPlayer = other.gameObject;

            if (_gameManager != null)
            {
                _gameManager.CheckHalfWay(hitPlayer);
                Debug.Log(hitPlayer.name + "is halfway");
            }
        }
    }
}