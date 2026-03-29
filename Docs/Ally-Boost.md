Ally Boost is an optional cooperative multiplayer mechanic in Band BoomBox that allows players to help each other during a song. When playing with others, skilled players can build up boosts by playing well and share them with teammates who need assistance. In single player games, Ally Boost has no effect.

**Note: Currently, Ally Boost is only available in local multiplayer sessions. It is not available in network games, though contributions to fix this are welcome!**

## How Ally Boost Works

### Building Boosts

As you play notes accurately during a song, you'll earn **Ally Boost Ticks**. As you accumulate ticks, you'll eventually earn an **Ally Boost Token** that can be used to help your teammates:

- **Crit hits:** Earn 15 ticks per note (only possible when Turbo mode is active)
- **Perfect hits:** Earn 10 ticks per note  
- **Cool, Ok, Bad, or Wrong hits:** Earn 0 ticks
- **Misses:** Lose 5 ticks

The first Ally Boost Token requires 200 ticks to earn. Each subsequent boost requires 100 additional ticks beyond the previous threshold (300 ticks total for the second boost, 400 for the third, etc.).

Your current Ally Boost progress and available Boost Tokens are displayed at the top of your Note Highway during gameplay.

<img width="300" alt="AllyBoostHUD" src="https://github.com/user-attachments/assets/a253aff5-66e6-4f72-824d-7e6deac86eaa" />

### Using Boosts

Ally Boosts are used **automatically**—you don't need to press any special buttons to activate them. When a teammate is about to get a **Cool** judgment (a slightly mistimed hit), the game will automatically consume one Ally Boost token from another player, if one is available. 
Note that you *cannot* use your own accumulated Ally Boost Tokens - only those provided by your teammates are available to you.

### Effects of Ally Boost

When an Ally Boost is applied to a Cool hit, it is instead upgraded to a **Perfect** hit, providing the full benefits of hitting a note perfectly, such as individual performance points, team score points and multiplier gain.

When an Ally Boost is applied to one of your Cool hits, this indicator will be displayed on the Note Highway:
[Image]

Note that Ally Boosts *only* apply to Cool hits. They will not have any effect on less accurate hits, misses, or "wrong" button presses. You'll still need to stay somewhat close to the timing windows to benefit from them!

## Ally Boost Modes

Each player can configure how they participate in the Ally Boost system. On the Player Options screen before selecting a song, you can select from four modes:
- **On (Default)** - Allows a player to both provide Ally Boosts from others, as well as receive them from others. This is the recommended setting for most multiplayer sessions.
- **Receive Only** - Allows a player to receive Ally Boosts from others, but not provide them. 
- **Provide Only** - Allows a player to provide Ally Boosts to others, but not receive them.
- **Off** - Disables both providing and receiving Ally Boosts. This allows players to play completely independently without participating in the Ally Boost system. Recommended for serious competitive multiplayer sessions.

Each player's selected mode is displayed at the top of their Note Highway during gameplay, using the following icons:

<table>
<tr>
<td>
<img width="100" alt="AllyBoostIndicator" src="https://github.com/user-attachments/assets/a12e6f68-6420-4a24-879e-d0cf987dce35" />
</td>
<td>
<img width="100"alt="AllyBoostIndicator_ProvideOnly" src="https://github.com/user-attachments/assets/cec98277-87b1-496a-bad4-069196cfb889" />
</td>
<td>
<img width="100" alt="AllyBoostIndicator_ReceiveOnly" src="https://github.com/user-attachments/assets/9d3d8282-8e3d-40dc-81fe-b1afface6630" />
</td>
<td>
<img width="100"alt="AllyBoostIndicator_Off" src="https://github.com/user-attachments/assets/687ccd97-6576-4ff7-b5d0-6981be30b7da" />
</td>
</tr>
<tr>
<td>On</td>
<td>Provide Only</td>
<td>Receive Only</td>
<td>Off</td>
</tr>
</table>



