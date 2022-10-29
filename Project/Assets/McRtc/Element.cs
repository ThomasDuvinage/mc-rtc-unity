using System.Collections;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public abstract class Element : MonoBehaviour
    {
        public string id;
        public bool persist = true;
        public bool connected { get; private set; } = false;
        public float last_update_delay { get; private set; } = float.MaxValue;
        public float last_update_time { get; private set; } = 0;

        public void tick()
        {
            if (!connected)
            {
                connected = true;
                last_update_time = Time.time;
            }
            last_update_delay = Time.time - last_update_time;
            last_update_time = Time.time;
        }

        public void disconnect()
        {
            connected = false;
            if (!persist)
            {
                OnDisconnect();
            }
        }

        protected abstract void OnDisconnect();
    }
}