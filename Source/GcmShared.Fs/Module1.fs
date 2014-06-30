// Learn more about F# at http://fsharp.net

namespace GcmShared

open Military
open Utilities
open System

exception TestExc of string

type Name = 
    struct
        val mutable First: string
        val mutable Middle: string
        val mutable Last: string
        new(f, m, l) = { First = f; Middle = m; Last = l }
    end
    override this.ToString() = 
        sprintf "%s %s%s" 
            this.First 
            (if String.IsNullOrEmpty(this.Middle) then "" else (this.Middle + " ")) 
            this.Last

    static member Empty
        with get() = Name("", "", "")

type Ext = System.Runtime.CompilerServices.ExtensionAttribute

type UnitAttr =
    | Firearm = 0
    | Marksmanship = 1
    | Open = 2
    | Horsemanship = 3
    | Close = 4
    | Edged = 5

module UnitTypes = 
    [<Literal>]
    let None = -1
    [<Literal>]
    let Infantry = 0
    [<Literal>]
    let Artillery = 1
    [<Literal>]
    let Cavalry = 2

    let IsInfantryOrCavalry(t:int) =
        match t with
        | Infantry -> true
        | Cavalry -> true
        | _ -> false
        
module Levels = 
    [<Literal>]
    let Side = 1
    [<Literal>]
    let Army = 2
    [<Literal>]
    let Corps = 3
    [<Literal>]
    let Division = 4
    [<Literal>]
    let Brigade = 5
    [<Literal>]
    let Unit = 6

[<Ext>]
module FMil = 
    let oneThird = 1.0 / 3.0
    
    let UnitImageID(u:Unit) = 
       match u.Data.Type with
       | UnitTypes.Infantry -> "i1"
       | UnitTypes.Artillery -> "a1"
       | _ -> "" 
    // This is based on stats in the ExportUnitData.  Never change unit after it has been exported!
    let GetStatsLevel(u:Unit) = 
        let opn = u.ExportData.Open * 5 // Discipline
        let edg = u.ExportData.Edged * 3
        let fir = u.ExportData.Firearm * 11
        let mrk = u.ExportData.Marksmanship * 11
        let hrs = u.ExportData.Horsemanship * 2
        let cls = u.ExportData.Close * 1
        let xp = (int u.ExportData.Experience) * 10;

        match u.Data.Type with
        | UnitTypes.Infantry -> double(opn + edg + fir + mrk + cls + xp) / 41.0
        | UnitTypes.Cavalry -> double(opn + edg + fir + mrk + cls + xp) / 41.0
        | UnitTypes.Artillery -> double(opn + fir + mrk + hrs + cls + xp) / 40.0
        | _ -> raise (TestExc("Invalid Unit Type"))
        
    // Weight is increased by 1% at 50 men, and 10% at 500 men.
    let UnitSizeFactor(unit:Unit) = 
        let pow = ((double unit.ExportData.Men / 1000.0) + 1.0)
        let bas = 1.20
        Math.Pow(bas, pow) - bas + 1.0

    // This is based on stats in the ExportUnitData.  Never change unit after it has been exported!
    let GetUnitWeight(unit:Unit) = 
        match unit.Data.Type with
        | UnitTypes.Infantry -> (GetStatsLevel unit + 2.5) * double unit.ExportData.Men * UnitSizeFactor unit
        | UnitTypes.Cavalry -> (GetStatsLevel unit + 2.5) * double unit.ExportData.Men * 2.0 * UnitSizeFactor unit
        | UnitTypes.Artillery -> (GetStatsLevel unit) * 250.0
        | _ -> raise (TestExc("Invalid Unit Type"))

    let ClampSkill v max = 
        match v with
        | x when x < 0.0 -> 0
        | x when x > double max -> max
        | _ -> int v

    let ClampUnitSkill v = 
        ClampSkill v 9

    let ClampCommanderSkill v = 
        ClampSkill v 6
    
    let ClampExperience v = 
        ClampSkill v 9

    let BaseFirearm(u:Unit) = 
        u.Data.Firearm + u.Data.Experience |> ClampUnitSkill
    let BaseEdged(u:Unit) = 
        u.Data.Edged + u.Data.Experience |> ClampUnitSkill
    let BaseClose(u:Unit) = 
        u.Data.Close + u.Data.Experience |> ClampUnitSkill
    let BaseOpen(u:Unit) = 
        u.Data.Open + u.Data.Experience |> ClampUnitSkill
        
    [<Ext>] 
    let OfLevel me level = 
        Seq.filter<Organization> (fun o -> o.Data.Level = level) me
    
    [<Ext>] 
    let OfAtLeastLevel me level = 
        Seq.filter<Organization> (fun o -> o.Data.Level <= level) me

    [<Ext>] 
    let ComputeMarksmanship(u:Unit) =
       ClampUnitSkill (u.Data.Marksmanship + u.Data.Experience)

    [<Ext>] 
    let ComputeFirearm(u:Unit) =
        ClampUnitSkill (u.Data.Firearm + u.Data.Experience + (-1.0 + u.Commander.Data.Control * oneThird) * 1.3) 
    
    [<Ext>] 
    let ComputeEdged(u:Unit) =
        ClampUnitSkill (u.Data.Edged + u.Data.Experience + (-1.0 + u.Commander.Data.Command * oneThird) * 1.5)
    
    [<Ext>] 
    let ComputeClose(u:Unit) =
       ClampUnitSkill (u.Data.Close + u.Data.Experience + (-1.0 + u.Commander.Data.Command * oneThird) * 1.6)
    
    [<Ext>] 
    let ComputeHorsemanship(u:Unit)  =
       ClampUnitSkill ((u.Data.Horsemanship + u.Data.Experience) * 1.3)
    
    [<Ext>] 
    let ComputeOpen(u:Unit) =
       ClampUnitSkill (u.Data.Open + u.Data.Experience + (-1.0 + u.Commander.Data.Ability * oneThird) * 1.4)
       
    [<Ext>] 
    let CommanderDifference (u:Unit) (a:UnitAttr) =
        let diff = 
            match a with
            | UnitAttr.Close -> ComputeClose u - BaseClose u
            | UnitAttr.Edged -> ComputeEdged u - BaseEdged u
            | UnitAttr.Firearm -> ComputeFirearm u - BaseFirearm u
            | UnitAttr.Open -> ComputeOpen u - BaseOpen u
            | _ -> 0
        match diff with
        | 0 -> ""
        | v when v < 0 -> string diff
        | _ -> "+" + string diff
        
    [<Ext>] 
    let GetUnitType (o:Organization) =
        match o.AllUnits with
        | s when Seq.isEmpty s -> UnitTypes.None
        | _ -> Seq.head o.AllUnits |> fun u -> u.Data.Type
       
    let SuborganizationsOutOfOrder (o:Organization) =
        o.Organizations |> Seq.fold 
                        (fun t (o:Organization) ->  match t with
                                                    | UnitTypes.Infantry -> GetUnitType o
                                                    | UnitTypes.None -> UnitTypes.None
                                                    | _ -> match GetUnitType o with
                                                           | UnitTypes.Infantry -> UnitTypes.None
                                                           | _ -> GetUnitType o)
                        UnitTypes.Infantry 
            = UnitTypes.None // If the above function returns UnitTypes.None, then the organizations are out of order.

        
    [<Ext>] 
    let AllFightingBrigades (o:Organization) =
        Seq.filter (fun (x:Organization) -> GetUnitType x = UnitTypes.Infantry && x.Data.Level = Levels.Brigade) o.AllOrganizations
        
    [<Ext>] 
    let AllArtilleryBatteries (o:Organization) =
        Seq.filter (fun (x:Organization) -> ((GetUnitType x) = UnitTypes.Artillery) && x.Data.Level = Levels.Brigade) o.AllOrganizations

    let ProperSuborganizations (o:Organization) =
        Seq.append (AllFightingBrigades o) (AllArtilleryBatteries o)
    
    /// Returns all brigade level organizations
    let Brigades (o:Organization) =
        OfLevel o.Organizations Levels.Brigade
        
    let OfType t orgs =
        Seq.filter<Organization> (fun o -> GetUnitType o = t) orgs

    let GetUnitExportData (u:Unit) = 
        let e = UnitExportData()
        e.ClassId <- u.Data.ClassId
        e.WeaponId <- u.Data.WeaponId
        e.Flag1 <- u.Data.Flag1
        e.Flag2 <- u.Data.Flag2
        e.Close <- ComputeClose u
        e.Edged <- ComputeEdged u
        e.Firearm <- ComputeFirearm u
        e.Horsemanship <- ComputeHorsemanship u
        e.Marksmanship <- ComputeMarksmanship u
        e.Open <- ComputeOpen u
        e.Experience <- int u.Data.Experience
        e.Men <- u.Data.Men
        e.Name <- u.Data.Name
        e
    type Organization with
        member this.Same(o:Organization) = this.Commander.Data.Id = o.Commander.Data.Id
    
    [<Ext>]
    let FindCommander (o:Organization) id =
        Seq.find<Commander> (fun c -> c.Data.Id = id) o.AllCommanders 
        
    [<Ext>]
    let FindUnit (o:Organization) id =
        Seq.find<Unit> (fun u -> u.Data.Id = id) o.AllUnits 
        
    [<Ext>]
    let AllFightingUnits (o:Organization) =
        Seq.filter<Unit> (fun u -> u.Data.Type <> UnitTypes.Artillery) o.AllUnits 
        
    [<Ext>]
    let AllUnitsOfType (o:Organization) t =
        Seq.filter<Unit> (fun u -> u.Data.Type = t) o.AllUnits 
    
    [<Ext>]
    let AllArtilleryUnits (o:Organization) =
        Seq.filter<Unit> (fun u -> u.Data.Type = UnitTypes.Artillery) o.AllUnits 
        
    [<Ext>]
    let IsAtLevel (o:Organization) level =
        o.Data.Level = level
        
    [<Ext>] /// Returns the total number of men in the command
    let GetHeadCount (command:IForce) =
        match command with
        | :? Unit as u -> u.Data.Men
        | :? Organization as o -> o.AllUnits |> Seq.sumBy (fun u -> u.Data.Men)
        | _ -> raise (TestExc("Invalid Command"))
        
    [<Ext>]
    let GetName (c:Commander) = 
        Name(c.Data.FirstName, c.Data.MiddleInitial, c.Data.LastName)



module SMil = 
    open FMil

    /// Copy attributes into the unit's ExportData for writing to scenario
    let ExportUnitData(u:Unit) =
        u.ExportData <- GetUnitExportData u
        
    let GetUnitWeightOrExport (u:Unit) =
        if u.ExportData = null then do
            ExportUnitData u
        FMil.GetUnitWeight(u)

    /// Put the brigades in this division into canonical order (infantry brigades first, then artillery brigades)
    let ReorderSuborganizations(o:Organization) =
        if (SuborganizationsOutOfOrder o) then
            o.ReorderOrganizations (ProperSuborganizations o)

    /// Merge two brigades
    let private Merge (src:Organization) (dest:Organization) = 
        for u in src.Units |> List.ofSeq do
            dest.AddUnit u
        src.Parent.RemoveOrganization src
       
    //After a unit is stolen from a player, we reset its player specific settings
    let CleanPlayerSettingsOnRegiment (u:Unit) = 
       u.Data.RecruitLimit <- 600
       u.Data.Active <- true

    let newExpRatio = 30
    let oldExpRatio = 100 - newExpRatio
    let newExpBase = 1.0

    //After a unit is stolen from a player, we chop down its experience.  
    let ResetRegimentExperienceAfterRemoval (u:Unit) = 
        let newExpRatio = Rand.Int(0, 50)
        let oldExpRatio = 100 - newExpRatio
        let newExpBase = 1.0
        u.Data.Experience <- ((newExpBase * float newExpRatio) + (u.Data.Experience * float oldExpRatio)) / 100.0
    
    let rec private MergeBrigades (brigades:list<Organization>) c m =
        if c = m then ()
        else match brigades with
             | [] -> ()
             | _ -> if brigades.Head.NumUnits <= 2 then
                        match brigades.Tail |> List.tryFind (fun b -> b.NumUnits <= 3) with
                        | None -> ()
                        | Some d -> Merge brigades.Head d
                                    MergeBrigades brigades.Tail (c + 1) m
                    else
                        MergeBrigades (brigades.Tail @ [brigades.Head]) (c + 1) m

    /// Merge all brigades of 2 or less regiments into other brigades.
    let MergeSmallBrigades o t =
        let brigades = Brigades o |> OfType t |> List.ofSeq
        MergeBrigades brigades 0 brigades.Length



type AutomatchPlayer =
    {
        Name:string
        Points:int
        PlayerID:int
    }

module Automatch =
    let Match (players:List<AutomatchPlayer>) =
        for player in players do
            Console.WriteLine(player.Name)
        
