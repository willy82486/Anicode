using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�g�L�a�Ϥ��Ҧ��I(1)
public class AI_Mission1 : MonoBehaviour
{
    private Game game;
    private bool preStageBattle = true;
    private int preProgramCounter = -1;
    private bool Mission1 = false;
    private bool[] table;

    private void Start()
    {
        game = GameObject.Find("GameHandler").gameObject.GetComponent<Game>();
        table = new bool[16];
    }
    private void AI_add_code()
    {
        game.Players[1].Code.Insert(InstructionType.Move, 0, 0, new int[1] { 0 });//
        game.Players[1].Code.Insert(InstructionType.Attack, 1, 0);
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
        if (Mission1)
        {
            Debug.Log("You win");
            game.EndGame = true;
            game.Winner = true;
        }

    }
    private void Check()
    {
        table[game.Players[0].Pos] = true;
        bool temp = true;
        for(int i = 0; i < 16; i++) {
            if(!table[i]) {
                temp = false;
                break;
            }
        }

        if (temp)
            Mission1 = true;
        Debug.Log(Mission1);
    }

}