using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    const float _skinWidth = .015f;
    public int HorizontalRayCount = 4;
    public int VerticalRayCount = 4;

    public LayerMask layerMask;

    float _horizontalRaySpacing;
    float _verticalRaySpacing;

    RayCastOrigins _rayCastOrigins; 
    [HideInInspector]
    public CollisionInfos collisionInfos;
    BoxCollider2D _collider;

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }


    public void Move(Vector2 moveAmount){
        CalculateRayOrigins();
        collisionInfos.Reset();

        if(moveAmount.y!=0)
            VerticalCollisions(ref moveAmount);
        if(moveAmount.x!=0)
            HorizontalCollisons(ref moveAmount);

        transform.Translate(moveAmount);
    }

    void VerticalCollisions(ref Vector2 moveAmount){
        float directionY = Mathf.Sign(moveAmount.y);
        float rayDistance = _skinWidth + Mathf.Abs(moveAmount.y);

        Vector2 rayOrigin = (directionY == -1)?_rayCastOrigins.bottomLeft:_rayCastOrigins.topLeft;
        rayOrigin += moveAmount.x*Vector2.right;
        for(int i = 0; i< HorizontalRayCount; i++){
           
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.up*directionY,rayDistance,layerMask);

            Debug.DrawRay(rayOrigin,Vector2.up*directionY*rayDistance,Color.red);

            if(hit){
                moveAmount.y = (hit.distance-_skinWidth)*directionY;
                //to prevent movement into walls by further raycasts
                rayDistance = hit.distance;

                collisionInfos.above = directionY == 1f;
                collisionInfos.below = directionY == -1f;
            }

            rayOrigin += _horizontalRaySpacing*Vector2.right;
        }

        
    }

    void HorizontalCollisons(ref Vector2 moveAmount){
        float directionX = Mathf.Sign(moveAmount.x);
        float rayDistance = _skinWidth + Mathf.Abs(moveAmount.x);

        Vector2 rayOrigin = (directionX == -1)?_rayCastOrigins.bottomLeft:_rayCastOrigins.bottomRight;
        rayOrigin += moveAmount.y*Vector2.up;
        for(int i = 0; i< VerticalRayCount; i++){
           
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right*directionX,rayDistance,layerMask);

            Debug.DrawRay(rayOrigin,Vector2.right*directionX*rayDistance,Color.red);

            if(hit){
                moveAmount.x = (hit.distance-_skinWidth)*directionX;
                //to prevent movement into walls by further raycasts
                rayDistance = hit.distance;

                
                collisionInfos.left = directionX == -1f;
                collisionInfos.right = directionX == -1f;
            }

            rayOrigin += _verticalRaySpacing*Vector2.up;
        }
    }

    void CalculateRaySpacing(){
        Bounds bounds = _collider.bounds;
        bounds.Expand(-2*_skinWidth);

        //make sure we have atleast 2 rays;
        HorizontalRayCount = Mathf.Clamp(HorizontalRayCount,2,int.MaxValue);
        VerticalRayCount = Mathf.Clamp(VerticalRayCount,2,int.MaxValue);

        _horizontalRaySpacing = bounds.size.x/(HorizontalRayCount-1);
        _verticalRaySpacing = bounds.size.y/(VerticalRayCount-1);
    }

    void CalculateRayOrigins(){
        Bounds bounds = _collider.bounds;
        bounds.Expand(-2*_skinWidth);

        _rayCastOrigins.topLeft = new Vector2(bounds.min.x,bounds.max.y);
        _rayCastOrigins.topRight = new Vector2(bounds.max.x,bounds.max.y);
        _rayCastOrigins.bottomLeft = new Vector2(bounds.min.x,bounds.min.y);
        _rayCastOrigins.bottomRight = new Vector2(bounds.max.x,bounds.min.y);

    }

    struct RayCastOrigins{
        public Vector2 topRight,topLeft;
        public Vector2 bottomLeft,bottomRight;
    }

    public struct CollisionInfos{
        public bool above,below;
        public bool left,right;

        public void Reset(){
            above = below = left = right = false;
        }
    }
}
