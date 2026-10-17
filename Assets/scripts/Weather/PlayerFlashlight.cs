using UnityEngine;
using UnityEngine.Rendering.Universal;

// Drop onto the Player GameObject.
// Auto-finds GameClock, PlayerFacing, and searches children for a Light2D.
// Only manual step: add a child GameObject to Player with a Light2D (Spot) component.
public class PlayerFlashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float innerAngle = 20f;
    public float outerAngle = 35f;
    public float intensity  = 1.2f;
    public Color lightColor = new Color(1f, 0.95f, 0.8f, 1f);
    public float range      = 5f;

    [Header("Toggle")]
    public KeyCode toggleKey = KeyCode.T;

    private Light2D flashlight;
    private GameClock clock;
    private PlayerFacing facing;
    private bool manualOverride = false;
    private bool manualState    = false;

    private static readonly float RotationUp    =  90f;
    private static readonly float RotationDown  = -90f;
    private static readonly float RotationLeft  = 180f;
    private static readonly float RotationRight =   0f;

    private void Start()
    {
        // Auto-find all dependencies
        clock  = GameClock.Instance ?? FindAnyObjectByType<GameClock>();
        facing = GetComponent<PlayerFacing>();

        // Auto-find Light2D in children
        flashlight = GetComponentInChildren<Light2D>();

        if (flashlight == null)
        {
            Debug.LogWarning("[PlayerFlashlight] No Light2D found in children. Add a child GameObject with a Spot Light2D.");
            return;
        }

        if (clock   == null) Debug.LogWarning("[PlayerFlashlight] No GameClock found in scene.");
        if (facing  == null) Debug.LogWarning("[PlayerFlashlight] No PlayerFacing found on Player.");

        flashlight.pointLightInnerAngle  = innerAngle;
        flashlight.pointLightOuterAngle  = outerAngle;
        flashlight.intensity             = intensity;
        flashlight.color                 = lightColor;
        flashlight.pointLightOuterRadius = range;
        flashlight.enabled               = false;
    }

    private void Update()
    {
        if (flashlight == null) return;

        if (Input.GetKeyDown(toggleKey))
        {
            manualOverride = true;
            manualState    = !manualState;
        }

        bool shouldBeOn = manualOverride ? manualState : (clock != null && clock.isNight);
        flashlight.enabled = shouldBeOn;

        if (shouldBeOn && facing != null)
            UpdateRotation(facing.LastDirection);
    }

    private void UpdateRotation(Vector2 direction)
    {
        float angle;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            angle = direction.x > 0 ? RotationRight : RotationLeft;
        else
            angle = direction.y > 0 ? RotationUp : RotationDown;

        flashlight.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ResetToAuto() => manualOverride = false;
}
