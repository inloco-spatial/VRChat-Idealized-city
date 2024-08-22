using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace FootStepSystem{
    public class FootStepSimple : UdonSharpBehaviour
    {
        [SerializeField] GameObject FootStepPrefab;

        [Space(10)]
        [Header("ユーザー設定項目")]

        [Tooltip("自分だけに鳴らすか")]
        public bool LocalFlag = false;

        [Tooltip("アップロード時に入れるCapacity")]
        [SerializeField] int WorldCapacity = 16;

        [Tooltip("足音が鳴り始める速度。walk speedの2/3推奨")]
        [SerializeField] float SpeedThreshold = 3f;

        [Tooltip("速度によるピッチの変化係数。run speedと掛けて1未満推奨")]
        [SerializeField] float SpeedCoefficient = 0.15f;

        [Tooltip("デフォルトのピッチ")]
        [SerializeField] float defaultPitch = 1;
        
        [Tooltip("デフォルトで使う足音")]
        [SerializeField] AudioClip FootStepSound;
        AudioSource[] playerAudioSourceArray;
        VRCPlayerApi[] players;
        bool[] previousGrounded;
        bool presentGrounded = true;
        VRCPlayerApi _localPlayer;
        float playerVelocity = 0;
        int playerIndex = 0, maxPlayrNum = 0;
        
        void Start()
        {
            maxPlayrNum = LocalFlag ? 1 : 2 * WorldCapacity;

            playerAudioSourceArray = new AudioSource[maxPlayrNum];
            players = new VRCPlayerApi[maxPlayrNum];
            previousGrounded = new bool[maxPlayrNum];

            GameObject temp;
            for(int i=0; i<maxPlayrNum; i++){
                temp = Object.Instantiate(FootStepPrefab);
                temp.name = i.ToString();
                playerAudioSourceArray[i] = temp.GetComponent<AudioSource>();
                temp.transform.parent = this.transform;
                previousGrounded[i] = true;
            }

            _localPlayer = Networking.LocalPlayer;
            GetPlayersAllay();
        }
        void Update()
        {
            if(_localPlayer != null){
                playerIndex = 0;
                foreach(VRCPlayerApi player in players){
                    if(player == null) continue;
                    if(!player.IsValid()) continue;
                    if(LocalFlag && player != _localPlayer) continue;

                    presentGrounded = player.IsPlayerGrounded();
                    if(previousGrounded[playerIndex] || presentGrounded){
                        if(playerAudioSourceArray[playerIndex].clip == null){
                            playerAudioSourceArray[playerIndex].clip = FootStepSound;
                            playerAudioSourceArray[playerIndex].Play();
                        }

                        playerVelocity = player.GetVelocity().magnitude;
                        if(playerVelocity > SpeedThreshold){
                            playerAudioSourceArray[playerIndex].mute = false;
                            playerAudioSourceArray[playerIndex].pitch = (float)(playerVelocity * SpeedCoefficient + defaultPitch);
                            playerAudioSourceArray[playerIndex].transform.position = player.GetPosition();
                        }else{
                            playerAudioSourceArray[playerIndex].mute = true;
                        }

                    }else{
                        playerAudioSourceArray[playerIndex].mute = true;
                    }
                    previousGrounded[playerIndex] = presentGrounded;
                    playerIndex++;   
                }
            }
            
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            GetPlayersAllay();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            //いなくなった人のが残らないように一旦一律ミュート
            foreach(AudioSource eachAudio in playerAudioSourceArray){
                eachAudio.mute = true;
            }
        }
        private void GetPlayersAllay(){
            if(maxPlayrNum < 1) return;
            if(maxPlayrNum == 1){
                players[0] = _localPlayer;
                return;
            }else{
                VRCPlayerApi.GetPlayers(players);
            }   
        }
    }
}
