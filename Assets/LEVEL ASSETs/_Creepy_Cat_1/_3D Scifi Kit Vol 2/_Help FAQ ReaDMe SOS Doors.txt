Hi! 

First! Thanks for buying the kit :) Really!

I hope it will give you satisfaction. But please, no matter the opinion you have, 
please take 5 minutes of your time to leave a review and put the stars that seems 
right to you... That really help us.

Please do not to use the review as a platform to show your anger or to request support. For that use : 
    ----
    
black.creepy.cat@gmail.com

Why? Just because i check my emails more than the unity store reviews... 

---------------------
About the rendering : 
---------------------

To get the same as the video, you need to install the package post processing 
from the package manager window. Define your build project to LINEAR GAMMA 
and DEFFERED RENDERING. And use the fx profile i made for you!

https://docs.unity3d.com/Manual/LinearRendering-LinearOrGammaWorkflow.html


-------------------------------------------------------------------------------------
Here are a series of questions and answers about the kit (I love talking to myself) :
-------------------------------------------------------------------------------------

---------------------------------------------------------------------
"Hell! I computed the lightmap but my rendering is black or/and glitched?"
---------------------------------------------------------------------

- Don't panic, it's a unity problem with the lightmap. To fix it; Clear the GI cache first, and then delete the 
  directory called "Example_01" and recompute the lightmap.

-------------------------------------------
"I want this product working with URP/HDRP"
-------------------------------------------

All Unity package can be converted! If the publisher use Unity Standard Shaders, good news! It's the case :)

Follow this excellent Unity tutorial to make this package working on HDRP/URP : 
https://www.youtube.com/watch?v=VD5Qr4Rt7-Q

Check all this guy videos! Very good channel...


---------------------------------------------------------------------------
"Why there is a "MouseLook" error when i import your package with Occulus!"
---------------------------------------------------------------------------
- It's just a name conflict :) Just remove the component : MouseLook on the camera and delete the script Common Scripts => MouseLook


---------------------------------------
"Those damned doors did not open? Why?"
---------------------------------------

- Keep calm :) Do not write a crappy review now :) The problem is due to the static mode! Unity do not want move 
  objects flagged as static… You just have to search the gameobjects that are named "Door_Left" and "Door_Right", select
  them all and flag them as static before computing the lightmap.

  I can not tell you how many times, after two hours of lightmap computing, i realized that the doors were 
  static)
  
  
--------------------------------------------------------------
"Creepy Cat! You suck so much! what this bling-bling world???"
--------------------------------------------------------------
- If you find the level to be too much metallic, you can easily attenuate the effect! Just open
  photoshop (or your preferred 3D software), and play with the brightness/contrast of the metalic maps (_Metal.tiff)
  to increase/remove the effect (try to play with the alpha channel too...  :)
  
I wanted to make a level more bling bling and clean. I do not like the dark and murky sci-fi environments :) 


--------------------------------------------------------------
"It's cool! But i want to do my own creepy/dark textures?"
--------------------------------------------------------------
On this issue, I would not say that this will be super easy and fast. If you have experience with photoshop, 
you can easily recreate the textures, or make a good adaptation for your project.

To help you, I also provided the flat textures (_Flat.png in the zip file). If you use them in photoshop 
with the normal maps (_Norm.png), it will be possibly easy to recreate all the layers and then apply the effect/
textures of your choice.

It's a bit of work, yes! But it's possible. The kit has been made this way.

Note : Do not panic, i do not plan to move/modify/scale the position of the elements 
under the textures (i say this if you want to batch your own), but i think i will be adding some 
new textures in the future.

----------------------------------------------------------------------------------------------
"Creepy Cat! You suck so much again! Why do you not provide textures in photoshop files? Directly?"
----------------------------------------------------------------------------------------------
It's just because i don't make them under photoshop! Instead i use my own 3D software... To get good
diffuse/normal/ao/metal maps, i have my own generation method, based on my experience. 

My personal cooking, if you prefer :) That's why I do not have files with layers. 

But a motivated team that will use my kit as a graphic base, will be able to easily re-custom all the kit. 


--------------------
"Any update planed?"
--------------------
Maybe, i can make a quick update soon, to finish the rest of the objects i want to give (but my mind for the moment
is exhausted, and needs recharge... it's always like this with big projects). Later there will be more consistent
updates based on the users demands... 

I would take the ideas that I think best suitable for the whole pack.


-------------------------------------------------------------------------------------
"Oh my god! there is a missing object/texture! (There is always a missing object/texture in a kit...)
-------------------------------------------------------------------------------------
To fix the missing texures in your kit, i can provide you some parts of the textures!

Indeed, the kit was made with the rules of the modular design, and the textures are no exception. 
It is possible, for example, using the texture: 

Wall_Atlas_05_Dif.png 

to create (for those who have experience with a 3D software) an engine room, or whatever
you want/need? just stay within each of textures and make your texture based on them.

Wall_Atlas_01_Dif.png

for example, can be used to create other special missing walls textures for your project! 

Because yes, creating a video game takes a bit of work .... 

To expect a kit to solve all your game design issues for $30 is unrealistic.

This kit is a starting base! I will try one day to make a video to explain an example on how to
re-use the textures to create a new ones.

The prefab "Station_Radar_01" is a pure example of my previous explanations. It was easily made
with the kit textures, same for the prefab "Station_Base_01" etc...

And for the last time again :) Thanks for buying the kit :)


-----------
"KNOW BUGS" 
-----------

- I am notified about a random coloring bug on mac, but not on pc. If the scene coloring is "red" switch the camera 
  to "Legacy Deferred" 																						
																						
																						
																						
													Creepy Cat
























																																																																																																									