using System.Text.Json;
using Procession.Core;

int passed = 0;
void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
void Test(string name, Action action) { action(); Console.WriteLine("PASS " + name); passed++; }
EncounterDirector Active()
{
    var d = new EncounterDirector(random: () => .5);
    d.StartArrival(20);
    d.Activate(new Dictionary<long, ChanterKind> { [1] = ChanterKind.Bound, [2] = ChanterKind.Veiled,
        [3] = ChanterKind.Resonant, [4] = ChanterKind.Hollow }, 5);
    return d;
}
T RoundTrip<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value))!;

Test("arrival has no lock or drain until setup completes", () => {
    var d = new EncounterDirector(); d.StartArrival(20);
    Check(!d.LocksStorm && d.DrainMultiplier == 1, "premature storm lock");
    Check(d.Tick(19, false) == EncounterSignal.None, "early arrival");
    Check(d.Tick(1, false) == EncounterSignal.ArrivalReady, "missed arrival");
});
Test("eight shared defenders plus one one-time bell", () => {
    var d = Active();
    for (long id = 10; id < 18; id++) Check(d.RegisterDefender(id, 12), "slot missing");
    Check(!d.RegisterDefender(18, 12), "ninth defender accepted");
    Check(!d.RegisterDefender(19, 100, true), "duplicate bell accepted");
    Check(d.DefenderDied(5) && !d.RegisterDefender(20, 100, true), "bell replaced after death");
});
Test("duplicate and overlapping identities cannot inflate or free allowance", () => {
    var d = Active();
    Check(!d.RegisterDefender(1, 12) && !d.RegisterDefender(5, 12), "owner collision");
    Check(d.RegisterDefender(10, 12) && !d.RegisterDefender(10, 12), "duplicate guard");
    Check(!d.DefenderDied(99) && d.State.PauseSeconds == 0, "foreign kill changed pause");
});
Test("weak kills never shorten an existing strong pause", () => {
    var d = Active(); d.RegisterDefender(10, 60); d.RegisterDefender(11, 12);
    d.DefenderDied(10); var first = d.State.PauseSeconds; d.Tick(2, true); d.DefenderDied(11);
    Check(Math.Abs(d.State.PauseSeconds - (first - 2)) < .001, "pause shortened or added");
});
Test("strong kill extends rather than sums pause", () => {
    var d = Active(); d.RegisterDefender(10, 12); d.RegisterDefender(11, 100);
    d.DefenderDied(10); d.Tick(1, true); d.DefenderDied(11);
    Check(d.State.PauseSeconds == 20, "delays added or cap not applied");
});
Test("first reinforcement waits a fresh interval after pause", () => {
    var d = Active(); d.RegisterDefender(10, 12); d.DefenderDied(10);
    Check(d.Tick(5, true) == EncounterSignal.None, "spawn at pause boundary");
    Check(d.Tick(3.9, true) == EncounterSignal.None, "spawn before cadence");
    Check(d.Tick(.1, true) == EncounterSignal.ReinforcementDue, "missing paced reinforcement");
});
Test("long tick cannot create catch-up spawn burst", () => {
    var d = Active(); Check(d.Tick(100, true) == EncounterSignal.ReinforcementDue, "missing due spawn");
    d.RegisterDefender(10, 12);
    Check(d.Tick(0, true) == EncounterSignal.None, "catch-up queued");
});
Test("failed placement backs off a whole fresh cadence", () => {
    var d = Active(); d.Tick(5, true); d.RetryReinforcementLater();
    Check(d.Tick(3, true) == EncounterSignal.None, "failed spawn hot loop");
    Check(d.Tick(1, true) == EncounterSignal.ReinforcementDue, "retry missing");
});
Test("opening empty slots fill with ordinary cadence", () => {
    var d = Active(); for (long id = 10; id < 14; id++) d.RegisterDefender(id, 12);
    Check(d.Tick(3, true) == EncounterSignal.None, "initial fill too fast");
    Check(d.Tick(1, true) == EncounterSignal.ReinforcementDue, "initial fill blocked forever");
});
Test("four deaths release drain in exact stages and final death wins", () => {
    var d = Active(); Check(d.DrainMultiplier == 3, "four multiplier");
    Check(!d.ChanterDied(1) && d.DrainMultiplier == 2, "three multiplier");
    Check(!d.ChanterDied(2) && d.DrainMultiplier == 1.5, "two multiplier");
    Check(!d.ChanterDied(3) && d.DrainMultiplier == 1 && d.LocksStorm, "last chanter failed to lock");
    Check(d.ChanterDied(4) && !d.LocksStorm && d.State.Phase == EncounterPhase.Victory, "storm not released");
    Check(!d.ChanterDied(4), "duplicate victory");
});
Test("specialist death removes its family with no revival", () => {
    var d = Active(); d.ChanterDied(1);
    Check(!d.FamilyAllowed(ChanterKind.Bound) && d.FamilyAllowed(ChanterKind.Veiled), "family shutdown");
    d.ChanterDied(1); Check(d.State.LivingChanters.Count == 3, "repeat death changed roster");
});
Test("absence is group wide and returning resets it", () => {
    var d = Active(); d.Tick(899, false); Check(d.LocksStorm, "early despawn");
    d.Tick(1, true); Check(d.State.AbsenceSeconds == 0, "return did not reset absence");
    d.Tick(899, false); Check(d.LocksStorm, "absence added across visit");
    Check(d.Tick(1, false) == EncounterSignal.Abandoned && !d.LocksStorm, "permanent storm lock");
});
Test("save reload preserves cap pause and dead chanters", () => {
    var d = Active(); d.RegisterDefender(10, 50); d.RegisterDefender(11, 12);
    d.DefenderDied(10); d.ChanterDied(2); d.Tick(3, false);
    var restored = new EncounterDirector(RoundTrip(d.State), random: () => .5);
    Check(restored.State.Defenders.Count == 1 && restored.State.LivingChanters.Count == 3, "ownership lost");
    Check(restored.State.PauseSeconds == d.State.PauseSeconds && restored.State.AbsenceSeconds == 3, "clock reset");
    Check(!restored.FamilyAllowed(ChanterKind.Veiled), "dead specialist revived");
});
Test("corrupt active zero-chanter state releases lock without awarding victory", () => {
    var d = new EncounterDirector(new() { Phase = EncounterPhase.Active });
    Check(!d.LocksStorm && d.State.Phase == EncounterPhase.Abandoned, "unsafe recovery");
});
Test("unloaded absence still expires after save reload", () => {
    var d = Active(); d.Tick(600, false);
    var restored = new EncounterDirector(RoundTrip(d.State)); restored.Tick(300, false);
    Check(!restored.LocksStorm && restored.State.Phase == EncounterPhase.Abandoned, "unloaded lock");
});
Test("ordinary storms are ineligible and each storm decision is stable", () => {
    var p = new WorldProgression();
    Check(!p.DecideStorm("light", false) && !p.DecideStorm("medium", false), "non-heavy event");
    Check(p.DecideStorm("heavy", true) && p.DecideStorm("heavy", true, 0), "decision changed mid-storm");
    Check(!p.DecideStorm("chance-zero", true, 0), "zero chance bypass");
});
Test("control cannot be used before separate boss victory", () => {
    var p = new WorldProgression(); Check(!p.Install(0, "base", "owner"), "unearned control");
    bool threw = false; try { p.RecoverControl(); } catch (InvalidOperationException) { threw = true; }
    Check(threw, "unearned recovery");
});
Test("world control is owner/admin only and single-authority", () => {
    var p = new WorldProgression(); int token = p.EarnControl();
    Check(p.Install(token, "base", "owner"), "cannot install");
    Check(!p.Install(token, "other", "owner"), "two authorities");
    Check(!p.SetSuppressed("base", "stranger", false, true), "unauthorized toggle");
    Check(p.SetSuppressed("base", "admin", true, true), "admin denied");
    Check(!p.DecideStorm("heavy", true), "world suppression failed");
});
Test("mid-storm toggles affect only next storm", () => {
    var p = new WorldProgression(); p.Install(p.EarnControl(), "base", "owner");
    Check(p.DecideStorm("first", true), "first event missing");
    p.SetSuppressed("base", "owner", false, true);
    Check(p.DecideStorm("first", true), "active event canceled");
    Check(!p.DecideStorm("second", true), "next event not suppressed");
    p.SetSuppressed("base", "owner", false, false);
    Check(!p.DecideStorm("second", true) && p.DecideStorm("third", true), "late or duplicate arrival");
});
Test("unloaded controller/reload preserves world scope and storm latch", () => {
    var p = new WorldProgression(); p.Install(p.EarnControl(), "base", "owner");
    p.SetSuppressed("base", "owner", false, true); p.DecideStorm("storm", true);
    var restored = new WorldProgression(RoundTrip(p.State));
    Check(restored.State.Suppressed && !restored.DecideStorm("storm", true), "controller reset on reload");
    Check(!restored.DecideStorm("next", true), "unloaded controller ineffective");
});
Test("removal returns future events to allowed", () => {
    var p = new WorldProgression(); p.Install(p.EarnControl(), "base", "owner");
    p.SetSuppressed("base", "owner", false, true);
    Check(!p.Remove("base", "stranger", false), "unauthorized removal");
    Check(p.Remove("base", "owner", false) && p.DecideStorm("future", true), "removal stranded suppression");
});
Test("recovery invalidates old physical controllers", () => {
    var p = new WorldProgression(); int old = p.EarnControl(); p.Install(old, "base", "owner");
    p.SetSuppressed("base", "owner", false, true); int replacement = p.RecoverControl();
    Check(replacement > old && !p.Install(old, "old", "owner"), "old controller not revoked");
    Check(p.Install(replacement, "new", "owner") && !p.State.Suppressed, "replacement not usable");
    Check(p.EarnControl() == replacement, "repeat victory rotated token");
});
Test("storm multiplier scales drain without multiplying lost positive recovery", () => {
    // Geographic recovery 0.5, ordinary storm drain 0.2: requested 3x is 0.6 drain.
    double result = StabilityMath.ApplyStormDrain(.8, 1.5, 3);
    Check(Math.Abs((1 - result) - .6) < .000001, "recovery loss counted as drain");
});
Test("geographic drain remains additive rather than multiplied", () => {
    // Baseline geographic drain 0.2 plus storm contribution 0.8 * 3 => 2.6.
    double result = StabilityMath.ApplyStormDrain(0, .8, 3);
    Check(Math.Abs((1 - result) - 2.6) < .000001, "geographic drain multiplied");
});
Test("one survivor returns exactly ordinary storm drain", () => {
    Check(StabilityMath.ApplyStormDrain(.3, 1.5, 1) == .3, "last survivor changed ordinary drain");
    Check(StabilityMath.ApplyStormDrain(1.1, 1.5, 3) == 1.1, "invented drain in recovery region");
});
Console.WriteLine($"{passed} behavioral checks passed.");
