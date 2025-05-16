using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool interact;
		public bool attack;
		public bool attackTriggered;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputAction.CallbackContext context)
		{
			move = context.ReadValue<Vector2>();
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			if (cursorInputForLook)
			{
				look = context.ReadValue<Vector2>();
			}
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.performed || context.started)
				jump = true;
			if (context.canceled)
				jump = false;
		}

		public void OnSprint(InputAction.CallbackContext context)
		{
			if (context.performed || context.started)
				sprint = true;
			if (context.canceled)
				sprint = false;
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				aim = true;
			}
			else if (context.canceled)
			{
				aim = false;
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.started)
			{
				attack = true;
				attackTriggered = true;
			}

			if (context.canceled)
			{
				attack = false;
			}
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.started || context.performed)
				interact = true;
			if (context.canceled)
				interact = false;
		}


#endif


		// public void MoveInput(Vector2 newMoveDirection)
		// {
		// 	move = newMoveDirection;
		// }

		// public void LookInput(Vector2 newLookDirection)
		// {
		// 	look = newLookDirection;
		// }

		// public void JumpInput(bool newJumpState)
		// {
		// 	jump = newJumpState;
		// }

		// public void SprintInput(bool newSprintState)
		// {
		// 	sprint = newSprintState;
		// }

		// public void AimInput(bool newAimState)
		// {
		// 	aim = newAimState;
		// }

		// public void AttackInput(bool newAttackState)
		// {
		// 	attack = newAttackState;
		// }

		// private void InteractInput(bool newInteractState)
		// {
		// 	interact = newInteractState;
		// }

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

}