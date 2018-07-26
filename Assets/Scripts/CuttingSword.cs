namespace VRTK.Examples
{
    using BLINDED_AM_ME;
    using UnityEngine;

    public class CuttingSword : VRTK_InteractableObject
    {
        private float impactMagnifier = 120f;
        private float collisionForce = 0f;
        private float maxCollisionForce = 4000f;
        private VRTK_ControllerReference controllerReference;

        public Material cutMaterial;
        public float splitStrenght;

        public float CollisionForce()
        {
            return collisionForce;
        }

        public override void Grabbed(VRTK_InteractGrab grabbingObject)
        {
            base.Grabbed(grabbingObject);
            controllerReference = VRTK_ControllerReference.GetControllerReference(grabbingObject.controllerEvents.gameObject);
        }

        public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject)
        {
            base.Ungrabbed(previousGrabbingObject);
            controllerReference = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            controllerReference = null;
            interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void OnTriggerEnter(Collision collision)
        {
            if(VRTK_ControllerReference.IsValid(controllerReference) && IsGrabbed()) {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * impactMagnifier;
                var hapticStrength = collisionForce / maxCollisionForce;
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, hapticStrength, 0.5f, 0.01f);
            } else {
                collisionForce = collision.relativeVelocity.magnitude * impactMagnifier;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if(!IsGrabbed()) {
                return;
            }

            var controllerEvents = GetGrabbingObject().GetComponent<VRTK_ControllerEvents>();
            if(controllerEvents != null && controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.TouchpadPress)) {
                var axis = controllerEvents.GetAxis(VRTK_ControllerEvents.Vector2AxisAlias.Touchpad);

                if(axis.y > 0.5) {
                    splitStrenght += 0.05f;
                }
                if(axis.y < -0.5) {
                    splitStrenght -= 0.05f;
                }
                Debug.Log("new splitStrenght: " + splitStrenght);
            }
        }
    }
}