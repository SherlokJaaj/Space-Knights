using UnityEngine;

public class Torchka : MonoBehaviour
{
    internal TorchkaManager manager;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        var sprite = spriteRenderer.sprite;
        var normalizedPivot = sprite.pivot / sprite.rect.size;
        spriteRenderer.sprite = Sprite.Create(Instantiate(sprite.texture),
                                    sprite.rect, normalizedPivot, sprite.pixelsPerUnit);
    }

    // anyone in layer 'Damage' will trigger this
    private void OnTriggerStay2D(Collider2D other)
    {
        bool damage = manager.CheckForDamage(spriteRenderer.sprite.texture,
             spriteRenderer.transform.InverseTransformPoint(other.transform.position));

        if (other.GetComponent<Bullet>() && damage)
        {
            other.GetComponent<Bullet>().DestroySelf();
        }
    }
}
