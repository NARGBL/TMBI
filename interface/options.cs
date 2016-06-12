//options.cs
exec("./TMBI_GUI1.gui");

function tmbi_options()
{
	canvas.pushdialog(TMBI_GUI1);
}

function tmbi_defaultprefs()
{
	if(isfile("base/client/ui/cache/calibri_18.gft"))
		$tmbi::pref::font = "calibri";
	else
		$tmbi::pref::font = "Palatino Linotype";
	$tmbi::pref::autocheck = 1;
	$tmbi::pref::height = 160;
	$tmbi::pref::fadeinventory = 1;
	$tmbi::pref::transweight = "0.8";
	$tmbi::repurposekeybinds = 0;
	$tmbi::pref::nextscroll = 1;
	$tmbi::pref::maxrowcount = 5;
	$tmbi::pref::mousemover = 1;
	$tmbi::pref::color0 = "1 0.4 0 1";
	$tmbi::pref::color1 = "1 1 1 1";
	$tmbi::pref::color2 = "0 0 0 1";
}

function tmbi_options_apply()
{
	$tmbi::pref::font = TMBI_Options_val1.getvalue();
	$tmbi::pref::height = TMBI_Options_val2.getvalue();
	$tmbi::pref::transweight = TMBI_Options_val3.getvalue();
	$tmbi::pref::fadeinventory = TMBI_Options_val4.getvalue();
	$tmbi::repurposekeybinds = TMBI_Options_val5.getvalue();
	$tmbi::pref::autocheck = TMBI_Options_val6.getvalue();
	$tmbi::pref::nextscroll = TMBI_Options_val7.getvalue();
	$tmbi::pref::maxrowcount = TMBI_Options_val8.getvalue();
	$tmbi::pref::mousemover = TMBI_Options_val9.getvalue();
	$tmbi::pref::altshift = TMBI_Options_val10.getvalue();

	for(%a=0; %a<3; %a++)
		$tmbi::pref::color[%a] = $tmbi::colorselector::fullcolor[%a];
	canvas.popdialog(TMBI_GUI1);
	export("$tmbi::pref::*", "config/client/TMBI/prefs.cs");
	tmbi_updatesel();
}

function tmbi_options_cancel()
{
	TMBI_Options_val1.setvalue($tmbi::pref::font);
	tmbi_help(1);
	TMBI_Options_val2.setvalue($tmbi::pref::height);
	TMBI_Options_val3.setvalue($tmbi::pref::transweight);
	TMBI_Options_val4.setvalue($tmbi::pref::fadeinventory);
	TMBI_Options_val5.setvalue($tmbi::repurposekeybinds);
	TMBI_Options_val6.setvalue($tmbi::pref::autocheck);
	TMBI_Options_val7.setvalue($tmbi::pref::nextscroll);
	TMBI_Options_val8.setvalue($tmbi::pref::maxrowcount);
	TMBI_Options_val9.setvalue($tmbi::pref::mousemover);
	TMBI_Options_val10.setvalue($tmbi::pref::altshift);

	for(%a=0; %a<3; %a++)
		$tmbi::colorselector::fullcolor[%a] = $tmbi::pref::color[%a];
	tmbi_setcolormode(0);

	if(TMBI_GUI1.isawake())
		canvas.popdialog(TMBI_GUI1);
}

function tmbi_options_defaults()
{
	if(isfile("base/client/ui/cache/calibri_18.gft"))
		TMBI_Options_val1.setvalue("Calibri");
	else
		TMBI_Options_val1.setvalue("Palatino Linotype");
	tmbi_help(1);
	TMBI_Options_val2.setvalue(160);
	TMBI_Options_val3.setvalue("0.8");
	TMBI_Options_val4.setvalue(1);
	TMBI_Options_Val5.setvalue(0);
	TMBI_Options_val6.setvalue(1);
	TMBI_Options_val7.setvalue(1);
	TMBI_Options_val8.setvalue(5);
	TMBI_Options_val9.setvalue(0);
	TMBI_Options_Val10.setvalue(0);
	$tmbi::colorselector::fullcolor0 = "1 0.4 0 1";
	$tmbi::colorselector::fullcolor1 = "1 1 1 1";
	$tmbi::colorselector::fullcolor2 = "0 0 0 1";
	tmbi_setcolormode($tmbi::colorselector::current);
}
