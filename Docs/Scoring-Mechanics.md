# Scoring Mechanics

Band BoomBox uses two different scoring systems at the same time:

- A player score that measures an individual player's performance.
- A team score that measures the overall performance of the whole team.

---

## Player Scoring

Each player's personal score is based on their performance on the notes they hit. The game tracks one main value for each player:

- Performance Points (used to calculate the player's percentage)

### How Performance Points Are Awarded

When a player hits a note, the game assigns a judgment based on timing. The better the judgment, the more performance points the player receives.

| Judgment | Performance Points |
|---------|--------------------:|
| Crit | 3 |
| Perfect | 3 |
| Cool | 2 |
| Ok | 1 |
| Bad | 0 |
| Wrong | 0 |
| Miss | 0 |

> Cool hits are counted as Perfect hits whenever Ally Boosts are applied. For more details, see [Ally-Boost.md](Ally-Boost.md).

A player's current percentage is calculated as:

- Performance Points earned / Maximum possible performance points for the chart

This means your visible score percentage is a measure of how much of the chart's maximum possible performance you achieved, not just how many notes you hit.

> NOTE: Turbo has no effect on individual scores, since Crit hits award the same number of Performance Points as Perfect hits.

### Combo

The individual score also tracks each player's current combo, defined as the number of notes successfully hit without making a mistake. This combo is incremented whenever the player hits a note successfully, and resets when they make a mistake. 

---

## Team Scoring

In addition to individual scores, the whole team also shares a combined score, displayed at the bottom of the screen during gameplay. This score is tracked separately from any single player's performance.

### How Team Score Is Earned

Each successful note contributes a base team score value based on the judgment:

| Judgment | Team Score Points |
|---------|------------------:|
| Crit | 50 |
| Perfect | 50 |
| Cool | 30 |
| Ok | 15 |
| Bad | 0 |
| Wrong | 0 |
| Miss | 0 |

> Cool hits are counted as Perfect hits whenever Ally Boosts are applied. For more details, see [Ally-Boost.md](Ally-Boost.md).

These points are then multiplied by the current score multiplier.

### Score Multiplier

The score multiplier, also known as Momentum, is shown on the right side on the screen. It starts at 1.0x and can increase or decrease depending on how well the team is doing. Accurate hits raise it, whereas mistakes will lower it. It will also naturally decay over time, back to its initial value of 1.0x. The higher the multiplier, the faster it will decay, and the more difficult it will be to maintain it.

### Multiplier Gain and Loss

The amount of multiplier gained or lost depends on the judgment:

| Judgment | Change |
|---------|----------------:|
| Crit | +0.065 |
| Perfect | +0.05 |
| Cool | +0.03 |
| Ok | +0.01 |
| Bad | -0.01 |
| Wrong | -0.05 |
| Miss | -0.25 |

> Cool hits are counted as Perfect hits whenever Ally Boosts are applied. For more details, see [Ally-Boost.md](Ally-Boost.md).

Any increase to the multiplier is _itself_ boosted by the Multiplier Gain Rate, displayed above the Turbo Energy meter on the left side of the screen. This gain rate is usually 0%, but is significantly increased whenever one or more players have Turbo activated. For more details, see [Turbo.md](Turbo.md).

### Team Combo

Similar to individual combos, the team score also tracks a shared combo across the whole team. This combo is incremented whenever _any_ player hits a note successfully, and resets when _any_ player makes a mistake. A higher team combo slightly increases the Multiplier Gain Rate, making it easier to build a larger team score during a strong run.

### High Score Categories

Team scores are stored separately depending on how many players were in the session. The categories are as follows:

| Category | Players | Max Team Stars |
|----------|--------:|---------------:|
| Solo | 1 | 6 |
| Duet | 2 | 7 |
| Squad | 3-4 | 8 |
| Crowd | 5-8 | 9 |
| Legion | 9+ | 10 |

In the Song Select screen, only team scores matching the current category (based on the number of players currently joined in) will be displayed.


