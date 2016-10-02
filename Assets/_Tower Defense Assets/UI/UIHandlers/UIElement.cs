using UnityEngine;
using System.Collections;

namespace UI
{
    public class UIElement : MonoBehaviour
    {
        protected Animator animator;

        protected bool _isOpen = false;
        protected GUIManager.UILayer _layer;

        public virtual bool isOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                if (animator)
                {
                    animator.SetBool("isOpen", _isOpen);
                }
            }
        }
        public virtual GUIManager.UILayer layer
        {
            get
            {
                return _layer;
            }
            set
            {
                _layer = value;
                transform.SetParent(GUIManager.layers[(int)_layer].transform);
            }
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
        }
    }
}