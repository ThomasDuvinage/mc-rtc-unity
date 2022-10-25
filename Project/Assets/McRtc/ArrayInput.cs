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
                if(value.Length == m_data.Length)
                {
                    Client.SendArrayInputRequest(id, value);
                    m_data = value;
                }
                else
                {
                    Debug.LogError($"You attempted to set data of size {value.Length} in {id} which expects size {m_data.Length}");
                }
            }
        }
        public string[] labels
        {
            get { return m_labels; }
        }
        private float[] m_data;
        private string[] m_labels;
        public void UpdateArray(string[] labels, float[] data)
        {
            if(labels.Length == 0 && m_labels.Length != data.Length)
            {
                labels = new string[data.Length];
                for(int i = 0; i < labels.Length; ++i)
                {
                    labels[i] = i.ToString();
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