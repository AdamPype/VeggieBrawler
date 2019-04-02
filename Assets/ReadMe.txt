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