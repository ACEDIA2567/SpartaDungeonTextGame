using System.Reflection.Emit;
using System;
using System.Security.Cryptography;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Dynamic;
using Newtonsoft.Json;
using System.IO;
#nullable disable // Console.ReadLine() null 리터널 문제

namespace SpartaTextGame
{
    enum equipmentType { weapon, armor, accessories };

    // 플레이어 정보 구조체
    [Serializable]
    struct Info
    {
        public int level = 1;   // 레벨
        public string name; // 직업
        public float str;     // 공격력
        public int def;     // 방어력
        public int hp;      // 체력
        public float gold;    // 골드
        public int currentNum = 0; // 현재 장비 개수

        public equipment weapon;
        public equipment armor;

        public int exp = 0;
        public int maxExp = 1;

        // 구조체 생성 시 기본 정보 등록
        public Info()
        {
            level = 1;
            name = "이름없는 모험가";
            str = 10;
            def = 5;
            hp = 100;
            gold = 1500;
            weapon = new equipment();
            armor = new equipment();
        }

        public void InfoShow()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine($"LV. {level}");
            Console.WriteLine($"이름: {name} ( 전사 )");
            Console.WriteLine($"공격력: {str} + {weapon.vaule}");
            Console.WriteLine($"방어력: {def} + {armor.vaule}");
            Console.WriteLine($"체  력: {hp}");
            Console.WriteLine($"Gold: {gold}G");
            Console.WriteLine("-------------------------------\n");

            Console.WriteLine("0. 나가기");
            Console.WriteLine("\n원하시는 행동을 입력해주세요. ");
            Console.Write(">> ");
            string text = Console.ReadLine();
            if(text != "0")
            {
                Console.WriteLine("잘못된 입력입니다.\n");
                InfoShow();
            }
        }

        // 플레이어 정보 초기화
        public void ClearInfo()
        {
            level = 1;
            name = "이름없는 모험가";
            str = 10;
            def = 5;
            hp = 100;
            gold = 1500;
            weapon = new equipment();
            armor = new equipment();
        }

        public void ExpUp()
        {
            exp += 1;
            // 경험치가 가득 차고 레벨이 5레벨 미만이라면
            if(exp == maxExp && level < 5)
            {
                Console.WriteLine("-------------------------------\n");
                Console.WriteLine("축하합니다! 레벨업을 하셨습니다.");
                Console.WriteLine($"레벨 {level} -> {++level}");
                Console.WriteLine($"공격력 {str} -> {str += 0.5f}");
                Console.WriteLine($"방어력 {def} -> {def += 1}");
                maxExp += 1;
                exp = 0;
            }
        }

        public void Die(bool GameEnd)
        {
            // 체력이 0이하라면
            if (hp <= 0)
            {
                Console.WriteLine("당신의 체력이 0이하가 되어 사망하셨습니다.");
                Console.WriteLine("0. 부활하기[모든 것을 잃고 다시 시작합니다.]");
                Console.WriteLine("1. 영원한 안식으로...");
                Console.Write(">> ");
                string str = Console.ReadLine();
                switch(str)
                {
                    case "0": // 부활
                        Console.WriteLine("모든 것을 잃고 다시 시작합니다.");
                        ClearInfo(); // 정보 초기화
                        GameEnd = false;
                        break;
                    case "1": // 사망
                        Console.WriteLine("사망하여 게임을 종료합니다.");
                        GameEnd = true;
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        Die(GameEnd);
                        break;
                }
                
            }
        }
    }

    // 장비 구조체
    struct equipment
    {
        public string name;      // 장비 이름
        public string equipType; // 장비 타입 정보(공격력, 방어력)
        public string introduct; // 장비 소개
        public bool choice;      // 장비 타입 선택
        public bool equipCheck;  // 장비 착용 여부
        public bool buyCheck;    // 장비 구매 여부
        public int vaule = 0;    // 스탯 수치
        public int gold;         // 장비 가격
        public equipmentType type;

        public equipment(string name, bool choicem, int value, string introduct, int gold, equipmentType type)
        {
            this.name = name;
            this.choice = choicem;
            this.vaule = value;
            this.equipCheck = false;
            this.introduct = introduct;
            this.gold = gold;
            this.type = type;

            // 장비 능력에서 방어력과 공격력 선택
            if (choice)
            {
                equipType = "공격력";
            }
            else
            {
                equipType = "방어력";
            }
        }
    }

    class Program
    {
        static Info playerInfo = new Info();
        static equipment[] equipments = new equipment[9];       // 상점의 장비
        static equipment[] playerEquipments = new equipment[9]; // 플레이어의 장비
        static bool gameEnd = true;

        // 게임 첫 시작 텍스트
        static void FirstText()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("-------------------------------\n");
        }

        // 인벤토리
        static void Inventory()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
            Console.WriteLine("[아이템 목록]");

            int count = 0; // 장비 순서
            foreach (equipment equipment in playerEquipments)
            {
                if (equipment.name != null)
                {
                    count++;
                    // 해당 장비가 장착 중인 장비라면
                    if (playerInfo.weapon.name == equipment.name || playerInfo.armor.name == equipment.name)
                    {
                        Console.WriteLine($"- {count} [E]{equipment.name} | {equipment.equipType} +{equipment.vaule} | {equipment.introduct}\n");
                    }
                    else
                    {
                        Console.WriteLine($"- {count} {equipment.name} | {equipment.equipType} +{equipment.vaule} | {equipment.introduct}\n");
                    }
                }
            }

            Console.WriteLine("\n1. 장착 관리");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("\n원하시는 행동을 입력해주세요.");
            Console.Write(">> ");
            string text = Console.ReadLine();
            
            switch (text)
            {
                case "0": // 나가기
                    break;
                case "1": // 장착 관리
                    PlayerEquipment();
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다.\n");
                    Inventory();
                    break;
            }
        }

        // 장비 장착
        static void PlayerEquipment()
        {
            bool check = true;
            Console.WriteLine("\n장착할 아이템 이름을 입력해주세요.");
            Console.Write(">> ");
            string str = Console.ReadLine();
            foreach (equipment equipment in playerEquipments)
            {
                // 검색한 아이템 이름이 있는지 확인하고 있다면
                if (str == equipment.name)
                {
                    // 장비가 무기라면
                    if (equipment.type == equipmentType.weapon)
                    {
                        // 선택한 무기로 변경
                        playerInfo.weapon = equipment;
                        Console.WriteLine($"{playerInfo.weapon.name}을(를) 장착했습니다.");
                    }
                    else if (equipment.type == equipmentType.armor)
                    {
                        // 선택한 방어구로 변경
                        playerInfo.armor = equipment;
                        Console.WriteLine($"{playerInfo.armor.name}을(를) 장착했습니다.");
                    }
                    check = false;
                }
            }
            if (check)
            {
                Console.WriteLine("입력하신 아이템의 이름은 존재하지 않습니다.\n");
                Inventory();
            }
        }

        // 상점
        static void Shop(equipment[] equipments)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine(playerInfo.gold + "G");

            Console.WriteLine("\n[아이템 목록]\n");
            foreach (equipment equipment in equipments)
            {
                string buyText = true == equipment.buyCheck ? "구매완료" : equipment.gold.ToString() + "G";
                Console.WriteLine($"- {equipment.name} | {equipment.equipType} +{equipment.vaule} | {equipment.introduct} | {buyText}\n");
            }

            Console.WriteLine("\n2. 아이템 판매");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("\n원하시는 행동을 입력해주세요. ");
            Console.Write(">> ");
            string text = Console.ReadLine();
            switch (text)
            {
                case "0":
                    break;
                case "1":
                    BuyEquipment();
                    break;
                case "2":
                    SellEquipment();
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다.\n");
                    Shop(equipments);
                    break;
            }
        }

        static void SellEquipment()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine(playerInfo.gold + "G\n");
            Console.WriteLine("[아이템 목록]");
            int count = 0;
            foreach (equipment equipment in playerEquipments)
            {
                if(equipment.name != null)
                {
                    Console.WriteLine($"- {count + 1} {equipment.name} | {equipment.equipType} +{equipment.vaule} | {equipment.introduct}");
                    count++;
                }
            }

            bool check = true;
            Console.WriteLine("\n판매할 아이템 이름을 입력해주세요.");
            Console.Write(">> ");
            string str = Console.ReadLine();
            count = 0;
            foreach (equipment equipment in playerEquipments)
            {
                // 검색한 아이템 이름이 있는지 확인하고 있다면
                if (str == equipment.name)
                {
                    // 장비가 무기라면
                    if (equipment.type == equipmentType.weapon)
                    {
                        // 플레이어의 무기 초기화
                        playerInfo.weapon = new equipment();
                    }
                    else if (equipment.type == equipmentType.armor)
                    {
                        // 플레이어의 방어구 초기화
                        playerInfo.armor = new equipment();
                    }
                    Console.WriteLine($"{equipment.name}을(를) 판매했습니다.\n");
                    playerEquipments[count] = new equipment();
                    equipments[count].buyCheck = false; // 해당 아이템 구매 여부 변경
                    playerEquipments[count].buyCheck = false; // 플레이어 장비도 구매로 변경
                    playerInfo.gold += (int)(equipment.gold * 0.85f); // 구매했으니 골드 감소
                    check = false;
                }
                count++;
            }
            if (check)
            {
                Console.WriteLine("입력하신 아이템의 이름은 존재하지 않습니다.\n");
                Inventory();
            }
        }

        static void BuyEquipment()
        {
            Console.Write("구매할 아이템의 이름을 적어주세요: ");
            string ItemName = Console.ReadLine();

            int count = 0; // 장비 순서
            foreach (equipment equipment in equipments)
            {
                // 구매할 아이템의 이름이 존재한지 비교
                if(ItemName == equipment.name)
                {
                    // 장비한 아이템이 구매된 건지 확인
                    if(equipment.buyCheck)
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.\n ");
                        break;
                    }
                    else
                    {
                        string buyText = true == equipment.buyCheck ? "구매완료" : equipment.gold.ToString() + "G";
                        Console.WriteLine("\n구매할 아이템의 정보입니다.");
                        Console.WriteLine($"- {equipment.name} | {equipment.equipType} +{equipment.vaule} | {equipment.introduct} | {buyText}");

                        Console.WriteLine("아이템을 구매하시겠습니까?");
                        Console.WriteLine("1. 네");
                        Console.WriteLine("2. 아니요");
                        Console.Write("\n원하시는 행동을 입력해주세요:");
                        Console.Write(">> ");
                        string str = Console.ReadLine();

                        switch (str)
                        {
                            case "1": // 네
                                if(equipment.gold <= playerInfo.gold)
                                {
                                    Console.WriteLine($"{equipment.name}을(를) 구매하였습니다.\n");
                                    playerEquipments[count] = equipment;
                                    equipments[count].buyCheck = true; // 해당 아이템 구매 여부 변경
                                    playerEquipments[count].buyCheck = true; // 플레이어 장비도 구매로 변경
                                    playerInfo.gold -= equipment.gold; // 구매했으니 골드 감소
                                }
                                else
                                {
                                    Console.WriteLine("Gold가 부족하여 구매를 실패하였습니다.");
                                }
                                break;
                            case "2": // 아니요
                                Console.WriteLine("아이템 구매를 취소하였습니다.");
                                break;
                            default:
                                Console.WriteLine("잘못된 입력입니다.");
                                break;
                        }
                        
                    }
                    break;
                }
                // 모든 아이템을 비교하고 아이템이 존재하지 않을 때
                else if (equipment.name == equipments[equipments.Length-1].name)
                {
                    Console.WriteLine("입력한 아이템은 존재하지 않습니다.");
                    Shop(equipments);
                }
                count++;
            }
        }

        static void Dungeon()
        {
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine("던전입장");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("1. 쉬운 던전   | 방어력 5 이상 권장");
            Console.WriteLine("2. 일반 던전   | 방어력 11 이상 권장");
            Console.WriteLine("3. 어려운 던전 | 방어력 17 이상 권장");
            Console.WriteLine("0. 나가기\n");
            Console.Write("원하시는 행동을 입력해주세요:");
            Console.Write(">> ");
            string str = Console.ReadLine();
            switch (str)
            {
                case "0":
                    Console.WriteLine("던전을 나갔습니다.");
                    // 나가기
                    break;
                case "1":
                    DungeonStart(1, 5, 1000);
                    break;
                case "2":
                    DungeonStart(2, 11, 1700);
                    break;
                case "3":
                    DungeonStart(3, 17, 2500);
                    break;
                default:
                    Console.WriteLine("잘못된 선택지 입니다. 다시 입력해주세요!");
                    Dungeon();
                    break;
            }
        }

        static void DungeonStart(int level, int defAvg, float ClearGold)
        {
            // 랜덤 변수 생성
            Random random = new Random();
            // 권장 방어력보다 낮다면
            if (playerInfo.def + playerInfo.armor.vaule < defAvg)
            {
                Console.WriteLine("권장 방어력보다 낮습니다.");
                // 40%로 던전 실패
                if (random.Next(0, 10) > 5)
                {
                    Console.WriteLine("40%로 던전 공략에 실패하였습니다.");
                    // 보상 X , 체력 절반 감소
                    playerInfo.hp -= (random.Next(20, 36) - playerInfo.def - defAvg) / 2;
                }
                else
                {
                    Console.WriteLine("60%로 던전에서 도망쳤습니다.");
                    // 보상만 X
                }
            }
            // 권장 방어력보다 높다면
            else
            {
                // 던전이 클리어 되고 방어력 수치에 따른 체력 감소 및 골드 획득
                Console.WriteLine("-----------------------------------------\n");
                Console.WriteLine("던전 클리어");
                Console.WriteLine("축하합니다!!");
                if (level == 1)
                {
                    Console.Write("쉬운");
                }
                else if (level == 2)
                {
                    Console.Write("일반");
                }
                else
                {
                    Console.Write("어려운");
                }
                Console.WriteLine("던전을 클리어 하였습니다.\n");
                Console.WriteLine("[탐험 결과]");
                Console.WriteLine($"체력 {playerInfo.hp} -> {playerInfo.hp -= 
                    random.Next(20, 36) - playerInfo.def - defAvg}");
                int attackMin = (int)playerInfo.str + playerInfo.weapon.vaule;
                int attackMax = ((int)playerInfo.str + playerInfo.weapon.vaule) * 2;
                int percentage = random.Next(attackMin, attackMax);
                Console.WriteLine($"Gold {playerInfo.gold} G -> " +
                    $"{playerInfo.gold += ClearGold + ClearGold * (percentage / 100.0f)} G\n");
                Console.WriteLine("0. 나가기");
                Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                Console.Write(">> ");
                string str = Console.ReadLine();
                while (str != "0")
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.\n");
                    Console.WriteLine("0. 나가기");
                    Console.WriteLine("\n원하시는 행동을 입력해주세요.");
                    Console.Write(">> ");
                    str = Console.ReadLine();
                }
                playerInfo.ExpUp();
            }
        }

        static void Rest()
        {
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine("휴식하기");
            Console.WriteLine($"500 G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {playerInfo.gold} G\n");
            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기\n");
            Console.Write("원하시는 행동을 입력해주세요:");
            Console.Write(">> ");
            string str = Console.ReadLine();
            switch (str)
            {
                case "0":
                    // 나가기
                    break;
                case "1":
                    if (playerInfo.gold >= 500)
                    {
                        Console.WriteLine("휴식을 완료했습니다.");
                        playerInfo.gold -= 500;
                        playerInfo.hp = 100;
                    }
                    else
                    {
                        Console.WriteLine("Gold가 부족합니다.");
                        Rest();
                    }
                    break;
                default:
                    Console.WriteLine("잘못된 선택지 입니다. 다시 입력해주세요!");
                    Rest();
                    break;
            }
            Console.WriteLine();
        }

        // 아이템 생성
        static equipment[] CreatItem(equipment[] equipments)
        {
            // 무기
            equipments[0] = new equipment("나무 검", true, 2, "마을의 어린이들이 연습용으로 사용하는 검이다.", 100, equipmentType.weapon);
            equipments[1] = new equipment("녹슨 검", true, 4, "아직 쓸모있지만 언제 부숴질지 모르는 검이다.", 500, equipmentType.weapon);
            equipments[2] = new equipment("철퇴", true, 7, "짧지만 그만큼 힘이 뭉쳐있다고 전해지는 몽둥이다.", 1500, equipmentType.weapon);
            equipments[3] = new equipment("철 검", true, 10, "웬만한 모험가들은 들수도 없이 무거운 검이다.", 3000, equipmentType.weapon);
            equipments[4] = new equipment("엑스칼리버", true, 20, "적의 침략에 맞서라 지킬 힘을 줄테니", 7777, equipmentType.weapon);

            // 방어구
            equipments[5] = new equipment("천갑옷", false, 3, "처음 모험을 떠나는 모험가들의 기본 갑옷이다.", 300, equipmentType.armor);
            equipments[6] = new equipment("철갑옷", false, 5, "숙련된 모험가들이 착용하는 철로 만든 갑옷이다.", 1000, equipmentType.armor);
            equipments[7] = new equipment("판금갑옷", false, 10, "나라의 한손가락의 강함을 가진 모험가의 갑옷이다.", 3000, equipmentType.armor);
            equipments[8] = new equipment("영광의 판금갑옷", false, 15, "두려워하는 전사는 반드시 패할것이니", 10000, equipmentType.armor);

            return equipments;
        }

        // 선택지(계속 실행)
        static void Select(Info playerInfo, equipment[] equipments)
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("0. 저장 하기");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.Write("\n원하시는 행동을 입력해주세요");
            Console.Write(">> ");
            string str = Console.ReadLine();
            Console.WriteLine("\n");
            switch (str)
            {
                case "0":
                    SaveInfo();
                    break;
                case "1":
                    playerInfo.InfoShow();
                    break;
                case "2":
                    Inventory();
                    break;
                case "3":
                    Shop(equipments);
                    break;
                case "4":
                    Dungeon();
                    break;
                case "5":
                    Rest();
                    break;
                default:
                    Console.WriteLine("잘못된 선택지 입니다. 다시 입력해주세요!");
                    Select(playerInfo, equipments);
                    break;
            }
        }

        static void SaveInfo()
        {
            // 게임 저장
            Console.WriteLine();
            string json = JsonConvert.SerializeObject(playerInfo);
            string json1 = JsonConvert.SerializeObject(playerEquipments);
            string json2 = JsonConvert.SerializeObject(equipments);
            File.WriteAllText("infoSave.json", json);
            File.WriteAllText("playerEqSave.json", json1);
            File.WriteAllText("shopeqSave.json", json2);
            Console.WriteLine("데이터가 저장되었습니다.\n");
            gameEnd = false;
        }

        static void Main(string[] args)
        {
            // 아이템 생성
            CreatItem(equipments);

            // 첫 시작
            FirstText();
            if (File.Exists("infoSave.json"))
            {
                Console.WriteLine("저장된 정보가 있습니다.");
                Console.WriteLine("0. 네");
                Console.WriteLine("1. 아니요");
                Console.Write("이어서 하시겠습니까?");
                Console.Write(">> ");
                string str = Console.ReadLine();
                while (str == "1" && str == "0")
                {
                    Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.\n");
                    Console.WriteLine("0. 네");
                    Console.WriteLine("1. 아니요");
                    Console.Write("이어서 하시겠습니까?");
                    Console.Write(">> ");
                    str = Console.ReadLine();
                }
                // 게임 데이터를 불러옴
                if (str == "0")
                {
                    // 저장한 데이터 불러오기
                    string json = File.ReadAllText("infoSave.json");
                    string json1 = File.ReadAllText("playerEqSave.json");
                    string json2 = File.ReadAllText("shopeqSave.json");
                    Info loadedInfo = JsonConvert.DeserializeObject<Info>(json);
                    equipment[] loadedpleq = JsonConvert.DeserializeObject<equipment[]>(json1);
                    equipment[] loadedsheq = JsonConvert.DeserializeObject<equipment[]>(json2);

                    Console.WriteLine("정보를 불러옵니다.");
                    playerInfo = loadedInfo;
                    playerEquipments = loadedpleq;
                    equipments = loadedsheq;
                }
                else
                {
                    Console.WriteLine("처음 부터 시작합니다.");
                }
            }

            // 게임 계속 진행
            while (gameEnd)
            {
                playerInfo.Die(gameEnd);
                Select(playerInfo, equipments);
            }

            Console.WriteLine("게임을 종료합니다!");
        }
    }
}
