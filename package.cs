//package.cs

if(ispackage(tmbi))
	deactivatepackage(tmbi);

//these functions need to be completely overwritten
//they also need to be called AFTER all other packages
function scrollbricks(%x)
{
	tmbi_debug("scrollbricks [" @ %x @ "] [" @ $tmbi_vertical @"]");

	if(%x == 0)
	{
		tmbi_debug("Does this even happen?");
		return;
	}

	if($tmbi_vertical || ($tmbi_control && !$tmbi_control_override))
	{
		if(iseventpending($tmbi::verticalspamloop))
			cancel($tmbi::verticalspamloop);

		if(%x < 0)
		{
			tmbi_debug("up");
			$tmbi::verticalspamloop = schedule(500, 0, tmbi_verticalspamloop, 1);
			tmbi_up();
		}
		else
		{
			tmbi_debug("down");
			$tmbi::verticalspamloop = schedule(500, 0, tmbi_verticalspamloop, 0);
			tmbi_down();
		}
	}
	else
	{
		if(iseventpending($tmbi::horizontalspamloop))
			cancel($tmbi::horizontalspamloop);

		if($pref::input::reversebrickscroll && !$tmbi_vertical && !$tmbi_horizontal) //thank ye muzzles
			%x*=-1;

		if(%x > 0)
		{
			tmbi_debug("right");

			if($tmbi_horizontal)
				$tmbi::horizontalspamloop = schedule(500, 0, tmbi_horizontalspamloop, 1);
			tmbi_right();
		}
		else
		{
			tmbi_debug("left");

			if($tmbi_horizontal)
				$tmbi::horizontalspamloop = schedule(500, 0, tmbi_horizontalspamloop, 0);
			tmbi_left();
		}
	}
}

function PlayGui::hideBrickBox(%this, %a, %b, %c)
{
	if(%a > 0 && $tmbi::open || %a < 0 && !$tmbi::open)
	{
		tmbi_toggle();
	}
	else
	{
		tmbi_debug("bad toggle attempted");
	}
}

function BSD_ClickFav(%x)
{
	tmbi_clickfav(%x);
}

function BSD_ClickIcon(%x)
{
	tmbi_selectbrick(%x);
}

function BSD_RightClickIcon(%x)
{
	if(BrickSelectorDlg.isawake()) //else is witchcraft
	{
		$tmbi_instantusing = 1;
		canvas.popdialog(BrickSelectorDlg);
		$tmbi_instantusing = 0;
	}
	commandtoserver('instantusebrick', %x);
}

function BSD_ClickClear() //replaced
{
	tmbi_clearcart();
}

function openbsd(%x)
{
	if(%x)
		tmbi_openbsd();
}

package tmbi
{
	//added functionality
	function invup(%x)
	{
		$tmbi_vertical = true;
		parent::invup(%x);
		$tmbi_vertical = false;

		if(!%x && iseventpending($tmbi::verticalspamloop))
			cancel($tmbi::verticalspamloop);
	}

	function invdown(%x)
	{
		$tmbi_vertical = true;
		parent::invdown(%x);
		$tmbi_vertical = false;

		if(!%x && iseventpending($tmbi::verticalspamloop))
			cancel($tmbi::verticalspamloop);
	}

	function invleft(%x)
	{
		$tmbi_horizontal = true;
		parent::invleft(%x);
		$tmbi_horizontal = false;

		if(!%x && iseventpending($tmbi::horizontalspamloop))
			cancel($tmbi::horizontalspamloop);
	}

	function invright(%x)
	{
		$tmbi_horizontal = true;
		parent::invright(%x);
		$tmbi_horizontal = false;

		if(!%x && iseventpending($tmbi::horizontalspamloop))
			cancel($tmbi::horizontalspamloop);
	}

	function handlesetinvdata(%WhoCares, %iSureDont, %slot, %dtb)
	{
		parent::handlesetinvdata(%WhoCares, %iSureDont, %slot, %dtb);

		if($tmbi::repurposekeybinds)
		{
			$tmbi::brick[getword($tmbi::current, 0), %slot] = %dtb;
			tmbi_update();
		}
		else if($tmbi::open && %slot == $currScrollBrickSlot)
		{
			tmbi_UINAME.settext("<color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":16><just:center>"@ %dtb.uiname);
		}
	}

	//I really want to just overwrite this function
	//However, I will attempt to play nicely
	//The fact that it calls the server command to use the inventory slot is an issue
	function directSelectInv(%slot)
	{
		tmbi_debug("SLOT BEFORE: " @ $currscrollbrickslot);
		parent::directSelectInv(%slot);
		tmbi_debug("SLOT AFTER: " @ $currscrollbrickslot);
		//if(!$tmbi::open || getword($tmbi::current, 1) $= %slot)
		//	tmbi_toggle();

		if($tmbi::open) // && $tmbi::brick[getword($tmbi::current, 0), %slot] !$= "")
		{
			tmbi_setcurrent(getword($tmbi::current, 0) SPC $currscrollbrickslot);
		}
	}

	function disconnectedCleanup(%x)
	{
		$tmbi::rebindrejected = 0;
		$tmbi::repurposekeybinds = 0;
		return parent::disconnectedCleanup(%x);
	}

	function OptionsDlg::onsleep(%gui, %idk)
	{
		parent::onsleep(%gui, %idk);
		tmbi_update(); //may have changed gui options, update takes care of that
	}

	function BSD_ShowTab(%x)
	{
		if($tmbi::selshopbrick !$= "")
			$BSD_activeBitmap[$tmbi::selshopbrick].setvisible(0);
		$tmbi::selshopbrick = "";
		parent::BSD_ShowTab(%x);
	}

	//all of this is to setup and maintain the visual replacement
	function brickselectordlg::onwake(%this, %idk)
	{
		//nothing yet
		parent::onwake(%this, %idk);
		tmbi_firsttimeinit();

		if(getword(BSD_Window.getextent(), 1) > getword(playgui.getextent(), 1))
		{
			tmbi_debug("oh dear, your pitifully small screen can't handle the awesome.");
			BSD_Window.resize(getword(BSD_Window.getposition(),0), 0, 640, getword(playgui.getextent(), 1));
		}

		if($tmbi_closepos !$= "")
		{
			tmbi_Debug("Last close position at: "@ $tmbi_closepos);
			BSD_Window.resize(getword($tmbi_closepos, 0), getword($tmbi_closepos, 1), 640, getword(BSD_Window.getextent(), 1));
		}

		if(getword(BSD_Window.getposition(), 1) < 0)
		{
			tmbi_debug("repositioning ...");
			BSD_Window.resize(getword(BSD_Window.getposition(),0), 0, 640, getword(BSD_Window.getextent(), 1));
		}
		TMBI_MainInventory.setvisible(0);
		BSD_Window.pushtoback(tmbi_resizer); //this keeps moving around for some reason
		commandtoserver('bsd');
	}

	function brickselectordlg::onsleep(%this, %idk)
	{		
		if(BSD_FavsHelper.visible) //I am surprised that I bother to put stuff like this in
			BSD_SetFavs();
		$tmbi_closepos = BSD_Window.getposition();
		parent::onsleep(%this, %idk);
		TMBI_MainInventory.setvisible(1);

		HUD_BrickNameBG.setvisible(0); //because deleting is mean
		HUD_BrickBox.setvisible(0);

		tmbi_update();

		if($tmbi::wasopen)
		{
			$tmbi::wasopen = false;
			tmbi_toggle();
		}
	}

	function playgui::onrender(%gui)
	{
		parent::onrender(%gui);

		if(serverconnection.tmbi_setup) //make sure it isn't just a silly gui refresh
		{
			//these things are like little gremlins that keep coming back aaaargh
			HUD_BrickNameBG.setvisible(0); //because deleting is mean
			HUD_BrickBox.setvisible(0);
			return;
		}
		serverconnection.tmbi_setup = true;
		createuinametable(); //someone hates me
		tmbi_clickfav(1);
		tmbi_setinventorydata();
		tmbi_update();

		for(%i=0; %i<10; %i++)
		{
			//$InvData[%i] = $tmbi::brick[0, %i];
			commandtoserver('buybrick', %i, $tmbi::brick[0, %i]);
		}
	}

	function playgui::createinvhud(%this)
	{
		%r = parent::createinvhud(%this);
		tmbi_activateinventory();
		return %r;
	}

	function TMBI_GUI_1::onwake(%gui, %idk)
	{
		tmbi_options_cancel();
		return parent::onwake(%gui, %idk);
	}

	//support for macros
	function ToggleBuildMacroRecording(%x)
	{
		parent::ToggleBuildMacroRecording(%x);

		if(%x && $recordingbuildmacro && !$tmbi::repurposekeybinds)
		{
			$BuildMacroSO.event[$BuildMacroSO.numEvents] = "Server\tInstantUseBrick\t"@ $tmbi::brick[getword($tmbi::current,0), getword($tmbi::current,1)]; //yay compatibility
			$BuildMacroSO.numEvents++;
		}
	}

	function LoadMacroFromFile()
	{
		if(isobject($buildmacroso))
			%id = $buildmacroso.getid();
		//$tmbi_macroloadbypass = 1;
		parent::loadmacrofromfile();
		//$tmbi_macroloadbypass = 0;

		if(!isobject($buildmacroso))
			return;

		if($buildmacroso.getid() == %id || $tmbi::repurposekeybinds)
			return;

		for(%a=0; %a<$buildmacroso.numevents; %a++)
		{
			if(getfield($buildmacroso.event[%a], 1) $= "instantusebrick")
			{
				%field = getfield($buildmacroso.event[%a], 2);

				if(isobject($uinametable[%field]))
				{
					$buildmacroso.event[%a] = tmbi_setfield($buildmacroso.event[%a], 2, $uinametable[%field]);
					continue;
				}
				else if(isobject(%field)) //maybe they did it without tmbi or something I guess
					continue;
				else
				{
					for(%b=%a; %b<$buildmacroso.numevents; %b++)
						$buildmacroso.event[%b] = $buildmacroso.event[%b+1];
					$buildmacroso.numevents--;
				}
			}

			if(getfield($buildmacroso.event[%a], 1) $= "UseInventory" && !$tmbi::repurposekeybinds && !%q)
			{
				messageboxok("Attention!", "The macro that you have loaded may not be fully functional in your current build mode.\n\nPlease go to TMBI options in the brick selector and enable Vanilla Building Mode for best results.");
				%q = 1;
			}
		}
	}

	function SaveMacroToFile(%conf) //meh, couldn't think of a better way
	{
		if($tmbi::repurposekeybinds)
		{
			parent::savemacrotofile(%conf);
		}
		else
		{
			for(%a=0; %a<$buildmacroso.numevents; %a++)
			{
				if(getfield($buildmacroso.event[%a], 1) $= "instantusebrick")
				{
					%temparray[%a] = getfield($buildmacroso.event[%a], 2);
					$buildmacroso.event[%a] = tmbi_setfield($buildmacroso.event[%a], 2, %temparray[%a].uiname);
				}
			}
			parent::savemacrotofile(%conf);

			for(%a=0; %a<$buildmacroso.numevents; %a++)
			{
				if(getfield($buildmacroso.event[%a], 1) $= "instantusebrick")
					$buildmacroso.event[%a] = tmbi_setfield($buildmacroso.event[%a], 2, %temparray[%a]);
			}
		}
	}
};
activatepackage(tmbi);
