# Initial Vision
This document depicts my initial vision of Evolution Simulator.

## Description
The environment has a bunch of parameters:
- wind speed/direction
- amount of water, depth of water
- color of terrain
- amount of food
- food regeneration rate
- seasons

In the beginning of the game, a bunch of creatures are spawned randomly throughout the environment. The creatures' have randomized parameters. Some creature parameters:
- speed
- weight, volume, shape (affected by physics (e.g. wind))
- energy efficiency
- field of view
- sound hearing radius
- "policy" (move straight towards closest food, move towards bundle of food even if further, look out for preditors, etc)
- time to reproduction (how long until species duplicates)
- time till death if no food

As time passes, some creatures who have not had enough food, will die off. Some creatures whose time to reproduction has exceeded, will duplicate. Basically, if a creatures survives long enough, it will duplicate. But when duplicated, some of the stats will be randomly adjusted by a little (or a lot - random chance). What kind of cool looking and moving creatures will appear during each run?!

## Software development process and methodologies
Highly iterative and lean. Focus on getting a working product as fast as possible. Implement the bare bone number of parameters (both for the environment and the creatures) to get a working product. Don't focus on art, code standards, documentation, etc until a working product is reached. You can always "polish" these things later. Focus on working product on every iteration. I want to have extremely short iterations. Daily iterations is the goal. If I can't do that, I will do 2-3 iterations per week. Last resort is weekly iterations.




