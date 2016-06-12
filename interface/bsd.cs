//bsd.cs

//click a brick in the shop
function tmbi_selectbrick(%x)
{
	if($tmbi::selshopbrick !$= "")
		$BSD_activeBitmap[$tmbi::selshopbrick].setvisible(0);

	if($tmbi::selinvbrick !$= "")
	{
		$tmbi::selbrick[getword($tmbi::selinvbrick,0), getword($tmbi::selinvbrick,1)] = %x;
		$tmbi::selinvbrick = "";
		$tmbi::selshopbrick = ""; //just to be safe or something I dunno
		$tmbi::lastselshopbrick = "";
	}
	else
	{
		if($tmbi::lastselshopbrick == %x && getsimtime() - $tmbi::selshopclicktime < 500)
		{
			//double click
			//force brick into inventory

			for(%a=0; %a<$tmbi::selrowcount; %a++) //hobo moving into an empty box
			{
				if(!$tmbi::selhiddenrow[%a])
				{
					for(%b=0; %b<10; %b++)
					{
						if($tmbi::selbrick[%a, %b] $= "")
						{
							%found = true;
							break;
						}
					}

					if(%found)
						break; //I wish torque had a double break thingy
				}
			}

			if(!%found) //shove them out of the way like a boss
			{
				%firstbrick = true;

				for(%a=$tmbi::selrowcount-1; %a>=0; %a--)
				{
					if(!$tmbi::selhiddenrow[%a])
					{
						for(%b=9; %b>=0; %b--)
						{
							if(%firstbrick)
							{
								%lastbrick = $tmbi::selbrick[%a, %b];
								%firstbrick = false;
							}
							else
							{
								%thisbrick = $tmbi::selbrick[%a, %b];
								$tmbi::selbrick[%a, %b] = %lastbrick;
								%lastbrick = %thisbrick;
							}
						}
					}
				}
				%a = $tmbi::selrowcount-1;
				%b = 9;
			}
			$tmbi::selbrick[%a, %b] = %x;
			$tmbi::selshopbrick = "";
			$tmbi::lastselshopbrick = "";
		}
		else //togglol
		{
			if($tmbi::selshopbrick == %x)
				$tmbi::selshopbrick = "";
			else
				$tmbi::selshopbrick = %x;
			$tmbi::lastselshopbrick = %x;
		}
	}
	$tmbi::selshopclicktime = getsimtime();
	tmbi_updatesel();
}

function tmbi_selectinv(%row, %x)
{
	if($tmbi::selshopbrick !$= "")
		$BSD_activeBitmap[$tmbi::selshopbrick].setvisible(0);

	if($tmbi::selshopbrick)
	{
		$tmbi::selbrick[%row, %x] = $tmbi::selshopbrick;
		$tmbi::selshopbrick = ""; //just to be safe
		$tmbi::selinvbrick = "";
		%val = 1;
	}
	else
	{
		if($tmbi::lastselinvbrick $= (%row SPC %x) && getsimtime() - $tmbi::selinvclicktime < 500) //those parentheses are really gay imo
		{
			$tmbi::selbrick[%row, %x] = "";
			$tmbi::selinvbrick = "";
			%val = 1;
		}
		else
		{
			if($tmbi::selinvbrick !$= "")
			{
				if($tmbi::selinvbrick $= (%row SPC %x))
					$tmbi::selinvbrick = "";
				else
				{
					if($tmbi::selbrick[getword($tmbi::selinvbrick,0), getword($tmbi::selinvbrick,1)] == $tmbi::selbrick[%row, %x])
						$tmbi::selinvbrick = %row SPC %x;
					else
					{
						%temp = $tmbi::selbrick[%row, %x];
						$tmbi::selbrick[%row, %x] = $tmbi::selbrick[getword($tmbi::selinvbrick,0), getword($tmbi::selinvbrick,1)];
						$tmbi::selbrick[getword($tmbi::selinvbrick,0), getword($tmbi::selinvbrick,1)] = %temp;
						$tmbi::selinvbrick = "";
						%val = 1;
					}
				}
			}
			else
				$tmbi::selinvbrick = %row SPC %x;
		}
	}
	if(%val)
		$tmbi::lastselinvbrick = "";
	else
		$tmbi::lastselinvbrick = %row SPC %x;
	$tmbi::selinvclicktime = getsimtime();
	tmbi_updatesel();
}

function tmbi_addrow() //looks good
{
	if(!tmbi_isint(%x = $tmbi::pref::maxrowcount))
		%x = 5;
	if($tmbi::selrowcount >= %x)
		return;
	$tmbi::selrowcount++;
	tmbi_updatesel();
}

function tmbi_removerow(%x) //looks good
{
	if($tmbi::selrowcount == 1)
		return;

	for(%a=%x; %a<$tmbi::selrowcount; %a++)
	{
		$tmbi::selhiddenrow[%a] = $tmbi::selhiddenrow[%a+1];

		for(%b=0; %b<10; %b++)
			$tmbi::selbrick[%a, %b] = $tmbi::selbrick[%a+1, %b];
	}
	$tmbi::selrowcount--;
	tmbi_updatesel();
}

function tmbi_hiderow(%x) //looks good
{
	$tmbi::selhiddenrow[%x] = !$tmbi::selhiddenrow[%x];
	("TMBI_SELROWHIDDEN_"@ %x).setvisible($tmbi::selhiddenrow[%x]); //I reversed hiddenrow and rowhidden just to confuse myself for shits and giggles
	for(%a=0; %a<10; %a++)
	{
		if($tmbi::selinvbrick $= (%x SPC %a))
			$tmbi::selinvbrick = "";
	}
}

function tmbi_clearcart() //looks good
{
	for(%a=0; %a<$tmbi::selrowcount; %a++)
	{
		$tmbi::selhiddenrow[%a] = "";
		tmbi_clearrow(%a);
	}
	$tmbi::selrowcount = 1;
	tmbi_updatesel();
}

function tmbi_clearrow(%x) //looks good
{
	for(%a=0; %a<10; %a++)
	{
		$tmbi::selbrick[%x, %a] = "";

		if($tmbi::selinvbrick $= (%x SPC %a))
			$tmbi::selinvbrick = "";
	}
	tmbi_updatesel();
}

function tmbi_clickfav(%x) //SHIT UI NAME VERSUS DATABLOCK
{
	if(BSD_FavsHelper.visible)
	{
		//play nice
		exec("config/client/Favorites.cs");

		for(%a=0; %a<10; %a++)
			$Favorite::Brick[%x, %a] = $tmbi::selbrick0_[%a].uiname;
		export("$Favorite::Brick*", "config/client/Favorites.cs");

		//now for the real fun
		if(isfile(%path = "config/client/TMBI/favs.cs"))
			exec(%path);

		for(%a=0; %a<$tmbi::favorite::rowcount[%x]; %a++) //good practice
		{
			for(%b=0; %b<10; %b++)
				$tmbi::favorite::brick[%x, %a, %b] = "";
			$tmbi::favorite::hiddenrow[%a] = "";
		}

		for(%a=0; %a<$tmbi::selrowcount; %a++)
		{
			$tmbi::favorite::hiddenrow[%x, %a] = $tmbi::selhiddenrow[%a];

			for(%b=0; %b<10; %b++)
			{
				if(($tmbi::favorite::brick[%x, %a, %b] = $tmbi::selbrick[%a, %b].uiname) !$= "")
					%gat = true;
			}
		}
		$tmbi::favorite::rowcount[%x] = $tmbi::selrowcount;
		export("$tmbi::favorite::*", "config/client/TMBI/favs.cs");

		if(!%gat) //have a better way? stick it up your butt
			(BSD_FavButton @ %x).mcolor = "255 255 255 128";
		else
			(BSD_FavButton @ %x).mcolor = "255 255 255 255";
		BSD_SetFavs();
	}
	else
	{
		tmbi_clearcart();
		$tmbi::favorite::rowcount[%x] = "";

		if(isfile("config/client/TMBI/favs.cs"))
			exec("config/client/TMBI/favs.cs");

		if($tmbi::favorite::rowcount[%x])
		{
			$tmbi::selrowcount = $tmbi::favorite::rowcount[%x];

			for(%a=0; %a<$tmbi::selrowcount; %a++)
			{
				$tmbi::selhiddenrow[%a] = $tmbi::favorite::hiddenrow[%x, %a];

				for(%b=0; %b<10; %b++)
					$tmbi::selbrick[%a, %b] = $uinametable[$tmbi::favorite::brick[%x, %a, %b]];
			}
		}
		else
		{
			//in case of pfft
			exec("config/client/Favorites.cs");

			for(%a=0; %a<10; %a++)
				$tmbi::selbrick0_[%a] = $uinametable[$Favorite::Brick[%x, %a]];
			$tmbi::selrowcount = 1;
		}
		tmbi_updatesel();
	}
}

function tmbi_setinventorydata()
{
	for(%a=0; %a<$tmbi::rowcount; %a++) //housekeeping
	{
		for(%b=0; %b<10; %b++)
			$tmbi::brick[%a, %b] = "";
		$tmbi::hiddenrow[%a] = "";
	}

	for(%a=0; %a<$tmbi::selrowcount; %a++)
	{
		for(%b=0; %b<10; %b++)
			$tmbi::brick[%a, %b] = $tmbi::selbrick[%a, %b];

		if($tmbi::repurposekeybinds)
		{
			if(%a == 0)
			{
				for(%b=0; %b<10; %b++)
					commandtoserver('buybrick', %b, $tmbi::brick[0, %b]);
				%x = 0;
			}
			else
				%x = 1;
			$tmbi::hiddenrow[%a] = %x;
		}
		else
			$tmbi::hiddenrow[%a] = $tmbi::selhiddenrow[%a];
	}
	$tmbi::rowcount = $tmbi::selrowcount;
}

function tmbi_clickdone() //looks good
{
	tmbi_setinventorydata();

	if(!tmbi_gotbricks())
	{
		tmbi_debug("setting wasopen to false");
		$tmbi::wasopen = false;
	}
	canvas.popdialog(BrickSelectorDlg);

	//probably should buy row of bricks here
	tmbi_debug("buying current row");

	for(%i=0; %i<10; %i++)
	{
		if($tmbi::repurposekeybinds)
			commandtoserver('buybrick', %i, $tmbi::brick[getword($tmbi::current,0), %i]);
		if($tmbi::brick[getword($tmbi::current,0), %i] $= "")
			$InvData[%i] = -1; //unfortunately I picked up on this too late
		else
			$InvData[%i] = $tmbi::brick[getword($tmbi::current,0), %i];
	}
}

function tmbi_closebsd()
{
	canvas.popdialog(BrickSelectorDlg);
}

function tmbi_openbsd() //NEEDS TO RETURN WHEN LOADING
{
	if(!playgui.isawake())
		return;

	if(!brickselectordlg.isawake())
	{
		if($BuildingDisabled)
		{
			clientcmdcenterprint("\c6Building is currently disabled.", 4);
			return;
		}

		if($tmbi::open)
		{
			$tmbi::wasopen = true;
			tmbi_toggle();
		}

		if($tmbi::selshopbrick !$= "")
		{
			$BSD_activeBitmap[$tmbi::selshopbrick].setvisible(0);
			$tmbi::selshopbrick = "";
		}
		$tmbi::selinvbrick = "";
		canvas.pushdialog(brickselectordlg);
	}
	else
		tmbi_clickdone();
}

function tmbi_clickcancel()
{
	canvas.popdialog(BrickSelectorDlg);
}

//clicking the button
function tmbi_reset()
{
	tmbi_clearcart();
	$tmbi::selrowcount = $tmbi::rowcount;

	for(%a=0; %a<$tmbi::rowcount; %a++)
	{
		$tmbi::selhiddenrow[%a] = $tmbi::hiddenrow[%a];

		for(%b=0; %b<10; %b++)
			$tmbi::selbrick[%a, %b] = $tmbi::brick[%a, %b];
	}

	if(!$tmbi::selrowcount)
		$tmbi::selrowcount = 1;
	tmbi_updatesel();
}

//the main script to interface system, quite inefficient, but extremely versatile
function tmbi_updatesel()
{
	if(!isobject(tmbi_sellist))
		return;

//	if(isobject(TMBI_Resizer))
//	{
//		$tmbi::resizerpos = TMBI_Resizer.getposition();
//		TMBI_Resizer.delete();
//	}
	tmbi_sellist.clear();

	if(!$tmbi::selrowcount)
		%var = 0;
	else
		%var = 63*$tmbi::selrowcount - 2;

	if(%var != getword(tmbi_sellist.getextent(), 1))
		tmbi_sellist.resize(1, getword(tmbi_sellist.getposition(), 1), 617, %var);

	for(%a=0; %a<$tmbi::selrowcount; %a++)
	{
		%row = tmbi_generatenewselrow(%a);
		//do anything at all?

		tmbi_sellist.add(%row);
	}

	if($tmbi::selshopbrick !$= "")
		$BSD_activeBitmap[$tmbi::selshopbrick].setvisible(1);
	TMBI_TEXTDISPLAY.settext($tmbi::selbrick[getword($tmbi::selinvbrick,0), getword($tmbi::selinvbrick,1)].uiname);

	//resizer went here

	for(%i=1; %i<=7; %i++)
		eval("tmbi_btn"@%i@".setcolor(\""@$tmbi::pref::color0@"\");"); //bleh
}
