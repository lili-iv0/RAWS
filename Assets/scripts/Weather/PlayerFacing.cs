using UnityEngine;

// Drop onto the Player GameObject.
// Auto-finds SpriteRenderer on same GameObject.
// Assign four directional sprites in Inspector.
public class PlayerFacing : MonoBehaviour
{
    [Header("Directional Sprites")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private SpriteRenderer sr;

    public Vector2 LastDirection { get; private set; } = Vector2.down;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogWarning("[PlayerFacing] No SpriteRenderer found on Player.");
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0)
        {
            if (Mathf.Abs(x) > Mathf.Abs(y))
                LastDirection = x > 0 ? Vector2.right : Vector2.left;
            else
                LastDirection = y > 0 ? Vector2.up : Vector2.down;
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (sr == null) return;
        if      (LastDirection == Vector2.up)    sr.sprite = spriteUp;
        else if (LastDirection == Vector2.down)  sr.sprite = spriteDown;
        else if (LastDirection == Vector2.left)  sr.sprite = spriteLeft;
        else if (LastDirection == Vector2.right) sr.sprite = spriteRight;
    }
}
