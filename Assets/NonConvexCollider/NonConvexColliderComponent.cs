using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Plawius.NonConvexCollider
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class NonConvexColliderComponent : MonoBehaviour
    {
        public Parameters Params = Parameters.Default();

        [SerializeField] private List<MeshCollider> m_colliders = new List<MeshCollider>();
        [SerializeField] private bool m_isTrigger = false;
        [SerializeField] private PhysicMaterial m_material = null;
        private bool m_isDirty = true;

        #if UNITY_EDITOR
        [SerializeField] private bool m_showColliders = false;
        #endif
        
        public List<MeshCollider> ConvexColliders
        {
            get
            {
                return m_colliders;
            }
        }
        
        public bool IsTrigger
        {
            get { return m_isTrigger; }
            set
            {
                if (m_isTrigger != value)
                {
                    m_isTrigger = value;
                    m_isDirty = true;
                }
            }
        }
        public PhysicMaterial Material
        {
            get { return m_material; }
            set
            {
                if (m_material != value)
                {
                    m_material = value;
                    m_isDirty = true;
                }
            }
        }

        [SerializeField] private NonConvexColliderAsset m_colliderAsset;
        public NonConvexColliderAsset ColliderAsset
        {
            get
            {
                return m_colliderAsset;
            }
            private set
            {
                m_colliderAsset = value;
            }
        }

        private void OnEnable()
        {
            SyncState(true);
        }
        
        private void OnDisable()
        {
            SyncState(false);
        }

        void Update()
        {
            if (m_isDirty)
            {
                foreach (var coll in ConvexColliders)
                {
                    coll.isTrigger = IsTrigger;
                    coll.material = Material;
                }
                m_isDirty = false;
            }
        }

        #if UNITY_EDITOR
        public void SyncState()
        {
            SyncState(enabled);
        }
        
        private void OnValidate()
        {
            SyncState();
        }
        #endif

        private void SyncState(bool isEnabled)
        {
#if UNITY_EDITOR
            // Editor only checks
            
            // asset was deleted, m_colliderAsset is missing, ConvexColliders is out of sync
            if (ConvexColliders.Count > 0 && m_colliderAsset == null)
            {
                SetPhysicsCollider(null); // just set to null, this should delete all ConvexColliders
                Assert.IsTrue(ConvexColliders.Count == 0);
            }
            
            // undo happened. asset is set, ConvexColliders is empty/contains null (out of sync)
            if ((ConvexColliders.Count == 0 || ConvexColliders[0] == null) && m_colliderAsset != null)
            {
                SetPhysicsCollider(m_colliderAsset); // just set to null, this should delete all ConvexColliders
                Assert.IsTrue(ConvexColliders.Count != 0);
            }
#endif
            for (var i = 0; i < ConvexColliders.Count; i++)
            {
                var coll = ConvexColliders[i];
                coll.isTrigger = IsTrigger;
                coll.sharedMaterial = Material;
                coll.enabled = isEnabled;
                coll.convex = true;
                
                coll.sharedMesh = m_colliderAsset.ConvexMeshes[i];

                #if UNITY_EDITOR
                coll.hideFlags = m_showColliders ? HideFlags.None : HideFlags.HideInInspector | HideFlags.NotEditable;
                #endif
            }

            m_isDirty = false;
        }

        public void SetPhysicsCollider(NonConvexColliderAsset newColliderAsset)
        {
            var wasActive = gameObject.activeSelf;
            gameObject.SetActive(false);
            RemoveAllCollidersFrom(ColliderAsset);
            DisableAllColliders();
            AddAllCollidersFrom(newColliderAsset);
            gameObject.SetActive(wasActive);
        }

        private void DisableAllColliders()
        {
            var colliders = GetComponents<Collider>();
            foreach (var c in colliders)
                c.enabled = false;
        }

        private void AddAllCollidersFrom(NonConvexColliderAsset colliderAsset)
        {
            m_colliders.Clear();
            if (colliderAsset == null)
                return;
            
            for (var i = 0; i < colliderAsset.ConvexMeshes.Length; ++i)
            {
                var mesh = colliderAsset.ConvexMeshes[i];
                var mc = gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mesh;
                mc.convex = true;
                mc.isTrigger = IsTrigger;
                mc.material = Material;
                m_colliders.Add(mc);
            }
            
            m_colliderAsset = colliderAsset;
        }

        private void RemoveAllCollidersFrom(NonConvexColliderAsset colliderAsset)
        {
            m_colliders.Clear();
            
            if (colliderAsset == null)
                return;
            
            var colldiers = GetComponents<MeshCollider>();
            for (var i = colldiers.Length - 1; i >= 0; --i)
            {
                var c = colldiers[i];
                if (c.sharedMesh == null || colliderAsset.ConvexMeshes.Contains(c.sharedMesh))
                {
#if UNITY_EDITOR
                    DestroyImmediate(c, true);
#else
                    Destroy(c);
#endif
                }
            }
            
            if (m_colliderAsset == colliderAsset)
                m_colliderAsset = null;
        }
    }

}