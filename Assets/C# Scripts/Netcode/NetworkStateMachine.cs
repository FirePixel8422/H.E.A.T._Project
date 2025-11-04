using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;


namespace FirePixel.Networking
{
    public class NetworkStateMachine : NetworkBehaviour
    {
        #region Animation Data

        [Header("Start Animation")]
        [SerializeField] private string[] currentAnimation = { "Idle", "ShakeGooglyEyes" };


        [Header("Animation Names")]
        [SerializeField] private string idleAnimation = "Idle";

        [SerializeField] private string crouchAnimationUp = "CrouchUp";
        [SerializeField] private string crouchAnimationDown = "CrouchDown";
        [SerializeField] private string[] crouchWalkAnimations = { "L", "R", "F", "D" };

        [SerializeField] private string[] weaponHipAnimations = { "Pistol", "AR", "UMP", "Smimper" };
        [SerializeField] private string[] weaponAdsAnimations = { "Pistol", "AR", "UMP", "Smimper" };

        [SerializeField] private string[] walkAnimations = { "L", "R", "F", "D" };
        [SerializeField] private string runAnimation = "Run";

        [SerializeField] private string jumpAnimation = "Jump";
        [SerializeField] private string fallAnimation = "Falling";


        private int[] currentAnimationHashes;

        private int idleAnimationHash;

        private int crouchAnimationDownHash;
        private int[] crouchWalkAnimationHashes;

        private int[] weaponHipAnimationHashes;
        private int[] weaponAdsAnimationHashes;

        private int[] walkAnimationHashes;
        private int runAnimationHash;

        private int jumpAnimationHash;
        private int fallAnimationHash;

        #endregion


        [SerializeField] private Animator[] anims;
        [SerializeField] private Animator gunAnimator;
        
        public Animator Anim
        {
            get
            {
                if (anim != null)
                {
                    return anim;
                }
                return anims[IsOwner ? 0 : 1];
            }
        }
        [SerializeField] private Animator anim;

        private RagDollController ragDollController;

        private int animationLayerCount;

        private Coroutine[] autoTransitiosCOs;

        private bool IsJumping => currentAnimationHashes[0] == jumpAnimationHash;
        [SerializeField] private bool dead;




        public override void OnNetworkSpawn()
        {
            anim = anims[IsOwner ? 0 : 1];

            ragDollController = GetComponentInChildren<RagDollController>(true);

            animationLayerCount = anim.layerCount;
            autoTransitiosCOs = new Coroutine[animationLayerCount];

            // Start Animations
            currentAnimationHashes = new int[currentAnimation.Length];
            for (int i = 0; i < currentAnimation.Length; i++)
            {
                currentAnimationHashes[i] = Animator.StringToHash(currentAnimation[i]);
            }

            // Idle
            idleAnimationHash = Animator.StringToHash(idleAnimation);

            // Crouch
            crouchAnimationDownHash = Animator.StringToHash(crouchAnimationDown);
            crouchWalkAnimationHashes = HashArray(crouchWalkAnimations);

            // Weapons
            weaponHipAnimationHashes = HashArray(weaponHipAnimations);
            weaponAdsAnimationHashes = HashArray(weaponAdsAnimations);

            // Movement
            walkAnimationHashes = HashArray(walkAnimations);
            runAnimationHash = Animator.StringToHash(runAnimation);

            // Air
            jumpAnimationHash = Animator.StringToHash(jumpAnimation);
            fallAnimationHash = Animator.StringToHash(fallAnimation);

            // Get and set the start animation hashes
            for (int i = 0; i < animationLayerCount; i++)
            {
                currentAnimationHashes[i] = Animator.StringToHash(currentAnimation[i]);

                anim.speed = 1;
                anim.CrossFadeInFixedTime(currentAnimationHashes[i], 0, i);
            }
        }
        private int[] HashArray(string[] names)
        {
            int[] hashes = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                hashes[i] = Animator.StringToHash(names[i]);
            }
            return hashes;
        }


        #region Change/Transition Animation + Server Sync Functions

        /// <returns>true if the animation has changed, false otherwise</returns>
        private bool TryTransitionAnimation(int animationHash, float transitionDuration = 0.25f, float speed = 1, int layer = 0, bool isGunAnimator = false)
        {
            //if the new animation is the same as current, return false
            if (currentAnimationHashes[layer] == animationHash) return false;

            //DebugLogger.Log($"Transitioning to animation: {animationHash} with duration: {transitionDuration}, speed: {speed}, layer: {layer}");

            SyncAnimation_ServerRPC(ClientManager.LocalClientGameId, animationHash, transitionDuration, speed, layer, isGunAnimator);

            TransitionAnimation(animationHash, transitionDuration, speed, layer, isGunAnimator);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TransitionAnimation(int animationHash, float transitionDuration, float speed, int layer, bool isGunAnimator = false)
        {
            if (isGunAnimator)
            {
                gunAnimator.speed = speed;
                gunAnimator.CrossFadeInFixedTime(animationHash, transitionDuration, layer);
            }
            else
            {
                currentAnimationHashes[layer] = animationHash;

                anim.speed = speed;
                anim.CrossFadeInFixedTime(animationHash, transitionDuration, layer);
            }
        }

        /// <summary>
        /// Sent Animation Data trough server, back to all clients except sender.
        /// </summary>
        [ServerRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SyncAnimation_ServerRPC(int fromClientGameId, int animationHash, float transitionDuration, float speed = 1, int layer = 0, bool isGunAnimator = false)
        {
            SyncAnimation_ClientRPC(animationHash, transitionDuration, speed, layer, isGunAnimator, GameIdRPCTargets.SendToOppositeClient(fromClientGameId));
        }

        [ClientRpc(RequireOwnership = false, Delivery = RpcDelivery.Reliable)]
        private void SyncAnimation_ClientRPC(int animationHash, float transitionDuration, float speed = 1, int layer = 0, bool isGunAnimator = false, GameIdRPCTargets rpcTargets = default)
        {
            if (rpcTargets.IsTarget == false) return;

            TransitionAnimation(animationHash, transitionDuration, speed, layer, isGunAnimator);
        }

        #endregion


        #region Movement State Functions

        public void UpdateMovementState(bool moving, bool crouching, bool sprinting, int moveDirectionId, float transitionDuration = 0.25f)
        {
            if (dead) return;

            if (moving)
            {
                if (crouching)
                    CrouchWalk(moveDirectionId, transitionDuration);
                else if (sprinting)
                    Sprint(transitionDuration);
                else
                    Walk(moveDirectionId, transitionDuration);
            }
            else
            {
                if (crouching)
                    Crouch(transitionDuration);
                else
                    Idle(transitionDuration);
            }
        }

        private void Idle(float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(idleAnimationHash, transitionDuration);
        }

        private void Crouch(float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(crouchAnimationDownHash, transitionDuration);
        }
        private void CrouchWalk(int moveDirectionId, float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(crouchWalkAnimationHashes[moveDirectionId], transitionDuration);
        }

        private void Walk(int moveDirectionId, float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(walkAnimationHashes[moveDirectionId], transitionDuration);
        }
        private void Sprint(float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(runAnimationHash, transitionDuration);
        }

        #endregion


        public void ChangeWeaponAnimation(bool hipFire, int weaponId, float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(hipFire ? weaponHipAnimationHashes[weaponId] : weaponAdsAnimationHashes[weaponId], transitionDuration, layer: 1);
        }


        public void Jump(float transitionDuration = 0.25f)
        {
            TryTransitionAnimation(jumpAnimationHash, transitionDuration);
            
            StartCoroutine(AutoFall(fallAnimationHash, transitionDuration));
        }

        private IEnumerator AutoFall(float transitionDuration, float speed = 1, int layer = 0)
        {
            yield return null; // Wait 1 frame so animator updates to the new state

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(layer);
            float remainingTime = (1f - state.normalizedTime) * state.length;

            yield return new WaitForSeconds(remainingTime);

            TryTransitionAnimation(fallAnimationHash, transitionDuration, speed, layer);

            AutoTransition(walkAnimationHashes[0], transitionDuration);
        }


        public void Die(Vector3 ragdollDirection, Vector3 ragdollImpactPoint, float transitionDuration = 0.25f)
        {
            dead = true;

            //ragDollController.StartRagdoll(ragdollDirection, ragdollImpactPoint);
        }


        /// <summary>
        /// Create an auto transition to target animation after current animation finishes playing.
        /// </summary>
        private void AutoTransition(int animationHash, float transitionDuration, float speed = 1, int layer = 0)
        {
            if (autoTransitiosCOs[layer] != null)
            {
                StopCoroutine(autoTransitiosCOs[layer]);
            }
            autoTransitiosCOs[layer] = StartCoroutine(AutoTransitionCoroutine(animationHash, transitionDuration, speed, layer));
        }
        private IEnumerator AutoTransitionCoroutine(int animationHash, float transitionDuration, float speed = 1, int layer = 0)
        {
            yield return null; // Wait 1 frame so animator updates to the new state

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(layer);
            float remainingTime = (1f - state.normalizedTime) * state.length;

            yield return new WaitForSeconds(remainingTime);

            TryTransitionAnimation(animationHash, transitionDuration, speed, layer);
        }
    }
}