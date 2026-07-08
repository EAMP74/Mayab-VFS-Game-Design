using System.Threading.Tasks;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Animator _deathAnimaion;
    private PlayerController _playerController;

    public async void Die(GameObject losePanel)
    {
        _playerController = GetComponent<PlayerController>();
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }
        _deathAnimaion = GetComponentInChildren<Animator>();
        if (_deathAnimaion != null)
        {
            _deathAnimaion.SetTrigger("isDead");
        }
        if (losePanel != null)
        {
            await Task.Delay(5000);
            losePanel.SetActive(true);
        }
    }
}
