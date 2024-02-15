# Mods
This game supports mods that can change the way the game works and/or looks.
Multiple mods can be used, each must have their own folder in this folder.
A mod folder can have the following content:

- `description.json`
    - This file is the one giving general meta data regarding the mod.
      It is in a JSON format, where it needs an `ID`, `Version`, `Name` and `Description` value.
- `Content`
    - This folder can contain MonoGame compiled resources (can also be normal PNGs or JPEGs)
- `Textures`
    - This folder can contain JSON files that define what textures are bound to what IDs in the game.
      Each JSON file in here must contain a single `TextureSet` array, consisting of an object for each texture definition.
      Each of said objects must have a `ID` and `Content` value in them.
      The `Content` value is the path to a resource file in the `Content` folder relative from the `Content` folder.
- `Enemies`
    - Can contain Enemy JSON definitions
- `EnemyTypes`
    - Can contain EnemyType JSON definitions
- `GameStyles`
    - Can contain GameStyle JSON definitions
- `Maps`
    - Can contain Map JSON definitions
- `Projectiles`
    - Can contain Projectile JSON definitions
- `Turrets`
    - Can contain Turret JSON definitions