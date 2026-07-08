using UnityEngine;
using System.Collections.Generic;

public class ProximityCourageController : MonoBehaviour
{
    [Header("Courage Controller")]
    [SerializeField] private CourageController _playerCourage;

    [Header("Scared Area")]
    [SerializeField] private float _scaredRadius = 5f;

    [Header("Gizmo")]
    [SerializeField] private bool _drawGizmo = true;
    [SerializeField] private bool _drawSolid = false;
    [SerializeField] private Color _gizmoColor = Color.orange;

    private SphereCollider _sphereCollider;

    private class NearbyEnemy
    {
        public EnemyAI Enemy;
        public float timer;
    }

    private readonly Dictionary<EnemyAI, NearbyEnemy> _nearbyEnemies = new();

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        
        if (_playerCourage == null)
            _playerCourage = GetComponentInParent<CourageController>();

        ConfigureCollider();
    }

    private void OnValidate()
    {
        _scaredRadius = Mathf.Max(0.1f, _scaredRadius);

        _sphereCollider = GetComponent<SphereCollider>();

        if (_playerCourage == null)
            _playerCourage = GetComponentInParent<CourageController>();

        ConfigureCollider();
    }

    private void Update()
    {
        UpdateNearbyEnemies();
    }

    private void ConfigureCollider()
    {
        if (_sphereCollider == null) return;

        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = _scaredRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
        if (enemy == null) return;

        if (_nearbyEnemies.ContainsKey(enemy)) return;

        _nearbyEnemies.Add(enemy, new NearbyEnemy { Enemy = enemy, timer = 0f });
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
        if (enemy == null) return;
        _nearbyEnemies.Remove(enemy);
    }

    private void OnDisable()
    {
        _nearbyEnemies.Clear();
    }

    private void UpdateNearbyEnemies()
    {
        if (_playerCourage == null) return;

        if (_nearbyEnemies.Count == 0) return;

        List<EnemyAI> enemiesToRemove = null;

        foreach (KeyValuePair<EnemyAI, NearbyEnemy> kvp in _nearbyEnemies)
        {
            EnemyAI enemy = kvp.Key;
            NearbyEnemy nearbyEnemy = kvp.Value;
            if (enemy == null || enemy.Data == null)
            {
                enemiesToRemove ??= new List<EnemyAI>();
                enemiesToRemove.Add(enemy);
                continue;
            }

            if (enemy.CurrentState == EnemyAI.State.Dead)
            {
                enemiesToRemove ??= new List<EnemyAI>();
                enemiesToRemove.Add(enemy);
                continue;
            }

            EnemyData data = enemy.Data;

            if (data.scareProxy <= 0f) continue;

            float interval = Mathf.Max(0.1f, data.scareProxyTimer);

            nearbyEnemy.timer += Time.deltaTime;

            if (nearbyEnemy.timer < interval) continue;

            nearbyEnemy.timer = 0f;

            Vector3 sourcePos = enemy.transform.position;

            Vector3 scareDir = (_playerCourage.transform.position - sourcePos).normalized;

            _playerCourage.LoseFromProximity(data.scareProxy, sourcePos, scareDir);
        }

        if (enemiesToRemove == null) return;

        foreach (EnemyAI enemy in enemiesToRemove)
        {
            _nearbyEnemies.Remove(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmo) return;

        DrawProximityGizmo(_gizmoColor, true);
    }

    private void OnDrawGizmosSelected()
    {
        DrawProximityGizmo(Color.yellow, false);
    }

    private void DrawProximityGizmo(Color color, bool isWire)
    {
        SphereCollider currentCollider = GetComponent<SphereCollider>();

        Vector3 center = transform.position;

        if (currentCollider != null)
        {
            center = transform.TransformPoint(currentCollider.center);
        }

        float largestScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));

        float worldRadius = _scaredRadius * largestScale;

        Gizmos.color = color;

        Gizmos.DrawWireSphere(center, worldRadius);

        if (!_drawSolid) return;

        Color transparentColor = color;
        transparentColor.a = 0.15f;

        Gizmos.color = transparentColor;
        Gizmos.DrawSphere(center, worldRadius);
    }
}