## Useless Skater - GitHub Game Off 2018 Entry

**Useless Skater** is a Skateboarding Ski Jump game. Try to jump as far as you can, just be sure to stick the landing or it will hurt, a lot!

### IN DEVELOPMENT

This game is in development and will be released the 1st of December 2018 on [https://itch.io](itch.io)

### Tech

* Engine - Unity

#### Animations

Animations are pretty special in this game. The character is a basic ragdoll setup with joints that stick the feet to the skateboard. There is a copy of the skeleton that is driven by animations but the rendering is using the bones of the ragdoll. The ragdoll then uses torques with the attached rigid bodies to try to mimic the actual animation. This makes the ragdoll "active" more or less. It will be animated, but still have the physics applied to it.
