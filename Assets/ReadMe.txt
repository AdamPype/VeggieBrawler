INSTRUCTIONS:

Juice:
Call CameraScript.Shake(power) in any script to shake the screen.
Call FixedTime.FreezeTime(seconds) in any script to freeze the game. (good for powerful impacts)

Sound:
Attach a SoundManager script to a gameobject that you want to produce sound.
In the inspector, add the sound files under "sounds". Write names for the sounds under "names".
In a script, add a SoundManager variable, and in Start link the component to it.
Now, call _soundManager.Play(soundName) to play a sound.
You can add a lot of optional variables as well, such as whether it should loop, if it's spatial and the volume.

(TIP: Giving multiple sounds the same name will select a random one that matches the name.)

--------------------
Character Select:
	Adding new Character Portraits
To add new character portraits, simply add a new child to the "Character Selection" Gameobject. It will automatically orden them because of its Grid Layout Group.
Every Portrait must have the "Character Template" Script attached to it. In this script, you put the gameobject of your character & The Characters Name.
This gameobject will be shown when a player selects this portrait by pressing the "A" Button.

	Character Selecting
Hovering over a Portrait and pressing "A" will pick that character. When picked, the mouse will be locked.
The player can un-choose their character by pressing "B" this will allow them to move the mouse again and pick a new character.

	Other UI Elements
There Are Lerping UI Elements, this visually notifies the player that they picked a character or are able to start the game.
In script you can change what position they Lerp to, their return position will always be their original position when the scene is loaded.

	Character Visualization
The Left/Right Side of the UI Visualize the chosen character. This is done with a rendertexture.
There is a Mask on the P1_Panel and P2_Panel to make sure the rendertexture can't be shown outside of these panels.
If You want you can move these Panels around and change there shape, the rendertexture will still render in them the same way.

	But Tibau, I want to lerp even MORE stuff
If you want to lerp move stuff (which is perfectly normal), you can find a LerpUI Method in both the "CharacterSelectManager" & "PlayerUI" script.
Call this with the rectTransform (since we're dealing with UI) and your Endposition. Don't forget to also save a startPosition of what you're going
to be lerping in the Start Method (Just like I did with the Border in the "PlayerUI" Script) This allows it to be Un-Lerped again by the same Method.

	Altering Character Select
If You don't want the Whole Character Visualization Thing, you can remove the LoadCharacter() & UnLoadCharacter() Methods, be careful to still lock the mouse if you want to
The CharacterSelectManager is the Script that will eventually save which characters were chosen and go to the Game Scene.

	UI Hierarchy
Yeah I'm actually making a part about this I'm sorry
The bottom most thing in the UI Hierarchy will be the top most thing on your screen.
So, if you want your mouse to be above something in the game, you will place the Mouse gameObject under it in the Hierarchy (f.e. Character Selection is above P1Mouse)
Since I wanted the StartGameUI to be shown above the Mouse UI, I placed it lower in the Hierarchy 
--------------------
	