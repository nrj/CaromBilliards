using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class Ball : MonoBehaviour
{
    public event Action<BallCollisionVO> OnCollision = delegate { };

    public int uid;

    public BallType ballType;

    public SpriteRenderer chargeBar;

    private MaterialPropertyBlock _materialProperties;

    void Start()
    {
        UpdateMaterialColor();
    }

    void OnValidate()
    {
        UpdateMaterialColor();
    }
    void UpdateMaterialColor()
    {
        Color color;
        switch (ballType)
        {
            default:
            case BallType.cue:
                color = Color.white;
                break;
            case BallType.red:
                color = Color.red;
                break;
            case BallType.yellow:
                color = Color.yellow;
                break;
        }
        if (_materialProperties == null)
        {
            _materialProperties = new MaterialPropertyBlock();
        }
        _materialProperties.SetColor("_Color", color);
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.SetPropertyBlock(_materialProperties);
    }

    public void SetCharge(float amount, Vector3 direction)
    {
        // Compute the width from 0 - max
        var maxHeight = 10.0f;
        var barHeight = Mathf.Lerp(0, maxHeight, Mathf.Clamp01(amount));
        chargeBar.size = new Vector2(chargeBar.size.x, barHeight);
        // Set the bar color between green - red
        var barColor = Color.Lerp(Color.green, Color.red, amount);
        chargeBar.color = barColor;
        // Compute the angle from our direction
        var yAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var barRotation = chargeBar.transform.rotation.eulerAngles;
        var newBarRotation = Quaternion.Euler(barRotation.x, yAngle, barRotation.z);
        chargeBar.transform.rotation = newBarRotation;
        // Position the bar a little bit in front of the ball
        var barPadding = 1f;
        var barPosition = direction * (barPadding + 0.5f * barHeight);
        chargeBar.transform.localPosition = -barPosition;
        chargeBar.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // We only care about collisions with other balls
        var otherBall = collision.gameObject.GetComponent<Ball>();
        if (otherBall == null)
        {
            return;
        }
        // Construct our value object describing the collision
        var ballCollision = new BallCollisionVO()
        {
            srcType = this.ballType,
            srcId = this.uid,
            dstType = otherBall.ballType,
            dstId = otherBall.uid,
            magnitude = collision.relativeVelocity.magnitude
        };
        // Delegate the collision
        OnCollision(ballCollision);
    }
}
