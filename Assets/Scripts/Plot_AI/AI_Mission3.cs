using UnityEngine;
//������ĤH5��(1)
public class AI_Mission3 : MonoBehaviour {
    private Game game;
    private bool preStageBattle = true;
    private int preProgramCounter = -1;
    private bool Mission1 = false;
    public int cnt = 0;

    private void OnEnable() {
        game = GameObject.Find("GameHandler").GetComponent<Game>();
    }

    private void AI_add_code() {
        if (game.Round == 1) {
            game.Players[1].Code.Insert(InstructionType.Move, 0, 0, new int[1] { 1 });
            game.Players[1].Code.Insert(InstructionType.Move, 1, 0, new int[1] { 2 });
        }
        else {
            if (game.Round % 2 == 0) {
                if (Random.Range(0, 2) == 0) {
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                }
                else {
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                    game.Players[1].Code.Insert(InstructionType.Attack, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0);
                }
            }
            else {
                if (Random.Range(0, 2) == 0) {
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                    game.Players[1].Code.Insert(InstructionType.Move, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0, new int[1] { Random.Range(0, 4) });
                }
                else {
                    game.Players[1].Code.Insert(InstructionType.Attack, (ushort)Random.Range(0, game.Players[1].Code.Size + 1), 0);
                }
            }
        }
    }

    private void Update() {
        if (!game.IsBattle) {
            if (preStageBattle) {
                WinCheck();
                AI_add_code();
                preStageBattle = false;
            }
        }
        else {
            if (!preStageBattle) {
                preStageBattle = true;
                preProgramCounter = -1;
            }
            if (game.Players[0].ProgramCounter != (ushort)preProgramCounter) {
                preProgramCounter = game.Players[0].ProgramCounter;
                Check();
            }
        }
    }

    private void WinCheck() {
        if (Mission1) {
            Debug.Log("You win");
            game.EndGame = true;
            game.Winner = true;
        }
    }

    private void Check() {
        if (game.Players[0].Code[(ushort)preProgramCounter] != null) {
            if (game.Players[0].Code[(ushort)preProgramCounter].Type == InstructionType.Attack) {
                for (int i = 0; i < 4; i++) {
                    if (game.Neighbor[game.Players[0].Pos, i] == game.Players[1].Pos) {
                        cnt++;
                        if (cnt > 5)
                            Mission1 = true;
                    }
                }
            }
        }
    }
}