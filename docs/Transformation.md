# Transformation

_`Essentials.Core.Transformation` is needed for this._
<br />
<br />
Transformation is a feature focused on transforming objects. It has currently only one use case and that is snapping objects to other nearby objects. You can access the Transformation in the **Essentials >> Transformation** menu.

<img width="835" alt="image" src="https://github.com/NotRewd/Unity-Essentials/assets/48103943/82708be1-04fe-4121-8533-ae03aaa1d43a">

_If the options are greyed out, check if the selected object has a collider attached to it. The same goes for the surface you want to snap it onto._

## Using It In Code

You can also use the Transformation in the code directly during runtime. Below are listed all the methods you can call.

## Methods

### Vector3 Transformation.GetSnapPrediction(GameObject gameObject, SnapDirection direction = SnapDirection.Down)

Returns the predicted snap position with the specified snap direction where the object would snap.

### void Transformation.SnapObjectTo(GameObject gameObject, SnapDirection direction = SnapDirection.Down)

Snaps the specified GameObject to the specified snap direction.

### void Transformation.SnapObjectTo(Transform transform, SnapDirection direction = SnapDirection.Down)

Snaps the specified transform to the specified snap direction.

### Vector3 Transformation.GetSnapDirection(SnapDirection direction)

Returns the Vector3 global direction of the specified snap direction.
