This zip contains a stripped-down version of the GCM Launcher that lets you create customized random scenarios.

It also contains the source code and data files for recompiling and running this offline GCM Launcher.  

It doesn't depend on the GCM website to work.  When you first run GcmOffline.exe it will copy a bunch of mods named "GCM_Offline" into your Scourge of War\Mods directory and activate these mods.  The GCM_Offline mod has the GCM gameplay mod as it was on 6/29/2014.  

If I make future changes to GCM I probably won't also make the same changes to this offline version.

I don't know if anyone will ever use this, but hopefully someone will take it and make something better than GCM in the future, or just a good alternative way of playing the game.

The code is licensed under a liberal version of the MIT license, so you can do whatever you want with it.  I recommend that if you modify and redistribute it, you publish the source code and data for your modifications too, but you're not required to.  If you do modify it, please change the name and icon so people don't get it confused with this version.

The code is mostly C# and uses .NET framework version 4 or higher, and was successfully compiled with Visual Studio 2010.  I assume it should work in Visual Studio 2010 Express, but it may need to be modified some for that, I'm not sure.

I apologize for bad code, inconsistent formatting, and any nonsense you find here.  The code was written from 2009 to 2014 in my spare time.  Sometimes I was in a hurry and often I made dumb decisions with no excuse at all.  I don't have the motivation to make time to clean it up, so I'm publishing it as is, maybe it's better than nothing.  :)

Breakdown of the folders:
GCM Offline
  This has the same contents as the GCM_Offline.zip that is published separately for users.
Source\External
  This has some python code I use to generate a bunch of plain-old-data objects.  Many of them aren't used in the offline version.  There are also some scratch spreadsheets in here I used for generating some of the data files and other random stuff.
Source\Gcm
  The main Launcher project.
Source\GcmShared
  This had all the code that lived in both the Launcher and on the website.  Most of the GCM logic for generating armies and scenarios lives here.
Source\GcmShared.Fs
  I wrote a little of the code in F# when I was playing around with that language.
Source\Military
  This has the basic generic military structures used in the GCM code.
Source\Utilities
  This has a bunch of code that isn't directly related to GCM.  Some of the code here isn't used in GCM.  Another version of this with additional code is available at https://github.com/philipmcg/csharp-utilities.


6/29/2014
Philip McGarvey (Garnier)
www.philipmcg.com
www.sowmp.com
