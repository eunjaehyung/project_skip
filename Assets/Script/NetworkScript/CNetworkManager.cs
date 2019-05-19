using UnityEngine;
using System;
using System.Collections;
using FreeNet;
using FreeNetUnity;
using GameServer;

public class CNetworkManager : MonoBehaviour {

	CFreeNetUnityService gameserver;
	string received_msg;

	public MonoBehaviour message_receiver;

	public void Awake()
	{
		this.received_msg = "";

		this.gameserver = gameObject.AddComponent<CFreeNetUnityService>();

		this.gameserver.appcallback_on_status_changed += on_status_changed;

		this.gameserver.appcallback_on_message += on_message;
	}


	public void connect()
	{
		this.gameserver.connect("127.0.0.1", 7979);
	}

	public bool is_connected()
	{
		return this.gameserver.is_connected();
	}

	void on_status_changed(NETWORK_EVENT status)
	{
		switch (status)
		{
			case NETWORK_EVENT.connected:
				{
					CLogManager.log("on connected");
					this.received_msg += "on connected\n";

					GameObject.Find("MainTitle").GetComponent<NetworkTitle>().on_connected();
				}
				break;

			case NETWORK_EVENT.disconnected:
				CLogManager.log("disconnected");
				this.received_msg += "disconnected\n";
				break;
		}
	}

	void on_message(CPacket msg)
	{
		this.message_receiver.SendMessage("on_recv", msg);
	}

	public void send(CPacket msg)
	{
		this.gameserver.send(msg);
	}
}
