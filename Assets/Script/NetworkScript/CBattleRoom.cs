using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using GameServer;

public class CBattleRoom : MonoBehaviour {

	enum GAME_STATE
	{
		READY = 0,
		STARTED
	}


	CNetworkManager network_manager;

	GAME_STATE game_state;


	void Awake()
	{
		this.network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();

		this.game_state = GAME_STATE.READY;
	}

	void clear()
	{
	}

	/// <summary>
	/// °ÔÀÓ¹æ¿¡ ÀÔÀåÇÒ ¶§ È£ÃâµÈ´Ù. ¸®¼Ò½º ·ÎµùÀ» ½ÃÀÛÇÑ´Ù.
	/// </summary>
	public void start_loading(byte player_me_index)
	{
		clear();

		this.network_manager.message_receiver = this;

		CPacket msg = CPacket.create((short)PROTOCOL.LOADING_COMPLETED);
		this.network_manager.send(msg);
	}


	/// <summary>
	/// ÆÐÅ¶À» ¼ö½Å ÇßÀ» ¶§ È£ÃâµÊ.
	/// </summary>
	/// <param name="protocol"></param>
	/// <param name="msg"></param>
	void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.GAME_START:
				on_game_start(msg);
				network_manager.txtServerMsg.text = "Multi Game Start";
				break;

			case PROTOCOL.PLAYER_MOVED:
				break;

			case PROTOCOL.START_PLAYER_TURN:
				break;

			case PROTOCOL.ROOM_REMOVED:
				break;

			case PROTOCOL.GAME_OVER:
				break;
		}
	}
	
	void on_game_start(CPacket msg)
	{

	}
}
