namespace MadAvians
open UnityEngine

type ProjectileDragging () =
    inherit MonoBehaviour ()

    (** Maximum Stretch *)
    [<SerializeField>]
    let mutable maxStretch = 3.0f

    (** Catapult Line Front *)
    [<DefaultValue>][<SerializeField>]
    val mutable catapultLineFront:LineRenderer
    
    (** Catapult Line Back *)
    [<DefaultValue>][<SerializeField>]
    val mutable catapultLineBack:LineRenderer
    
    [<SerializeField>][<DefaultValue>] val mutable private spring:SpringJoint2D
    [<SerializeField>][<DefaultValue>] val mutable private catapult:Transform
    [<SerializeField>][<DefaultValue>] val mutable private rayToMouse:Ray
    [<SerializeField>][<DefaultValue>] val mutable private leftCatapultToProjectile:Ray
    [<SerializeField>][<DefaultValue>] val mutable private prevVelocity:Vector2

    let mutable maxStretchSqr:float32 = 0.0f
    let mutable circleRadius:float32 = 0.0f
    let mutable clickedOn:bool = false

    let IsNull x = 
        let y = box x // In case of value types
        obj.ReferenceEquals(y, Unchecked.defaultof<_>) || // Regular null check
        y.Equals(Unchecked.defaultof<_>) // Will call Unity overload if needed

    let Vec23 (v2:Vector2):Vector3 =
        new Vector3(v2.x, v2.y)

    let Vec32 (v3:Vector3):Vector2 =
        new Vector2(v3.x, v3.y)

    
    (** *)
    member this.Awake () =
        this.spring <- this.GetComponent<SpringJoint2D>()
        this.catapult <- this.spring.connectedBody.transform
        
    (** *)
    member this.Start () =
        this.LineRendererSetup()
        this.rayToMouse <- new Ray(this.catapult.position, Vector3.zero)
        this.leftCatapultToProjectile <- new Ray(this.catapultLineFront.transform.position, Vector3.zero)
        maxStretchSqr <- maxStretch * maxStretch
        let circle = this.GetComponent<Collider2D>() :?> CircleCollider2D
        circleRadius <- circle.radius
                        

    (** *)
    member this.Update () =
        if clickedOn = true then
            this.Dragging()
           
        if IsNull(this.spring) then
            this.catapultLineFront.enabled <- false
            this.catapultLineBack.enabled <- false

        else
            if not(this.GetComponent<Rigidbody2D>().isKinematic)
             && this.prevVelocity.sqrMagnitude > this.GetComponent<Rigidbody2D>().velocity.sqrMagnitude
              then
                Object.Destroy(this.spring)
                this.GetComponent<Rigidbody2D>().velocity <- this.prevVelocity

            if not clickedOn then
                this.prevVelocity <- this.GetComponent<Rigidbody2D>().velocity

            this.LineRendererUpdate()

        // Poll touch input
        if Input.touchCount > 0 then
            match Input.GetTouch(0).phase with
            | TouchPhase.Began -> this.OnMouseDown()
            | TouchPhase.Ended -> this.OnMouseUp()
            | TouchPhase.Canceled -> this.OnMouseUp()
            | _ -> ()

              
    (** *)
    member this.LineRendererSetup () =  
        this.catapultLineFront.SetPosition(0, this.catapultLineFront.transform.position)
        this.catapultLineBack.SetPosition(0, this.catapultLineBack.transform.position)
        
        this.catapultLineFront.sortingLayerName <- "Foreground"
        this.catapultLineBack.sortingLayerName <- "Foreground"
        
        this.catapultLineFront.sortingOrder <- 3
        this.catapultLineBack.sortingOrder <- 1
        
        
    (** *)
    member this.OnMouseDown () = 
        this.spring.enabled <- false
        clickedOn <- true
        
        
    (** *)
    member this.OnMouseUp () = 
        this.spring.enabled <- true
        this.GetComponent<Rigidbody2D>().isKinematic <- false
        clickedOn <- false
        
        
    (** *)
    member this.Dragging() =
        let mutable mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition)
        let catapultToMouse = Vec32(mouseWorldPoint - this.catapult.position)

        if catapultToMouse.sqrMagnitude > maxStretchSqr then
            this.rayToMouse.direction <- Vec23(catapultToMouse)
            mouseWorldPoint <- this.rayToMouse.GetPoint(maxStretch)
        
        mouseWorldPoint.z <- 0.0f
        this.transform.position <- mouseWorldPoint
        

    (** *)
    member this.LineRendererUpdate() =
        let catapultToProjectile = Vec32(this.transform.position - this.catapultLineFront.transform.position)
        this.leftCatapultToProjectile.direction <- Vec23(catapultToProjectile)
        let holdPoint = this.leftCatapultToProjectile.GetPoint(catapultToProjectile.magnitude + circleRadius)
        this.catapultLineFront.SetPosition(1, holdPoint)
        this.catapultLineBack.SetPosition(1, holdPoint)


    (** *)
    member this.OnEnable() =
        // Subcribe to events when object is enabled
        ()


