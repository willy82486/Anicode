using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mission3_2 : MonoBehaviour
{
    private Game game;
    private bool preStageBattle = true;
    private int preProgramCounter = -1;
    private bool Mission1 = false;
    private bool Mission2 = false;
    private bool loop_Flag = false;
    private int loop_Level = 0;
    private int assign_Level = 0;
    private int move_Level = 0;

    private void Start()
    {
        game = GameObject.Find("GameHandler").gameObject.GetComponent<Game>();
    }
    private void AI_add_code()
    {
        game.Players[1].Code.Insert(InstructionType.Move, 0, 0, new int[1] { 3 });
        game.Players[1].Code.Insert(InstructionType.Move, 0, 0, new int[1] { 1 });
    }

    private void Update()
    {
        if (!game.IsBattle)
        {
            if (preStageBattle)
            {
                WinCheck();
                AI_add_code();
                preStageBattle = false;
            }
        }
        else
        {
            if (!preStageBattle) preStageBattle = true;
            if (game.Players[0].ProgramCounter != (ushort)preProgramCounter)
            {
                preProgramCounter = game.Players[0].ProgramCounter;
                Check();
            }

        }
    }
    private void WinCheck()
    {
        if (Mission1 && Mission2)
        {
            Debug.Log("You win");
            game.EndGame = true;
            game.Winner = true;
        }

        else if (game.Round == 4)
        {
            Debug.Log("You lose");
            game.EndGame = true;
            game.Winner = false;
        }
    }
    private void Check()
    {
        //�bloop�̭��]move
        if (game.Players[0].Code[(ushort)preProgramCounter] != null)
        {
            if (game.Players[0].Code[(ushort)preProgramCounter].Type == InstructionType.Loop && loop_Flag == false)
            {
                //loop �ڤ�ͩR > v3 
                if (game.Players[0].Code[(ushort)preProgramCounter].Arguments[0] == 2 && game.Players[0].Code[(ushort)preProgramCounter].Arguments[1] == 1 && game.Players[0].Code[(ushort)preProgramCounter].Arguments[2] == 3)
                {
                    Mission1 = true;
                    loop_Flag = true;
                    loop_Level = game.Players[0].Code.GetLevel((ushort)preProgramCounter);
                }
            }
            if (game.Players[0].Code[(ushort)preProgramCounter].Equals(new Instruction(InstructionType.Move, new int[1] { 0 })))
            {
                // move(�W)
                move_Level = game.Players[0].Code.GetLevel((ushort)preProgramCounter);
                if (move_Level > loop_Level && loop_Flag)
                    Mission2 = true;
                else
                    loop_Flag = false;
            }
        }
        Debug.Log("Mission1 : ");
        Debug.Log(Mission1);
        Debug.Log("Mission2 : ");
        Debug.Log(Mission2);
    }
}