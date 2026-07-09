using UnityEngine;
using System.Collections.Generic;

public class RoomActivator : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private GameObject _waveSpawner;
    [SerializeField] private bool _canRestartWave;

    [Header("Fog")]
    [SerializeField] private GameObject _fogBox;

    [Header("Doors")]
    [SerializeField] private List<GameObject> _doors;

    private bool _isActivated = false;
    private bool _isFinished = false;
    private bool _inRoom = false;

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

    private void OnTriggerEnter(Collider player)
    {
        if (!player.CompareTag("Player")) return;

        _inRoom = !_inRoom;

        if (_fogBox != null)
        {
            _fogBox.SetActive(false);
        }

        ReactivateRoom();

        if (_isFinished || _isActivated) return;

        _playerCourage = player.GetComponent<CourageController>();

        if (_playerCourage == null)
        {
            _playerCourage = player.GetComponentInParent<CourageController>();
        }

        ActivateRoom();
    }

    private void ActivateRoom()
    {
        if (!_inRoom) return;

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

    private void ReactivateRoom()
    {
        if (_inRoom) return;
        if (_canRestartWave)
        {
            _isFinished = false;
            _isActivated = false;
        }
    }
}
