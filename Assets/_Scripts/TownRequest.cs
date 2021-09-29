using Den.Tools;
using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownRequest
{
    private string name;
    private Vector2 coord;
    private int patchesInSize;


    public Vector2 Coord { get => coord; set => coord = value; }
    public string Name { get => name; set => name = value; }
    public int PatchesInSize { get => patchesInSize; set => patchesInSize = value; }

   public  TownRequest(string name, Vector2 coord  ) {

        this.coord = coord;
        this.name = name;
        this.patchesInSize = RandomGen.NextValidRandomPatchAmountFromTGOSRange();
    }

    public TownRequest(string name, Vector2 coord, int patches)
    {

        this.coord = coord;
        this.name = name;
        this.patchesInSize = patches;
    }


}
