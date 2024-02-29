using System;
using UnityEngine;

public class CarApproachIndicator : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private WarningData _warningData;
    
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer($"Vehicles")) return;


        var transform1 = collision.transform;
        var direction = transform1.position - playerTransform.position;
        var forward = transform1.forward;
        var isMovingTowardsPlayer = Vector3.Dot(direction.normalized, forward) < -0.6f;
        var distance = direction.magnitude;

        if (isMovingTowardsPlayer)
        {
            DisplayWarningIndicator(direction, distance);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer($"Vehicles"))
        {
            RemoveWarningIndicator();
        }
    }

    private void DisplayWarningIndicator(Vector3 direction, float distance)
    {
        var angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        var directionLabel = GetDirectionLabel(angle);
        var warningLevel = GetWarningLevel(distance);

        var newWarningData = new WarningData(directionLabel, warningLevel);

        if (!newWarningData.Equals(_warningData))
        {
            _warningData = newWarningData;
            GameManager.Instance.ShowWarning(_warningData, true);
        }
        
    }
    
    private eDirection GetDirectionLabel(float angle)
    {
        switch (angle)
        {
            case > -45 and < 45:
                return eDirection.Up;
            case > 45 and < 120:
                return eDirection.Right;
            case < -45 and > -120:
                return eDirection.Left;
            default:
                return eDirection.Down;
        }
    }

    private eWarningLevel GetWarningLevel(float distance)
    {
        switch (distance)
        {
            case > 10:
                return eWarningLevel.Low;
            case > 5:
                return eWarningLevel.Medium;
            default:
                return eWarningLevel.High;
        }
    }

    

    private void RemoveWarningIndicator()
    {
        GameManager.Instance.ShowWarning(_warningData, false);
    }
}
public enum eDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum eWarningLevel
{
    Low,
    Medium,
    High
}

public class WarningData : IEquatable<WarningData>
{
    private eDirection _direction;
    private eWarningLevel _warningLevel;

    public eDirection Direction => _direction;

    public eWarningLevel WarningLevel => _warningLevel;

    public WarningData(eDirection direction, eWarningLevel warningLevel)
    {
        _direction = direction;
        _warningLevel = warningLevel;
    }

    public bool Equals(WarningData other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _direction == other._direction && _warningLevel == other._warningLevel;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((WarningData)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)_direction, (int)_warningLevel);
    }
}


