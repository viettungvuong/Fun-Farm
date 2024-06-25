using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public Material electrocutionMaterial;
    public float flashDuration = 0.1f;

    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashRoutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        spriteRenderer.material = electrocutionMaterial;
        electrocutionMaterial.SetFloat("_FlashAmount", 1);
        yield return new WaitForSeconds(flashDuration);
        electrocutionMaterial.SetFloat("_FlashAmount", 0);
        spriteRenderer.material = originalMaterial;
        flashRoutine = null;
    }
}
