//ghostmover.cs

exec("./ghostpackage.cs");
$tmbi::pref::mousemover = true;

//support for ghost brick manipulator
function tmbi_looktick()
{
	tmbi_debug("ticking to find");

	if(iseventpending($tmbi_looktick))
		cancel($tmbi_looktick);

	if($tmbi_lookdtb $= "")
		$tmbi_lookdtb = $tmbi_lookdtb = tmbi_getcurrbrickdatablock();

	if($tmbi_lookstart $= "")
		$tmbi_lookstart = getsimtime();

	if($tmbi_lookpos $= "")
		$tmbi_lookpos = vectoradd(tmbi_getfocuspos(), "0 0 " @ $tmbi_lookdtb.bricksizez/2);

	if($tmbi_lastlookobj $= "")
		$tmbi_lastlookobj = serverconnection.getobject(serverconnection.getcount()-1);

	for(%a=serverconnection.getcount()-1; %a>-1; %a--)
	{
		//watches serverconnection for anything that looks like your ghostbrick
		%obj = serverconnection.getobject(%a);

		if(%obj.getid() <= $tmbi_lastlookobj)
			break;

		if(%obj.getclassname() $= "fxdtsbrick")
		{
			if(!%obj.isplanted() && %obj.getdatablock().getid() == $tmbi_lookdtb)
			{
				%dist = vectordist($tmbi_lookpos, %obj.getposition());

				if(%dist < $tmbi_lookbestdist || $tmbi_lookbestdist $= "")
				{
					$tmbi_lookbestdist = %dist;
					$tmbi_lookbestobj = %obj;
				}
			}
		}
	}
	$tmbi_lastlookobj = serverconnection.getobject(serverconnection.getcount()-1);

	if(getsimtime() - $tmbi_lookstart > 500 || ($tmbi_lookbestdist !$= "" && $tmbi_lookbestdist < 5))
	{
		tmbi_lookend();
		return;
	}
	$tmbi_looktick = schedule(1, 0, tmbi_looktick);
}

function tmbi_lookend()
{
	if(iseventpending($tmbi_looktick))
		cancel($tmbi_looktick);

	if($tmbi_lookbestobj $= "")
	{
		tmbi_debug("found nothing...");
		return;
	}
	$tmbi_ghostbrick = $tmbi_lookbestobj;

	if($tmbi_mousefire && $tmbi::pref::mousemover)
	{
		if(getsimtime() - $tmbi_lookstart > 500)
			tmbi_toggleghostmover(1);
		else
			$tmbi_clickactivatedelay = schedule(getsimtime() - $tmbi_lookstart, 0, tmbi_toggleghostmover, 1);
	}
}

function tmbi_toggleGhostMover(%x)
{
	if(iseventpending($tmbi_ghostmovetick))
		cancel($tmbi_ghostmovetick);

	if(!%x)
	{
		//dunno
	}
	else
	{
		tmbi_ghostmovetick(1);
	}
}

function tmbi_ghostmovetick(%x)
{
	if(iseventpending($tmbi_ghostmovetick))
		cancel($tmbi_ghostmovetick);

	if(!isobject($tmbi_ghostbrick))
	{
		clientcmdcenterprint("\c2You need to clear and place a new ghost brick for this to work.", 3);
		return;
	}

	if(%x)
		$tmbi_ghostpos = $tmbi_ghostbrick.getposition();
	if(getword(serverconnection.getcontrolobject().getmuzzlevector(0), 2) < 0 || HUD_SuperShift.isvisible())
		%lookin = vectoradd(tmbi_getfocuspos(), "0 0 "@$tmbi_ghostbrick.getdatablock().bricksizez/10);
	else
		%lookin = vectorsub(tmbi_getfocuspos(), "0 0 "@$tmbi_ghostbrick.getdatablock().bricksizez/10);

	if($tmbi_ghostpos $= "")
		$tmbi_ghostpos = %lookin;
	%test = tmbi_shiftfromto($tmbi_ghostpos, %lookin, $tmbi_ghostbrick.getdatablock(), $tmbi_ghostbrick.getangleid());
	$tmbi_ghostpos = %lookin;

	if(%test)
	{
		if(iseventpending($tmbi_ghostmovesync))
			cancel($tmbi_ghostmovesync);
		$tmbi_ghostmovesync = schedule(serverconnection.getping()+100, 0, tmbi_ghostmovesync);
	}
	$tmbi_ghostmovetick = schedule(1, 0, tmbi_ghostmovetick);
}

//gets out of sync when you turn your blockhead.  Hard to compensate
function tmbi_ghostmovesync()
{
	if(iseventpending($tmbi_ghostmovesync))
		cancel($tmbi_ghostmovesync);

	if(isobject($tmbi_ghostbrick))
		$tmbi_ghostpos = $tmbi_ghostbrick.getposition();
}

//pulled from buildbot mostly
function tmbi_shiftfromto(%posa, %posb, %dtb, %dir)
{
	%posa = tmbi_snaptobrickgrid(%posa, %dtb, %dir);
	%posb = tmbi_snaptobrickgrid(%posb, %dtb, %dir);

	if(%posa $= %posb)
		return 0;

	%shiftx = getword(%posb, 0) - getword(%posa, 0);
	%shifty = getword(%posb, 1) - getword(%posa, 1);
	%shiftz = getword(%posb, 2) - getword(%posa, 2);

	if(%dir2 = tmbi_getplayerdirection())
	{
		if(%dir2 == 1)
		{
			%a = %shifty;
			%shifty = %shiftx * -1;
			%shiftx = %a;
		}
		else if(%dir2 == 2)
		{
			%shiftx = %shiftx * -1;
			%shifty = %shifty * -1;
		}
		else if(%dir2 == 3)
		{
			%a = %shiftx;
			%shiftx = %shifty * -1;
			%shifty = %a;
		}
	}
	tmbi_debug("shifting: "@%shiftx SPC %shifty SPC %shiftz);

	if($tmbi::pref::altshift)
	{
		commandtoserver('shiftbrick', %shiftx, 0, 0);
		commandtoserver('shiftbrick', 0, %shifty, 0);
		commandtoserver('shiftbrick', 0, 0, %shiftz);
	}
	else
		commandtoserver('shiftbrick', %shiftx, %shifty, %shiftz);
	return 1;
}

//this function is actually more accurate than the default system, interestingly enough
function tmbi_snaptobrickgrid(%pos, %dtb, %dir) //beautiful
{
	%sx = %dtb.bricksizex;
	%sy = %dtb.bricksizey;
	%sz = %dtb.bricksizez;

	if(%sx $= "" || %sy $= "" || %sz $= "")
	{
		tmbi_debug("Bad Datablock sent: "@ %dtb);
		return;
	}
	%x = getword(%pos, 0)*2;
	%y = getword(%pos, 1)*2;
	%z = mceil(getword(%pos, 2)*5); //sometimes bricks are at 0.1997 instead of 0.2 because badspot gave them cancer

	if(HUD_SuperShift.isvisible()) //super shift is enabled, use a whimsical adjusted brickgrid instead of normal one.
	{
		%org = $tmbi_ghostbrick.getposition();

		if(%org !$= "") //if we have a ghost brick
		{
			//convert original position to brick grid
			%ox = getword(%org, 0)*2;
			%oy = getword(%org, 1)*2;
			%oz = mceil(getword(%org, 2)*5);

			if($tmbi_ghostbrick.getangleid() % 2) //if it is facing north or south
			{
				%y = tmbi_mround((%y - %oy)/%sx)*%sx;
				%x = tmbi_mround((%x - %ox)/%sy)*%sy;
			}
			else //east or west
			{
				//can also be done with modulus, but screw that
				%x = tmbi_mround((%x - %ox)/%sx)*%sx;
				%y = tmbi_mround((%y - %oy)/%sy)*%sy;
			}
			%z = mfloor((%z - %oz)/%sz)*%sz; //ack
		}
	}
	else
	{
		if(!tmbi_isint(%dir/2)) //asdf this entire everything
		{
			if(!tmbi_isint(%sx/2))
				%y = mfloor(%y);
			else
				%y = tmbi_mround(%y);

			if(!tmbi_isint(%sy/2)) //modulus is for squares
				%x = mfloor(%x);
			else
				%x = tmbi_mround(%x);
		}
		else
		{
			if(!tmbi_isint(%sx/2))
				%x = mfloor(%x);
			else
				%x = tmbi_mround(%x);

			if(!tmbi_isint(%sy/2))
				%y = mfloor(%y);
			else
				%y = tmbi_mround(%y);
		}
	}
	return %x SPC %y SPC %z;
}

//0 - East
//1 - South
//2 - West
//3 - North
function tmbi_getplayerdirection()
{
	if(isObject(%p = serverconnection.getcontrolobject()))
	{
		if(%p.getdatablock().classname $= "CameraData" && $tmbi_lastplayerdir !$= "")
			return $tmbi_lastplayerdir;
		else
		{
			%va = getword(%p.getforwardvector(),0);
			%vb = getword(%p.getforwardvector(),1);

			if(mabs(%va) > mabs(%vb))
			{
				if(%va > 0)
					$tmbi_lastplayerdir = 0;
				else
					$tmbi_lastplayerdir = 2;
			}
			else
			{
				if(%vb > 0)
					$tmbi_lastplayerdir = 1;
				else
					$tmbi_lastplayerdir = 3;
			}
			return $tmbi_lastplayerdir;
		}
	}
	else
		return 4;
}

//fun trick:
//overwrite this function in console with the following command to obtain a pet brick when using the ghost brick mover
//function tmbi_getfocuspos(){return vectorsub(serverconnection.getcontrolobject().getposition(), serverconnection.getcontrolobject().getforwardvector());}
function tmbi_getfocuspos()
{
	%dist = getfocusdistance();
	%eye = tmbi_getmyeyepoint();
	%vec = serverconnection.getcontrolobject().getmuzzlevector(0);
	%vec = vectorscale(%vec, %dist);
	return vectoradd(%eye, %vec);
}

//kinda wonky function, but still accurate for the default blockhead in reasonable ranges.
function tmbi_getmyeyepoint()
{
	%player = serverconnection.getcontrolobject();

	if(!isobject(%player))
		return "0 0 0";
	%pos = %player.getposition();
	%vec = %player.getforwardvector();
	%scale = %player.getscale();
	%x = getword(%pos, 0) + (getword(%vec, 0)*0.14 + 0.002)*getword(%scale, 0); //me no likey
	%y = getword(%pos, 1) + (getword(%vec, 1)*0.14 + 0.002)*getword(%scale, 1);
	%z = getword(%pos, 2) + (getword(%player.getdatablock().boundingbox, 2)/4.92 - $mvtriggercount3*1.53 + 0.002)*getword(%scale, 2); //optimized for standard blockhead, good luck otherwise
	return %x SPC %y SPC %z;
}

function tmbi_getcurrbrickdatablock()
{
	if($tmbi_instantusebrick !$= "")
		return $tmbi_instantusebrick;
	else
		return $invdata[$currscrollbrickslot];
}

function tmbi_isint(%x) //I don't trust the sometimes default one
{
	if(%x $= mfloor(%x)) //also makes it sure it is a string
		return 1;
	return 0;
}

function tmbi_mround(%x)
{
	return mfloor(%x + 0.5);
}

if($tmbi_ghostmover_bound)
	return;
$tmbi_ghostmover_bound = true;
$remapdivision[$remapcount] = "Ghost Brick Mover";
$remapname[$remapcount] = "Hold to use Ghost Mover";
$remapcmd[$remapcount] = "tmbi_toggleGhostMover";
$remapcount++;
