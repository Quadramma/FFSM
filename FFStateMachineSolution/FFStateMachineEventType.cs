using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public enum FFStateMachineEventType
    {
        beforeInit,
        afterInit,
        beginUpdate,
        afterUpdate,

        click,

        normal,

        action,
        actionConsequence,

        changeState,
        triggerEnter,
        triggerExit,

        managerBeforeInit,
        managerAfterInit,

    }
}
