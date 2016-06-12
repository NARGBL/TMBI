//TMBI System - Too Many Bricks Inventory System
//--------------------
//By Nexus 4833
//====================

$tmbi::version = "3.0 Beta";

function tmbi_debug(%msg)
{
	if($tmbi::debug)
		echo("DEBUG: "@ %msg);
}

if($tmbi_executed)
{
	if(isobject(TMBI_GUI1))
		TMBI_GUI1.delete();

	if(isobject(TMBI_GUI2))
		TMBI_GUI2.delete();

	if(isobject(tmbi_scrollbox))
		tmbi_scrollbox.delete();
	tmbi_debug("warning: potential problems in this thingy over in here.");
}
$tmbi_executed = 1;
exec("./support.cs");
exec("./versioncheck.cs");
exec("./package.cs");

exec("./interface/options.cs");
exec("./interface/help.cs");
exec("./interface/bsd.cs");
exec("./interface/overlay.cs");
exec("./interface/guibuilder.cs");
exec("./interface/resizer.cs");

exec("./interface/colorpicker.cs");

//other modules
exec("./modules/brickscrolling/brickscrolling.cs");
exec("./modules/ctrlscroll/ctrlscroll.cs");
exec("./modules/ghostmover/ghostmover.cs");

tmbi_defaultprefs();
$tmbi::rebindmsg = "This server uses your building keybinds for something other than building.  Your build mode has been changed to allow this (You can use the arrowkeys and mousewheel in your inventory).\n\nIf you want to change back, click no.";
$tmbi::debug = 0; //console spam galore

if(isfile("config/client/tmbi/prefs.cs"))
	exec("config/client/tmbi/prefs.cs");
tmbi_options_cancel();

if($tmbi_neo_bound)
	return;
$tmbi_neo_bound = true;
$neoName[$neoCount] = "TMBI Options";
$neoCmd[$neoCount] = "tmbi_options();";
$neoIcon[$neoCount] = "add-ons/client_tmbi/interface/ui/neoicon.png"; //defaults to a question mark otherwise
$neoPrefClose[$neoCount] = false; //whether we want to overlay to close when this button is pressed
$neoCount++;
