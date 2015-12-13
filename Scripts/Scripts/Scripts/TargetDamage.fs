namespace MadAvians
open UnityEngine

type TargetDamage () =
    inherit MonoBehaviour ()

    (** The amount of damage our target can take *)
    [<SerializeField>]
    let mutable hitPoints = 2

    (** The reference to our "damaged" sprite *)
    [<DefaultValue>][<SerializeField>]
    val mutable damagedSprite:Sprite

    (** The speed threshold of colliding objects before the target takes damage *)
    [<SerializeField>]
    let mutable damageImpactSpeed = 0.0f

    (** The current amount of health our target has taken *)
    let mutable currentHitPoints = 0

    (** The square value of Damage Impact Speed, for efficient calculation *)
    let mutable damageImpactSpeedSqr = 0.0f

    (** The reference to this GameObject's sprite renderer *)    
    [<SerializeField>][<DefaultValue>] val mutable private spriteRenderer:SpriteRenderer

    (** *)
    member this.Start () =
        //  Get the SpriteRenderer component for the GameObject's Rigidbody
        this.spriteRenderer <- this.GetComponent<SpriteRenderer>()

        //  Initialize the Hit Points
        currentHitPoints <- hitPoints

        //  Calculate the Damage Impact Speed Squared from the Damage Impact Speed
        damageImpactSpeedSqr <- damageImpactSpeed * damageImpactSpeed


    (** *)
    member this.OnCollisionEnter2D (collision:Collision2D) = 
        //  Check the colliding object's tag, and if it is not "Damager", exit this function
        if collision.collider.tag = "Damager" then

            //  Check the colliding object's velocity's Square Magnitude, and if it is less than the threshold, exit this function
            if collision.relativeVelocity.sqrMagnitude >= damageImpactSpeedSqr then
                //  We have taken damage, so change the sprite to the damaged sprite
                this.spriteRenderer.sprite <- this.damagedSprite
                //  Decriment the Current Health of the target
                currentHitPoints <- currentHitPoints-1

                //  If the Current Health is less than or equal to zero, call the Kill() function
                if currentHitPoints <= 0 then
                    this.Kill()


    (** *)
    member this.Kill() =
        //  As the particle system is attached to this GameObject, when Killed, switch off all of the visible behaviours...
        this.spriteRenderer.enabled <- false
        this.GetComponent<Collider2D>().enabled <- false
        this.GetComponent<Rigidbody2D>().isKinematic <- true

        //  ... and Play the particle system
        this.GetComponent<ParticleSystem>().Play()

