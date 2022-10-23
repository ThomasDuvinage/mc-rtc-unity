using System;
using System.Collections;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public class TransformElement : Element
    {
        public bool read = true;
        public bool write = true;
        public bool send_local_transform = false;
        public void UpdateTransform(bool ro, PTransform pt)
        {
            if(write)
            {
                PTransform tf;
                if (send_local_transform)
                {
                    tf = PTransform.FromLocalTransform(gameObject);
                }
                else
                {
                    tf = PTransform.FromTransform(gameObject);
                }
                Client.SendTransformRequest(id, tf);
            }
            if (read)
            {
                pt.SetLocalTransform(gameObject);
            }
        }

        public void DeleteTransform()
        {
        }
    }
}