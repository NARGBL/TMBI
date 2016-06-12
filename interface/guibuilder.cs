//guibuilder.cs

//x and y are both row numbers, but y ignores hidden rows, used for rendering.
function tmbi_generatenewinvrow(%x, %y)
{
	if(%x == getword($tmbi::current, 0))
		%color = getwords($tmbi::pref::color2, 0, 1) SPC getword($tmbi::pref::color2, 2)+((1-getword($tmbi::pref::color2, 2))*0.3) SPC getword($tmbi::pref::color2, 3)*(96/255); // 0 0 100 96
	else
		%color = getwords($tmbi::pref::color2, 0, 2) SPC getword($tmbi::pref::color2, 3)*(64/255);
	%row = new GuiSwatchCtrl("TMBI_ROW_"@%x)
	{
		profile = "HUDBitmapProfile";
		horizSizing = "center";
		vertSizing = "bottom";
		position = "0" SPC 66*%y;
		extent = "640 64";
		minExtent = "8 2";
		visible = "1";
		color = "0 0 0 0";
	};
	%row.setcolor(%color); //so I don't need to do a dumb 255 conversion thing

	for(%a=0; %a<10; %a++)
	{
		%b = new GuiSwatchCtrl()
		{
			profile = "HUDBitmapProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = (2 + 64*%a) SPC "4";
			extent = "60 56";
			minExtent = "8 2";
			visible = "1";
			color = "0 0 0 0";
		};
		%b.setcolor(getwords($tmbi::pref::color2, 0, 2) SPC getword($tmbi::pref::color2, 3)*(64/255));
		%d = new GuiBitmapCtrl("TMBI_ACTIVESLOT_"@%x@"_"@%a)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 64*%a SPC "0";
			extent = "64 64";
			minExtent = "8 2";
			visible = $tmbi::current $= (%x SPC %a);
			bitmap = "base/client/ui/brickicons/brickIconActive";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "1";
		};

		if($tmbi::brick[%x, %a] $= "")
			%val = "";
		else if(!isobject($tmbi::brick[%x, %a]) || ($uinametable[$tmbi::brick[%x, %a].uiname] !$= $tmbi::brick[%x, %a])) //housekeeping
		{
			$tmbi:brick[%x, %a] = "";
			%val = "";
		}
		else if($tmbi::brick[%x, %a].iconname $= "")
			%val = "base/client/ui/brickIcons/Unknown";
		else
			%val = $tmbi::brick[%x, %a].iconname;
		%c = new GuiBitmapCtrl("TMBI_SLOT_"@%x@"_"@%a)
		{
			profile = "HUDBitmapProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 64*%a SPC "0";
			extent = "64 64";
			minExtent = "8 2";
			visible = "1";
			bitmap = %val;
		};

		if($pref::Hud::RecolorBrickIcons)
		{
			%color = getcoloridtable($currSprayCanIndex);
			%colorA = 1-((1-getword(%color, 3))*$tmbi::pref::transweight); //on a diet, weighs less
			%c.setcolor(getwords(%color, 0, 2) SPC %colorA);
		}

		if(%a == 9)
			%text = "0";
		else
			%text = %a+1;
		%e = new GuiMLTextCtrl("TMBI_INVTEXT_"@%x@"_"@%a)
		{
			profile = "GuiMLTextProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 64*%a SPC "2";
			extent = "20 18";
			minExtent = "8 2";
			visible = (getword($tmbi::current, 0) == %x);
			lineSpacing = "2";
			allowColorChars = "0";
			maxChars = "-1";
			text = "<just:center><color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":18>"@ %text;
			maxBitmapHeight = "-1";
			selectable = "1";
		};
		%row.add(%b);
		%row.add(%d);
		%row.add(%c);
		%row.add(%e);
	}
	return %row;
}

//builds row in the brick selector
function tmbi_generatenewselrow(%x) //Next row pos [0 63] compared to last one
{
	%row = new GuiSwatchCtrl("TMBI_SELROW_"@%x)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "0" SPC 63*%x;
		extent = "617 61";
		minExtent = "8 2";
		visible = "1";
		color = "130 130 230 255";
	};

	//each row gets 3 buttons
	for(%i=0; %i<3; %i++)
	{
		if(%i == 0)
			%text = "Hide";
		else if(%i == 1)
			%text = "Clear";
		else
			%text = "Remove";
		%gui = new GuiBitmapButtonCtrl()
		{
			profile = "BlockButtonProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "3" SPC 3 + 18*%i;
			extent = "48 18";
			minExtent = "8 2";
			visible = "1";
			command = "tmbi_"@ %text @"row("@ %x @");";
			accelerator = "";
			text = %text;
			groupNum = "-1";
			buttonType = "PushButton";
			bitmap = "base/client/ui/button1";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			mKeepCached = "0";
			mColor = "0 0 0 0";
		};
		%gui.setcolor($tmbi::pref::color0);
		%row.add(%gui);
	}
	%gui = new GuiSwatchCtrl()
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "54 3";
		extent = "559 55";
		minExtent = "8 2";
		visible = "1";
		color = "51 128 255 255";
	};

	//inv slots
	for(%i=0; %i<10; %i++)
	{
		%a = new GuiBitmapCtrl()
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 56*%i SPC "0";
			extent = "55 55";
			minExtent = "8 2";
			visible = "1";
			bitmap = "base/client/ui/brickicons/brickiconbg";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "1";
		};

		if($tmbi::selbrick[%x, %i] $= "")
			%val = "";
		else if(!isobject($tmbi::selbrick[%x, %i]) || ($uinametable[$tmbi::selbrick[%x, %i].uiname] != $tmbi::selbrick[%x, %i])) //housekeeping
		{
			$tmbi:selbrick[%x, %i] = "";
			%val = "";
		}
		else if($tmbi::selbrick[%x, %i].iconname $= "")
			%val = "base/client/ui/brickIcons/Unknown";
		else
			%val = $tmbi::selbrick[%x, %i].iconname;
		%b = new GuiBitmapCtrl("TMBI_SELICON_"@ %x @"_"@ %i)
		{
			profile = "HUDBitmapProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 56*%i SPC "0";
			extent = "55 55";
			minExtent = "8 2";
			visible = "1";
			bitmap = %val; //Datablock.iconname, default is base/client/ui/brickIcons/Unknown
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "1";
		};
		%c = new GuiBitmapCtrl("TMBI_SELACTIVE_"@ %x @"_"@ %i)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 56*%i SPC "0";
			extent = "55 55";
			minExtent = "8 2";
			visible = $tmbi::selinvbrick $= (%x SPC %i);
			bitmap = "base/client/ui/brickicons/brickIconActive";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "1";
		};
		%d = new GuiBitmapButtonCtrl()
		{
			profile = "BlockButtonProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = 56*%i SPC "0";
			extent = "55 55";
			minExtent = "8 2";
			visible = "1";
			command = "tmbi_selectinv("@ %x @","@ %i @");";
			text = " ";
			groupNum = "-1";
			buttonType = "PushButton";
			bitmap = "base/client/ui/brickicons/brickIconBtn";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			mKeepCached = "0";
			mColor = "255 255 255 255";
		};
		%gui.add(%a);
		%gui.add(%b);
		%gui.add(%c);
		%gui.add(%d);
	}
	%row.add(%gui);
	%gui = new GuiSwatchCtrl("TMBI_SELROWHIDDEN_"@ %x)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "54 3";
		extent = "559 55";
		minExtent = "8 2";
		visible = $tmbi::selhiddenrow[%x];
		color = "0 0 0 150";
	};
	%row.add(%gui);
	return %row;
}

//activation code
//should be called onceish
//this one builds the actual inventory seen on the playgui
function tmbi_activateinventory()
{
	if(isobject(TMBI_MainInventory))
	{
//		tmbi_update();
		return;
	}

	for(%i=0; %i<10; %i++)
	{
		if($tmbi::brick[0, %i] $= "")
			$InvData[%i] = -1;
		else
			$InvData[%i] = $tmbi::brick[0, %i];
	}
	$tmbi::current = "0 0";

//	for(%a=$remapcount-1; %a>-1; %a--) //best bet that it is near end
//	{
//		if(getfield(OptRemapList.getrowtextbyid(%a), 1) $= "LCONTROL")
//		{
//			$tmbi::prevctrl = $remapcmd[%a];
//			break;
//		}
//	}
	HUD_BrickNameBG.setvisible(0); //because deleting is mean
	HUD_BrickBox.setvisible(0);

	new GuiSwatchCtrl(TMBI_MainInventory)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "center";
		vertSizing = "top";
		position = "0 0"; //do it later
		extent = "640" SPC $tmbi::pref::height;
		minExtent = "8 2";
		visible = "1";
		color = "0 0 0 0";

		new GuiSwatchCtrl(tmbi_scroll_area)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "center";
			vertSizing = "height";
			position = "0 16";
			extent = "640" SPC $tmbi::pref::height-16;
			minExtent = "8 2";
			visible = "1";
			color = "100 100 100 0";

			new GuiSwatchCtrl(tmbi_inv_list)
			{
				profile = "GuiDefaultProfile";
				horizSizing = "center";
				vertSizing = "bottom";
				position = "0 0";
				extent = "640 0"; //will expand to accomodate more rows
				minExtent = "8 2";
				visible = "1";
				color = "0 0 0 0";

				//Rows insert into here with 66px spacing
			};
		};
		new GuiBitmapCtrl(tmbi_transition)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "center";
			vertSizing = "bottom";
			position = "0 16";
			extent = "640 24";
			minExtent = "8 2";
			visible = $tmbi::pref::fadeinventory;
			bitmap = "./ui/transition.png";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "0";
		};
		new GuiBitmapCtrl(tmbi_barthing)
		{
			profile = "GuiDefaultProfile";
			horizSizing = "center";
			vertSizing = "bottom";
			position = "0 0";
			extent = "640 16";
			minExtent = "8 2";
			visible = "1";
			bitmap = "./ui/top.png";
			wrap = "0";
			lockAspectRatio = "0";
			alignLeft = "0";
			overflowImage = "0";
			keepCached = "0";

			new GuiMLTextCtrl(tmbi_UINAME)
			{
				profile = "GuiMLTextProfile";
				horizSizing = "center";
				vertSizing = "top";
				position = "0 0";
				extent = "640 16";
				minExtent = "8 2";
				visible = "1";
				lineSpacing = "2";
				allowColorChars = "0";
				maxChars = "-1";
				text = "<color:"@tmbi_coltohex($tmbi::pref::color1)@"><font:"@ $tmbi::pref::font @":16><just:center>Too Many Bricks Inventory";
				maxBitmapHeight = "-1";
				selectable = "1";
			};
		};
	};
	playgui.add(TMBI_MainInventory);
//	tmbi_update();
}

//more activation code, used in a different context however
//builds the extra components of the brick selector dialog (BSD)
function tmbi_firsttimeinit()
{
	BSD_DoneButton.setvisible(0);
	BSD_ClearBtn.setvisible(0);
	BSD_InvBox.setvisible(0);
	BSD_Window.settext("TMBI Brick Selector");
	BSD_FavsHelper.delete(); //this one is full of fiddlesticks
	%gui = new GuiSwatchCtrl(BSD_FavsHelper) //fiddlesticks I tell you!
	{
		profile = "BlockWindowProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "192 25";
		extent = "269 27";
		minExtent = "8 2";
		visible = "0";
		color = "78 83 235 240";

		new GuiTextCtrl()
		{
			profile = "BlockWindowProfile";
			horizSizing = "center";
			vertSizing = "bottom";
			position = "22 3";
			extent = "225 18";
			minExtent = "8 2";
			visible = "1";
			text = "^^^ Click a number to set favorites ^^^";
			maxLength = "255";
		};
	};
	BSD_Window.add(%gui);

	for(%a=0; %a<BSD_ScrollBox.getcount(); %a++)
	{
		%gui = BSD_ScrollBox.getobject(%a);
		%gui.vertsizing = "height";
	}

	if(isobject(tmbi_sellist)) //sometimes the above guys magically come back
		return;
	BSD_Window.accelerator = "enter";
	BSD_Window.command = "tmbi_clickdone();";
	BSD_Window.resizeheight = 1;
	BSD_Window.minsize = "640 480";
	BSD_Window.resize(getword(BSD_Window.getposition(), 0), getword(BSD_Window.getposition(),1) - 60, 640, 600);
	BSD_ScrollBox.vertsizing = "height";

	%gui = new GuiScrollCtrl(TMBI_ScrollBox)
	{
		profile = "GuiScrollProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "3 419";
		extent = "634 156";
		minExtent = "8 2";
		visible = "1";
		willFirstRespond = "0";
		hScrollBar = "alwaysOff";
		vScrollBar = "alwaysOn";
		constantThumbHeight = "0";
		childMargin = "0 0";
		rowHeight = "40";
		columnWidth = "30";

		new GuiSwatchCtrl(tmbi_sellist) 
		{
			profile = "GuiDefaultProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "1 1";
			extent = "617 0";
			minExtent = "8 2";
			visible = "1";
			color = "0 0 0 0";
		};
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn7)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "512 566";
		extent = "100 30";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_clickdone();";
		accelerator = "";
		text = "DONE";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn6)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "444 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_options();";
		accelerator = "";
		text = "Options";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn5)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "376 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_clickcancel();";
		accelerator = "escape";
		text = "Cancel";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn4)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "308 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_reset();";
		accelerator = "";
		text = "Reset";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn3)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "240 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_clearcart();";
		accelerator = "";
		text = "Empty";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn2)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "172 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_help(0);";
		accelerator = "";
		text = "Help";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl(tmbi_btn1)
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "104 576";
		extent = "60 20";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_addrow();";
		accelerator = "";
		text = "Add Row";
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "base/client/ui/button1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	%gui = new GuiSwatchCtrl()
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = "4 576";
		extent = "100 20";
		minExtent = "8 2";
		visible = "1";
		color = "0 0 0 0";

		new GuiTextCtrl(TMBI_TEXTDISPLAY)
		{
			profile = "GuiTextProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "3 1";
			extent = "63 18";
			minExtent = "8 2";
			visible = "1";
			text = "Brick UIName";
			maxLength = "255";
		};
	};
	BSD_Window.add(%gui);
	%gui = new GuiBitmapButtonCtrl()
	{
		profile = "BlockButtonProfile";
		horizSizing = "right";
		vertSizing = "bottom";
		position = "500 4";
		extent = "96 18";
		minExtent = "8 2";
		visible = "1";
		command = "tmbi_versioncheck(1);";
		text = $tmbi::version;
		groupNum = "-1";
		buttonType = "PushButton";
		bitmap = "./ui/btn1";
		lockAspectRatio = "0";
		alignLeft = "0";
		overflowImage = "0";
		mKeepCached = "0";
		mColor = "255 255 255 255";
	};
	BSD_Window.add(%gui);
	$tmbi::resizerpos = "3 418"; //updates and junk, idk

	%gui = new GuiMouseEventCtrl(TMBI_Resizer)
	{
		profile = "GuiDefaultProfile";
		horizSizing = "right";
		vertSizing = "top";
		position = $tmbi::resizerpos;
		extent = "618 6";
		minExtent = "8 2";
		visible = "1";
		lockMouse = "0";
	};
	BSD_Window.add(%gui);

	tmbi_clearcart();
	tmbi_clickfav(1);

	if($tmbi::pref::autocheck)
		tmbi_versioncheck();
}
