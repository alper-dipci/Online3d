using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;

    private Camera cam;

    private float target = 1f;
    private float ReduceSpeed = 2f;
    private void Start()
    {
        cam = Camera.main;
    }
    public void updateHealthBar(float currentHealth, float maxHealth)
    {
        target = currentHealth / maxHealth;
    }
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, target, ReduceSpeed * Time.deltaTime);
    }
}
