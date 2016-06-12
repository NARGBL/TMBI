//ghostpackage.cs

package ghostmover
{
	function mousefire(%x)
	{
		parent::mousefire(%x);
		$tmbi_mousefire = %x;

		if(!%x)
		{
			if($tmbi::pref::mousemover)
			{
				if(iseventpending($tmbi_clickactivatedelay))
					cancel($tmbi_clickactivatedelay);
				tmbi_toggleghostmover(0);
			}
		}
		else
		{
			if(isobject(serverconnection.getcontrolobject()))
			{
				if(isobject(serverconnection.getcontrolobject().getmountedimage(0)))
				{
					if(serverconnection.getcontrolobject().getmountedimage(0).shapefile $= "base/data/shapes/brickWeapon.dts")
					{
						if(!isobject($tmbi_ghostbrick))
						{
							if(iseventpending($tmbi_looktick))
								cancel($tmbi_looktick);
							$tmbi_lookdtb = tmbi_getcurrbrickdatablock();
							$tmbi_lookstart = getsimtime();
							$tmbi_lookpos = vectoradd(tmbi_getfocuspos(), "0 0 " @ $tmbi_lookdtb.bricksizez/2);
							$tmbi_lookbestdist = "";
							$tmbi_lookbestobj = "";
							$tmbi_ghostpos = "";
							tmbi_looktick();
						}
						else
						{
							if($tmbi::pref::mousemover)
							{
								if(getfocusdistance() > 12.67)
									tmbi_toggleghostmover(1);
								else
									$tmbi_clickactivatedelay = schedule(500, 0, tmbi_toggleghostmover, 1);
							}
						}
					}
				}
			}
		}
	}

	function cancelbrick(%x)
	{
		parent::cancelbrick(%x);

		if(%x && isobject($tmbi_ghostbrick))
			$tmbi_ghostbrick = ""; //failsafe for if it finds the wrong ghostbrick
	}

	function BSD_RightClickIcon(%x)
	{
		parent::BSD_RightClickIcon(%x);
		$tmbi_instantusebrick = %x;
	}

	function directSelectInv(%x)
	{
		parent::directSelectInv(%x);
		$tmbi_instantusebrick = "";
	}
};
activatepackage(ghostmover);