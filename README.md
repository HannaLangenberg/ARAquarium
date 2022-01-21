# AquAR
A project from wintersemester 2021 using Unity and C#.
<br/>_This being said: Please <b>do not clone</b> this repository._
 
## The Application
AquAR is an AR application for android devices to visualize an aquarium in your room. A variety of different aquaria, fish, plants and decor items are available.
<br/><p align="center"><img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/SplashScreen.jpg" width="550"></p>
 
<br/>Depending on which aquarium is chosen, some fish will be disabled in the selection menu, as the aquarium would not provide enough space for them. Likewise, some fish disable others when selected. For example, discus and weatherloach should not be placed in the same aquarium, so when chosing one, the other will always be disabled.
A small notification at the top of the screen informs the user to avoid confusion. The notification will automatically disappear after a few seconds.
<br/><p align="center"><img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_MeldungNichtVertragen.jpg" width="550"></p>

<br/>When selecting a fish, the application makes sure that schooling fish will always be present in their minimum number, more or not at all. Missing space is the only reason why there might spawn fewer fish than their usual minimum number. In this case the user will be informed about the missing space.
<br/><p align="center"><img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_MeldungAnzahl.jpg" width="550"></p>

All placable objects have a collider to ensure that none of the objects spawn inside one another. In the case that an object could not be placed, the user will be informed.
<br/><p align="center"><img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Warnung.jpg" width="550"></p> 
 
If the user would like to learn more about an object, the detailwindow provides the most important information.
<p float="left">
  <img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_FischDetails.jpg" width="300" />
  <img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Pflanzendetails.jpg" width="300" /> 
  <img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Dekodetails.jpg" width="300" />
</p>

The program takes a few measures to make the aquarium appear a bit more natural:
<br/>A random rotation along the y-axis is applied to every object and additionally the plants and decor items are scaled randomly. The scale factor is individually set for each aquarium to ensure that no objects can outgrow their home.
<br/><p align="center"><img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Rotation.jpg" width="550"></p>

Once the user is happy with the aquarium they can enter the full ARMode. While choosing an area to place the object at, the aquarium follows an indicator. This indicator hints detected planes where the aquarium could be placed. This detection is done by ARCore. After placing the aquarium, the user can walk around and inspect it.
<p float="left">
  <img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Cursor.jpg" width="450" />
  <img src="https://github.com/HannaLangenberg/ARAquarium/blob/main/Assets/Screenshots/S_Platziert.jpg" width="450" />
</p>

### The team:
Hanna Langenberg (me), Niklas K., Marc R.

 
### The project is now submitted to our professor and no further changes (except maybe the ReadMe) will be made until we receive our grade.
</br>

_Sadly I was not able to publish this repository on github earlier as there were some problems with crashing my LFS. The libraries for the android 64 bit architecture were too large and it took me some time to figure out the correct order in which to add the gitignore._
