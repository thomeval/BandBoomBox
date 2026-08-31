# One Hand Tapping

In Band BoomBox chart composition, **One Hand Tapping** refers to placing multiple consecutive notes within the same lane, forcing the player to hit a sequence of notes using only one hand while the other hand remains idle.

---

## Overview

Band BoomBox arranges notes across distinct input lanes on the note highway, corresponding to specific hands or button groups on a controller or keyboard:

- **Top Lane (Trigger / Shoulder Lane):** Used on Expert, N.E.R.F., and optionally Extra difficulties for shoulder/trigger buttons (`LB`, `RB`, `LT`, `RT`).
- **Middle Lane (Directional / D-Pad Lane):** Controlled with the left hand (`Left`, `Down`, `Up`, `Right`).
- **Bottom Lane (Button / Face Button Lane):** Controlled with the right hand (`A`, `B`, `X`, `Y`).

When a chart places several notes in sequence on the same lane—such as `Left` &rarr; `Down` &rarr; `Right` on the middle lane, or `A` &rarr; `X` &rarr; `B` on the bottom lane—the player is forced to perform rapid single-handed inputs.

```text
Avoid (One Hand Tapping):
  [Middle Lane]  (Left) ---> (Down) ---> (Right)
  [Bottom Lane]  --------------------------------- (Idle)

Recommended (Hand Alternation - 2-Lane):
  [Middle Lane]  (Left) -------------> (Down)
  [Bottom Lane]  ----------> (A) --------------> (B)
```

---

## Why One Hand Tapping Should Be Avoided

When composing charts, One Hand Tapping should generally be avoided for several important reasons:

### 1. Ergonomics and Player Fatigue
Standard game controllers rely on the player's thumbs to press the D-Pad and face buttons. Hitting consecutive notes on the same hand requires the thumb to quickly reposition and tap multiple distinct buttons in rapid succession. This leads to swift thumb fatigue, muscle strain, and physical awkwardness.

### 2. Disrupted Rhythmic Flow
Band BoomBox gameplay feels most dynamic and satisfying when players can alternate between hands (left-right-left-right) or engage both hands in a coordinated rhythm. Prolonged sequences on a single lane disrupt this natural cadence, leaving one hand completely passive while the other is overloaded.

### 3. Readability and Execution Difficulty
Fast single-lane sequences are harder for players to read and physically execute accurately on standard gamepads. Moving a single thumb across multiple directional buttons (for instance, jumping from `Left` across the D-Pad to `Right`) requires more travel time than alternating between hands.

### 4. Artificial Difficulty
Overusing consecutive same-lane notes introduces artificial, uncomfortable difficulty rooted in controller ergonomics rather than musical timing or rhythmic competence.

---

## Charting Best Practices: 2-Lane vs. 3-Lane Alternation

When authoring charts, it is essential to keep in mind whether the chart uses **2 lanes** or **3 lanes**, as determined by the target difficulty:

### 2-Lane Difficulties (Beginner, Medium, Mild, Hard)
- **Active Lanes:** Only the **Middle Lane** (Directions) and **Bottom Lane** (Buttons) are used.
- **Alternation Pattern:** Notes should typically alternate back and forth between the middle and bottom lanes (e.g., Middle &rarr; Bottom &rarr; Middle &rarr; Bottom).
- **Goal:** Keep both the left hand (D-pad) and right hand (face buttons) evenly engaged so that neither hand is overloaded with consecutive notes.

### 3-Lane Difficulties (Expert, N.E.R.F.)
- **Active Lanes:** All three lanes (**Top Lane**, **Middle Lane**, and **Bottom Lane**) are in use.
- **Alternation Pattern:** Notes should alternate across all three lanes (e.g., Top &rarr; Middle &rarr; Bottom &rarr; Middle &rarr; Top).
- **Goal:** Smoothly weave notes across triggers/shoulders, D-pad directions, and face buttons to create a balanced multi-finger/multi-hand flow without chaining multiple notes consecutively in any single lane.

```text
Recommended (Hand / Lane Alternation - 3-Lane):
  [Top Lane]     (LB) -----------------------------> (RB)
  [Middle Lane]  ----------> (Left) -------------> (Down)
  [Bottom Lane]  ---------------------> (A) -------------->
```

### Extra Difficulty
- **Active Lanes:** Can be charted as **either 2-lane or 3-lane**, depending on the chart author's design.
- **Alternation Pattern:** Follow the 2-lane or 3-lane alternation guidelines above based on which lane layout is chosen for the chart.

---

## General Charting Guidelines

- **Promote Lane Alternation:** Always design note patterns that move between available lanes rather than clustering in one lane.
- **Distribute Note Density Evenly:** Ensure that all active lanes share the workload over the duration of a song section.
- **Support Natural Movement:** When designing rapid note patterns, prioritize patterns that feel intuitive and comfortable on standard controllers and keyboard layouts.

---

## Exceptions

While One Hand Tapping (consecutive *different* notes in the same lane) should generally be avoided in standard chart composition, there are specific exceptions to this rule where single-lane patterns are acceptable or encouraged:

### 1. One Button Tapping (Repeated Notes)

**One Button Tapping** refers to patterns where the **exact same note** is repeated multiple times in succession (for example: `A` &rarr; `A` &rarr; `A`, or `Down` &rarr; `Down` &rarr; `Down`).

```text
Allowed (One Button Tapping):
  [Bottom Lane]  (A) ---> (A) ---> (A)
```

- **Why It Is Allowed:** Unlike One Hand Tapping across multiple different buttons, repeating the same note does not require the player's thumb to jump or reposition across the controller. The player can comfortably maintain their thumb on the exact same button and tap along with the rhythm.
- **Encouraged on Lower Difficulties:** One Button Tapping is especially encouraged on lower difficulty levels (such as **Beginner** and **Medium**), which utilize a limited set of buttons. This enables newer players to develop rhythmic timing without being overwhelmed by rapid button transitions.

### 2. Double Notes (Simultaneous Notes)

A **Double Note** occurs when two notes appear on different lanes at the exact same moment (e.g., hitting a Middle lane direction and a Bottom lane button simultaneously, such as `Down` + `A`, or a Top lane shoulder button and a Bottom lane button, such as `LB` + `A`).

Handling the notes directly preceding and following a double note depends on whether the chart uses 2 lanes or 3 lanes:

#### In 2-Lane Charts (Beginner, Medium, Mild, Hard):
- **Unavoidable Single-Lane Overlap:** A double note occupies both available lanes simultaneously. Consequently, regardless of which lane is chosen for the note directly before or after the double note, it will inevitably share a lane with one of the notes in the double note pair, making a same-lane transition unavoidable.
- **Rule Flexibility:** Because of this structural constraint in 2-lane charts, the note directly before and directly after a double note **can be placed on any lane**.
- **Lower Difficulty Recommendation:** On lower difficulty levels, using **One Button Tapping** in conjunction with the double note (such as using the exact same button directly before or after the double hit) is recommended to keep the pattern intuitive and minimize physical difficulty.

```text
Double Note in a 2-Lane Chart:
  [Middle Lane]  (Down) -----------------> (Down)
  [Bottom Lane]  ------------> [A   ] ---> (A)
                               [Down]
                               (Double Note)
```

#### In 3-Lane Charts (Expert, N.E.R.F., 3-Lane Extra):
- **Utilize the Free Lane:** Because three lanes are available and a double note only occupies two of them, a third lane is left free.
- **Placement Guideline:** The note preceding or following the double note should ideally be in the **lane that is not part of the double note** (e.g., if the double note occupies the Top and Bottom lanes with `LB` + `A`, the adjacent notes should be placed in the Middle lane).
- **Alleviation with One Button Tapping:** If an adjacent note must be placed on one of the lanes used by the double note, use **One Button Tapping** (matching the exact same note/button from the double note) to alleviate hand strain and avoid awkward same-lane button shifts.

```text
Recommended (3-Lane with Free Lane):
  [Top Lane]     ----------------> [LB  ] -----------------
  [Middle Lane]  (Left) ---------> [    ] ---------> (Down)
  [Bottom Lane]  ----------------> [A   ] -----------------
                                   (Double Note)

Acceptable (3-Lane with One Button Tapping):
  [Top Lane]     ----------------> [LB  ] -----------------
  [Middle Lane]  ----------------> [    ] -----------------
  [Bottom Lane]  (A) ------------> [A   ] ---------> (A)
                                   (Double Note)
```

### 3. Active Hold Notes (Held Lanes)

When a chart requires the player to sustain a **Hold Note** in one lane, the corresponding hand/finger is occupied holding down that input. Note placement rules during active holds depend on the chart's lane count:

#### In 2-Lane Charts (Beginner, Medium, Mild, Hard):
- **Acceptable One Hand Tapping:** When one lane is occupied by an active hold note, there is no room to place additional notes in that lane. Consequently, One Hand Tapping is **acceptable on the remaining free lane**.
- **Lower Density Requirement:** Because one hand is pinned holding a button, notes charted on the free lane during the hold should have a **lower note density** (slower tempo / spaced further apart) to avoid excessive fatigue and strain on the tapping hand.

```text
Acceptable (2-Lane Hold Pattern - Lower Density):
  [Middle Lane]  [Hold Start]==============================[Release]
  [Bottom Lane]  ------------> (A) -----------> (B) ------->
```

#### In 3-Lane Charts (Expert, N.E.R.F., 3-Lane Extra):
- **One Lane Held Down:** If only one lane is being held down, notes should **alternate between the other two free lanes** (e.g., if the Top Lane is held, notes should alternate between the Middle and Bottom lanes).
- **Two Lanes Held Down:** If two lanes are held down simultaneously, One Hand Tapping is **acceptable on the single remaining free lane**, maintaining a lower, comfortable note density.

```text
Recommended (3-Lane with 1 Lane Held):
  [Top Lane]     [Hold Start]==============================[Release]
  [Middle Lane]  -------------> (Left) ------------> (Down)
  [Bottom Lane]  (A) --------------------> (B) ------------>
```
