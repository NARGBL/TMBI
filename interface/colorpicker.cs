//colorpicker.cs

//color manager
//if you want to understand this in more detail you should probably just pm me
function tmbi_colorwheelmctrl::onmousedragged(%this, %a, %pos, %b)
{
	tmbi_managerecolor(%pos);
}

function tmbi_colorwheelmctrl::onmousedown(%this, %a, %pos, %b)
{
	tmbi_managerecolor(%pos);
}

function tmbi_managerecolor(%pos)
{
	tmbi_debug("  === INIT === ");
	tmbi_debug("    NEW BLOP");
	tmbi_debug("mouse pos "@%pos);
	%gpos = getscreenpos(tmbi_colorwheelimage);
	%centerx = getword(%gpos, 0) + 64;
	%centery = getword(%gpos, 1) + 64;
	tmbi_debug("Centers: "@%centerx SPC %centery);
	%offsetx = %centerx - getword(%pos,0);
	%offsety = getword(%pos,1) - %centery;
	tmbi_debug("offsets: "@%offsetx SPC %offsety);
	tmbi_pickcolor(%offsetx/-64, %offsety/-64);
}

function tmbi_pickcolor(%x, %y)
{
	tmbi_debug("RGB POS: "@%x SPC %y);
	%pi = matan(0,-1);
	tmbi_debug("pi: "@%pi);
	%angle = matan(%y, %x); //range pi to negative pi
	%r = msqrt(mpow(%x,2) + mpow(%y,2));

	if(%r > 1)
		%r = 1;
	tmbi_debug("pol: "@%r SPC %angle@" rad");

	for(%i=0; %i<3; %i++)
	{
		tmbi_debug(" ---color---");
		%mangle = %angle - mfloor(%angle/(2*%pi)) * (2*%pi); //locks onto 0 to 2pi range
		tmbi_debug(%angle SPC %mangle);

		if(%mangle > %pi*(1.5))
			%mangle -= 2*%pi;
		else if(%mangle > %pi/2)
		{
			%mangle -= %pi;
			%mangle *= -1;
		}
		tmbi_debug("final adjusted angle: "@%mangle);
		%min = (%mangle + %pi/6)/(%pi/3);
		tmbi_debug("min: "@%min);

		if(%min > 1)
			%min = 1;
		else if(%min < 0)
			%min = 0;
		%col = 1 - ((1-%min)*%r);

		if(%color $= "")
			%color = %col;
		else
			%color = %color SPC %col;
		%angle += %pi*(2/3);
	}
	tmbi_debug("color: "@%color);
	tmbi_setselectorcolor(%color);
}

function tmbi_setselectorcolor(%color)
{
	if(%color !$= "")
		$tmbi::colorselector::color = %color;

	if($tmbi::colorselector::shade > 1 || $tmbi::colorselector::shade $= "")
		$tmbi::colorselector::shade = 1;
	else if($tmbi::colorselector::shade < 0)
		$tmbi::colorselector::shade = 0;
	%cr = getword($tmbi::colorselector::color, 0) * $tmbi::colorselector::shade;
	%cg = getword($tmbi::colorselector::color, 1) * $tmbi::colorselector::shade;
	%cb = getword($tmbi::colorselector::color, 2) * $tmbi::colorselector::shade;
	%fullcol = %cr SPC %cg SPC %cb;

	if($tmbi::colorselector::current != 2 || $tmbi::colorselector::alpha > 1 || $tmbi::colorselector::alpha $= "")
		$tmbi::colorselector::alpha = 1;
	else if($tmbi::colorselector::alpha <= 0)
		$tmbi::colorselector::alpha = 0;
	$tmbi::colorselector::fullcolor[$tmbi::colorselector::current] = %fullcol SPC $tmbi::colorselector::alpha;
	tmbi_colorpreview.setcolor($tmbi::colorselector::fullcolor[$tmbi::colorselector::current]);

	if($tmbi::colorselector::current == 2)
		%fullcol = $tmbi::colorselector::fullcolor[$tmbi::colorselector::current];
	tmbi_colortext.settext(%fullcol);
	$tmbi_coloroverride = 1;
	tmbi_colorslider.setvalue($tmbi::colorselector::shade);
	tmbi_alphacolorslider.setvalue($tmbi::colorselector::alpha);
	$tmbi_coloroverride = 0;
}

function tmbi_nudgeslider()
{
	if($tmbi_coloroverride)
		return;
	tmbi_setselectorcolor();
}

function tmbi_nextcolormode()
{
	$tmbi::colorselector::current++;

	if($tmbi::colorselector::current > 2)
		$tmbi::colorselector::current = 0;
	tmbi_setcolormode($tmbi::colorselector::current);
}

function tmbi_prevcolormode()
{
	$tmbi::colorselector::current--;

	if($tmbi::colorselector::current < 0)
		$tmbi::colorselector::current = 2;
	tmbi_setcolormode($tmbi::colorselector::current);
}

function tmbi_setcolormode(%x)
{
	$tmbi::colorselector::current = %x;

	if(%x == 2)
	{
		%text = "HUD";
		%a = 1;
	}
	else
	{
		%a = 0;

		if(%x == 1)
			%text = "Secondary";
		else if(%x == 0)
			%text = "Primary";
		else
			return;
	}
	tmbi_alphacolorslider.setvisible(%a);
	tmbi_colormodetext.settext("<just:center><font:impact:18>"@%text);
	tmbi_setcolorfromtext($tmbi::colorselector::fullcolor[%x]);
}

function tmbi_setcolorfromtext(%color)
{
	tmbi_debug("here we gooooo: "@ %color);

	for(%a=0; %a<3; %a++)
	{
		%col[%a] = getword(%color, %a);
		tmbi_debug("initial color" SPC %a SPC %col[%a]);

		if(%hcol $= "" || %col[%a] > %hcol)
			%hcol = %col[%a];
	}
	tmbi_debug("highest color: "@ %hcol);

	for(%a=0; %a<3; %a++)
	{
		if(%hcol != 0)
			%col[%a] *= 1/%hcol; //will also lock the highest color to 1
		tmbi_debug("adjusted color" SPC %a SPC %col[%a]);

		if(%col[%a] > 1) //in case of rounding error
			%col[%a] = 1;
		else if(%col[%a] < 0) //in case of user input blargtarded
			%col[%a] = 0;
	}

	if($tmbi::colorselector::current == 2)
		$tmbi::colorselector::alpha = getword(%color, 3);
	$tmbi::colorselector::shade = %hcol;
	tmbi_setselectorcolor(%col0 SPC %col1 SPC %col2);
	tmbi_debug("final color: "@ $tmbi::colorselector::color);
}
