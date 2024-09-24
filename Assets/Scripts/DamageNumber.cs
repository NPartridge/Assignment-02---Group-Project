using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float fadeDuration = 1f;

    [SerializeField] private Color nonCritColour = Color.white;
    [SerializeField] private Color critColour = Color.red; 
    
    private TextMeshProUGUI damageText;
    private Color originalColor;
    private float timer;

    void Awake()
    {
        damageText = GetComponentInChildren<TextMeshProUGUI>();
        if (damageText == null)
        {
            Debug.LogError("Need a textmesh");
        }
        else
        {
            originalColor = damageText.color;
        }
    }

    void Update()
    {
        // We need a text mesh
        if (damageText == null) return;

        // Move the damage number upwards. We find the initial pos in ShowDamageNumber()
        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
        
        // This section here handles the number fading out. All we are doing is decreasing the alpha over time to fade out
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeDuration);
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        
        // After the number has faded enough we destroy it
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage, bool isCrit)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString();
            damageText.color = isCrit ? critColour : nonCritColour; // If the damage was a crit we use a different colour
            originalColor = damageText.color;
        }
        else
        {
            Debug.LogError("textmesh is null d");
        }
    }
}