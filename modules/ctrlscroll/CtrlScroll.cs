//CtrlScroll.cs

function tmbi_control(%x) //all keybinds should work like this
{
	if(!%x && $tmbi_control == 2)
		$BrickScrolling::Ready = 0;
	$tmbi_control = %x;

	if(%x)
	{
		if((getSimTime() - $tmbi_control_time) < 500)
		{
			$tmbi_control = 2;
			$BrickScrolling::Ready = 1;
		}
		$tmbi_control_time = getSimTime();
	}
}

//tmbi intercepts the lcontrol key for vertical scrolling
function tmbi_activatekeybind()
{
//	tmbi_debug("activating keybind");
	movemap.bind(keyboard, lcontrol, tmbi_control);
}

function tmbi_deactivatekeybind()
{
//	tmbi_debug("deactivating keybind");

//	if(isfunction($tmbi::prevctrl))
//		movemap.bind(keyboard, lcontrol, $tmbi::prevctrl);
//	else
		movemap.bind(keyboard, lcontrol, ""); //so it doesn't show up as a non-remappable command maybe idk
}

package ctrlscroll
{
	function scrollpaint(%x)
	{
		if($tmbi_control && !$tmbi_control_override)
		{
			if($pref::input::reversebrickscroll)
			{
				if(%x < 0) //scroll down
					shiftpaintcolumn(1);
				else
					shiftpaintcolumn(-1);
			}
			else
			{
				if(%x < 0)
					shiftpaintcolumn(-1);
				else
					shiftpaintcolumn(1);
			}
		}
		else
			parent::scrollpaint(%x);
	}

	function setScrollMode(%x)
	{
		if(%x == $SCROLLMODE_NONE)
		{
			tmbi_deactivatekeybind();
		}
		else
		{
			tmbi_activatekeybind();
		}
		return parent::setScrollMode(%x);
	}

	function invup(%x)
	{
		$tmbi_control_override = 1;
		parent::invup(%x);
		$tmbi_control_override = 0;
	}

	function invdown(%x)
	{
		$tmbi_control_override = 1;
		parent::invdown(%x);
		$tmbi_control_override = 0;
	}

	function invleft(%x)
	{
		$tmbi_control_override = 1;
		parent::invleft(%x);
		$tmbi_control_override = 0;
	}

	function invright(%x)
	{
		$tmbi_control_override = 1;
		parent::invright(%x);
		$tmbi_control_override = 0;
	}
};
activatepackage(ctrlscroll);
