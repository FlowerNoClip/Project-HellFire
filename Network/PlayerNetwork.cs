using FishNet.Object;
using FishNet.Managing;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using FishNet.Connection;

namespace FishNet.Component.Spawning {
    public class PlayerNetwork : NetworkBehaviour
    {  
        public string nickname;
        public MainMenuManager mainMenuManager;
        public PlayerNetwork LastAttacker { get; set; }

        // �������� ������ �г��� ���
        private static List<string> Tnicknames = new List<string>();
        private static List<string> CTnicknames = new List<string>();
    
        private RoundManager roundManager;
        private ChangeTeam changeTeam;
        public Button[] buttons;
        public bool teamCode;

        private void Awake()
        {
            mainMenuManager = MainMenuManager.Instance;
            changeTeam = ChangeTeam.Instance;
            buttons = new Button[2];
            buttons[0] = changeTeam.buttons[0];
            buttons[1] = changeTeam.buttons[1];
        }
        private void Start()
        {
            roundManager = RoundManager.Instance;
            mainMenuManager.playerNetwork = gameObject.GetComponent<PlayerNetwork>();
            if (mainMenuManager != null)
            {
                Debug.Log("MainMenuManager instance found.");
            }
            teamCode = true;


        }

        [ServerRpc(RequireOwnership = false)]
        public void SetLastAttacker(PlayerNetwork attacker)
        {
            LastAttacker = attacker;
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.IsOwner)
            {
                changeTeam.buttons[0].onClick.AddListener(CTTeam);
                changeTeam.buttons[1].onClick.AddListener(TTeam);
                nickname = SteamFriends.GetPersonaName();
                AddList(gameObject.GetComponent<PlayerNetwork>());
                AddNicknameServer(nickname);
                StartCoroutine(Setting());
            }
            else
            {
                GetComponent<PlayerNetwork>().enabled = false;
            }
        }

        public IEnumerator Setting()
        {
            yield return new WaitForSeconds(0.2f);

            roundManager.RespawnPlayers();
            yield return null;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddList(PlayerNetwork pnetwork)
        {
            // Ŭ���̾�Ʈ���� AddList�� ȣ���Ͽ� SyncList�� �����ϵ��� ��
            roundManager.SyncList(pnetwork);
        }
        
        public void LeaveRoom()
        {
            //base.OnStopClient();
            RemoveNicknameS(nickname);
            Debug.Log(nickname + "���� �����Ͽ����ϴ�.");
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddNicknameServer(string _nickname)
        {
                nickname = _nickname;
            // �������� �г��� �߰�
            if (teamCode)
            {
                if (Tnicknames.Contains(_nickname))
                {
                    return;
                }

                Tnicknames.Add(_nickname);
            }
            else
            {
                if (CTnicknames.Contains(_nickname))
                {
                    return;
                }

                CTnicknames.Add(_nickname);
            }

            // ��� Ŭ���̾�Ʈ�� �г��� ��� ����ȭ
            SyncNicknames(Tnicknames.ToArray(), CTnicknames.ToArray());
        }

        [ServerRpc(RequireOwnership = false)]
        private void RemoveNicknameS(string _nickname)
        {
            for (int i = 0; i < Tnicknames.Count; i++)
            {
                if (Tnicknames[i] == _nickname)
                {
                    Tnicknames.RemoveAt(i);
                    break;
                }
            }
        
            for (int i = 0; i < CTnicknames.Count; i++)
            {
                if (CTnicknames[i] == _nickname)
                {
                    CTnicknames.RemoveAt(i);
                    break;
                }
            }
        

            // ��� Ŭ���̾�Ʈ�� �г��� ��� ����ȭ
            SyncNicknames(Tnicknames.ToArray(), CTnicknames.ToArray());
        }

        [ObserversRpc]
        private void SyncNicknames(string[] tNicknames, string[] ctNicknames)
        {
            
            for (int i = 0; i < mainMenuManager.TnicknameText.Length; i++)
            {
                if (i < tNicknames.Length)
                {
                    mainMenuManager.TnicknameText[i].text = tNicknames[i];
                }
                else
                {
                    mainMenuManager.TnicknameText[i].text = string.Empty;
                }
            }

            for (int i = 0; i < mainMenuManager.CTnicknameText.Length; i++)
            {
                if (i < ctNicknames.Length)
                {
                    mainMenuManager.CTnicknameText[i].text = ctNicknames[i];
                }
                else
                {
                    mainMenuManager.CTnicknameText[i].text = string.Empty;
                }
            }
        }
        [ObserversRpc]
        public void ChangeTeams(string _nickname, bool _teamCode)
        {
            if (_teamCode)
            {
                // TnicknameText���� _nickname�� ã��
                int tIndex = -1;
                for (int i = 0; i < mainMenuManager.TnicknameText.Length; i++)
                {
                    if (mainMenuManager.TnicknameText[i].text == _nickname)
                    {
                        tIndex = i;
                        break;
                    }
                }

                // CTnicknameText���� �� ���ڿ� ã��
                int ctIndex = -1;
                for (int j = 0; j < mainMenuManager.CTnicknameText.Length; j++)
                {
                    if (string.IsNullOrEmpty(mainMenuManager.CTnicknameText[j].text))
                    {
                        ctIndex = j;
                        break;
                    }
                }

                // �� ���� ����
                if (tIndex != -1 && ctIndex != -1)
                {
                    RemoveNicknameS(_nickname);
                    teamCode = false;
                    AddNicknameServer(_nickname);
                }
            }

            else
            {
                // CTnicknameText���� _nickname�� ã��
                int ctIndex = -1;
                for (int i = 0; i < mainMenuManager.CTnicknameText.Length; i++)
                {
                    if (mainMenuManager.CTnicknameText[i].text == _nickname)
                    {
                        ctIndex = i;
                        break;
                    }
                }

                // TnicknameText���� �� ���ڿ� ã��
                int tIndex = -1;
                for (int j = 0; j < mainMenuManager.TnicknameText.Length; j++)
                {
                    if (string.IsNullOrEmpty(mainMenuManager.TnicknameText[j].text))
                    {
                        tIndex = j;
                        break;
                    }
                }

                // �� ���� ����
                if (ctIndex != -1 && tIndex != -1)
                {
                    RemoveNicknameS(_nickname);
                    teamCode = true;
                    AddNicknameServer(_nickname);
                }
            }
        }


        [ServerRpc(RequireOwnership = false)]
        private void RequestChangeTeamServerRpc(string _nickname, bool _teamCode)
        {
            // �������� ���� �� ���� ó�� ������ �����մϴ�.
            ChangeTeams(_nickname, _teamCode);


        }


        public void TTeam()
        {
            if (base.IsOwner)
            {
                RequestChangeTeamServerRpc(nickname, false);
                Debug.Log("��ư �׽�Ʈ");
            }
        

        }

        public void CTTeam()
        {
            if (base.IsOwner)
            {
                RequestChangeTeamServerRpc(nickname, true);
                Debug.Log("��ư �׽�Ʈ");
            }
        }
    }
}