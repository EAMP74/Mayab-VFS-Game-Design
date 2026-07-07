using UnityEngine;
using UnityEngine.Events;

public class CourageController : MonoBehaviour
{
    [Header("Courage")]
    [SerializeField] private float maxCourage = 100f;
    [SerializeField] private float initialCourage = 20f;
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

    public float CurrentCourage => currentCourage;
    public float MaxCourage => maxCourage;
    public float CouragePercent => maxCourage > 0f ? currentCourage / maxCourage: 0f;
    public float ProxyLossThisRoom => proxyLossThisRoom;
    public float RemainingProxyLoss => Mathf.Max(0f, courageLossOnProxy - proxyLossThisRoom);
    public float KillGainThisRoom => killGainThisRoom;
    public float RemainingKillGain => Mathf.Max(0f, courageGainOnKill - killGainThisRoom);
    public bool IsTerrified => isTerrified;
    public bool IsInvulnerable { get => isInvulnerable; set => isInvulnerable = value; }

    void Awake()
    {
        currentCourage = initialCourage;
    }

    private void Start()
    {
        OnCourageChanged?.Invoke(currentCourage, maxCourage);
    }

    public void BeginRoomEncounter()
    {
        roomEncounterActive = true;
        proxyLossThisRoom = 0f;
        killGainThisRoom = 0f;
    }

    public void EndRoomEncounter()
    {
        roomEncounterActive = false;
    }

    public void LoseFromProximity(float amount)
    {
        LoseFromProximity(amount, transform.position, Vector3.zero);
    }

    public void LoseFromProximity(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!CanChangeCourage() || amount <= 0f) return;

        float allowedAmount = amount;

        if (roomEncounterActive)
        {
            allowedAmount = Mathf.Min(amount, RemainingProxyLoss);
        }

        if (allowedAmount <= 0f) return;

        proxyLossThisRoom += allowedAmount;

        ApplyCourageLoss(allowedAmount, hitPoint, hitDirection);
    }

    public void LoseFromHit(float amount)
    {
        LoseFromHit(amount, transform.position, Vector3.zero);
    }

    public void LoseFromHit(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!CanChangeCourage() || amount <= 0f) return;
        ApplyCourageLoss(amount, hitPoint, hitDirection);
    }

    public void GainFromKill(float amount)
    {
        if (isTerrified || amount <= 0f) return;

        float allowedAmount = amount;

        if (roomEncounterActive)
        {
            allowedAmount = Mathf.Min(amount, RemainingKillGain);
        }

        if (allowedAmount <= 0f) return;

        killGainThisRoom += allowedAmount;
        ApplyCourageGain(allowedAmount);
    }

    public void GainFromEncouragement(float amount)
    {
        if (isTerrified || amount <= 0f) return;
        ApplyCourageGain(amount);
    }

    public void ResetCourage()
    {
        isTerrified = false;
        currentCourage = maxCourage;
        isInvulnerable = false;
        proxyLossThisRoom = 0f;
        killGainThisRoom = 0f;
        OnCourageChanged?.Invoke(currentCourage, maxCourage);
    }

    void GetTerrified()
    {
        if (isTerrified) return;

        isTerrified = true;
        OnTerrified?.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private bool CanChangeCourage()
    {
        return !isTerrified && !isInvulnerable;
    }

    private void ApplyCourageLoss(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        float previousCourage = currentCourage;
        currentCourage = Mathf.Max(0f, currentCourage - amount);
        float actualLoss = previousCourage - currentCourage;

        if (actualLoss <= 0f) return;

        OnCourageChanged?.Invoke(currentCourage, maxCourage);
        OnScared?.Invoke(hitPoint, hitDirection);

        if (currentCourage <= 0f)
            GetTerrified();
    }

    private void ApplyCourageGain(float amount)
    {
        float previousCourage = currentCourage;
        currentCourage = Mathf.Min(maxCourage, currentCourage + amount);
        float actualGain = currentCourage - previousCourage;

        if (actualGain <= 0f) return;

        OnCourageChanged?.Invoke(currentCourage, maxCourage);
        OnEncouraged?.Invoke();
    }
}
