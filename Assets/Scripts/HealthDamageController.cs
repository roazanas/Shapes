using DG.Tweening;
using System.Drawing;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthDamageController : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float damageAmount;
    [SerializeField] private float shockChancePercent = 10f;
    [SerializeField] private float shockDamagePercent = 25f;
    private float currentHealth;

    private GameObject healthBar;
    private Slider slider;

    RectTransform originalTransform;
    RectTransform clonedTransform;
    
    public Vector3 healthBarOffset = new(0f, -70f);

    [SerializeField] private float shockDuration;
    private float shockTimeLeft;
    [HideInInspector] public bool isShocked = false;
    private Rigidbody2D body;

    Image[] HBImages;
    UnityEngine.Color[] colors;
    SpriteRenderer sprite;

    private void Start()
    {
        InitHealthBar();
        currentHealth = maxHealth;
        slider = healthBar.GetComponent<Slider>();
        slider.maxValue = maxHealth;
        slider.value = slider.maxValue;

        body = GetComponent<Rigidbody2D>();
        HBImages = healthBar.GetComponentsInChildren<Image>();

        colors = new UnityEngine.Color[HBImages.Length-2];

        sprite = GetComponent<SpriteRenderer>();
    }

    private void InitHealthBar()
    {
        if (healthBar == null)
        {
            GameObject HBPrefab = Resources.Load<GameObject>("Prefabs/Health Bar");
            if (HBPrefab == null) { return; }
            GameObject instance = Instantiate(HBPrefab);

            instance.transform.SetParent(GameObject.Find("HP Canvas").transform);
            instance.SetActive(true);
            healthBar = instance;

            originalTransform = HBPrefab.GetComponent<RectTransform>();
            clonedTransform = healthBar.GetComponent<RectTransform>();
            clonedTransform.localPosition = originalTransform.localPosition;
            clonedTransform.localScale = originalTransform.localScale;
        }
    }

    private void UpdateHealthBar(bool isDeath = false)
    {
        if (healthBar == null || slider == null) { return; }

        Tween anim = slider.DOValue(currentHealth, 0.1f).SetEase(Ease.OutQuad);
        if (isDeath)
            anim.OnComplete(Die);
    }


    private void Die()
    { 
        if (healthBar != null) { Destroy(healthBar); }
        if (gameObject != null) { Destroy(gameObject); }
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
        DOTween.Kill(healthBar);
        DOTween.Kill(slider);
    }

    public void TakeDamage(float damage, Vector2 direction) 
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            UpdateHealthBar(true);
            return;
        }
        UpdateHealthBar();
        if (direction != Vector2.zero && Random.Range(1, 100) <= shockChancePercent)
        {
            GetShocked(direction);
        }
    }

    public void GetShocked(Vector2 direction, float shockDuration = 1f, float power = 40f)
    {
        if (isShocked) { return; }
        TakeDamage(maxHealth * shockDamagePercent / 100f, Vector2.zero);

        isShocked = true;
        float salt = Random.value;
        shockTimeLeft = shockDuration + salt;

        body.velocity = Vector2.zero;
        body.AddForce(direction * power, ForceMode2D.Impulse);
        body.AddTorque(power * 4 * (Random.value >= 0.5f ? -Random.value - 0.5f : Random.value + 0.5f));

        for (int i = 1; i < HBImages.Length-1; i++)
        {
            if (HBImages[i] == null) { continue; }
            colors[i-1] = HBImages[i].color;
            HBImages[i].DOColor(new UnityEngine.Color(242f / 255f, 211f / 255f, 97f / 255f),
                shockDuration / 2f)
                .SetEase(Ease.OutFlash)
                .SetId("HealthBarColorChange");
        }
        if (sprite == null) { return; }
        sprite.DOFade(0.25f, (shockDuration + salt) / 8f)
              .SetLoops(-1, LoopType.Yoyo)
              .SetEase(Ease.InOutSine)
              .SetId("ObjectFlick");
    }

    private void EndShock()
    {
        isShocked = false;
        body.totalForce = new Vector2(0.0f, 0.0f);
        for (int i = 1; i < HBImages.Length - 1; i++)
        {
            if (HBImages[i] == null) { continue; }
            HBImages[i].DOColor(colors[i - 1],
                shockDuration / 2f)
                .SetEase(Ease.OutBounce)
                .SetId("HealthBarColorChange");
        }
        DOTween.Kill("ObjectFlick");
    }

    private void Update()
    {
        if (isShocked) 
        {
            shockTimeLeft -= Time.deltaTime;
            if (shockTimeLeft <= 0)
            {
                EndShock();
            }
        }
    }

    private void LateUpdate()
    {
        clonedTransform.position = gameObject.transform.position + healthBarOffset / 90;
    }
}
