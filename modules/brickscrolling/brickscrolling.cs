//Brick Scrolling
//By: Hata (483) and Chrono, with help from Honor and Nexus
exec("./greekbinds.cs");

if(!$BrickScrolling::Binded)
{
	addKeyBind("Building", "Scroll Group Bricks", "ScrollBrickModifier");
	$BrickScrolling::Binded = true;
}

function ScrollBrickModifier(%val)
{
	$BrickScrolling::Ready = %val;
}

function ScrollBrickActual(%x)
{
	if(!$pref::input::reversebrickscroll)
		%x *= -1;
	if(%x < 0)
		%move = -1;
	else
		%move = 1;
	%slot = $currScrollBrickSlot;
	%data = $invData[%slot];
	%sub = %data.sub;
	%pos = %sub.id[%data] + %move;

	if(%pos < 0)
		%pos = %sub.getCount()-1;
	else if(%pos >= %sub.getCount())
		%pos = 0;
	%dataNew = %sub.getObject(%pos);

	if(%dataNew.subCategory $= $invData[%slot].subCategory)
	{
		commandtoserver('buybrick', %slot, %dataNew);
	}
}

package BrickGroupScroll
{
	function scrollBricks(%x)
	{
		if($BrickScrolling::Ready)
			ScrollBrickActual(%x);
		else
			Parent::scrollBricks(%x);
	}

	function BSD_LoadBricks()
	{
		if(isObject(BrickScrollGroup))
			BrickScrollGroup.delete();
		new SimGroup(BrickScrollGroup);
		RootGroup.add(BrickScrollGroup);
		return Parent::BSD_LoadBricks();
	}

	function BSD_addCategory(%name)
	{
		if(!isObject(BrickScrollGroup @ %name))
		{
			%sg = new SimGroup();
			BrickScrollGroup.add(%sg);
			%sg.setName(BrickScrollGroup @ %name);
		}
		return Parent::BSD_addCategory(%name);
	}

	function BSD_addSubCategory(%cat,%name)
	{
		if(!isObject("BrickScrollSet" @ %cat @ "_" @ %name))
		{
			%ss = new SimSet();
			%co = "BrickScrollGroup" @ %cat;
			%co.add(%ss);
			%ss.setName("BrickScrollSet" @ %cat @ "_" @ %name);
		}
		return Parent::BSD_addSubCategory(%cat,%name);
	}

	function BSD_CreateBrickButton(%data)
	{
		%sub = "BrickScrollSet" @ %data.category @ "_" @ %data.subcategory;

		if(isObject(%sub))
		{
			%sub.id[%data] = %sub.getCount();
			%sub.add(%data);
			%data.sub = %sub;
		}
		return Parent::BSD_CreateBrickButton(%data);
	}
};
activatePackage(BrickGroupScroll);
