//support.cs

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

//list control stuff and things
function tmbi_isonlist(%list, %item)
{
	for(%a=0; %a<getwordcount(%list); %a++)
	{
		if(%item $= getword(%list, %a))
			return 1;
	}
	return 0;
}

function tmbi_addtolist(%list, %item)
{
	if(%list $= "")
		%list = %item;
	else
		%list = %list SPC %item;
	return %list;
}

function tmbi_removefromlist(%list, %item)
{
	for(%a=0; %a<getwordcount(%list); %a++)
	{
		if(%item !$= getword(%list, %a))
			%newlist = tmbi_addtolist(%newlist, getword(%list, %a));
	}
	return %newlist;
}

function tmbi_setfield(%list, %index, %replace)
{
	for(%a=0; %a<getfieldcount(%list); %a++)
	{
		if(%a == %index)
			%text = %replace;
		else
			%text = getfield(%list, %a);

		if(%newlist $= "")
			%newlist = %text;
		else
			%newlist = %newlist TAB %text;
	}
	return %newlist;
}

function tmbi_coltohex(%col)
{
	for(%a=0; %a<3; %a++)
		%b[%a] = tmbi_dectohex(getword(%col, %a)*255);
	return %b0 @ %b1 @ %b2;
}

function tmbi_dectohex(%x)
{
	%a0 = mfloor(%x/16);
	%a1 = mfloor(%x - 16*%a0);

	for(%i=0; %i<2; %i++)
	{
		if(%a[%i] == 10)
			%a[%i] = "a";
		else if(%a[%i] == 11)
			%a[%i] = "b";
		else if(%a[%i] == 12)
			%a[%i] = "c";
		else if(%a[%i] == 13)
			%a[%i] = "d";
		else if(%a[%i] == 14)
			%a[%i] = "e";
		else if(%a[%i] == 15)
			%a[%i] = "f";
	}
	return %a0 @ %a1;
}

function getscreenpos(%gui)
{
	if(!isobject(%gui))
		return;
	%pos = %gui.getposition();
	%x = getword(%gui.getposition(), 0);
	%y = getword(%gui.getposition(), 1);

	while(isObject(%gui.getgroup()))
	{
		%gui = %gui.getgroup();

		if(%gui.getname() $= "Canvas")
			break;
		%pos = %gui.getposition();
		%x += getword(%pos, 0);
		%y += getword(%pos, 1);
	}
	return %x SPC %y;
}
