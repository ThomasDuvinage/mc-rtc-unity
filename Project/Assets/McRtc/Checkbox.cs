using System;
using System.Collections;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public class Checkbox : Element
    {
        public bool state
        {
            get { return m_state; }
            set
            {
                if (value != m_state)
                {
                    Client.SendCheckboxRequest(id);
                }
                m_state = value;
            }
        }
        private bool m_state = false;

        public void UpdateState(bool state)
        {
            m_state = state;
        }

        protected override void OnDisconnect()
        {
        }
    }
}
