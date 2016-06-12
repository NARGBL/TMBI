//help.cs
exec("./TMBI_GUI2.gui");

function tmbi_help(%x)
{
	if(%x == 1) //font
		TMBI_Options_ExText.settext("<just:center><font:"@ TMBI_Options_val1.getvalue() @":18>Example Text");
	else if(%x == 2) //height
		messageboxok("TMBI Info", "The maximum height is the limit on how many pixels tall the brick box can be on your HUD.");
	else if(%x == 3) //trans weight
		messageboxok("TMBI Info", "Some spraycan colors are transparent, but transparent brick icons are hard to see.  The lower this number, the less transparent brick icons can be.");
	else if(%x == 4) //transition
		messageboxok("TMBI Info", "When the inventory can't fit all the rows into the brickbox, this determines whether a fancy little fading transition appears or if it just cuts off.");
	else if(%x == 5) //repurpose
		messageboxok("TMBI Info", "On rare occasions, a server will use the inventory keybinds (0-9) to do something other than building.  TMBI overwrites these commands by default, so enabling this will turn it back on.\n\nBasically, it turns back into the same thing as the old brick inventory.");
	else if(%x == 6) //autochecker
		messageboxok("TMBI Info", "It is nice to have the latest stuff, and if you let it, TMBI will go online and check to see if a new version is available for you.  It will not download or install anything.");
	else if(%x == 7) //scroll
		messageboxok("TMBI Info", "Uxie likes to complain about how scrolling off the edge of a row jumps down to the next one, which is apparently confusing.  This pref toggles that, and makes it so scroll doesn't jump over empty slots and search for the nearest filled one.");
	else if(%x == 8) //rowcount
		messageboxok("TMBI Info", "Too many rows is hard to navigate and can cause the selector to lag when you move it around.\n\nIt is reccommended to keep this number reasonable, but I can't stop you from setting it to 9001 and adding every brick in the selector five times.");
	else if(%x == 9)
		messageboxok("TMBI Info", "This makes it so that mouse1 does the same thing as pressing the ghost mover keybind.");
	else if(%x == 10)
		messageboxok("TMBI Info", "Does the server have a bot detector that is hitting your ghost brick mover?  Check this box!  It may be a little slower, but it will still get the job done.");
	else //generic help
	{
		canvas.pushdialog(TMBI_GUI2);
		tmbi_helpbox.settext("<font:palatino linotype:18>Click a tab above for relevant information");
	}
}

function tmbi_helpabout()
{
	tmbi_helpbox.settext("<font:palatino linotype:18>The Too Many Bricks inventory mod is developed by Nexus BL_ID 4833, who is the co-founder of NARG.\n\nYou should visit the NARG forum page in the add-ons section of forum.blockland.us to learn more about the other mods that Nexus and others have developed for the community.\n\nTMBI also includes the Brick Scrolling mod developed by Hata.");
}

function tmbi_helpinterface()
{
	tmbi_helpbox.settext("<font:palatino linotype:18>TMBI does its best to make building more convenient and as intuitive as possible.\n\nYou can use the mousewheel to scroll sideways through rows and hold control while scrolling to go up and down.\n\nDouble tapping and holding control and then scrolling will let you cycle through similar bricks to the one you have equipped.\n\nIn the brick selector, you can now resize both the selector itself, and the inventory scroll box.\n\nTMBI comes with a ghost brick mover you can bind in control options or TMBI Options.");
}

function tmbi_helptips()
{
	tmbi_helpbox.settext("<font:palatino linotype:18>You can keep a row of bricks in your selector without it showing up while building by selecting the hide option next to it.\n\nYou should also poke through the options for interface preferences.\n\nIf you want to load a macro recorded before getting TMBI, you may need to turn on the vanilla building mode in options.");
}

function tmbi_helpwarnings()
{
	tmbi_helpbox.settext("<font:palatino linotype:18>DO NOT TRACE CONSOLE WHILE TMBI IS VERSION CHECKING!  For some reason, this can cause you to crash.  If you must trace while TMBI opens for the first time, you can disable auto version checking in options.\n\nALSO: TMBI disables server dependencies for using the 0-9 keys.  You can re-enable these dependencies in the options if the server requires it, but you will not be able to use multiple rows.");
}
