using System.Collections;
using UnityEngine;

public class PlayerCollision : MonoBehaviour{
    //Player Script
    [SerializeField] private Player player;

    //Layer Masks
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    
    //Ground Collision Variables
    [SerializeField] protected internal float groundRaycastLength;
    [SerializeField] protected internal bool isGrounded;

    //Wall Collision Variables
    [SerializeField] protected internal float wallRaycastLength;
    [SerializeField] protected internal bool onWall;
    [SerializeField] protected internal bool onRightWall;

    protected internal void CheckCollision()
    {
        //Raycast is used to check for collision with the ground within a certain distance
        isGrounded = Physics2D.Raycast(transform.position * groundRaycastLength, Vector2.down, groundRaycastLength, groundLayer) ||
                     Physics2D.Raycast(transform.position * groundRaycastLength, Vector2.up, groundRaycastLength, groundLayer);

        //check collision with the wall
        onWall = Physics2D.Raycast(transform.position, Vector2.right, wallRaycastLength, wallLayer) ||
                 Physics2D.Raycast(transform.position, Vector2.left, wallRaycastLength, wallLayer);
        onRightWall = Physics2D.Raycast(transform.position, Vector2.right, wallRaycastLength, wallLayer);
    }

    protected internal void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * groundRaycastLength);

        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallRaycastLength);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallRaycastLength);
    }
}