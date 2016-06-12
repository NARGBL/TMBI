//versioncheck.cs

//version check system
//if you want to learn to do tcp networking on blockland this is probably a decent resource
function tmbi_versioncheck(%x)
{
	if(!%x && isfile("./rtbinfo.txt") && isfile("add-ons/system_returntoblockland/client.cs"))
		return;
	$tmbi_manualconnect = %x;

	if(isobject(TMBI_Downloader))
		TMBI_Downloader.delete();

	new tcpobject(TMBI_Downloader);
	$tmbi::downloadphase = 0;
	TMBI_Downloader.connect("forum.blockland.us:80");
}

function TMBI_Downloader::onConnected(%this)
{
	if($tmbi::downloadphase == 0)
	{
		//forum.blockland.us/index.php?topic=161834.720
		%req = "GET /index.php?topic=161834.720 HTTP/1.0\nHost: forum.blockland.us\n\n";
		%this.send(%req);
	}
	else
	{
		%req = "GET /index.php?action=dlattach;topic=161834.0;attach="@ $tmbi::versionfile SPC"HTTP/1.0\nHost: forum.blockland.us\n\n";
		%this.send(%req);
	}
}

function TMBI_Downloader::onConnectFailed(%this)
{
	if($tmbi_manualconnect)
		messageboxok("Attention!", "Unable to connect to the online version information.");
}

function TMBI_Downloader::onDisconnect(%this)
{
	if(!$tmbi_connected)
	{
		if($tmbi_manualconnect)
			messageboxok("Attention!", "Unable to connect to the online version information.");
	}
	else
		$tmbi_connected = "";
}

function TMBI_Downloader::onLine(%this, %line)
{
	tmbi_debug("TMBI TCP: "@ %line);

	if($tmbi::downloadphase == 0)
	{
		if(strpos(%line, "Versions.txt") > -1)
		{
			%subline = "http://forum.blockland.us/index.php?action=dlattach;topic=161834.0;attach=";
			$tmbi::versionfile = getsubstr(%line, strpos(%line, %subline)+strlen(%subline), strpos(%line, "\">")-(strpos(%line, %subline)+strlen(%subline)));

			if(tmbi_isint($tmbi::versionfile))
			{
				$tmbi_connected = 1;
				TMBI_Downloader.disconnect();
				$tmbi::downloadphase = 1;
				TMBI_Downloader.connect("forum.blockland.us:80");
			}
			else
				TMBI_Downloader.disconnect();
		}
	}
	else
	{
		if(getsubstr(%line, 0, 23) $= "NARG MOD VERSION TMBI: ")
		{
			$tmbi_connected = 1;
			$tmbi_availableversion = getsubstr(%line, 23, strlen(%line)-23);

			if($tmbi_availableversion $= $tmbi::version)
				tmbi_versionresult(1);
			else
				tmbi_versionresult(0);
			TMBI_Downloader.disconnect();
		}
	}
}

function tmbi_versionresult(%x)
{
	if(%x)
	{
		if($tmbi_manualconnect)
			messageboxok("Good News!", "Your Too Many Bricks Inventory mod is up to date.");
	}
	else
		messageboxok("Attention!", "There is a more current version of your Too Many Bricks Inventory mod.\n\nYour Version: "@$tmbi::version@"\n\nAvailable: "@$tmbi_availableversion @"\n\nDownload link: <a:http://forum.blockland.us/index.php?topic=161834.0>Forum Thread</a>");
}
