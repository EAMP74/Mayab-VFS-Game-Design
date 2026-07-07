using UnityEngine;
using UnityEngine.Events;

public class CourageController : MonoBehaviour
{
    [Header("Courage")]
    [SerializeField] private float maxCourage = 100f;
    [SerializeField] private bool destroyOnDeath;
    [SerializeField] private float destroyDelay;

    [Header("Room Caps")]
    [SerializeField] private float courageLossOnProxy = 30f;
    [SerializeField] private float courageGainOnKill = 20f;

    [Header("Events")]
    public UnityEvent<float, float> OnCourageChanged;
    public UnityEvent<Vector3, Vector3> OnScared;
    public UnityEvent OnTerrified;
    public UnityEvent OnEncouraged;

    private float currentCourage;
    private float proxyLossThisRoom;
    private float killGainThisRoom;

    private bool isTerrified;
    private bool isInvulnerable;
    private bool roomEncounterActive;

    public float CourageLossOnProxy => courageLossOnProxy;

    public float CurrentCourage => currentCourage;
    public float MaxCourage => maxCourage;
    public float HealthPercent => currentCourage / maxCourage;
    public bool IsTerrified => isTerrified;
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    void Awake()
    {
        currentCourage = maxCourage;
    }

    void Update()
    {
        //if (invulnerabilityTimer > 0f)
        //{
        //    invulnerabilityTimer -= Time.deltaTime;
        //    if (invulnerabilityTimer <= 0f)
        //        isInvulnerable = false;
        //}
    }

    public void GetScared(float amount)
    {
        GetScared(amount, transform.position, Vector3.zero);
    }

    public void GetScared(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (isTerrified || isInvulnerable) return;

        currentCourage = Mathf.Max(0f, currentCourage - amount);
        OnCourageChanged?.Invoke(currentCourage, maxCourage);
        OnScared?.Invoke(hitPoint, hitDirection);

        //if (maxCourageLoss > 0f)
        //{
        //    isInvulnerable = true;
        //    invulnerabilityTimer = maxCourageLoss;
        //}

        if (currentCourage <= 0f)
            GetTerrified();
    }

    public void GetEncourage(float amount)
    {
        if (isTerrified) return;
        currentCourage = Mathf.Min(maxCourage, currentCourage + amount);
        OnCourageChanged?.Invoke(currentCourage, maxCourage);
        OnEncouraged?.Invoke();
    }

    public void ResetCourage()
    {
        isTerrified = false;
        currentCourage = maxCourage;
        isInvulnerable = false;
        OnCourageChanged?.Invoke(currentCourage, maxCourage);
    }

    void GetTerrified()
    {
        isTerrified = true;
        OnTerrified?.Invoke();

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }
}
