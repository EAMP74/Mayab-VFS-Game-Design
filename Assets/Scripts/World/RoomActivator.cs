using UnityEngine;
using System.Collections.Generic;

public class RoomActivator : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private GameObject _waveSpawner;

    [Header("Door")]
    [SerializeField] private List<GameObject> _doors;

    private bool _isActivated = false;
    private bool _isFinished = false;

    private CourageController _playerCourage;

    private void Awake()
    {
        if (_waveSpawner != null)
        {
            _waveSpawner.gameObject.SetActive(false);
        }
        foreach (GameObject door in _doors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_isFinished || _isActivated) return;

        _playerCourage = other.GetComponent<CourageController>();

        if (_playerCourage == null)
        {
            _playerCourage = other.GetComponentInParent<CourageController>();
        }

        ActivateRoom();
    }

    private void ActivateRoom()
    {
        _isActivated = true;
        _playerCourage?.BeginRoomEncounter();
        if (_waveSpawner != null)
        {
            _waveSpawner.SetActive(true);
        }
        foreach (GameObject door in _doors)
        {
            if (door != null)
            {
                door.SetActive(true);
            }
        }
    }

    public void CompleteWave()
    {
        if (_isFinished) return;

        _isFinished = true;
        _isActivated = false;

        _playerCourage?.EndRoomEncounter();

        if (_waveSpawner != null)
        {
            _waveSpawner.gameObject.SetActive(false);
        }

        foreach (GameObject door in _doors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }
}
