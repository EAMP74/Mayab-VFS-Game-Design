using UnityEngine;
using UnityEngine.Events;

public class CourageController : MonoBehaviour
{
    [Header("Courage")]
    [SerializeField] private float _maxCourage = 100f;
    [SerializeField] private float _initialCourage = 20f;
    [SerializeField] private bool _destroyOnDeath;
    [SerializeField] private float _destroyDelay;

    [Header("Room Caps")]
    [SerializeField] private float _courageLossOnProxy = 30f;
    [SerializeField] private float _courageGainOnKill = 20f;

    [Header("Events")]
    public UnityEvent<float, float> OnCourageChanged;
    public UnityEvent<Vector3, Vector3> OnScared;
    public UnityEvent OnTerrified;
    public UnityEvent OnEncouraged;

    private float _currentCourage;
    private float _proxyLossThisRoom;
    private float _killGainThisRoom;

    private bool _isTerrified;
    private bool _isInvulnerable;
    private bool _roomEncounterActive;

    public float CurrentCourage => _currentCourage;
    public float MaxCourage => _maxCourage;
    public float CouragePercent => _maxCourage > 0f ? _currentCourage / _maxCourage: 0f;
    public float ProxyLossThisRoom => _proxyLossThisRoom;
    public float RemainingProxyLoss => Mathf.Max(0f, _courageLossOnProxy - _proxyLossThisRoom);
    public float KillGainThisRoom => _killGainThisRoom;
    public float RemainingKillGain => Mathf.Max(0f, _courageGainOnKill - _killGainThisRoom);
    public bool IsTerrified => _isTerrified;
    public bool IsInvulnerable { get => _isInvulnerable; set => _isInvulnerable = value; }

    void Awake()
    {
        _currentCourage = _initialCourage;
    }

    private void Start()
    {
        OnCourageChanged?.Invoke(_currentCourage, _maxCourage);
    }

    public void BeginRoomEncounter()
    {
        _roomEncounterActive = true;
        _proxyLossThisRoom = 0f;
        _killGainThisRoom = 0f;
    }

    public void EndRoomEncounter()
    {
        _roomEncounterActive = false;
    }

    public void LoseFromProximity(float amount)
    {
        LoseFromProximity(amount, transform.position, Vector3.zero);
    }

    public void LoseFromProximity(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!CanChangeCourage() || amount <= 0f) return;

        float allowedAmount = amount;

        if (_roomEncounterActive)
        {
            allowedAmount = Mathf.Min(amount, RemainingProxyLoss);
        }

        if (allowedAmount <= 0f) return;

        _proxyLossThisRoom += allowedAmount;

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
        if (_isTerrified || amount <= 0f) return;

        float allowedAmount = amount;

        if (_roomEncounterActive)
        {
            allowedAmount = Mathf.Min(amount, RemainingKillGain);
        }

        if (allowedAmount <= 0f) return;

        _killGainThisRoom += allowedAmount;
        ApplyCourageGain(allowedAmount);
    }

    public void GainFromEncouragement(float amount)
    {
        if (_isTerrified || amount <= 0f) return;
        ApplyCourageGain(amount);
    }

    public void ResetCourage()
    {
        _isTerrified = false;
        _currentCourage = _maxCourage;
        _isInvulnerable = false;
        _proxyLossThisRoom = 0f;
        _killGainThisRoom = 0f;
        OnCourageChanged?.Invoke(_currentCourage, _maxCourage);
    }

    void GetTerrified()
    {
        if (_isTerrified) return;

        _isTerrified = true;
        OnTerrified?.Invoke();

        if (_destroyOnDeath)
        {
            Destroy(gameObject, _destroyDelay);
        }
    }

    private bool CanChangeCourage()
    {
        return !_isTerrified && !_isInvulnerable;
    }

    private void ApplyCourageLoss(float amount, Vector3 hitPoint, Vector3 hitDirection)
    {
        float previousCourage = _currentCourage;
        _currentCourage = Mathf.Max(0f, _currentCourage - amount);
        float actualLoss = previousCourage - _currentCourage;

        if (actualLoss <= 0f) return;

        OnCourageChanged?.Invoke(_currentCourage, _maxCourage);
        OnScared?.Invoke(hitPoint, hitDirection);

        if (_currentCourage <= 0f)
            GetTerrified();
    }

    private void ApplyCourageGain(float amount)
    {
        float previousCourage = _currentCourage;
        _currentCourage = Mathf.Min(_maxCourage, _currentCourage + amount);
        float actualGain = _currentCourage - previousCourage;

        if (actualGain <= 0f) return;

        OnCourageChanged?.Invoke(_currentCourage, _maxCourage);
        OnEncouraged?.Invoke();
    }
}
