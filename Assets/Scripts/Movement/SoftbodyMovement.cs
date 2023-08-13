
using System;
using System.Collections.Generic;
using UnityEngine;
using Obi;


[RequireComponent(typeof(ObiSoftbody))]
public class SoftbodyMovement: MonoBehaviour
{
    public ObiSolver solver;
    [Header("IdleWalkRun")]
    public Transform referenceFrame;
    public float acceleration = 3;
    
    [Header("Jump")]
    public float jumpPower = 5;

    [Range(0,1)]
    public float airControl = 0.2f;
    
    [Header("Grapple")]
    public KeyCode grappleKey = KeyCode.E;
    public LayerMask whatIsGrappleable;
    public float maxGrappleDistance;
    public float overshootYAxis;

    [Header("Swing")] 
    public KeyCode swingKey = KeyCode.Q;
    public Transform orientation;
    public float horizontalAcceleration;
    public float forwardAcceleration;
    public float extendCableSpeed;
    public LineRenderer lr;
    public Transform gunTip;


    public enum MovementState
    {
        IdleWalkRun,
        Jump,
        Grapple,
        Swing,
    }

    /*** private fields ***/
    private IState curState;
    private Dictionary<MovementState, IState> States = new Dictionary<MovementState, IState>();
    private ObiSoftbody softbody;
    
    private bool onGround = false;
    private Vector3 grapplePoint;
    private bool isGrappling = false;

    private bool isSwinging = false;


    public Vector3 GrapplePoint
    {
        get => grapplePoint;
        set =>  grapplePoint = value;
    }
    
    public bool IsGrappling
    {
        get => isGrappling;
        set => isGrappling = value;
    }
    
    public bool IsSwinging
    {
        get => isSwinging;
        set => isSwinging = value;
    }
    
    public bool OnGround
    {
        get => onGround;
        set => onGround = value;
    }
    

    private void Awake()
    {
        RegisterState(); //register every enum type to a state
        ChangeState(MovementState.IdleWalkRun);
    }


    void Start()
    {
        softbody = GetComponent<ObiSoftbody>();
        if (softbody == null)
        {
            Debug.LogError("No ObiSoftbody component found");
        }
        softbody.solver.OnCollision += Solver_OnCollision;
    }

    private void OnDestroy()
    {
        softbody.solver.OnCollision -= Solver_OnCollision;
    }
    
    void Update()
    {
        
        curState.Execute();
        

        if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            ChangeState(MovementState.Grapple);
        }

        if (Input.GetKeyDown(swingKey) && !isSwinging)
        {
            ChangeState(MovementState.Swing);
        }

    }
    
    void ChangeState(MovementState type)
    {
        if (!States.TryGetValue(type, out IState newState))
        {
            Debug.LogError("State not found");
            return;
        }
            
        if (curState != null)
            curState.Exit();

        curState = newState;
        curState.Enter();
    }

    private void Solver_OnCollision(ObiSolver solver, ObiSolver.ObiCollisionEventArgs e)
    {
        onGround = false;

        var world = ObiColliderWorld.GetInstance();
        foreach (Oni.Contact contact in e.contacts)
        {
            // look for actual contacts only:
            if (contact.distance > 0.01)
            {
                var col = world.colliderHandles[contact.bodyB].owner;
                if (col != null) // TODO: check tag
                {
                    onGround = true;
                    return;
                }
            }
        }
    }
    
    private void RegisterState()
    {
        States.Add(MovementState.IdleWalkRun,new IdleWalkRun(this));
        States.Add(MovementState.Jump, new Jump(this));
        States.Add(MovementState.Grapple, new Grapple(this));
        States.Add(MovementState.Swing, new Swing(this));
    }
    
    #region FSM States 
    public interface IState
    {
        public void Enter();
        public void Execute();
        public void Exit();
    }
    
    // must be onGround to enter this state
    class IdleWalkRun : IState
    { 
         private SoftbodyMovement _manager;
         private Transform _referenceFrame;
         private float _acceleration;
         

         public IdleWalkRun(SoftbodyMovement manager)
         {
             this._manager = manager;
         }
     
         public void Enter()
         {
             this._referenceFrame = _manager.referenceFrame;
             this._acceleration = _manager.acceleration;
         }
     
         public void Execute()
         {
             
             // can only jump during idleWalkRun state
             if (Input.GetKeyDown(KeyCode.Space) && _manager.onGround)
             {
                 _manager.ChangeState(MovementState.Jump);
             } 
             
             Vector3 direction = Vector3.zero;

             // Determine movement direction:
             if (Input.GetKey(KeyCode.W))
             {
                 direction += _referenceFrame.forward * _acceleration;
             }
             if (Input.GetKey(KeyCode.A))
             {
                 direction += -_referenceFrame.right * _acceleration;
             }
             if (Input.GetKey(KeyCode.S))
             {
                 direction += -_referenceFrame.forward * _acceleration;
             }
             if (Input.GetKey(KeyCode.D))
             {
                 direction += _referenceFrame.right * _acceleration;
             }

             // flatten out the direction so that it's parallel to the ground:
             direction.y = 0;

             // apply ground/air movement:
             float effectiveAcceleration = _acceleration;

             _manager.softbody.AddForce(direction.normalized * effectiveAcceleration, ForceMode.Acceleration);
             
             
         }
         
         public void Exit()
         {
           
         }
    }

    class Jump : IState
    {
         private SoftbodyMovement _manager;

         public Jump(SoftbodyMovement manager)
         {
             this._manager = manager;
         }

         public void Enter()
         {
             _manager.onGround = false;
             _manager.softbody.AddForce(Vector3.up * _manager.jumpPower, ForceMode.VelocityChange);
             _manager.ChangeState(MovementState.IdleWalkRun);
         }

         public void Execute()
         {
             
         }

         public void Exit()
         {
            
         }
         
    }
     
    class Grapple : IState
    {
         private SoftbodyMovement _manager;
         private Transform _referenceFrame;
         private DateTime _grappleStartTime;
         

         public Grapple (SoftbodyMovement manager)
         {
             this._manager = manager;
             this._referenceFrame = manager.referenceFrame;
         }

         public void Enter()
         {
             RaycastHit hit;
             
             
             if(Physics.Raycast(_referenceFrame.position, _referenceFrame.forward, out hit, _manager.maxGrappleDistance, _manager.whatIsGrappleable))
             {
                 // freeze the softbody first
                 Vector3 grapplePoint = hit.point;
                 _manager.GrapplePoint = grapplePoint;
                 _manager.isGrappling = true;
                 _grappleStartTime = DateTime.Now;
                 
                 Vector3 lowestPoint = new Vector3(_manager.transform.position.x, _manager.transform.position.y - 1f, _manager.transform.position.z);

                 float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
                 float highestPointOnArc = grapplePointRelativeYPos + _manager.overshootYAxis;

                 if (grapplePointRelativeYPos < 0) 
                     highestPointOnArc = _manager.overshootYAxis;
                 
                 JumpToPosition(grapplePoint, highestPointOnArc);

             }
             else
             {
                 _manager.ChangeState(MovementState.IdleWalkRun);
             }

         }

         public void Execute()
         {
             if (_manager.onGround && DateTime.Now - _grappleStartTime > TimeSpan.FromSeconds(1f))
             {
                 _manager.ChangeState(MovementState.IdleWalkRun);
             }
             
         }

         public void Exit()
         {
             _manager.isGrappling = false;
         }
         
         private void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
         {
             _manager.IsGrappling = true;
             _manager.onGround = false;
             
             Vector3 velocityToSet = CalculateJumpVelocity(_manager.transform.position, targetPosition, trajectoryHeight);
             //Debug.Log("Velocity to set: " + velocityToSet);
             for (int i = 0; i < _manager.softbody.solverIndices.Length; ++i){

                 // retrieve the particle index in the solver:
                 int solverIndex = _manager.softbody.solverIndices[i];
                 _manager.solver.velocities[solverIndex] = velocityToSet;

             }
         }
         
         private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
         {
             float gravity = Physics.gravity.y;
             float displacementY = endPoint.y - startPoint.y;
             Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

             Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
             Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
                                                    + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

             return velocityXZ + velocityY;
         }
    }

    class Swing : IState
    {
        private SoftbodyMovement _manager; 
        private Transform _referenceFrame;
        private SpringJoint joint = null;
        private Vector3 swingPoint;
        
        private Vector3 currentGrapplePosition;

        public Swing(SoftbodyMovement manager)
        {
            this._manager = manager;
            this._referenceFrame = manager.referenceFrame;
        }
         
        public void Enter()
        {
            RaycastHit hit;

            if(Physics.Raycast(_referenceFrame.position, _referenceFrame.forward, out hit, _manager.maxGrappleDistance, _manager.whatIsGrappleable))
            { 
                _manager.IsSwinging = true;
                
                swingPoint = hit.point;
                joint = _manager.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = swingPoint;

                float distanceFromPoint = Vector3.Distance(_manager.transform.position, swingPoint);

                // the distance grapple will try to keep from grapple point. 
                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;

                // customize values as you like
                joint.spring = 20f;
                joint.damper = 10f;
                joint.massScale = 10f;
                
                _manager.lr.positionCount = 2;
                currentGrapplePosition = _manager.gunTip.position;
            } 
            else
            {
                _manager.ChangeState(MovementState.IdleWalkRun);
            }

        }
         
        public void Execute()
        {
            if (Input.GetKeyUp(_manager.swingKey))
            {
                _manager.ChangeState(MovementState.IdleWalkRun);
                return;
            }
            
            OdmGearMovement();
            DrawRope();

        }

        public void Exit()
        {
            _manager.IsSwinging = false;
            _manager.lr.positionCount = 0;

            Destroy(joint);
        }

        private void OdmGearMovement()
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                direction += _referenceFrame.forward * _manager.horizontalAcceleration;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += -_referenceFrame.right * _manager.horizontalAcceleration;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += _referenceFrame.right * _manager.horizontalAcceleration;
            }
            
            float effectiveAcceleration = _manager.horizontalAcceleration;
            _manager.softbody.AddForce(direction.normalized * effectiveAcceleration, ForceMode.Acceleration);
            
            // shorten cable
            if (Input.GetKey(KeyCode.Space))
            {
                Vector3 directionToPoint = swingPoint - _manager.transform.position;
                _manager.softbody.AddForce(directionToPoint.normalized * _manager.forwardAcceleration, ForceMode.Acceleration);

                float distanceFromPoint = Vector3.Distance(_manager.transform.position, swingPoint);

                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;
            }
            
            // extend cable
            if (Input.GetKey(KeyCode.S))
            {
                float extendedDistanceFromPoint = Vector3.Distance(_manager.transform.position, swingPoint) + _manager.extendCableSpeed;

                joint.maxDistance = extendedDistanceFromPoint * 0.8f;
                joint.minDistance = extendedDistanceFromPoint * 0.25f;
            }
        }
        
        private void DrawRope()
        {
            // if not grappling, don't draw rope
            if (!joint) return;

            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
            
            _manager.lr.SetPosition(0, _manager.gunTip.position);
            _manager.lr.SetPosition(1, currentGrapplePosition);
        }
    }
    
    


    #endregion
    
}
