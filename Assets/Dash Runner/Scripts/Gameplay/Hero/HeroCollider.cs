using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCollider : MonoBehaviour
{
    /// <summary>
    /// Reference to the Hero script.
    /// </summary>
    Hero hero;

    void Start()
    {
        hero = GetComponentInParent<Hero>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        List<Collider2D> handledColliders = new List<Collider2D>();
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (!handledColliders.Contains(contact.collider))
            {
                Destructible destructibleScript = contact.collider.GetComponent<Destructible>();
                if (destructibleScript == null)
                {
                    hero.GameOver(collision.contacts[0]);
                    break;
                }
                else
                {
                    if (hero.IsDashing)
                    {
                        destructibleScript.Destroy();
                    }
                    else
                    {
                        hero.GameOver(collision.contacts[0]);
                    }
                }

                handledColliders.Add(contact.collider);
            }
        }
    }
}
