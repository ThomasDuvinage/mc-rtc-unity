using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public class ArrayInput : Element
    {
        public float[] data
        {
            get { return m_data; }
            set
            {
                if (value.Length == m_data.Length)
                {
                  Client.SendArrayInputRequest(id, value);
                  m_data = value;
                }
            }
        }
        public string[] labels
        {
            get { return m_labels; }
        }
        private float[] m_data = new float[0];
        private string[] m_labels = new string[0];
        public void UpdateArray(string[] labels, float[] data)
        {
            if(labels.Length < data.Length)
            {
                m_labels = new string[data.Length];
                for (int i = 0; i < labels.Length; ++i)
                {
                    m_labels[i] = labels[i];
                }
                for (int i = labels.Length; i < data.Length; ++i)
                {
                    m_labels[i] = i.ToString();
                }
            }
            else
            {
                m_labels = labels;
            }
            m_data = data;
        }

        public void Disconnect()
        {
            m_labels = new string[0];
            m_data = new float[0];
        }
    }
}
