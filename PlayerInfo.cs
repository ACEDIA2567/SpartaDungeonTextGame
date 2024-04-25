using System;
using System.Runtime.Remoting.Activation;

public struct Info
{
	public Info()
	{
		int level = 1;	 // 레벨
		string chad; // 직업
		int str;     // 공격력
		int def;     // 방어력
		int hp;      // 체력
		int gold;    // 골드

        InfoView();
    }

	public void InfoView()
	{
        Console.WriteLine("상태 보기");
        Console.WriteLine("캐릭터의 정보가 표시됩니다.");

        Console.WriteLine($"LV. {level});
        Console.WriteLine($"Chad ({chad})");
        Console.WriteLine($"공격력: {str}");
        Console.WriteLine($"방어력: {def}");
        Console.WriteLine($"체  력: {hp}");
        Console.WriteLine($"Gold: {gold}G");
    }
}
