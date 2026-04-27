Goals and Rivals are optional personal challenge mechanics in Band BoomBox that let you measure your performance against three different benchmarks: your previous best score (PB), another player's best score (RB), a target grade of your choosing. These are configured before a song begins and are tracked live during gameplay via the *Pace Display*. All three of these benchmarks can be active simultaneously.

## The Pace Display

![Pace Display Diagram](https://github.com/user-attachments/assets/86e5766b-4995-4498-a636-b304fe4e4f0c)

The Pace Display is shown on your Note Highway during gameplay and gives you a real-time view of how your current performance compares to up to three benchmarks:

| Column | Label | Description |
|--------|-------|-------------|
| Personal Best | **PB** | Your own best score on this chart |
| Rival Best | **RB** | Your selected rival's best score on this chart |
| Goal | **A+, etc** | The minimum score needed to achieve your chosen target grade |

Each column shows the difference (in Performance Points) between your current score and the benchmark, adjusted based on for how far through the song you are:

- A green value (e.g. `+150`) means you are currently ahead of that benchmark.
- A red value (e.g. `-80`) means you are currently behind that benchmark.
- A black `0` means you are exactly tied with that benchmark.
- `--` is shown when a benchmark is not available (e.g. no rival selected, no previous score, or no goal set).

Goals and Rivals are independent of each other - you can use either, both, or neither. Using both gives you the most feedback during a song and the greatest opportunity for EXP bonuses. The Pace Display shows all three benchmarks (PB, RB, and Goal) simultaneously so you can keep track of each at a glance.

---

## Goals

A Goal is a target grade you set for yourself. If you set a goal, the game will track whether you are on pace to achieve it and reward or penalize your experience gain at the end of the song based on the outcome. To set a goal, go to the Player Options screen before selecting a song, scroll to the `Goal` option, and press left or right to cycle through the available target grades. Alternatively, you can select `No Goal` to play without one. This selection will be saved to your profile and remain selected until you decide to change it.

![Goal Player Option](https://github.com/user-attachments/assets/c21308fd-a482-4dde-bbde-6df40acb197e)

> NOTE: In multiplayer games, every player is free to select their own goal.

### Goal EXP Effects

At the end of a song, your chosen goal will award you bonus experience points, with higher grades awarding a bigger bonus, *but only if you actually meet or exceed the target grade*. If, however, you fail to meet your goal, you will receive a significant EXP penalty instead.

| Target Grade | Requirement | EXP Bonus |
|--------------|-----------|-----------|
| D | 50% | x1.00 (no bonus) |
| D+ | 55% | x1.02 |
| C | 60% | x1.05 |
| C+ | 65% | x1.07 |
| B | 70% | x1.10 |
| B+ | 75% | x1.15 |
| A | 80% | x1.20 |
| A+ | 85% | x1.30 |
| S | 90% | x1.40 |
| S+ | 93% | x1.45 |
| SS | 96% | x1.50 | 
| M | 98% | x1.50 |

[Screenshot]

---

## Rivals

A Rival is another player profile whose best score is used as a live comparison target during gameplay. Competing against a rival also influences the EXP you earn at the end of a song. To set a rival, go to the Player Options screen before selecting a song, then choose `Select Rival` to open the Rival Select screen. From there, pick any profile from the list, or choose `No Rival` to play without one. Your selected rival is saved to your profile and will be used automatically in future sessions until you decide to change it.

![Rival Player Option](https://github.com/user-attachments/assets/fae67fec-f84c-4bbe-ad1c-a328c611dc3a)

> **Note:** You cannot select your own profile as a rival.


### How Rivals Work During Gameplay

When you confirm your difficulty on the **Difficulty Select** screen, the game looks up your rival's best score on the chosen chart and difficulty. That score is used as the *RB (Rival's Best)* baseline on the Pace Display.

If your rival has never played that chart, or if you have no rival set, the RB column will show `--`.

### Rival EXP Effects

At the end of a song, beating (or losing to) your rival affects the experience you earn. However, this works differently depending on whether your rival is currently playing with you or not.

When playing a game together with your rival (either locally or in a Network game), your individual score is compared to your rival's score, *regardless of which difficulty they are playing on*. The higher percentage is declared the winner.

When playing a game without your rival present, but with a rival set, your score is compared to your rival's stored best score on the same chart and difficulty. The higher percentage is declared the winner.

| Outcome | EXP Modifier |
|---------|-------------|
| No rival set | No change |
| No rival score available | No change |
| Rival defeated | Bonus EXP (scales with rival's grade; up to ×1.50) |
| Tied with rival | Same bonus as defeating them |
| Rival victorious | ×0.95 (-5% penalty) |

> **Note:** that the bonus for defeating a rival scales with the rival's score grade. The higher the grade your rival achieved, the more EXP you earn for beating them. In addition, if your rival is currently playing with you, the bonus for defeating them is doubled compared to when you beat their stored high score without them present.

