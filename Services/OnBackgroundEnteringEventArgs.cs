using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuinCalc.Services
{
  public class OnBackgroundEnteringEventArgs : EventArgs
  {
    public SuspensionState SuspensionState { get; set; }

    public Type Target { get; private set; }

    public OnBackgroundEnteringEventArgs(SuspensionState suspensionState, Type target)
    {
      SuspensionState = suspensionState;
      Target = target;
    }
  }
}
