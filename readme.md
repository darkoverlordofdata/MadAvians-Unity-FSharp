# FSharp + Unity + Angry Birds Style Game Demo

https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/making-angry-birds-style-game?playlist=17093

Pitfalls while converting this from CSharp, 

#### 1
the 1st pass at converting this line
```csharp
    Vector2 catapultToMouse = mouseWorldPoint - catapult.position;
```
```fsharp
    let catapultToMouse = mouseWorldPoint - catapult.position
```

Careful, that just changed catapultToMouse from a Vector2 to a Vector3. And sqrMagnitude is now invalid, and we are broken.
```fsharp
    if catapultToMouse.sqrMagnitude > maxStretchSqr then
```

#### 2
Null check not working:
```fsharp
    if this.spring = null
```

Need to use this:
```fsharp
    let IsNull x = 
        let y = box x // In case of value types
        obj.ReferenceEquals(y, Unchecked.defaultof<_>) || // Regular null check
        y.Equals(Unchecked.defaultof<_>) // Will call Unity overload if needed
```

