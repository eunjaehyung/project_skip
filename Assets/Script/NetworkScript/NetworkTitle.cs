using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FreeNet;
using GameServer;

public class NetworkTitle : MonoBehaviour {

	enum USER_STATE
	{
		NOT_CONNECTED,
		CONNECTED,
		WAITING_MATCHING
	}

	Texture bg;
	CBattleRoom battle_room;

	CNetworkManager network_manager;
	USER_STATE user_state;

	

	// Use this for initialization
	void Start () {
		this.user_state = USER_STATE.NOT_CONNECTED;
		this.bg = Resources.Load("images/title_blue") as Texture;
		this.battle_room = GameObject.Find("BattleRoom").GetComponent<CBattleRoom>();
		this.battle_room.gameObject.SetActive(false);

		this.network_manager = GameObject.Find("NetworkManager").GetComponent<CNetworkManager>();

		this.user_state = USER_STATE.NOT_CONNECTED;
		enter();
	}


	public void enter()
	{
		StopCoroutine("after_connected");

		this.network_manager.message_receiver = this;

		if (!this.network_manager.is_connected())
		{
			this.user_state = USER_STATE.CONNECTED;
			this.network_manager.connect();
		}
		else
		{
			on_connected();
		}
	}


	IEnumerator after_connected()
	{
		yield return new WaitForEndOfFrame();

		while (true)
		{
			if (this.user_state == USER_STATE.CONNECTED)
			{
				if (Input.GetMouseButtonDown(0))
				{
					this.user_state = USER_STATE.WAITING_MATCHING;

					CPacket msg = CPacket.create((short)PROTOCOL.ENTER_GAME_ROOM_REQ);
					this.network_manager.send(msg);

					StopCoroutine("after_connected");
				}
			}

			yield return 0;
		}
	}

	public void on_connected()
	{
		this.user_state = USER_STATE.CONNECTED;

		StartCoroutine("after_connected");
	}

	public void on_recv(CPacket msg)
	{
		PROTOCOL protocol_id = (PROTOCOL)msg.pop_protocol_id();

		switch (protocol_id)
		{
			case PROTOCOL.START_LOADING:
				{
					byte player_index = msg.pop_byte();

					this.battle_room.gameObject.SetActive(true);
					this.battle_room.start_loading(player_index);
					gameObject.SetActive(false);
				}
				break;
		}
	}
}
