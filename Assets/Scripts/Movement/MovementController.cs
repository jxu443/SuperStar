
using System;
using System.Collections.Generic;
using UnityEngine;
using Obi;


[RequireComponent(typeof(ObiSoftbody))]
public class MovementController: MonoBehaviour
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
    public KeyCode grappleKey = KeyCode.Alpha4;
    public LayerMask whatIsGrappleable;
    public float maxGrappleDistance;
    public float overshootYAxis;

    public enum MovementState
    {
        IdleWalkRun,
        Jump,
        Grapple,
    }

    /*** private fields ***/
    private IState curState;
    private Dictionary<MovementState, IState> States = new Dictionary<MovementState, IState>();
    private ObiSoftbody softbody;
    
    private bool onGround = false;
    private Vector3 grapplePoint;
    private bool isGrappling = false;


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
        
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            ChangeState(MovementState.Jump);
        }

        if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            ChangeState(MovementState.Grapple);
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
                if (col != null)
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
         private MovementController _manager;
         private Transform _referenceFrame;
         private float _acceleration;
         

         public IdleWalkRun(MovementController manager)
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
         private MovementController _manager;

         public Jump(MovementController manager)
         {
             this._manager = manager;
         }

         public void Enter()
         {
             _manager.onGround = false;
             _manager.softbody.AddForce(Vector3.up * _manager.jumpPower, ForceMode.VelocityChange);
         }

         public void Execute()
         {
             if (_manager.onGround)
             {
                 _manager.ChangeState(MovementState.IdleWalkRun);
             }
         }

         public void Exit()
         {
            
         }
         
     }
     
     class Grapple : IState
     {
         private MovementController _manager;
         private Transform _referenceFrame;
         private DateTime _grappleStartTime;
         

         public Grapple (MovementController manager)
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
             //_manager.softbody.AddForce(velocityToSet * _manager.airControl, ForceMode.VelocityChange);
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
     #endregion
    
}
