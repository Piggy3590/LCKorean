using BepInEx.Logging;
using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using Newtonsoft.Json.Linq;
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
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace LCKorean.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SaveItemsInShip")]
        private static void SaveItemsInShip_Prefix()
        {
            try
            {
                UntranslateItem();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("아이템 번역을 해제하는 과정에서 오류가 발생했습니다!\n" + e);
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("SaveItemsInShip")]
        private static void SaveItemsInShip_Postfix()
        {
            try
            {
                TranslateItem();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("아이템을 재번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("Disconnect")]
        private static void Disconnect_Postfix()
        {
            try
            {
                UntranslateItem();
            }
            catch (Exception e)
            {
                Plugin.mls.LogError("아이템을 재번역하는 과정에서 오류가 발생했습니다!\n" + e);
            }
        }


        static void UntranslateItem()
        {
            Plugin.mls.LogInfo("Untranslating Items");
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.itemName)
                {
                    case "붐박스":
                        item.itemName = "Boombox";
                        break;
                    case "손전등":
                        item.itemName = "Flashlight";
                        break;
                    case "제트팩":
                        item.itemName = "Jetpack";
                        break;
                    case "열쇠":
                        item.itemName = "Key";
                        break;
                    case "자물쇠 따개":
                        item.itemName = "Lockpicker";
                        break;
                    case "장치":
                        item.itemName = "Apparatus";
                        break;
                    case "프로 손전등":
                        item.itemName = "Pro-flashlight";
                        break;
                    case "철제 삽":
                        item.itemName = "Shovel";
                        break;
                    case "기절 수류탄":
                        item.itemName = "Stun grenade";
                        break;
                    case "연장형 사다리":
                        item.itemName = "Extension ladder";
                        break;
                    case "TZP-흡입제":
                        item.itemName = "TZP-Inhalant";
                        break;
                    case "무전기":
                        item.itemName = "Walkie-talkie";
                        break;
                    case "잽건":
                        item.itemName = "Zap gun";
                        break;
                    case "마법의 7번 공":
                        item.itemName = "Magic 7 ball";
                        break;
                    case "에어혼":
                        item.itemName = "Airhorn";
                        break;
                    case "종":
                        item.itemName = "Bell";
                        break;
                    case "큰 나사":
                        item.itemName = "Big bolt";
                        break;
                    case "병 묶음":
                        item.itemName = "Bottles";
                        break;
                    case "빗":
                        item.itemName = "Brush";
                        break;
                    case "사탕":
                        item.itemName = "Candy";
                        break;
                    case "금전 등록기":
                        item.itemName = "Cash register";
                        break;
                    case "화학 용기":
                        item.itemName = "Chemical jug";
                        break;
                    case "광대 나팔":
                        item.itemName = "Clown horn";
                        break;
                    case "대형 축":
                        item.itemName = "Large axle";
                        break;
                    case "틀니":
                        item.itemName = "Teeth";
                        break;
                    case "쓰레받기":
                        item.itemName = "Dust pan";
                        break;
                    case "달걀 거품기":
                        item.itemName = "Egg beater";
                        break;
                    case "V형 엔진":
                        item.itemName = "V-type engine";
                        break;
                    case "황금 컵":
                        item.itemName = "Golden cup";
                        break;
                    case "멋진 램프":
                        item.itemName = "Fancy lamp";
                        break;
                    case "그림":
                        item.itemName = "Painting";
                        break;
                    case "플라스틱 물고기":
                        item.itemName = "Plastic fish";
                        break;
                    case "레이저 포인터":
                        item.itemName = "Laser pointer";
                        break;
                    case "금 주괴":
                        item.itemName = "Gold bar";
                        break;
                    case "헤어 드라이기":
                        item.itemName = "Hairdryer";
                        break;
                    case "돋보기":
                        item.itemName = "Magnifying glass";
                        break;
                    case "금속 판":
                        item.itemName = "Metal sheet";
                        break;
                    case "쿠키 틀":
                        item.itemName = "Cookie mold pan";
                        break;
                    case "머그잔":
                        item.itemName = "Mug";
                        break;
                    case "향수 병":
                        item.itemName = "Perfume bottle";
                        break;
                    case "구식 전화기":
                        item.itemName = "Old phone";
                        break;
                    case "피클 병":
                        item.itemName = "Jar of pickles";
                        break;
                    case "약 병":
                        item.itemName = "Pill bottle";
                        break;
                    case "리모컨":
                        item.itemName = "Remote";
                        break;
                    case "반지":
                        item.itemName = "Ring";
                        break;
                    case "장난감 로봇":
                        item.itemName = "Toy robot";
                        break;
                    case "고무 오리":
                        item.itemName = "Rubber Ducky";
                        break;
                    case "빨간색 소다":
                        item.itemName = "Red soda";
                        break;
                    case "운전대":
                        item.itemName = "Steering wheel";
                        break;
                    case "정지 표지판":
                        item.itemName = "Stop sign";
                        break;
                    case "찻주전자":
                        item.itemName = "Tea kettle";
                        break;
                    case "치약":
                        item.itemName = "Toothpaste";
                        break;
                    case "장난감 큐브":
                        item.itemName = "Toy cube";
                        break;
                    case "벌집":
                        item.itemName = "Hive";
                        break;
                    case "레이더 부스터":
                        item.itemName = "Radar-booster";
                        break;
                    case "양보 표지판":
                        item.itemName = "Yield sign";
                        break;
                    case "산탄총":
                        item.itemName = "Shotgun";
                        break;
                    case "탄약":
                        item.itemName = "Ammo";
                        break;
                    case "페인트 스프레이":
                        item.itemName = "Spray paint";
                        break;
                    case "사제 섬광탄":
                        item.itemName = "Homemade flashbang";
                        break;
                    case "선물":
                        item.itemName = "Gift";
                        break;
                    case "플라스크":
                        item.itemName = "Flask";
                        break;
                    case "비극":
                        item.itemName = "Tragedy";
                        break;
                    case "희극":
                        item.itemName = "Comedy";
                        break;
                    case "방귀 쿠션":
                        item.itemName = "Whoopie cushion";
                        break;
                    case "식칼":
                        item.itemName = "Kitchen knife";
                        break;
                    case "부활절 달걀":
                        item.itemName = "Easter egg";
                        break;
                }
            }
        }

        static void UntranslateModdedItem()
        {
            Plugin.mls.LogInfo("Untranslating Modded Items");
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.itemName)
                {

                    case "알코올 플라스크":
                        item.itemName = "Alcohol Flask";
                        break;
                    case "모루":
                        item.itemName = "Anvil";
                        break;
                    case "야구 방망이":
                        item.itemName = "Baseball bat";
                        break;
                    case "맥주 캔":
                        item.itemName = "Beer can";
                        break;
                    case "벽돌":
                        item.itemName = "Brick";
                        break;
                    case "망가진 엔진":
                        item.itemName = "Broken engine";
                        break;
                    case "양동이":
                        item.itemName = "Bucket";
                        break;
                    case "페인트 캔":
                        item.itemName = "Can paint";
                        break;
                    case "수통":
                        item.itemName = "Canteen";
                        break;
                    case "자동차 배터리":
                        item.itemName = "Car battery";
                        break;
                    case "조임틀":
                        item.itemName = "Clamp";
                        break;
                    case "시계":
                        item.itemName = "Clock";
                        break;
                    case "멋진 그림":
                        item.itemName = "Fancy Painting";
                        break;
                    case "선풍기":
                        item.itemName = "Fan";
                        break;
                    case "소방 도끼":
                        item.itemName = "Fireaxe";
                        break;
                    case "소화기":
                        item.itemName = "Fire extinguisher";
                        break;
                    case "소화전":
                        item.itemName = "Fire hydrant";
                        break;
                    case "통조림":
                        item.itemName = "Food can";
                        break;
                    case "게임보이":
                        item.itemName = "Gameboy";
                        break;
                    case "쓰레기":
                        item.itemName = "Garbage";
                        break;
                    case "망치":
                        item.itemName = "Hammer";
                        break;
                    case "기름통":
                        item.itemName = "Jerrycan";
                        break;
                    case "키보드":
                        item.itemName = "Keyboard";
                        break;
                    case "랜턴":
                        item.itemName = "Lantern";
                        break;
                    case "도서관 램프":
                        item.itemName = "Library lamp";
                        break;
                    case "식물":
                        item.itemName = "Plant";
                        break;
                    case "플라이어":
                        item.itemName = "Pliers";
                        break;
                    case "뚫어뻥":
                        item.itemName = "Plunger";
                        break;
                    case "레트로 장난감":
                        item.itemName = "Retro Toy";
                        break;
                    case "스크류 드라이버":
                        item.itemName = "Screwdriver";
                        break;
                    case "싱크대":
                        item.itemName = "Sink";
                        break;
                    case "소켓 렌치":
                        item.itemName = "Socket Wrench";
                        break;
                    case "고무 오리":
                        item.itemName = "Squeaky toy";
                        break;
                    case "여행 가방":
                        item.itemName = "Suitcase";
                        break;
                    case "토스터기":
                        item.itemName = "Toaster";
                        break;
                    case "공구 상자":
                        item.itemName = "Toolbox";
                        break;
                    case "실크햇":
                        item.itemName = "Top hat";
                        break;
                    case "라바콘":
                        item.itemName = "Traffic cone";
                        break;
                    case "환풍구":
                        item.itemName = "Vent";
                        break;
                    case "물뿌리개":
                        item.itemName = "Watering Can";
                        break;
                    case "바퀴":
                        item.itemName = "Wheel";
                        break;
                    case "와인 병":
                        item.itemName = "Wine bottle";
                        break;
                    case "렌치":
                        item.itemName = "Wrench";
                        break;
                    case "주사기":
                        item.itemName = "Syringe";
                        break;
                    case "주사기총":
                        item.itemName = "Syringe Gun";
                        break;
                    case "코너 파이프":
                        item.itemName = "Corner Pipe";
                        break;
                    case "작은 파이프":
                        item.itemName = "Small Pipe";
                        break;
                    case "파이프":
                        item.itemName = "Flow Pipe";
                        break;
                    case "뇌가 담긴 병":
                        item.itemName = "Brain Jar";
                        break;
                    case "호두까기 인형 장난감":
                        item.itemName = "Toy Nutcracker";
                        break;
                    case "시험관":
                        item.itemName = "Test Tube";
                        break;
                    case "시험관 랙":
                        item.itemName = "Test Tube Rack";
                        break;
                    case "호두까기 인형 눈":
                        item.itemName = "Nutcracker Eye";
                        break;
                    case "파란색 시험관":
                        item.itemName = "Blue Test Tube";
                        break;
                    case "노란색 시험관":
                        item.itemName = "Yellow Test Tube";
                        break;
                    case "빨간색 시험관":
                        item.itemName = "Red Test Tube";
                        break;
                    case "초록색 시험관":
                        item.itemName = "Green Test Tube";
                        break;
                    case "쇠지렛대":
                        item.itemName = "Crowbar";
                        break;
                    case "플젠":
                        item.itemName = "Plzen";
                        break;
                    case "컵":
                        item.itemName = "Cup";
                        break;
                    case "전자레인지":
                        item.itemName = "Microwave";
                        break;
                    case "비눗방울 총":
                        item.itemName = "bubblegun";
                        break;
                    case "망가진 P88":
                        item.itemName = "Broken P88";
                        break;
                    case "직원":
                        item.itemName = "employee";
                        break;
                    case "지뢰":
                        item.itemName = "Mine";
                        break;
                    case "탄약 상자":
                        item.itemName = "Ammo crate";
                        break;
                    case "음료수":
                        item.itemName = "Drink";
                        break;
                    case "라디오":
                        item.itemName = "Radio";
                        break;
                    case "마우스":
                        item.itemName = "Mouse";
                        break;
                    case "모니터":
                        item.itemName = "Monitor";
                        break;
                    case "건전지":
                        item.itemName = "Battery";
                        break;
                    case "대포":
                        item.itemName = "Cannon";
                        break;
                    case "건강 음료":
                        item.itemName = "Health Drink";
                        break;
                    case "화학 약품":
                        item.itemName = "Chemical";
                        break;
                    case "소독용 알코올":
                        item.itemName = "Disinfecting Alcohol";
                        break;
                    case "앰풀":
                        item.itemName = "Ampoule";
                        break;
                    case "혈액 팩":
                        item.itemName = "Blood Pack";
                        break;
                    case "라이터":
                        item.itemName = "Flip Lighter";
                        break;
                    case "고무 공":
                        item.itemName = "Rubber Ball";
                        break;
                    case "비디오 테이프":
                        item.itemName = "Video Tape";
                        break;
                    case "구급 상자":
                        item.itemName = "First Aid Kit";
                        break;
                    case "금메달":
                        item.itemName = "Gold Medallion";
                        break;
                    case "금속 파이프":
                        item.itemName = "Steel Pipe";
                        break;
                    case "도끼":
                        item.itemName = "Axe";
                        break;
                    case "비상용 망치":
                        item.itemName = "Emergency Hammer";
                        break;
                    case "카타나":
                        item.itemName = "Katana";
                        break;
                    case "은메달":
                        item.itemName = "Silver Medallion";
                        break;
                    case "휴대용 라디오":
                        item.itemName = "Pocket Radio";
                        break;
                    case "곰 인형":
                        item.itemName = "Teddy Plush";
                        break;
                    case "Hyper Acid 실험 기록":
                        item.itemName = "Experiment Log Hyper Acid";
                        break;
                    case "희극 가면 실험 기록":
                        item.itemName = "Experiment Log Comedy Mask";
                        break;
                    case "저주받은 동전 실험 기록":
                        item.itemName = "Experiment Log Cursed Coin";
                        break;
                    case "바이오 HXNV7 실험 기록":
                        item.itemName = "Experiment Log BIO HXNV7";
                        break;
                    case "파란색 폴더":
                        item.itemName = "Blue Folder";
                        break;
                    case "빨간색 폴더":
                        item.itemName = "Red Folder";
                        break;
                    case "코일":
                        item.itemName = "Coil";
                        break;
                    case "타자기":
                        item.itemName = "Typewriter";
                        break;
                    case "서류 더미":
                        item.itemName = "Documents";
                        break;
                    case "스테이플러":
                        item.itemName = "Stapler";
                        break;
                    case "구식 컴퓨터":
                        item.itemName = "Old Computer";
                        break;
                    case "브론즈 트로피":
                        item.itemName = "Bronze Trophy";
                        break;
                    case "바나나":
                        item.itemName = "Banana";
                        break;
                    case "스턴봉":
                        item.itemName = "Stun Baton";
                        break;
                    case "바이오-HXNV7":
                        item.itemName = "BIO-HXNV7";
                        break;
                    case "복구된 비밀 일지":
                        item.itemName = "Recovered Secret Log";
                        break;
                    case "황금 단검 실험 기록":
                        item.itemName = "Experiment Log Golden Dagger";
                        break;
                    case "대합":
                        item.itemName = "Clam";
                        break;
                    case "거북이 등딱지":
                        item.itemName = "Turtle Shell";
                        break;
                    case "생선 뼈":
                        item.itemName = "Fish Bones";
                        break;
                    case "뿔 달린 껍질":
                        item.itemName = "Horned Shell";
                        break;
                    case "도자기 찻잔":
                        item.itemName = "Porcelain Teacup";
                        break;
                    case "대리석":
                        item.itemName = "Marble";
                        break;
                    case "도자기 병":
                        item.itemName = "Porcelain Bottle";
                        break;
                    case "도자기 향수 병":
                        item.itemName = "Porcelain Perfume Bottle";
                        break;
                    case "발광구":
                        item.itemName = "Glowing Orb";
                        break;
                    case "황금 해골":
                        item.itemName = "Golden Skull";
                        break;
                    case "코스모코스 지도":
                        item.itemName = "Map of Cosmocos";
                        break;
                    case "젖은 노트 1":
                        item.itemName = "Wet Note 1";
                        break;
                    case "젖은 노트 2":
                        item.itemName = "Wet Note 2";
                        break;
                    case "젖은 노트 3":
                        item.itemName = "Wet Note 3";
                        break;
                    case "젖은 노트 4":
                        item.itemName = "Wet Note 4";
                        break;
                    case "우주빛 파편":
                        item.itemName = "Cosmic Shard";
                        break;
                    case "우주 생장물":
                        item.itemName = "Cosmic Growth";
                        break;
                    case "천상의 두뇌 덩어리":
                        item.itemName = "Chunk of Celestial Brain";
                        break;
                    case "파편이 든 양동이":
                        item.itemName = "Bucket of Shards";
                        break;
                    case "우주빛 손전등":
                        item.itemName = "Cosmic Flashlight";
                        break;
                    case "잊혀진 일지 1":
                        item.itemName = "Forgotten Log 1";
                        break;
                    case "잊혀진 일지 2":
                        item.itemName = "Forgotten Log 2";
                        break;
                    case "잊혀진 일지 3":
                        item.itemName = "Forgotten Log 3";
                        break;
                    case "안경":
                        item.itemName = "Glasses";
                        break;
                    case "생장한 배양 접시":
                        item.itemName = "Grown Petri Dish";
                        break;
                    case "배양 접시":
                        item.itemName = "Petri Dish";
                        break;
                    case "코스모채드":
                        item.itemName = "Cosmochad";
                        break;
                    case "죽어가는 우주빛 손전등":
                        item.itemName = "Dying Cosmic Flashlight";
                        break;
                    case "죽어가는 우주 생장물":
                        item.itemName = "Dying Cosmic Growth";
                        break;
                    case "혈액 배양 접시":
                        item.itemName = "Blood Petri Dish";
                        break;
                    case "악마 코스모채드":
                        item.itemName = "Evil Cosmochad";
                        break;
                    case "악마 코스모":
                        item.itemName = "Evil Cosmo";
                        break;
                    case "릴 코스모":
                        item.itemName = "Lil Cosmo";
                        break;
                    case "죽어가는 생장물 배양 접시":
                        item.itemName = "Dying Grown Petri Dish";
                        break;
                    case "감시하는 배양 접시":
                        item.itemName = "Watching Petri Dish";
                        break;
                    case "현미경":
                        item.itemName = "Microscope";
                        break;
                    case "원통형 바일":
                        item.itemName = "Round Vile";
                        break;
                    case "사각형 바일":
                        item.itemName = "Square Vile";
                        break;
                    case "타원형 바일":
                        item.itemName = "Oval Vile";
                        break;
                    case "해링턴 일지 1":
                        item.itemName = "Harrington Log 1";
                        break;
                    case "해링턴 일지 2":
                        item.itemName = "Harrington Log 2";
                        break;
                    case "해링턴 일지 3":
                        item.itemName = "Harrington Log 3";
                        break;
                    case "해링턴 일지 4":
                        item.itemName = "Harrington Log 4";
                        break;
                    case "생장물이 든 병":
                        item.itemName = "Jar of Growth";
                        break;
                    case "테이프 플레이어 일지 1":
                        item.itemName = "Tape Player Log 1";
                        break;
                    case "테이프 플레이어 일지 2":
                        item.itemName = "Tape Player Log 2";
                        break;
                    case "테이프 플레이어 일지 3":
                        item.itemName = "Tape Player Log 3";
                        break;
                    case "테이프 플레이어 일지 4":
                        item.itemName = "Tape Player Log 4";
                        break;
                }
            }
        }

        static void TranslateItem()
        {
            Plugin.mls.LogInfo("Translating Items");
            foreach (Item item in StartOfRound.Instance.allItemsList.itemsList)
            {
                switch (item.itemName)
                {
                    case "Boombox":
                        item.itemName = "붐박스";
                        item.toolTips[0] = "음악 전환하기 : [RMB]";
                        break;
                    case "Flashlight":
                        item.itemName = "손전등";
                        item.toolTips[0] = "전등 전환하기 : [RMB]";
                        break;
                    case "Jetpack":
                        item.itemName = "제트팩";
                        item.toolTips[0] = "제트팩 사용하기 : [RMB]";
                        break;
                    case "Key":
                        item.itemName = "열쇠";
                        item.toolTips[0] = "열쇠 사용하기 : [RMB]";
                        break;
                    case "Lockpicker":
                        item.itemName = "자물쇠 따개";
                        item.toolTips[0] = "문에 설치하기 : [RMB]";
                        break;
                    case "Apparatus":
                        item.itemName = "장치";
                        break;
                    case "Pro-flashlight":
                        item.itemName = "프로 손전등";
                        item.toolTips[0] = "전등 전환하기 : [RMB]";
                        break;
                    case "Shovel":
                        item.itemName = "철제 삽";
                        item.toolTips[0] = "삽 휘두르기: [RMB]";
                        break;
                    case "Stun grenade":
                        item.itemName = "기절 수류탄";
                        item.toolTips[0] = "수류탄 사용하기 : [RMB]";
                        break;
                    case "Extension ladder":
                        item.itemName = "연장형 사다리";
                        item.toolTips[0] = "사다리 꺼내기 : [RMB]";
                        break;
                    case "TZP-Inhalant":
                        item.itemName = "TZP-흡입제";
                        item.toolTips[0] = "TZP 흡입하기 : [RMB]";
                        break;
                    case "Walkie-talkie":
                        item.itemName = "무전기";
                        item.toolTips[0] = "전원 버튼 : [Q]";
                        item.toolTips[1] = "목소리 송신하기 : [RMB]";
                        break;
                    case "Zap gun":
                        item.itemName = "잽건";
                        item.toolTips[0] = "위협 감지하기 : [RMB]";
                        break;
                    case "Magic 7 ball":
                        item.itemName = "마법의 7번 공";
                        break;
                    case "Airhorn":
                        item.itemName = "에어혼";
                        item.toolTips[0] = "에어혼 사용하기 : [RMB]";
                        break;
                    case "Bell":
                        item.itemName = "종";
                        break;
                    case "Big bolt":
                        item.itemName = "큰 나사";
                        break;
                    case "Bottles":
                        item.itemName = "병 묶음";
                        break;
                    case "Brush":
                        item.itemName = "빗";
                        break;
                    case "Candy":
                        item.itemName = "사탕";
                        break;
                    case "Cash register":
                        item.itemName = "금전 등록기";
                        item.toolTips[0] = "금전 등록기 사용하기 : [RMB]";
                        break;
                    case "Chemical jug":
                        item.itemName = "화학 용기";
                        break;
                    case "Clown horn":
                        item.itemName = "광대 나팔";
                        item.toolTips[0] = "광대 나팔 사용하기 : [RMB]";
                        break;
                    case "Large axle":
                        item.itemName = "대형 축";
                        break;
                    case "Teeth":
                        item.itemName = "틀니";
                        break;
                    case "Dust pan":
                        item.itemName = "쓰레받기";
                        break;
                    case "Egg beater":
                        item.itemName = "달걀 거품기";
                        break;
                    case "V-type engine":
                        item.itemName = "V형 엔진";
                        break;
                    case "Golden cup":
                        item.itemName = "황금 컵";
                        break;
                    case "Fancy lamp":
                        item.itemName = "멋진 램프";
                        break;
                    case "Painting":
                        item.itemName = "그림";
                        break;
                    case "Plastic fish":
                        item.itemName = "플라스틱 물고기";
                        break;
                    case "Laser pointer":
                        item.itemName = "레이저 포인터";
                        item.toolTips[0] = "레이저 전환하기 : [RMB]";
                        break;
                    case "Gold bar":
                        item.itemName = "금 주괴";
                        break;
                    case "Hairdryer":
                        item.itemName = "헤어 드라이기";
                        item.toolTips[0] = "헤어 드라이기 사용하기 : [RMB]";
                        break;
                    case "Magnifying glass":
                        item.itemName = "돋보기";
                        break;
                    case "Metal sheet":
                        item.itemName = "금속 판";
                        break;
                    case "Cookie mold pan":
                        item.itemName = "쿠키 틀";
                        break;
                    case "Mug":
                        item.itemName = "머그잔";
                        break;
                    case "Perfume bottle":
                        item.itemName = "향수 병";
                        break;
                    case "Old phone":
                        item.itemName = "구식 전화기";
                        break;
                    case "Jar of pickles":
                        item.itemName = "피클 병";
                        break;
                    case "Pill bottle":
                        item.itemName = "약 병";
                        break;
                    case "Remote":
                        item.itemName = "리모컨";
                        item.toolTips[0] = "리모컨 사용하기 : [RMB]";
                        break;
                    case "Ring":
                        item.itemName = "반지";
                        break;
                    case "Toy robot":
                        item.itemName = "장난감 로봇";
                        break;
                    case "Rubber Ducky":
                        item.itemName = "고무 오리";
                        break;
                    case "Red soda":
                        item.itemName = "빨간색 소다";
                        break;
                    case "Steering wheel":
                        item.itemName = "운전대";
                        break;
                    case "Stop sign":
                        item.itemName = "정지 표지판";
                        item.toolTips[0] = "표지판 사용하기 : [RMB]";
                        break;
                    case "Tea kettle":
                        item.itemName = "찻주전자";
                        break;
                    case "Toothpaste":
                        item.itemName = "치약";
                        break;
                    case "Toy cube":
                        item.itemName = "장난감 큐브";
                        break;
                    case "Hive":
                        item.itemName = "벌집";
                        break;
                    case "Radar-booster":
                        item.itemName = "레이더 부스터";
                        item.toolTips[0] = "부스터 켜기 : [RMB]";
                        break;
                    case "Yield sign":
                        item.itemName = "양보 표지판";
                        item.toolTips[0] = "표지판 사용하기 : [RMB]";
                        break;
                    case "Shotgun":
                        item.itemName = "산탄총";
                        item.toolTips[0] = "격발 : [RMB]";
                        item.toolTips[1] = "재장전 : [E]";
                        item.toolTips[2] = "안전 모드 해제 : [Q]";
                        break;
                    case "Ammo":
                        item.itemName = "탄약";
                        break;
                    case "Spray paint":
                        item.itemName = "페인트 스프레이";
                        item.toolTips[0] = "스프레이 뿌리기 : [RMB]";
                        item.toolTips[1] = "캔 흔들기 : [Q]";
                        break;
                    case "Homemade flashbang":
                        item.itemName = "사제 섬광탄";
                        item.toolTips[0] = "사제 섬광탄 사용하기 : [RMB]";
                        break;
                    case "Gift":
                        item.itemName = "선물";
                        item.toolTips[0] = "선물 열기 : [RMB]";
                        break;
                    case "Flask":
                        item.itemName = "플라스크";
                        break;
                    case "Tragedy":
                        item.itemName = "비극";
                        item.toolTips[0] = "가면 쓰기 : [RMB]";
                        break;
                    case "Comedy":
                        item.itemName = "희극";
                        item.toolTips[0] = "가면 쓰기 : [RMB]";
                        break;
                    case "Whoopie cushion":
                        item.itemName = "방귀 쿠션";
                        break;
                    case "Kitchen knife":
                        item.itemName = "식칼";
                        item.toolTips[0] = "찌르기 : [RMB]";
                        break;
                    case "Easter egg":
                        item.itemName = "부활절 달걀";
                        break;
                }
            }
        }
    }
}