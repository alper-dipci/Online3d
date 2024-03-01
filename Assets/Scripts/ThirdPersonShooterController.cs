using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;
using System;

public class ThirdPersonShooterController : NetworkBehaviour, IDamagable
{
	[SerializeField] private Gun currentGun;
	[SerializeField] private HealthBar healthBar;

	public float maxHealth = 100;
	public float currentHealth = 100;

	private ThirdPersonController thirdPersonController;
	private Cinemachine.CinemachineVirtualCamera focusCamera;
	private Animator animator;
	private Vector3 mouseWorldPosition = Vector3.zero;
	private Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

	private bool isAiming = false;
	public override void OnNetworkSpawn()
	{
		if (!IsOwner) return;
		animator = GetComponent<Animator>();
		thirdPersonController = GetComponent<ThirdPersonController>();
		focusCamera = GameObject.FindGameObjectWithTag("FocusCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>();
		healthBar.updateHealthBar(currentHealth, maxHealth);
		//gecici degisecekkkkkk!!
		currentGun = GetComponent<Gun>();

		base.OnNetworkSpawn();
	}
	private void Update()
	{
		if (!IsOwner) return;
		handleSmoothZoomAnim();
	}

	private void handleSmoothZoomAnim()
	{
		if (isAiming)
		{
			Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
			if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f))
			{
				mouseWorldPosition = raycastHit.point;
			}
			else
			{
				mouseWorldPosition = ray.GetPoint(100f);
			}
			Vector3 worldAimTarget = mouseWorldPosition;
			worldAimTarget.y = transform.position.y;
			Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
			transform.forward = Vector3.Lerp(transform.forward, aimDirection, 20f * Time.deltaTime);

			//animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, 10f * Time.deltaTime));
		}

		else
		{
			//animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, 10f * Time.deltaTime));
		}

	}

	public void Shoot()
	{
		currentGun.Shoot();
	}
	public void Aim(bool isCameraFocused)
	{
		isAiming = isCameraFocused;

		if (isCameraFocused)
		{
			focusCamera.Priority = 30;
			thirdPersonController.setSensivity(thirdPersonController.aimSensivity);
		}

		else
		{
			thirdPersonController.setSensivity(thirdPersonController.sensivity);
			focusCamera.Priority = 10;
		}
	}

	public void getHit(float amount)
	{
		currentHealth -= amount;
		healthBar.updateHealthBar(currentHealth, maxHealth);
	}

	public ulong getNetworkObjectId()
	{
		return NetworkObjectId;
	}
}
