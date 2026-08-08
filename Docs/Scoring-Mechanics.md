# Scoring Mechanics

Band BoomBox uses two different scoring systems at the same time:

- A player score that measures an individual player's performance.
- A team score that measures the overall performance of the whole team.

---

## Player Scoring

Each player's individual score shown as a percentage above the Note Highway. This is calculated based on *Performance Points*, which are awarded for every note hit based on the following table:


| Judgment | Performance Points |
|---------|--------------------:|
| Crit | 3 |
| Perfect | 3 |
| Cool | 2* |
| Ok | 1 |
| Bad | 0 |
| Wrong | 0 |
| Miss | 0 |

> NOTE: Cool hits are counted as Perfect hits whenever Ally Boosts are applied. For more details, see [Ally-Boost.md](Ally-Boost.md).

A player's current score percentage is simply their current Performance Points, divided by the maximum possible Performance Points for the current song. This grade is then used to calculate the player's Performance Grade, which is displayed on the Song Results Screen. 

The individual score also tracks each player's current combo, defined as the number of notes this player has successfully hit without making a mistake. This combo is incremented whenever the player hits a note successfully, and resets when they make a mistake. At the end of the song, each player's *Max Combo* will be displayed on the Song Results Screen.

> NOTE: Turbo has no effect on individual scores, since Crit hits award the same number of Performance Points as Perfect hits.

---

## Team Scoring

In addition to individual scores, the whole team also shares a combined score, displayed at the bottom of the screen during gameplay. This score is tracked separately from any single player's performance.

### How Team Score Is Earned

Each note hit contributes a base team score value based on how accurately it was hit:

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

The score multiplier, also known as Momentum, is shown on the right side on the screen. It starts at 1.0x and will increase or decrease depending on how well the team is doing. Accurate hits raise it, whereas mistakes will lower it. It will also naturally decay over time, back to its initial value of 1.0x. The higher the multiplier, the faster it will decay, and the more difficult it will be to maintain its current value.

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

Similar to individual combos, the team score also tracks a shared combo across the whole team. This combo is incremented whenever _any_ player hits a note successfully, and resets when _any_ player makes a mistake. A higher team combo slightly increases the Multiplier Gain Rate, making it easier to build a larger team score during a strong run. At the end of the song, the team's *Max Combo* will be displayed on the Song Results Screen.

### Stars

The team's overall performance is primarily tracked using Stars. During gameplay, the current number of stars earned, as well as progress to the next one, will be shown next to the Team Score, on the bottom of the screen. The better the team performs, the more Stars awarded. Note that the amount of Team Score required for each star is dynamically adjusted based on the number of players present, and the total number of notes each player needs to hit. Therefore, a star rating of five stars is _always_ attainable, regardless of the number of players present, or their selected difficulties. The maximum possible stars is determined by the number of players present, as indicated in the next section.

> NOTE: Although it is possible to earn more than 5 stars, only 5 stars will actually be drawn in this case. The game will indicate more than 5 stars by changing their colour instead.

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


