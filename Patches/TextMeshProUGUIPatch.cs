using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(TextMeshProUGUI))]
    internal class TextMeshProUGUIPatch
    {
        
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        private static void Awake_Postfix(TextMeshProUGUI __instance)
        {
            try
            {
                Translate(__instance);
            }catch (Exception e)
            {
                Plugin.mls.LogError("TMP 텍스트를 번역하는 과정에서 오류가 발생했습니다!\n"+ e);
            }
        }

        static void Translate(TextMeshProUGUI __instance)
        {
            if (SceneManager.GetActiveScene().name == "ColdOpen1")
            {
                __instance.text = __instance.text.Replace("Detecting difficulties...", "문제 감지 중...");
                __instance.text = __instance.text.Replace("Running reboot diagnostic...", "재부팅 진단 실행 중...");
                __instance.text = __instance.text.Replace("UNABLE TO START.", "시작할 수 없습니다.");
                __instance.text = __instance.text.Replace("PLEASE FIX ISSUES", "문제를 해결해주세요");
                __instance.text = __instance.text.Replace("I BELIEVE IN YOU", "당신을 믿습니다");
            }
            else if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                __instance.text = __instance.text.Replace("Welcome back!", "돌아오신 것을 환영합니다!");
                __instance.text = __instance.text.Replace("This update includes a new moon, a new creature, and a bunny suit, as well as many adjustments.", "이번 업데이트에는 새로운 위성, 생명체, 토끼 슈트와 많은 조정 사항이 포함되어 있습니다.");
                __instance.text = __instance.text.Replace("This update includes the Company Cruiser truck and a couple new creatures.", "이번 업데이트에는 회사 크루저 트럭과 몇 가지 새로운 생명체가 포함되어 있습니다.");
                __instance.text = __instance.text.Replace("This update introduces the mineshaft, a new creature, and new scrap to collect.", "이번 업데이트에서는 폐광, 새로운 생명체, 그리고 수집할 수 있는 새로운 폐품이 추가되었습니다.");
                __instance.text = __instance.text.Replace("Others must update their game to play with you on this version.", "이 버전에서 당신과 함께 플레이하려면 다른 사람들도 게임을 업데이트해야 합니다.");
                __instance.text = __instance.text.Replace("Good luck!", "행운을 빕니다!");
                __instance.text = __instance.text.Replace("Have fun!", "즐거운 시간 되세요!");

                __instance.text = __instance.text.Replace("Thanks to PuffoThePufferfish and Blueray901165 for helping shape this game. They were the closest with it through the entire process, helping me test and improve it every step of the way.",
                    "이 게임을 만드는 데 도움을 준 PuffoThePufferfish와 Blueray901165에게 감사드립니다. 이 두 사람은 게임을 만드는 모든 과정에서 가장 가까이에 있었고, 게임을 테스트하고 개선하는 데 도움을 주었습니다.");
                __instance.text = __instance.text.Replace("Thanks to my patrons who have generously supported me throughout this game's long and sometimes-rocky development:",
                    "이 게임의 길고 때로는 험난했던 개발 기간 동안 아낌없는 성원을 보내주신 후원자 여러분께 감사드립니다:");
                __instance.text = __instance.text.Replace("Thanks to Zenonclaw for modelling the shovel and the old elevator, which was scrapped long ago but still used for its parts.",
                    "삽과 오래 전에 폐기되었지만 일부분 사용되고 있는 엘리베이터를 모델링해준 Zenonclaw에게 감사드립니다.");
                __instance.text = __instance.text.Replace("Thanks to ZedFox for one of the boombox tracks (the good one) and the disco ball music.",
                    "붐박스 트랙 중 하나(좋은 거)와 디스코 볼 음악을 제공한 ZedFox에게 감사드립니다.");
                __instance.text = __instance.text.Replace("Thanks to youreashotgun for the snare flea TV channel.",
                    "올무 벼룩 TV 채널을 보내주신 youreashotgun에게 감사드립니다.");
                __instance.text = __instance.text.Replace("And thanks to Noah, Seth, Null, Sam, Zenonclaw, ZedFox, and Joseph for helping playtest throughout early development.",
                    "그리고 초기 개발 기간 동안 플레이 테스트를 도와준 Noah, Seth, Null, Sam, Zenonclaw, ZedFox와 Joseph에게도 감사의 인사를 전합니다.");
                __instance.text = __instance.text.Replace("Thanks to psyberartist for \"copperplate\" licensed under CC BY 2.0 Deed:",
                    "CC BY 2.0 저작자표시허락을 받은 \"copperplate\"에 대해 psyberartist에게 감사드립니다:");
                __instance.text = __instance.text.Replace("Sound effects from Freesound.org, licensed under CC-BY (Attribution). Thank you:",
                    "CC-BY(저작자표시)에 따라 라이선스가 부여된 Freesound.org의 효과음의 저자에게도 감사의 말씀을 드립니다:");
            }
            else if (SceneManager.GetActiveScene().name == "InitSceneLaunchOptions")
            {
                __instance.text = __instance.text.Replace("This experience has been designed for in-game voice chat, so I recommend giving it a try.",
                    "이 게임은 게임 내 음성 채팅을 사용하는 것을 전제로 설계되었습니다. 게임 내 음성 채팅 사용을 권장합니다.");
                __instance.text = __instance.text.Replace("Adjust screen brightness until the symbol on the right is barely visible.",
                    "오른쪽 아이콘이 거의 보이지 않을 때까지 화면 밝기를 조정하세요.");
            }
            switch (__instance.text)
            {
                case "  Online":
                    __instance.text = "  온라인";
                    break;
                case "> Online":
                    __instance.text = "> 온라인";
                    break;
                case "(Recommended)":
                    __instance.text = "(권장)";
                    break;
                case " SET-UP":
                    __instance.text = "설정";
                    break;
                case "SET-UP":
                    __instance.text = "설정";
                    break;

                case "Tip: This may occur due to an antivirus or other software halting the save file from being read.":
                    __instance.text = "팁: 이는 바이러스 백신이나 기타 소프트웨어로 인해 저장 파일 읽기가 중단되었기 때문에 발생할 수 있습니다.";
                    break;
                case "Your files could not be loaded and may be corrupted. To start the game, the files can be deleted.":
                    __instance.text = "파일을 불러올 수 없습니다. 저장 파일이 손상되었을 수 있습니다. 파일을 삭제하면 게임이 정상적으로 실행될 것입니다.";
                    break;
                case "The game will now close so you can restart it.":
                    __instance.text = "이제 게임이 종료됩니다. 게임을 재시작하세요.";
                    break;
                case "[ Delete files and restart]":
                    __instance.text = "[모든 저장 파일 삭제하기]";
                    break;

                case "> Host":
                    __instance.text = "> 호스트";
                    break;
                case "> Join a crew":
                    __instance.text = "> 팀에 합류하기";
                    break;
                case "> Join LAN session":
                    __instance.text = "> LAN 세션 합류하기";
                    break;
                case "> Settings":
                    __instance.text = "> 설정";
                    break;
                case "> Credits":
                    __instance.text = "> 크레딧";
                    break;
                case "> Quit":
                    __instance.text = "> 종료";
                    break;

                case "> Resume":
                    __instance.text = "> 계속하기";
                    break;
                case "Would you like to leave the game?":
                    __instance.text = "정말 게임을 떠나시겠습니까?";
                    break;
                case "> Invite friends":
                    __instance.text = "> 친구 초대하기";
                    break;

                case "ACCESSIBILITY":
                    __instance.text = "접근성";
                    break;
                case "Unconfirmed changes!":
                    __instance.text = "변경 사항이 저장되지 않음!";
                    break;
                case "CONTROLS":
                    __instance.text = "조작";
                    break;
                case "REMAP CONTROLS":
                    __instance.text = "조작 키 재설정";
                    break;
                case "DISPLAY":
                    __instance.text = "디스플레이";
                    break;
                case "ENABLED":
                    __instance.text = "활성화됨";
                    break;
                case "Save File":
                    __instance.text = "저장 파일";
                    break;
                case "Server name:":
                    __instance.text = "서버 이름:";
                    break;
                case "Host LAN Server:":
                    __instance.text = "LAN 서버 호스트하기:";
                    break;

                case "Sort: worldwide":
                    __instance.text = "정렬: 전 세계";
                    break;
                case "Sort: Friends":
                    __instance.text = "정렬: 친구";
                    break;
                case "Sort: near":
                    __instance.text = "정렬: 가까운 서버";
                    break;
                case "Sort: far":
                    __instance.text = "정렬: 먼 서버";
                    break;

                case "Fullscreen":
                    __instance.text = "전체 화면";
                    break;
                case "Use monitor (V-Sync)":
                    __instance.text = "모니터 사용 (수직 동기화)";
                    break;

                case "Display mode:":
                    __instance.text = "디스플레이 모드:";
                    break;
                case "Frame rate cap:":
                    __instance.text = "프레임 제한:";
                    break;

                case "(Launched in LAN mode)":
                    __instance.text = "(LAN 모드로 실행됨)";
                    break;

                case "Servers":
                    __instance.text = "서버";
                    break;
                case "Weekly Challenge Results":
                    __instance.text = "주간 챌린지 결과";
                    break;
                case "Loading server list...":
                    __instance.text = "서버 목록 불러오는 중...";
                    break;
                case "Loading ranking...":
                    __instance.text = "순위 불러오는 중...";
                    break;
                case "Loading...":
                    __instance.text = "로딩 중...";
                    break;
                case "Join":
                    __instance.text = "참가";
                    break;

                case "Version 50 is here!":
                    __instance.text = "버전 50이 출시되었습니다!";
                    break;
                case "Version 55 is here!":
                    __instance.text = "버전 55가 출시되었습니다!";
                    break;
                case "Version 60 is here!":
                    __instance.text = "버전 60이 출시되었습니다!";
                    break;

                case "Credits":
                    __instance.text = "크레딧";
                    break;
                case "An error occured!":
                    __instance.text = "오류가 발생했습니다!";
                    break;
                case "Do you want to delete File 1?":
                    __instance.text = "정말 파일 1을 삭제할까요?";
                    break;
                case "Confirm changes?":
                    __instance.text = "변경 사항을 저장할까요?";
                    break;
                case "You are in LAN mode. When allowing remote connections through LAN, please ensure you have sufficient network security such as a firewall and/or VPN.":
                    __instance.text = "LAN 모드에 있습니다. LAN을 통한 원격 연결을 허용하는 경우 방화벽 및/또는 VPN과 같은 네트워크 보안이 충분한지 확인하십시오.";
                    break;
                case "Enter a tag...":
                    __instance.text = "태그를 입력하세요...";
                    break;
                case "Enter server tag...":
                    __instance.text = "서버 태그를 입력하세요...";
                    break;
                case "Name your server...":
                    __instance.text = "서버 이름을 입력하세요...";
                    break;
                case "PRIVATE means you must send invites through Steam for players to join.":
                    __instance.text = "비공개로 설정하면 Steam을 통해 플레이어에게 초대를 보내야 합니다.";
                    break;
                case "MODE: Voice activation":
                    __instance.text = "모드: 음성 감지";
                    break;
                case "Push to talk:":
                    __instance.text = "눌러서 말하기";
                    break;
                case "Gamma/Brightness:":
                    __instance.text = "감마/밝기:";
                    break;
                case "Master volume:":
                    __instance.text = "주 음량:";
                    break;
                case "Look sensitivity:":
                    __instance.text = "마우스 감도:";
                    break;
                case "Invert Y-Axis":
                    __instance.text = "Y축 반전";
                    break;
                case "Arachnophobia Mode":
                    __instance.text = "거미공포증 모드";
                    break;
                case "Discard":
                    __instance.text = "취소";
                    break;
                case "Confirm":
                    __instance.text = "확인";
                    break;
                case "> Set to defaults":
                    __instance.text = "> 기본값으로 설정";
                    break;
                case "> Reset all to default":
                    __instance.text = "> 기본값으로 재설정";
                    break;
                case "> Back":
                    __instance.text = "> 뒤로";
                    break;
                case "> Confirm changes":
                    __instance.text = "> 변경 사항 저장";
                    break;
                case "> Confirm":
                    __instance.text = "> 확인";
                    break;
                case "> Cancel":
                    __instance.text = "> 취소";
                    break;
                case "> CONFIRM":
                    __instance.text = "> 확인";
                    break;
                case "  CONFIRM":
                    __instance.text = "  확인";
                    break;
                case "> Change keybinds":
                    __instance.text = "> 조작 키 변경";
                    break;
                case "[ Refresh ]":
                    __instance.text = "[ 새로고침 ]";
                    break;
                case "> Back to menu":
                    __instance.text = "> 메뉴로 돌아가기";
                    break;
                case "With challenge moon":
                    __instance.text = "챌린지 달 포함";
                    break;
                case "[ Back ]":
                    __instance.text = "[ 뒤로 ]";
                    break;
                case "[ Confirm ]":
                    __instance.text = "[ 확인 ]";
                    break;
                case "[ Remove my score ]":
                    __instance.text = "[ 점수 삭제하기 ]";
                    break;
                case "[ Play again ]":
                    __instance.text = "[ 다시 하기 ]";
                    break;
                case "Local-only":
                    __instance.text = "로컬 전용";
                    break;
                case "File 1":
                    __instance.text = "파일 1";
                    break;
                case "File 2":
                    __instance.text = "파일 2";
                    break;
                case "File 3":
                    __instance.text = "파일 3";
                    break;
                case "Input: ":
                    __instance.text = "입력: ";
                    break;
                case "CHALLENGE":
                    __instance.text = "챌린지";
                    break;
                case "[ Continue ]":
                    __instance.text = "[ 계속 ]";
                    break;
                case "Delete":
                    __instance.text = "삭제";
                    break;
                case "Public":
                    __instance.text = "공개";
                    break;
                case "Friends-only":
                    __instance.text = "친구 전용";
                    break;
                case "Allow remote connections":
                    __instance.text = "원격 연결 허용";
                    break;
                case "Go back":
                    __instance.text = "뒤로 가기";
                    break;
                case "File incompatible!":
                    __instance.text = "파일 호환되지 않음!";
                    break;
                case "Waiting for input":
                    __instance.text = "입력 대기 중";
                    break;

                case "HANDS FULL":
                    __instance.text = "양 손 사용 중";
                    break;
                case "Walk : [W/A/S/D]":
                    __instance.text = "걷기 : [W/A/S/D]";
                    break;
                case "Sprint: [Shift]":
                    __instance.text = "달리기: [Shift]";
                    break;
                case "Scan : [RMB]":
                    __instance.text = "스캔 : [RMB]";
                    break;
                case "SYSTEMS ONLINE":
                    __instance.text = Plugin.systemOnlineText;
                    break;
                case "Typing...":
                    __instance.text = "입력 중...";
                    break;
                case "Press \"/\" to talk.":
                    __instance.text = "\"/\"를 눌러 대화합니다.";
                    break;
                case "(Some were too far to receive your message.)":
                    __instance.text = "(일부는 너무 멀어 메세지를 받지 못했습니다.)";
                    break;
                case "Confirm: [V]   |   Rotate: [R]   |   Store: [X]":
                    __instance.text = "확정: [V]   |   회전: [R]   |   보관: [X]";
                    break;
                case "CRITICAL INJURY":
                    __instance.text = Plugin.injuryText;
                    break;
                case "Paycheck!":
                    __instance.text = Plugin.sellText;
                    break;
                case "TOTAL:":
                    __instance.text = "합계:";
                    break;
                case "YOU ARE FIRED.":
                    __instance.text = Plugin.firedText;
                    break;
                case "You will keep your employee rank. Your ship and credits will be reset.":
                    __instance.text = "직원 계급은 유지되지만, 당신의 함선과 크레딧은 초기화됩니다.";
                    break;
                case "You did not meet the profit quota before the deadline.":
                    __instance.text = "마감일 전까지 수익 할당량을 충족하지 못했습니다.";
                    break;
                case "TO MEET PROFIT QUOTA":
                    __instance.text = "수익 할당량 충족까지";
                    break;
                case "QUOTA REACHED!":
                    __instance.text = "<size=65>" + Plugin.quotaReached + "</size>";
                    break;
                case "NEW PROFIT QUOTA:":
                    __instance.text = "새로운 수익 할당량:";
                    break;
                case "Stats":
                    __instance.text = "통계";
                    break;
                case "[LIFE SUPPORT: OFFLINE]":
                    __instance.text = Plugin.deathText;
                    break;
                case "(Dead)":
                    __instance.text = "(사망)";
                    break;
                case "Tell autopilot ship to leave early : [RMB] (Hold)":
                    __instance.text = "함선에게 일찍 출발하라고 지시하기\n: [RMB] (Hold)";
                    break;
                case "HAZARD LEVEL:":
                    __instance.text = "위험 수준:";
                    break;

                case "Notes:":
                    __instance.text = "노트:";
                    break;
                case "PERFORMANCE REPORT":
                    __instance.text = "성과 보고서";
                    break;


                case "DEBUG/TEST":
                    __instance.text = "디버그/테스트";
                    break;
                case "Enemy type:":
                    __instance.text = "적 종류:";
                    break;
                case "Enemy:":
                    __instance.text = "적:";
                    break;
                case "Number to spawn:":
                    __instance.text = "생성할 수:";
                    break;
                case "Enter text...":
                    __instance.text = "텍스트를 입력하세요...";
                    break;
                case "Spawn creature":
                    __instance.text = "생명체 생성";
                    break;
                case "Spawn item":
                    __instance.text = "아이템 생성";
                    break;
                case "Toggle test room":
                    __instance.text = "테스트 방 전환";
                    break;
                case "Revive players":
                    __instance.text = "플레이어 소생";
                    break;
                case "Toggle invincibility":
                    __instance.text = "무적 모드 전환";
                    break;
                case "Item:":
                    __instance.text = "아이템:";
                    break;
                case "EMPLOYEE RANK":
                    __instance.text = "직원 계급";
                    break;
                case "RECEIVING SIGNAL":
                    __instance.text = "    신호 수신 중";
                    break;

                case "DOOR HYDRAULICS:":
                    __instance.text = "문 유압 장치:";
                    break;

                case "EMERGENCY WEATHER ALERT":
                    __instance.text = "<size=50>긴급 기상 경보";
                    break;
                case "METEOR SHOWERS DETECTED. UP TO 20-30 PER HOUR. TAKE SHELTER IMMEDIATELY.":
                    __instance.text = "유성우가 감지되었습니다. 시간당 최대 20-30개까지 발생할 수 있습니다. 즉시 대피하십시오.";
                    break;
                case "YOUR AUTOPILOT SATELLITE\nISSUED A\nWEATHER WARNING":
                    __instance.text = "<size=35>자동항법위성이\n기상 경보를\n발령했습니다";
                    __instance.lineSpacing = 32;
                    break;
            }
            __instance.text.Replace(" collected!", " 수집함!");
            __instance.text.Replace("Value: ", "가치: ");

            if (__instance.text.Contains("Boot Distributioner Application v0.04"))
            {
                __instance.text = "      BG IG, 시스템 행동 연합\r\n      Copyright (C) 2084-2108, Halden Electronics Inc.\r\n\r\nCPU 종류       :     BORSON 300 CPU at 2500 MHz\r\n메모리 테스트  :      4521586K OK\r\n\r\n부트 분배기 애플리케이션 v0.04\r\nCopyright (C) 2107 Distributioner\r\n    Sting X 롬 감지\r\n    웹 LNV 확장기 감지\r\n    심박수 감지 OK\r\n\r\n\r\nUTGF 장치 수신 중...\r\n\r\n신체    ID     신경     장치 클래스\r\n________________________________________\r\n\r\n2      52   Jo152       H515\r\n2      52   Sa5155      H515\r\n2      52   Bo75        H515\r\n2      52   Eri510      H515\r\n1      36   Ell567      H515\r\n1      36   Jos912      H515\r\n0\r\n";
            }
            else if (__instance.text.Contains("You have one day to make as much profit as possible."))
            {
                __instance.text = "주간 챌린지 달입니다. 하루 안에 가능한 한 많은 수익을 얻으세요. 원하는 만큼 다시 시도할 수 있습니다.";
            }
        }
    }
}
