//overlay.cs

function tmbi_toggle()
{
	if($BuildingDisabled)
	{
		if(!$tmbi::repurposekeybinds)
			clientcmdcenterprint("\c6Building is currently disabled.", 4);

		if(!$tmbi::open)
			return;
	}

	if($tmbi::open)
	{
		commandtoserver('unusetool');
		$tmbi::open = 0;
//		tmbi_deactivatekeybind();
	}
	else
	{
		if(!tmbi_gotbricks())
		{
			if(!$tmbi::repurposekeybinds)
				clientcmdcenterprint("\c6You have no bricks in your inventory.", 2);
			return;
		}
//		setscrollmode($SCROLLMODE_BRICKS);
//		tmbi_activatekeybind();
		$tmbi::open = 1;
//		$tmbi::scrollblockers = "";
	}
	tmbi_update();
}

function clientcmdrepurposekeybinds(%x)
{
	tmbi_Debug("server wants to repurpose keybinds");

	if($tmbi::rebindrejected)
		return;
	$tmbi::repurposekeybinds = %x;

	if($tmbi::repurposekeybinds)
		messageboxyesno("TMBI: Attention!", $tmbi::rebindmsg, "tmbi_update();", "$tmbi::repurposekeybinds=0;$tmbi::rebindrejected=1;tmbi_update();");
}

//going to the right in the HUD inventory.  Only used when 'Uxie' pref is disabled
function tmbi_right() //looks good
{
	if(!$tmbi::open)
	{
		if(!tmbi_gotbricks())
		{
			clientcmdcenterprint("\c6You have no bricks in your inventory.", 2);
			return;
		}
		tmbi_toggle();
	}
	tmbi_debug("passed open test");
	%a = getword($tmbi::current, 0);
	%b = getword($tmbi::current, 1);

	if(!tmbi_isint(%a))
		%a = 0;
	if(!tmbi_isint(%b))
		%b = 0;
	$tmbi::current = %a SPC %b;
	tmbi_debug("current: " @ $tmbi::current);
	%init = true;

	while($tmbi::brick[%a, %b] $= "" || %init)
	{
		tmbi_debug("entering main while block: "@%b);

		if(%init)
			%init = false;
		%b++;

		if(%b > 9)
		{
			%b = 0;

			if($tmbi::pref::nextscroll)
			{
				%safe = %a;
				%initb = true;

				while($tmbi::hiddenrow[%a] || %initb)
				{
					tmbi_debug("entering secondary while block: "@%a);
					if(%initb)
						%initb = false;
					%a++;

					if(%a >= $tmbi::rowcount)
						%a = 0;

					//shouldn't be needed, but I like retard-proofing
					if(%a == %safe)
						break;
				}
			}
		}

		if((%a SPC %b) $= $tmbi::current)
			return;

		if(!$tmbi::pref::nextscroll)
		{
			tmbi_debug("uxie is mad, bailing...");
			break;
		}
	}
	tmbi_setcurrent(%a SPC %b);
	//$tmbi::current = %a SPC %b;
	tmbi_debug("exiting: "@ $tmbi::current);
	//tmbi_update();
}

function tmbi_left() //looks good
{
	if(!$tmbi::open)
	{
		if(!tmbi_gotbricks())
		{
			clientcmdcenterprint("\c6You have no bricks in your inventory.", 2);
			return;
		}
		tmbi_toggle();
	}
	tmbi_debug("passed open test");
	%a = getword($tmbi::current, 0);
	%b = getword($tmbi::current, 1);

	if(!tmbi_isint(%a))
		%a = 0;
	if(!tmbi_isint(%b))
		%b = 0;
	$tmbi::current = %a SPC %b;
	tmbi_debug("current: " @ $tmbi::current);
	%init = true;

	while($tmbi::brick[%a, %b] $= "" || %init)
	{
		tmbi_debug("entering main while block: "@%b);
		if(%init)
			%init = false;
		%b--;

		if(%b < 0)
		{
			%b = 9;

			if($tmbi::pref::nextscroll)
			{
				%safe = %a;
				%initb = true;

				while($tmbi::hiddenrow[%a] || %initb)
				{
					tmbi_debug("entering secondary while block: "@%a);
					if(%initb)
						%initb = false;
					%a--;

					if(%a < 0)
						%a = $tmbi::rowcount-1;

					//shouldn't be needed, but I like retard-proofing
					if(%a == %safe)
						break;
				}
			}
		}

		if((%a SPC %b) $= $tmbi::current)
			return;

		if(!$tmbi::pref::nextscroll)
		{
			tmbi_debug("uxie is mad, bailing...");
			break;
		}
	}
	tmbi_setcurrent(%a SPC %b);
	//$tmbi::current = %a SPC %b;
	tmbi_debug("exiting: "@ $tmbi::current);
	//tmbi_update();
}

function tmbi_down()
{
	if(!$tmbi::open)
	{
		if(!tmbi_gotbricks())
		{
			clientcmdcenterprint("\c6You have no bricks in your inventory.", 2);
			return;
		}
		tmbi_toggle();
	}
	tmbi_debug("passed open test");
	%a = getword($tmbi::current, 0);
	%b = getword($tmbi::current, 1);

	if(!tmbi_isint(%a))
		%a = 0;
	if(!tmbi_isint(%b))
		%b = 0;
	$tmbi::current = %a SPC %b;
	tmbi_debug("current: " @ $tmbi::current);
	%bypass = true;
	%bypassb = true;
	%safe = %a;
	%c = 0;
	%d = %b;

	while($tmbi::brick[%a, %b] $= "" || %bypass)
	{
		tmbi_debug("entering main loop: "@ %b SPC %c);

		if(%bypass)
			%bypass = false;
		else
		{
			//this looks sketchy, basically it is alternating looking left and right to find nearest brick
			%c*=-1;

			if(%c >= 0)
				%c++; //alternating magic

			if(mabs(%c) > 10)
			{
				%c = 0;
				%bypassb = true;
				%b = %d;
			}
			else
				%b = %d + %c;
		}

		while($tmbi::hiddenrow[%a] || %bypassb)
		{
			tmbi_debug("entering secondary loop: "@ %a);

			if(%bypassb)
				%bypassb = false;
			%a++; //0 is top

			if(%a >= $tmbi::rowcount)
				%a = 0;

			if(%a == %safe)
				break;
		}

		if((%a SPC %b) $= $tmbi::current)
		{
			tmbi_debug("well that didn't work...");
			tmbi_left();
			return;
		}

		if(!$tmbi::pref::nextscroll)
		{
			tmbi_debug("uxie is mad, bailing...");
			break;
		}
	}
	tmbi_setcurrent(%a SPC %b);
	//$tmbi::current = %a SPC %b;
	tmbi_debug("exiting: "@ $tmbi::current);
	//tmbi_update();
}

function tmbi_up()
{
	if(!$tmbi::open)
	{
		if(!tmbi_gotbricks())
		{
			clientcmdcenterprint("\c6You have no bricks in your inventory.", 2);
			return;
		}
		tmbi_toggle();
	}
	tmbi_debug("passed open test");
	%a = getword($tmbi::current, 0);
	%b = getword($tmbi::current, 1);

	if(!tmbi_isint(%a))
		%a = 0;
	if(!tmbi_isint(%b))
		%b = 0;
	$tmbi::current = %a SPC %b;
	tmbi_debug("current: " @ $tmbi::current);
	%bypass = true;
	%bypassb = true;
	%safe = %a;
	%c = 0;
	%d = %b;

	while($tmbi::brick[%a, %b] $= "" || %bypass)
	{
		tmbi_debug("entering main loop: "@ %b SPC %c);

		if(%bypass)
			%bypass = false;
		else
		{
			%c*=-1;

			if(%c >= 0)
				%c++; //alternating magic

			if(mabs(%c) > 10)
			{
				%c = 0;
				%bypassb = true;
				%b = %d;
			}
			else
				%b = %d + %c;
		}

		while($tmbi::hiddenrow[%a] || %bypassb)
		{
			tmbi_debug("entering secondary loop: "@ %a);

			if(%bypassb)
				%bypassb = false;
			%a--; //0 is top

			if(%a < 0)
				%a = $tmbi::rowcount-1;

			if(%a == %safe)
				break;
		}

		if((%a SPC %b) $= $tmbi::current)
		{
			tmbi_debug("well that didn't work...");
			tmbi_right();
			return;
		}

		if(!$tmbi::pref::nextscroll)
		{
			tmbi_debug("uxie is mad, bailing...");
			break;
		}
	}
	tmbi_setcurrent(%a SPC %b);
	//$tmbi::current = %a SPC %b;
	tmbi_debug("exiting: "@ $tmbi::current);
	//tmbi_update();
}

function tmbi_usebrick(%row, %slot)
{
	if($tmbi::repurposekeybinds)
	{
		commandtoserver('useinventory', %slot);
	}
	else
		commandtoserver('instantusebrick', $tmbi::brick[%row, %slot]);

	if($RecordingBuildMacro)
	{
		if($tmbi::repurposekeybinds)
			$BuildMacroSO.event[$BuildMacroSO.numEvents] = "Server\tUseInventory\t"@ %slot;
		else
			$BuildMacroSO.event[$BuildMacroSO.numEvents] = "Server\tInstantUseBrick\t"@ $tmbi::brick[%row, %slot]; //yay compatibility
		$BuildMacroSO.numEvents++;
	}
}

//Bad to rely on this slow function too much, offload simpler updates to faster functions
//Completely rebuilds the inventory
function tmbi_update()
{
	tmbi_debug("updating...");
	if(!isobject(TMBI_MainInventory))
	{
		tmbi_activateinventory();
		return;
	}
	HUD_BrickNameBG.setvisible(0); //because deleting is mean
	HUD_BrickBox.setvisible(0);
	TMBI_MainInventory.setvisible(1);

	//first update
	if($tmbi::current $= "")
	{
		$tmbi::current = "0 0";
	}

	if($tmbi::open || (!$pref::HUD::HideBrickBox && tmbi_gotbricks()))
	{
		if(!isobject(tmbi_inv_list))
			return;
		tmbi_inv_list.clear();

		if(tmbi_gotbricks())
		{
			%b=0;

			for(%a=0; %a<$tmbi::rowcount; %a++)
			{
				if(!$tmbi::hiddenrow[%a])
				{
					%row = tmbi_generatenewinvrow(%a, %b);
					tmbi_inv_list.add(%row);

					if(%a == getword($tmbi::current, 0))
					{
						for(%c=0; %c<10; %c++)
							$BSD_InvData[%c] = $tmbi::brick[%a, %c];
						%current = %b;
					}
					%b++;
				}
			}

			if(%current $= "")
			{
				tmbi_debug("problem syncing inventory row, trying again...");
				tmbi_up();
				return;
			}

			if($tmbi::brick[getword($tmbi::current,0), getword($tmbi::current,1)] $= "")
			{
				if($tmbi::pref::nextscroll)
				{
					tmbi_debug("problem syncing inventory, trying again...");
					tmbi_right();
					return;
				}
			}
			//else
			//	$tmbi::currdtb = $tmbi::brick[getword($tmbi::current,0), getword($tmbi::current,1)];
			%invheight = %b*66 - 2;
			tmbi_transition.setvisible(0);

			if(%invheight+16 > $tmbi::pref::height)
			{
				if($tmbi::pref::fadeinventory)
					tmbi_transition.setvisible(1);
				%height = $tmbi::pref::height;
			}
			else
				%height = %invheight + 16;
			tmbi_debug(%invpos SPC %invheight SPC %height);
			%invpos = %height/2 - 66*%current - 40; //yay math
			tmbi_debug(%invpos SPC %invheight SPC %height);

			if(%invpos > 0)
				%invpos = 0;
			%invpos += 8*tmbi_transition.isvisible();
			tmbi_debug(%invpos SPC %invheight SPC %height);

			if(%invpos*-1 + %height - 16 > %invheight)
				%invpos = %height - %invheight - 16;
			tmbi_debug(%invpos SPC %invheight SPC %height);
			tmbi_scroll_area.resize(0, 16, 640, %height-16);
			tmbi_inv_list.resize(0, %invpos, 640, %invheight);
			tmbi_UINAME.settext("<color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":16><just:center>"@ $tmbi::brick[getword($tmbi::current,0), getword($tmbi::current,1)].uiname);

			if(!$tmbi_instantusing)
				tmbi_usebrick(getword($tmbi::current,0), getword($tmbi::current,1));
		}
		else
		{
			tmbi_toggle();
			return;
		}
	}
	else
	{
		if($pref::HUD::showToolTips)
			%height = 16;
		else
			%height = 0;
		tmbi_UINAME.settext("<color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":16><just:center>Too Many Bricks Inventory");
	}
	TMBI_MainInventory.resize((getword(playgui.getextent(),0)-640)/2, getword(playgui.getextent(),1)-%height, 640, %height);

	if(%height > 82)
		%btmpos = %height + 57;
	else
		%btmpos = 139;
	%btmpos = getword(playgui.getextent(), 1) - %btmpos;
	bottomPrintDlg.resize((getword(playgui.getextent(), 0)-619)/2, %btmpos, 619, 145);
	tmbi_barthing.setcolor($tmbi::pref::color2);
	tmbi_transition.setcolor($tmbi::pref::color2);
	tmbi_debug("completed update");
}

//this function does a number of things
//in addition to updating the data structures in place, it also updates some visuals
function tmbi_setcurrent(%new)
{
	%a = getword($tmbi::current, 0);
	%b = getword($tmbi::current, 1);

	if(!tmbi_isint(%a))
		%a = 0;
	if(!tmbi_isint(%b))
		%b = 0;
	$tmbi::current = %a SPC %b;

	%old = $tmbi::current;
	(TMBI_ACTIVESLOT_ @ getword(%old, 0) @ "_" @ getword(%old, 1)).setvisible(0);
	(TMBI_ACTIVESLOT_ @ getword(%new, 0) @ "_" @ getword(%new, 1)).setvisible(1);
	$tmbi::current = %new;
	$CurrScrollBrickSlot = getword(%new, 1);

	//check if row has changed
	%oldrow = getword(%old, 0);
	%newrow = getword(%new, 0);

	if(%oldrow !$= %newrow)
	{
		(TMBI_ROW_@ %oldrow).setcolor(getwords($tmbi::pref::color2, 0, 2) SPC getword($tmbi::pref::color2, 3)*(64/255));
		(TMBI_ROW_@ %newrow).setcolor(getwords($tmbi::pref::color2, 0, 1) SPC getword($tmbi::pref::color2, 2)+((1-getword($tmbi::pref::color2, 2))*0.3) SPC getword($tmbi::pref::color2, 3)*(96/255)); // 0 0 100 96

		for(%i=0; %i<10; %i++)
		{
			(TMBI_INVTEXT_ @ %oldrow @ "_" @ %i).setvisible(0);
			(TMBI_INVTEXT_ @ %newrow @ "_" @ %i).setvisible(1);

			//update data structure for compatibility reasons
			$InvData[%i] = $tmbi::brick[%newrow, %i];
			//commandtoserver('buybrick', %i, $tmbi::brick[%newrow, %i]);
		}
	}
	//necessary to count rows to see which ones are hidden
	%b = 0;

	for(%a=0; %a<$tmbi::rowcount; %a++)
	{
		if($tmbi::hiddenrow[%a])
			continue;
		%b++;
	}
	%invheight = %b*66 - 2;
	tmbi_transition.setvisible(0);

	if(%invheight+16 > $tmbi::pref::height)
	{
		if($tmbi::pref::fadeinventory)
			tmbi_transition.setvisible(1);
		%height = $tmbi::pref::height;
	}
	else
		%height = %invheight + 16;
	tmbi_debug(%invpos SPC %invheight SPC %height);
	%invpos = %height/2 - 66*getword($tmbi::current, 0) - 40; //yay math
	tmbi_debug(%invpos SPC %invheight SPC %height);

	if(%invpos > 0)
		%invpos = 0;
	%invpos += 8*tmbi_transition.isvisible();
	tmbi_debug(%invpos SPC %invheight SPC %height);

	if(%invpos*-1 + %height - 16 > %invheight)
		%invpos = %height - %invheight - 16;
	tmbi_debug(%invpos SPC %invheight SPC %height);
	tmbi_UINAME.settext("<color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":16><just:center>"@ $tmbi::brick[getword($tmbi::current,0), getword($tmbi::current,1)].uiname);
	tmbi_inv_list.resize(0, %invpos, 640, %invheight);

	tmbi_usebrick(getword($tmbi::current,0), getword($tmbi::current,1));
}

function tmbi_gotbricks()
{
	for(%a=0; %a<$tmbi::rowcount; %a++)
	{
		if(!$tmbi::hiddenrow[%a])
		{
			for(%b=0; %b<10; %b++)
			{
				if($tmbi::brick[%a, %b] !$= "")
					return true;
			}
		}
	}
	return false;
}
