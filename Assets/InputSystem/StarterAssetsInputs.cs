using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Unity.Netcode;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : NetworkBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		public bool isCameraFocused = false;
		public bool isShooting ;
		private PlayerInputClass playerinputClass;


		private ThirdPersonController thirdPersonController;
		private ThirdPersonShooterController thirdPersonShooterController;

		public static Action reloadInput;


        public override void OnNetworkSpawn()
        {
			if (!IsOwner) return;
			thirdPersonController = GetComponent<ThirdPersonController>();
			thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();

			playerinputClass = new PlayerInputClass();
			playerinputClass.Player.Enable();
			playerinputClass.Player.Jump.performed += OnJump;
			playerinputClass.Player.Sprint.performed += OnSprint;
			playerinputClass.Player.Escape.performed += OnEscape;
			playerinputClass.Player.Focus.performed += OnFocus;
			playerinputClass.Player.Reload.performed += OnReload;

			base.OnNetworkSpawn();
        }

        private void OnReload(InputAction.CallbackContext obj)
        {
			reloadInput?.Invoke();
        }

        private void Update()
        {
			if (!IsOwner) return;
			MoveInput(playerinputClass.Player.Move.ReadValue<Vector2>());
			LookInput(playerinputClass.Player.Look.ReadValue<Vector2>());
			if(playerinputClass.Player.Shoot.IsPressed())
				thirdPersonShooterController.Shoot();

		}

		public void OnFocus(InputAction.CallbackContext value)
		{
			isCameraFocused = !isCameraFocused;
			thirdPersonShooterController.Aim(isCameraFocused);
		}

		public void OnJump(InputAction.CallbackContext value)
		{
			JumpInput(value.performed);
		}

		public void OnSprint(InputAction.CallbackContext value)
		{
			SprintInput(value.performed);
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		public void OnEscape(InputAction.CallbackContext context)
		{
			cursorLocked = !cursorLocked;
			Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}