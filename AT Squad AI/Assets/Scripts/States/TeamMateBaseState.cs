using UnityEngine;

public abstract class TeamMateBaseState
{
    public abstract void EnterState(TeamMateStateManager teamMate);
    public abstract void OnUpdate(TeamMateStateManager teamMate);

}
