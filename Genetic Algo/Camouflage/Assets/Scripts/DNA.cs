using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    // gene for colour
    public float r;
    public float g;
    public float b;
    // gene for size
    public float s;

    public float timeToDie = 0f;  // how long has this entity survived

    private bool dead = false;
    private SpriteRenderer renderer;
    private Collider2D collider;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        renderer.color = new Color(r, g, b);
        transform.localScale = new Vector3(s, s, s);
    }

    private void OnMouseDown()
    {
        dead = true;
        timeToDie = PopulationManager.elapsed;
        renderer.enabled = false;
        collider.enabled = false;
    }
}
