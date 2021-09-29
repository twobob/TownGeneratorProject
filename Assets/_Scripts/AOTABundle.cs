using Den.Tools;
using Den.Tools.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AOTABundle
{
    
    public SplineSysWrapper[] SplineSysBundle;
    public TransitionsListWrapper[] TransitionsListBundle;
    public Coord coord;
    public string name;
    public bool isTileDataInBundle = false;
    public bool isBundled = false;

    public Town.Town town {
        get
        {
            if (TownGlobalObject.townsData.ContainsKey(TownGlobalObject.GetIndexAtCoord(coord)))
            {

                return TownGlobalObject.townsData[TownGlobalObject.GetIndexAtCoord(coord)];
            }
            else
            {
                Coord locality = TownGlobalObject.GetIndexAtCoord(coord);
                var SUPERlazyTown = TownGlobalObject.towns.GetOrAdd(locality, k => TownGlobalObject.MakeTown(k, k.x, k.z));
                Town.Town concrete = SUPERlazyTown;
                TownGlobalObject.townsData.TryAdd(locality, concrete);

                return TownGlobalObject.townsData[TownGlobalObject.GetIndexAtCoord(coord)];
            }
        }


        private set { }
    }

    //public Town.Town townConcurrent
    //{
    //    get { return TownGlobalObject.GetTown(TownGlobalObject.GetIndexAtCoord(coord)); }
    //    private set { TownGlobalObject.SetTown(TownGlobalObject.GetIndexAtCoord(coord), value ); }
    //}



    // There is no 
    // AotaBundle() empty constructor version but we had to add one for Denis's thing so lets use it.

    public AOTABundle()
    {
        this.isBundled = false;
        this.isTileDataInBundle = false;
        this.name = "BLANK";

    }

    
    // Populate the arrays. Then populate this with them.

    // AotaBundle(coord) gives the empty holder.

    public AOTABundle(Coord coord)
    {
        this.coord = coord;
        this.isBundled = false;
    }


    public static void MarkisBundledTrueStatic(ref AOTABundle bundle)
    {

      
        bundle.isBundled = true;


        // make .isBundled true. add coord
       // return bundle;
    }
     
    public static AOTABundle MarkIsTileDataInBundleTrueStatic(ref AOTABundle bundle)
    {
        // make .isBundled true. add coord
        bundle.isTileDataInBundle = true;

        return bundle;
    }

    public void MarkIsTileDataInBundleTrue()
    {
        // make .isBundled true. add coord
        isTileDataInBundle = true;

     
    }

    public void MarkisBundledTrue()
    {


        isBundled = true;

        // make .isBundled true. add coord
        // return bundle;
    }

    /// <summary>
    /// Will insert true for .isBundled
    /// </summary>
    /// <param name="coord">the tile coord</param>
    /// <param name="SplineSysBundlepass">the arrayed list of SplineSysWrapper</param>
    /// <param name="TransitionsListBundlepass">the arrayed list of  TransitionsListWrapper</param>
    public AOTABundle(Coord coord, SplineSysWrapper[] SplineSysBundlepass, TransitionsListWrapper[] TransitionsListBundlepass, string name)
    {
        //public Coord coord;
        //public string name;
        //public Town.Town town;

        this.name = name;
        this.isBundled = true;
        this.coord = coord;
        this.SplineSysBundle = Den.Tools.ArrayTools.Copy<SplineSysWrapper>(SplineSysBundlepass);
        this.TransitionsListBundle = Den.Tools.ArrayTools.Copy<TransitionsListWrapper>(TransitionsListBundlepass);
    }
    
    /// <summary>
    /// Lightweight holder for the Town data after it's been processed. 
    /// This should drastically reduce overheads once a town has been approached even once.
    /// </summary>
    /// <param name="splinelen">how many SpineSys are you storing</param>
    /// <param name="tranlen">how many TransList are you storing</param>
    public  AOTABundle(SplineSysWrapper[] SplineSysBundlepass, TransitionsListWrapper[] TransitionsListBundlepass)
    {
        this.SplineSysBundle = Den.Tools.ArrayTools.Copy<SplineSysWrapper>(SplineSysBundlepass);
        this.TransitionsListBundle = Den.Tools.ArrayTools.Copy<TransitionsListWrapper>(TransitionsListBundlepass);
    }
}

public class TransitionsListWrapper
{
    public MapMagic.Nodes.Outlet<TransitionsList> outlet;
    public string name;
    public string outletName;
    public TransitionsList transitionsList;


    public TransitionsListWrapper()
    {
        this.transitionsList = new TransitionsList();
        this.name = string.Empty;
        this.outletName = string.Empty;
    }


    //public TransitionsListWrapper(string name)
    //{
    //    this.transitionsList = new TransitionsList();
    //    this.name = name;
    //    this.outletName = string.Empty;
    //}

    //public TransitionsListWrapper(TransitionsList list)
    //{
    //    this.transitionsList = new TransitionsList(list);
    //}

    ///// <summary>
    ///// remember to fully populate this...
    ///// </summary>
    ///// <param name="name">the name of the list as a variable</param>
    ///// <param name="list">the list to copy from</param>
    //public TransitionsListWrapper(string name, TransitionsList list)
    //{
    //    this.transitionsList = new TransitionsList(list);
    //    this.name = name;
    //}
    

    //public TransitionsListWrapper(TransitionsList sys, MapMagic.Nodes.Outlet<TransitionsList> outlet)
    //{
    //    this.transitionsList = new TransitionsList(sys);
    //    this.outlet = outlet;
    //}

    public TransitionsListWrapper(string name, TransitionsList sys, MapMagic.Nodes.Outlet<TransitionsList> outlet)
    {
        this.outlet = outlet;
        this.transitionsList = new TransitionsList(sys);
        this.name = name;
    }

    /// <summary>
    /// Create a named wrapper. The outletName must match the declaration name of the outlet in the class calling Generate
    /// </summary>
    /// <param name="name">The TransitionsList variable name as defined at declaration</param>
    /// <param name="sys">The TransitionsList as used at runtime time</param>
    /// <param name="outlet">The outlet variable name as defined at declaration</param>
    public TransitionsListWrapper(string name, TransitionsList sys, string outletname)
    {
        this.outletName = outletname;
        this.transitionsList = new TransitionsList(sys);
        this.name = name;
    }

    /// <summary>
    /// Create an unnamed wrapper. The outletName must match the declaration name of the outlet in the class calling Generate
    /// </summary>
    /// <param name="sys">The TransitionsList as used at runtime time</param>
    /// <param name="outlet">The outlet variable name as defined at declaration</param>
    public TransitionsListWrapper(TransitionsList sys, string outletname)
    {
        this.outletName = outletname;
        this.transitionsList = new TransitionsList(sys);
    }

}

public class SplineSysWrapper {


    /// <summary>
    /// turns out this is read only. so... we need a reference of some kind.
    /// ID's might change. Guids seems empty. types wont help. Use name?
    /// </summary>
    public MapMagic.Nodes.Outlet<SplineSys> outlet;
    public string outletName;
    public string name;
    public SplineSys splineSys;

    public SplineSysWrapper() {
        this.splineSys = new SplineSys();
       // this.name = string.Empty;
    }

    public SplineSysWrapper(string name)
    {
        this.splineSys = new SplineSys();
        this.name = name;
    }

    public SplineSysWrapper(SplineSys sys)
    {
        this.splineSys = new SplineSys(sys);
    }

    public SplineSysWrapper(SplineSys sys, MapMagic.Nodes.Outlet<SplineSys>outlet)
    {
        this.splineSys = new SplineSys(sys);
        this.outlet = outlet;
    }

    public SplineSysWrapper(string name , SplineSys sys)
    {
        this.splineSys = new SplineSys(sys);
        this.name = name;
    }

    public SplineSysWrapper(string name, SplineSys sys, MapMagic.Nodes.Outlet<SplineSys> outlet)
    {
        this.outlet = outlet;
        this.splineSys = new SplineSys(sys);
        this.name = name;
    }


    public SplineSysWrapper(string name, SplineSys sys, string outletname)
    {
        this.outletName = outletname;
        this.splineSys = new SplineSys(sys);
        this.name = name;
    }


   


}

