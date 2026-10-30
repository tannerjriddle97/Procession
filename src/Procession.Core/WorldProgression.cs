namespace Procession.Core;

public sealed class WorldProgressionState
{
    public int SchemaVersion { get; set; } = 1;
    public bool CustodianDefeated { get; set; }
    public int ControllerGeneration { get; set; }
    public string? InstalledController { get; set; }
    public string? ControllerOwner { get; set; }
    public bool Suppressed { get; set; }
    public string? LastStormId { get; set; }
    public bool LastStormAllowed { get; set; }
}

/// <summary>World-wide control affects only the decision committed at a heavy-storm boundary.</summary>
public sealed class WorldProgression
{
    public WorldProgressionState State { get; }
    public WorldProgression(WorldProgressionState? state = null)
    {
        State = state ?? new();
        if (State.SchemaVersion != 1) throw new ArgumentException("Unsupported progression save schema.");
    }
    public int EarnControl()
    {
        State.CustodianDefeated = true;
        if (State.ControllerGeneration == 0) State.ControllerGeneration = 1;
        return State.ControllerGeneration;
    }
    public int RecoverControl()
    {
        if (!State.CustodianDefeated) throw new InvalidOperationException("The Custodian must be defeated first.");
        State.ControllerGeneration = checked(State.ControllerGeneration + 1);
        State.InstalledController = null; State.ControllerOwner = null; State.Suppressed = false;
        return State.ControllerGeneration;
    }
    public bool Install(int generation, string position, string owner)
    {
        if (!State.CustodianDefeated || generation != State.ControllerGeneration || string.IsNullOrWhiteSpace(position)
            || string.IsNullOrWhiteSpace(owner) || State.InstalledController is not null) return false;
        State.InstalledController = position; State.ControllerOwner = owner; State.Suppressed = false; return true;
    }
    public bool SetSuppressed(string position, string actor, bool isAdministrator, bool suppressed)
    {
        if (State.InstalledController != position || !isAdministrator && State.ControllerOwner != actor) return false;
        State.Suppressed = suppressed; return true;
    }
    public bool Remove(string position, string actor, bool isAdministrator)
    {
        if (State.InstalledController != position || !isAdministrator && State.ControllerOwner != actor) return false;
        State.InstalledController = null; State.ControllerOwner = null; State.Suppressed = false; return true;
    }
    public bool DecideStorm(string stormId, bool heavy, double chance = 1, double roll = 0)
    {
        if (string.IsNullOrWhiteSpace(stormId)) throw new ArgumentException("A persistent storm identity is required.");
        if (!double.IsFinite(chance) || chance < 0 || chance > 1 || !double.IsFinite(roll) || roll < 0 || roll > 1)
            throw new ArgumentOutOfRangeException(nameof(chance));
        if (State.LastStormId == stormId) return State.LastStormAllowed;
        State.LastStormId = stormId;
        State.LastStormAllowed = heavy && !State.Suppressed && roll < chance;
        return State.LastStormAllowed;
    }
}
