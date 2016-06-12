//resizer.cs

//resizer support
function TMBI_Resizer::OnMouseDown(%gui, %idk, %pos, %noidea)
{
	tmbi_activateresizer(%pos);
}

function TMBI_Resizer::OnMouseUp(%gui, %idk, %pos, %noidea)
{
	tmbi_deactivateresizer();
}

function TMBI_Resizer::OnMouseDragged(%gui, %idk, %pos, %noidea)
{
	if(!$tmbi_dragenabled)
		return;
	%y = getword(%pos, 1);
	%change = %y - $tmbi_lastresizepos;
	%currheight = getword(TMBI_ScrollBox.getextent(), 1);

	if(%currheight - %change > 315)
		%change = %currheight - 315;
	else if(%currheight - %change < 64)
		%change = %currheight - 64;

	if(%change == 0)
		return;

	$tmbi_lastresizepos = $tmbi_lastresizepos + %change;
	TMBI_ScrollBox.resize(3, getword(TMBI_ScrollBox.getposition(), 1) + %change, 634, getword(TMBI_ScrollBox.getextent(), 1) - %change);
	BSD_ScrollBox.resize(getword(BSD_ScrollBox.getposition(), 0), getword(BSD_ScrollBox.getposition(), 1), getword(BSD_ScrollBox.getextent(), 0), getword(BSD_ScrollBox.getextent(), 1) + %change);
}

function TMBI_Resizer::OnMouseLeave(%gui, %idk, %pos, %noidea)
{
	tmbi_deactivateresizer();
}

function tmbi_activateresizer(%pos)
{
	if($tmbi_dragenabled)
		return;
	tmbi_debug("activating resizer");
	TMBI_Resizer.resize(0, 0, 640, getword(BSD_Window.getextent(), 1));
	$tmbi_dragenabled = 1;
	$tmbi_lastresizepos = getword(%pos, 1);
}

function tmbi_Deactivateresizer()
{
	if(!$tmbi_dragenabled)
		return;
	tmbi_debug("deactivating resizer");
	TMBI_Resizer.resize(3, getword(TMBI_ScrollBox.getposition(), 1), 600, 6);
	$tmbi_dragenabled = 0;
}
