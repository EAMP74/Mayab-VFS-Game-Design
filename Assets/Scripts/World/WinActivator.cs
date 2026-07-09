using UnityEngine;

public class WinActivator : MonoBehaviour
{
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private WeaponController _weaponController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerController.enabled = false;
        _weaponController.enabled = false;
        if (_winScreen != null)
        {
            _winScreen.SetActive(true);
        }
    }
}
