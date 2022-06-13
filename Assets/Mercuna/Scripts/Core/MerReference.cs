// Copyright (C) 2018-2022 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

namespace Mercuna
{
    class MerReference
    {
        internal MerReference(object target, bool weak)
        {
            m_handle = GCHandle.Alloc(target, weak ? GCHandleType.Weak : GCHandleType.Normal);

            if (weak)
            {
                m_weakRefs = 1;
                m_strongRefs = 0;
            }
            else
            {
                m_weakRefs = 0;
                m_strongRefs = 1;
            }
        }

        ~MerReference()
        {
            if (m_handle.IsAllocated)
            {
                m_handle.Free();
            }
        }

        internal void AddRef(bool weak)
        {
            if (!weak && m_strongRefs == 0)
            {
                GCHandle newHandle = GCHandle.Alloc(m_handle.Target);
                m_handle.Free();
                m_handle = newHandle;
            }

            if (weak)
            {
                ++m_weakRefs;
            }
            else
            {
                ++m_strongRefs;
            }
        }

        // Returns true if object still referenced, false if no references left.
        internal bool ReleaseRef(bool weak)
        {
            if (!weak && m_strongRefs == 1 && m_weakRefs > 0)
            {
                GCHandle newHandle = GCHandle.Alloc(m_handle.Target, GCHandleType.Weak);
                m_handle.Free();
                m_handle = newHandle;
            }

            if (weak)
            {
                --m_weakRefs;
                Assert.IsFalse(m_weakRefs < 0);
            }
            else
            {
                --m_strongRefs;
                Assert.IsFalse(m_strongRefs < 0);
            }

            if (m_weakRefs > 0 || m_strongRefs > 0)
            {
                return true;
            }
            else
            {
                m_handle.Free();
                return false;
            }
        }

        internal object Get()
        {
            return m_handle.IsAllocated ? m_handle.Target : null;
        }

        internal bool IsValid()
        {
            return m_handle.IsAllocated && m_handle.Target != null;
        }

        private int m_strongRefs;
        private int m_weakRefs;

        private GCHandle m_handle;
        private WeakReference m_refToCollider = new WeakReference(null);
        private WeakReference m_refToRigidbody = new WeakReference(null);
        private WeakReference m_refToObstacle = new WeakReference(null);
        private WeakReference m_refToNavigation = new WeakReference(null);
        private WeakReference m_refToMoveController = new WeakReference(null);
        private bool m_hasCollider = true;
        private bool m_hasRigidBody = true;
        private bool m_hasObstacle = true;
        private bool m_hasNavigation = true;
        private bool m_hasMoveController = true;

        internal Collider collider
        {
            get
            {
                Collider col = (Collider)m_refToCollider.Target;                
                if (!col && m_hasCollider)
                {
                    col = ((GameObject)Get()).GetComponent<Collider>();
                    if (col)
                    {
                        m_refToCollider.Target = col;
                    }
                    else
                    {
                        m_hasCollider = false;
                    }
                }
                return col;
            }
        }

        internal Rigidbody rigidbody
        {
            get
            {
                Rigidbody rb = (Rigidbody)m_refToRigidbody.Target;
                if (!rb && m_hasRigidBody)
                {
                    rb = ((GameObject)Get()).GetComponent<Rigidbody>();
                    if (rb)
                    {
                        m_refToRigidbody.Target = rb;
                    }
                    else
                    {
                        m_hasRigidBody = false;
                    }
                }
                return rb;
            }
        }

        internal MercunaObstacle obstacle
        {
            get
            {
                MercunaObstacle obs = (MercunaObstacle) m_refToObstacle.Target;
                if (!obs && m_hasObstacle)
                {
                    obs = ((GameObject) Get()).GetComponent<MercunaObstacle>();
                    if (obs)
                    {
                        m_refToObstacle.Target = obs;
                    }
                    else
                    {
                        m_hasObstacle = false;
                    }
                }
                return obs;
            }
        }

        internal Mercuna3DNavigation navigation
        {
            get
            {
                Mercuna3DNavigation nav = (Mercuna3DNavigation) m_refToNavigation.Target;
                if (!nav && m_hasNavigation)
                {
                    nav = ((GameObject) Get()).GetComponent<Mercuna3DNavigation>();
                    if (nav)
                    {
                        m_refToNavigation.Target = nav;
                    }
                    else
                    {
                        m_hasNavigation = false;
                    }
                }
                return nav;
            }
        }

        internal MercunaMoveController moveController
        {
            get
            {
                MercunaMoveController move = (MercunaMoveController) m_refToMoveController.Target;
                if (!move && m_hasMoveController)
                {
                    move = ((GameObject) Get()).GetComponent<MercunaMoveController>();
                    if (move)
                    {
                        m_refToMoveController.Target = move;
                    }
                    else
                    {
                        m_hasMoveController = false;
                    }
                }
                return move;
            }
        }
    }

    class MerRefTable 
    {
        private int m_nextId;
        private Dictionary<int, MerReference> m_refTable = new Dictionary<int, MerReference>();

        public MerRefTable()
        {
            m_nextId = 1 + ((int)System.DateTime.UtcNow.Ticks & 0x7fff) * 0x10000;
        }

        public IntPtr GetRefId(object target, bool weak)
        {
            m_refTable[m_nextId] = new MerReference(target, weak);
            return new IntPtr(m_nextId++);
        }

        public void AddRef(IntPtr refId, bool weak)
        {
            MerReference r;
            m_refTable.TryGetValue(refId.ToInt32(), out r);
            if (r != null)
            {
                r.AddRef(weak);
            }
            else
            {
                CheckRefIdInvalid(refId.ToInt32());
            }
        }

        public void ReleaseRef(IntPtr refId, bool weak)
        {
            MerReference r;
            m_refTable.TryGetValue(refId.ToInt32(), out r);
            if (r != null)
            {
                if (!r.ReleaseRef(weak))
                {
                    m_refTable.Remove(refId.ToInt32());
                }
            }
            else
            {
                CheckRefIdInvalid(refId.ToInt32());
            }
        }

        internal MerReference GetRef(IntPtr refId)
        {
            MerReference r;
            m_refTable.TryGetValue(refId.ToInt32(), out r);
            if (r == null)
            {
                CheckRefIdInvalid(refId.ToInt32());
                return null;
            }

            return r;
        }

        public object Get(IntPtr refId)
        {
            MerReference r = GetRef(refId);
            return r?.Get();
        }

        public bool IsValid(IntPtr refId)
        {
            MerReference r;
            m_refTable.TryGetValue(refId.ToInt32(), out r);
            return r != null && r.IsValid();
        }

        public Collider GetCollider(IntPtr refId)
        {
            MerReference r = m_refTable[refId.ToInt32()];
            return r.collider;
        }

        public Rigidbody GetRigidbody(IntPtr refId)
        {
            MerReference r = m_refTable[refId.ToInt32()];
            return r.rigidbody;
        }

        public MercunaObstacle GetObstacle(IntPtr refId)
        {
            MerReference r = m_refTable[refId.ToInt32()];
            return r.obstacle;
        }

        public Mercuna3DNavigation GetNavigation(IntPtr refId)
        {
            MerReference r = m_refTable[refId.ToInt32()];
            return r.navigation;
        }

        public MercunaMoveController GetMoveController(IntPtr refId)
        {
            MerReference r = m_refTable[refId.ToInt32()];
            return r.moveController;
        }

        private void CheckRefIdInvalid(int refId)
        {
            if (refId > m_nextId) return;
            if (m_nextId - refId > 0x10000) return;

            // Unity log - don't go via Mercuna to prevent any chance of a loop.
            Debug.LogError(String.Format("Mercuna: Invalid reference ID in reference table: refId = {0}", refId));
        }
    }
}
