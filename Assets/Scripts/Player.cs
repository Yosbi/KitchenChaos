using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private GameInput gameInput;

    private bool isWalking = false;
    private Vector3 lastInteractDir;

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
            lastInteractDir = moveDir;

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // Has clearCounter
                clearCounter.Interact();
            }
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, 
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir, 
            moveDistance);

        if (!canMove) {
            // Cannot move towards exact moveDir
            // Attempting only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, 
                transform.position + Vector3.up * playerHeight,
                playerRadius,
                moveDirX, 
                moveDistance);

            if (canMove) {
                moveDir = moveDirX;
            } else {
                // Cannot move only on X
                // Attempting to only Z move
                 Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, 
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirZ, 
                    moveDistance);
                
                 if (canMove) {
                    moveDir = moveDirZ;
                } else {
                    // Cannot move at all
                }
            }

        }

        if (canMove)
            transform.position += moveDir * moveDistance;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    
        isWalking = moveDir != Vector3.zero;
    }
}
