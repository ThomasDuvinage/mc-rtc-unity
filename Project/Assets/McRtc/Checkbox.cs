using System;
using System.Collections;
using UnityEngine;

namespace McRtc
{
    [ExecuteAlways]
    public class Checkbox : Element
    {
      public string id;
      public bool state
      {
        get { return state; }
        set
        {
          state = value;
          if(!update_from_server)
          {
            client.SendCheckboxRequest(id, value);
          }
        }
      }
      private bool update_from_server = false;

      public void UpdateState(bool state)
      {
        if(state != this.state)
        {
          update_from_server = true;
          this.state = state;
          update_from_server = false;
        }
      }

      public void Disconnect()
      {
      }
  }
}
