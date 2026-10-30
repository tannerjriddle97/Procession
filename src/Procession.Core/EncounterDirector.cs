namespace Procession.Core;

public enum EncounterPhase { Dormant, Arriving, Active, Victory, Abandoned }
public enum ChanterKind { Bound, Veiled, Resonant, Hollow }
public enum EncounterSignal { None, ArrivalReady, ReinforcementDue, Abandoned }

public sealed class DefenderRecord
{
    public double MaximumHealth { get; set; }
}

/// <summary>All clocks are remaining real seconds; server downtime does not advance them.</summary>
public sealed class EncounterState
{
    public int SchemaVersion { get; set; } = 1;
    public EncounterPhase Phase { get; set; }
    public Dictionary<long, ChanterKind> LivingChanters { get; set; } = new();
    public Dictionary<long, DefenderRecord> Defenders { get; set; } = new();
    public long? OpeningBellId { get; set; }
    public bool OpeningBellSpawned { get; set; }
    public double ArrivalSeconds { get; set; }
    public double AbsenceSeconds { get; set; }
    public double PauseSeconds { get; set; }
    public double RefillSeconds { get; set; }
}

public sealed class EncounterSettings
{
    public int DefenderCap { get; set; } = 8;
    public double AbsenceLimitSeconds { get; set; } = 900;
    public double RefillMinimumSeconds { get; set; } = 3;
    public double RefillMaximumSeconds { get; set; } = 5;
    public double MinimumKillPauseSeconds { get; set; } = 5;
    public double MaximumKillPauseSeconds { get; set; } = 20;
    public double ReferenceHealth { get; set; } = 12;
    public double PauseSecondsPerHealth { get; set; } = .3;

    internal void Validate()
    {
        if (DefenderCap != 8) throw new ArgumentOutOfRangeException(nameof(DefenderCap), "The shared allowance is eight.");
        if (!double.IsFinite(AbsenceLimitSeconds) || AbsenceLimitSeconds <= 0
            || !double.IsFinite(RefillMinimumSeconds) || RefillMinimumSeconds < 3
            || !double.IsFinite(RefillMaximumSeconds) || RefillMaximumSeconds > 5 || RefillMaximumSeconds < RefillMinimumSeconds
            || !double.IsFinite(MinimumKillPauseSeconds) || MinimumKillPauseSeconds < 0
            || !double.IsFinite(MaximumKillPauseSeconds) || MaximumKillPauseSeconds < MinimumKillPauseSeconds
            || !double.IsFinite(ReferenceHealth) || !double.IsFinite(PauseSecondsPerHealth) || PauseSecondsPerHealth < 0)
            throw new ArgumentException("Invalid encounter pacing settings.");
    }
}

/// <summary>Server-independent encounter accounting. The adapter owns spawning, health and vanilla storms.</summary>
public sealed class EncounterDirector
{
    private readonly EncounterSettings settings;
    private readonly Func<double> random;
    public EncounterState State { get; }
    public bool LocksStorm => State.Phase == EncounterPhase.Active && State.LivingChanters.Count > 0;
    public double DrainMultiplier => !LocksStorm ? 1 : State.LivingChanters.Count switch { 4 => 3, 3 => 2, 2 => 1.5, _ => 1 };

    public EncounterDirector(EncounterState? state = null, EncounterSettings? settings = null, Func<double>? random = null)
    {
        this.settings = settings ?? new();
        this.settings.Validate();
        this.random = random ?? Random.Shared.NextDouble;
        State = state ?? new();
        if (State.SchemaVersion != 1) throw new ArgumentException("Unsupported encounter save schema.");
        if (State.LivingChanters is null || State.Defenders is null || State.LivingChanters.Count > 4 || State.Defenders.Count > 8)
            throw new ArgumentException("Corrupt encounter ownership data.");
        if (State.LivingChanters.Values.Distinct().Count() != State.LivingChanters.Count
            || State.LivingChanters.Keys.Any(State.Defenders.ContainsKey)
            || State.OpeningBellId is long bell && (State.Defenders.ContainsKey(bell) || State.LivingChanters.ContainsKey(bell)))
            throw new ArgumentException("Encounter owners must be unique.");
        foreach (double timer in new[] { State.ArrivalSeconds, State.AbsenceSeconds, State.PauseSeconds, State.RefillSeconds })
            if (!double.IsFinite(timer) || timer < 0) throw new ArgumentException("Corrupt encounter clock.");
        if (State.Phase == EncounterPhase.Active && State.LivingChanters.Count == 0) State.Phase = EncounterPhase.Abandoned;
    }

    public void StartArrival(double seconds)
    {
        if (State.Phase is EncounterPhase.Active or EncounterPhase.Arriving) throw new InvalidOperationException("An encounter already exists.");
        if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        State.LivingChanters.Clear(); State.Defenders.Clear();
        State.OpeningBellId = null; State.OpeningBellSpawned = false;
        State.ArrivalSeconds = seconds; State.AbsenceSeconds = 0; State.PauseSeconds = 0;
        State.RefillSeconds = NextInterval(); State.Phase = EncounterPhase.Arriving;
    }

    public void Activate(IReadOnlyDictionary<long, ChanterKind> chanters, long? bellId = null)
    {
        if (State.Phase != EncounterPhase.Arriving) throw new InvalidOperationException("Arrival must begin first.");
        if (chanters.Count != 4 || chanters.Values.Distinct().Count() != 4 || chanters.Keys.Any(id => id <= 0)
            || bellId is long bell && (bell <= 0 || chanters.ContainsKey(bell)))
            throw new ArgumentException("Exactly four distinct chanters are required.");
        State.LivingChanters = new(chanters);
        State.OpeningBellId = bellId; State.OpeningBellSpawned = bellId.HasValue;
        State.Phase = EncounterPhase.Active;
        State.ArrivalSeconds = 0; State.RefillSeconds = NextInterval();
    }

    public bool RegisterDefender(long id, double maxHealth, bool isBell = false)
    {
        if (State.Phase != EncounterPhase.Active || id <= 0 || State.LivingChanters.ContainsKey(id)
            || State.Defenders.ContainsKey(id) || State.OpeningBellId == id || !double.IsFinite(maxHealth) || maxHealth <= 0) return false;
        if (isBell)
        {
            if (State.OpeningBellSpawned) return false;
            State.OpeningBellId = id; State.OpeningBellSpawned = true; return true;
        }
        if (State.Defenders.Count >= settings.DefenderCap) return false;
        State.Defenders.Add(id, new() { MaximumHealth = maxHealth });
        State.RefillSeconds = NextInterval();
        return true;
    }

    public EncounterSignal Tick(double seconds, bool playerWithin30)
    {
        if (!double.IsFinite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
        if (State.Phase == EncounterPhase.Arriving)
        {
            State.ArrivalSeconds = Countdown(State.ArrivalSeconds, seconds);
            return State.ArrivalSeconds == 0 ? EncounterSignal.ArrivalReady : EncounterSignal.None;
        }
        if (!LocksStorm) return EncounterSignal.None;
        State.AbsenceSeconds = playerWithin30 ? 0 : State.AbsenceSeconds + seconds;
        if (State.AbsenceSeconds >= settings.AbsenceLimitSeconds)
        {
            Abandon(); return EncounterSignal.Abandoned;
        }
        // Crossing the pause boundary cannot consume the fresh per-spawn wait in the same tick.
        if (State.PauseSeconds > 0)
        {
            State.PauseSeconds = Countdown(State.PauseSeconds, seconds);
            return EncounterSignal.None;
        }
        if (State.Defenders.Count >= settings.DefenderCap) return EncounterSignal.None;
        State.RefillSeconds = Countdown(State.RefillSeconds, seconds);
        return State.RefillSeconds == 0 ? EncounterSignal.ReinforcementDue : EncounterSignal.None;
    }

    public bool DefenderDied(long id)
    {
        double health;
        if (State.OpeningBellId == id) { State.OpeningBellId = null; health = 100; }
        else if (State.Defenders.Remove(id, out DefenderRecord? defender)) health = defender.MaximumHealth;
        else return false;
        if (State.Phase == EncounterPhase.Active)
        {
            State.PauseSeconds = Math.Max(State.PauseSeconds, KillPause(health));
            State.RefillSeconds = NextInterval();
        }
        return true;
    }

    public bool ChanterDied(long id)
    {
        if (State.Phase != EncounterPhase.Active || !State.LivingChanters.Remove(id)) return false;
        if (State.LivingChanters.Count == 0) { State.Phase = EncounterPhase.Victory; State.PauseSeconds = 0; }
        return State.Phase == EncounterPhase.Victory;
    }

    public bool FamilyAllowed(ChanterKind specialist) => LocksStorm && State.LivingChanters.ContainsValue(specialist);
    public void RetryReinforcementLater() => State.RefillSeconds = NextInterval();
    public void Abandon() { State.Phase = EncounterPhase.Abandoned; State.PauseSeconds = 0; }
    public double KillPause(double health) => Math.Clamp(settings.MinimumKillPauseSeconds
        + settings.PauseSecondsPerHealth * Math.Max(0, health - settings.ReferenceHealth),
        settings.MinimumKillPauseSeconds, settings.MaximumKillPauseSeconds);

    private double NextInterval() => settings.RefillMinimumSeconds
        + Math.Clamp(random(), 0, 1) * (settings.RefillMaximumSeconds - settings.RefillMinimumSeconds);

    private static double Countdown(double remaining, double elapsed)
    {
        double value = remaining - elapsed;
        return value <= 1e-9 ? 0 : value;
    }
}
