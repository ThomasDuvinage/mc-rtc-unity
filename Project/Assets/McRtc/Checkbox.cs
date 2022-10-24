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
          if(!update_from_server && value != state)
          {
            client.SendCheckboxRequest(id);
          }
          state = value;
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
