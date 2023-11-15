using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public class NumberInput : Element
    {
        public float data
        {
            get { return m_data; }
            set
            {
                Client.SendNumberInputRequest(id, value);
                m_data = value;
            }
        }
        private float m_data = 0.0f;
        public void UpdateData(float data)
        {
            m_data = data;
        }

        protected override void OnDisconnect()
        {
            m_data = 0.0f;
        }
    }
}

