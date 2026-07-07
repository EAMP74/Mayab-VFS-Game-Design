using UnityEngine;
using System.Collections.Generic;

public class RoomActivator : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] GameObject waveSpawner;

    [Header("Door")]
    [SerializeField] List<GameObject> doors;
    //[SerializeField] Transform closeDoor;

    bool isActivated = false;
    bool isFinished = false;

    //private void Update()
    //{
    //    CompleteWave();
    //}

    private void Awake()
    {
        if (waveSpawner != null)
        {
            waveSpawner.gameObject.SetActive(false);
        }
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isFinished)
            {
                ActivateRoom();
            }
        }
    }

    private void ActivateRoom()
    {
        isActivated = true;
        if (waveSpawner != null)
        {
            waveSpawner.gameObject.SetActive(true);
        }
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(true);
            }
        }
    }

    public void CompleteWave()
    {
        isFinished = true;
        isActivated = false;
        if (waveSpawner != null)
        {
            waveSpawner.gameObject.SetActive(false);
        }
        foreach (GameObject door in doors)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }
    }
}
