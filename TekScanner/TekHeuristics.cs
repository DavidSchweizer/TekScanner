using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Tek1
{
    public enum HeuristicAction { haNone, haSetValue, haExcludeValue, haExcludeComplement, haImpossible };
    public abstract class TekHeuristic
    {
        protected static string[] actionDescriptions = { "none", "set value", "exclude value(s)", "exclude complement value(s)", "impossible" };

        string _description;
        private HeuristicAction _action;
        public HeuristicAction Action { get { return _action; } }
        protected void SetHeuristicAction(HeuristicAction value) { _action = value; }
        public string Description { get { return _description; } }
        public List<TekField> HeuristicFields;
        public List<TekField> AffectedFields;
        public List<int> HeuristicValues;
        protected TekRegion Region;
        public bool Enabled;

        public TekHeuristic()
        {
            Region = new TekRegion();
        }

        public TekHeuristic(string description, HeuristicAction action) : this()
        {
            _description = description;
            _action = action;
            HeuristicFields = new List<TekField>();
            AffectedFields = new List<TekField>();
            HeuristicValues = new List<int>();
            Enabled = true;
        }

        public virtual string AsString()
        {
            if (Action == HeuristicAction.haNone)
                return Description;                    
            string result = String.Format("{0} [fields: ", Description);
            foreach (TekField field in HeuristicFields)
                result = result + String.Format("{0} ", field.AsString());
            result = result + "; affects: ";
            foreach (TekField field in AffectedFields)
                result = result + String.Format("{0} ", field.AsString());
            result = result + "| values: ";
            foreach (int value in HeuristicValues)
                result = result + String.Format("{0} ", value);
            return result + "] " + actionDescriptions[(int)Action];
        }

        public void Reset()
        {
            HeuristicFields.Clear();
            AffectedFields.Clear();
            HeuristicValues.Clear();
            Region.Clear();
        }

        private void AddOnce(List<TekField> list, TekField field)
        {
            if (!list.Contains(field))
                list.Add(field);
        }

        protected void AddHeuristicField(TekField field)
        {
            AddOnce(HeuristicFields, field);
        }

        protected void AddHeuristicFields(params TekField[] fields)
        {
            foreach (TekField field in fields)
                AddOnce(HeuristicFields, field);
        }
        protected void AddHeuristicFields(List <TekField> fields)
        {
            foreach (TekField field in fields)
                AddOnce(HeuristicFields, field);
        }

        protected void AddAffectedField(TekField field)
        {
            AddOnce(AffectedFields, field);
        }
        protected void AddAffectedFields(params TekField[] fields)
        {
            foreach (TekField field in fields)
                AddOnce(AffectedFields, field);
        }
        protected void AddAffectedFields(List<TekField> fields)
        {
            foreach (TekField field in fields)
                AddOnce(AffectedFields, field);
        }

        protected void AddValue(int value)
        {
            if (!HeuristicValues.Contains(value))
                HeuristicValues.Add(value);
        }
        protected void AddValues(params int[] values)
        {
            foreach (int value in values)
                AddValue(value);
        }
        protected void AddValues(List<int> values)
        {
            foreach (int value in values)
                AddValue(value);
        }

        protected virtual void BeforeProcessingBoard(TekBoard board)
        {
            // override to setup local variables
        }

        public bool Applies(TekBoard board)
        {
            Reset();
            BeforeProcessingBoard(board);
            foreach (TekField field in board.Fields)
            {
                if (field.Value > 0 )
                    continue;
                if (HeuristicApplies(board, field))
                {
                    return true;
                }
                else
                {
                    Reset();
                }
            }
            return false;
        }

        abstract public bool HeuristicApplies(TekBoard board, TekField field);

        public void SetValue(TekMoves moves, TekField field, int value)
        {
            bool result = field.Value == 0;
            moves.PlayValue(field, value);
        }

        public void ExcludeValues(TekMoves moves, TekField field)
        {
            foreach (int value in HeuristicValues)
            {
                if (field.ValuePossible(value))
                    moves.ExcludeValue(field, value);
            }
        }

        public void ExcludeComplementValues(TekMoves moves, TekField field)
        {
            List<int> excludingValues = new List<int>();
            foreach (int value in field.PossibleValues)
            {
                if (!HeuristicValues.Contains(value))
                    excludingValues.Add(value);
            }
            foreach (int value in excludingValues)
                if (field.ValuePossible(value))
                    moves.ExcludeValue(field, value);
        }

        public void ExecuteAction(TekMoves moves)
        {
            switch (Action)
            {
                case HeuristicAction.haNone:
                    break;

                case HeuristicAction.haSetValue:
                    SetValue(moves, AffectedFields[0], HeuristicValues[0]);
                    break;

                case HeuristicAction.haExcludeValue:
                    foreach (TekField field in AffectedFields)
                    {
                        ExcludeValues(moves, field);
                        if (field.PossibleValues.Count == 1)
                            SetValue(moves, field, field.PossibleValues[0]);
                    }
                    break;

                case HeuristicAction.haExcludeComplement:
                    foreach (TekField field in AffectedFields)
                    {
                        ExcludeComplementValues(moves, field);
                    }
                    break;
            }
        }
    } // TekHeuristic

    public class SingleValueHeuristic : TekHeuristic
    // only one possible value in a field
    {
        public SingleValueHeuristic() : base("Single Value", HeuristicAction.haSetValue)
        {
        }
        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            if (field.PossibleValues.Count == 1)
            {
                AddHeuristicField(field);
                AddAffectedField(field);
                AddValue(field.PossibleValues[0]);
                return true;
            }
            return false;
        }
    } // SingleValueHeuristic

    public class HiddenSingleValueHeuristic : TekHeuristic
    // the field is the only possibility in it's area
    {
        public HiddenSingleValueHeuristic() : base("Hidden Single", HeuristicAction.haSetValue)
        {
        }
        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            foreach(int value in field.PossibleValues)
            {
                bool isSingle = true;
                foreach (TekField field2 in field.Area.Fields)
                    if (field2 != field && field2.ValuePossible(value))
                        isSingle = false;
                if (isSingle)
                {
                    AddHeuristicField(field);
                    AddAffectedField(field);
                    AddValue(value);
                    return true;
                }
            }
            return false;
        }
    } // HiddenSingleValueHeuristic

    public class CoupledPairHeuristic : TekHeuristic
    // two fields are adjacent and have the same two possible values
    // all common influencers can not have the same value(s)
    {
        public CoupledPairHeuristic() : base("Coupled Pair", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            if (field.PossibleValues.Count != 2)
                return false;
            Region.AddField(field);
            foreach (TekField field2 in field.Influencers)
            {
                if (field == field2)
                    continue;
                if (Region.IsPair(field2))
                { 
                    AddHeuristicFields(field, field2);
                    foreach (TekField f in field.CommonInfluencers(field2))
                    {
                        bool isAffected = false;
                        foreach (int value in field.PossibleValues)
                            if (f.ValuePossible(value))
                            {
                                isAffected = true;
                                break;
                            }
                        if (isAffected)
                            AddAffectedField(f);
                    }
                    AddValues(field.PossibleValues);
                    return AffectedFields.Count > 0;
                }
            }
            return false;
        }
    }

    public class HiddenPairHeuristic : TekHeuristic
    {
    // similar to a normal Coupled Pair. two fields in the same area have two values in common.
    // no other fields in that area can have those two values. The other values in the triggering
    // fields can be eliminated. 
    // probably this reverts automatically to Coupled Triplets or Compact Regions
    //
        public HiddenPairHeuristic() : base("Hidden Pair", HeuristicAction.haExcludeComplement)
        {
        }
        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            Dictionary<int, List<TekField>> FieldsPerValueInArea = field.Area.GetFieldsForValues();
            List<int> CandidateValues = new List<int>();
            List<TekField> CandidateFields = new List<TekField>();
            foreach (int value in field.PossibleValues)
            {
                FieldsPerValueInArea[value].Remove(field);
                if (FieldsPerValueInArea[value].Count == 1)
                {
                    CandidateValues.Add(value);
                    CandidateFields.Add(FieldsPerValueInArea[value][0]);
                }
            }
            TekField field2 = null;
            int value1 = 0, value2 = 0;
            for (int i = 0; i < CandidateFields.Count && field2 == null; i++)
            {
                for (int j = i+1; j < CandidateFields.Count && field2 == null; j++)
                    if (CandidateFields[j] == CandidateFields[i])
                    {
                        field2 = CandidateFields[i];
                        value1 = CandidateValues[i];
                        value2 = CandidateValues[j];
                    }
            }
            if (field2 == null)
                return false;
            AddHeuristicFields(field, field2);
            if (field.PossibleValues.Count > 2)
                AddAffectedField(field);
            if (field2.PossibleValues.Count > 2)
                AddAffectedField(field2);
            AddValues(value1, value2);
            return AffectedFields.Count > 0;
        }
    }

    public class CoupledTripletsHeuristic : TekHeuristic
        // similar to two pairs, but with three fields. 
        // If they are influencers of eachother and have two or three of the same
        // shared possible values, all common influencers of the fields can not have
        // the three shared values
        //
    {
        public CoupledTripletsHeuristic() : base("Coupled Triplets", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            Region.Clear();
            Region.AddField(field);
            foreach (TekField field2 in field.Influencers)
            {
                Region.AddField(field2);
                foreach(TekField field3 in field.Influencers)
                    if (field != field2 && field != field3 && field2 != field3)
                        if (Region.IsTriplet(field3))
                        {
                            Region.AddField(field3);
                            AddHeuristicFields(Region.Fields);
                            AddValues(Region.GetTotalPossibleValues());
                            // determine affected fields
                            foreach(TekField f in field.CommonInfluencers(field2, field3))
                            {
                                foreach(int value in f.PossibleValues)
                                    if (HeuristicValues.Contains(value))
                                    {
                                        AddAffectedField(f);
                                        break;
                                    }
                            }
                            if (AffectedFields.Count > 0)
                                return true;
                            else
                                Reset();
                        }
                Region.RemoveField(field2);
            }
            return false;
        }
    } // TripletHeuristic

    public class CascadingTripletsHeuristic : TekHeuristic
    {
        // The field is next to two other fields that form a triplet with a third field. 
        // The triplet has two fields with only two alternatives (different ones, of course)
        // Entering one of the values in the triggering field would set the values (cascade) of two of the triplet fields, 
        // causing the third triplet field to have no values left
        //
        public CascadingTripletsHeuristic() : base("Coupled Triplets (cascade)", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            foreach (TekField field2 in field.Influencers)
                if (field2.PossibleValues.Count == 2 || field2.PossibleValues.Count == 3)
                {
                    Region.AddField(field2);
                    foreach (TekField field3 in field.CommonInfluencers(field2))
                        if (field3.PossibleValues.Count == 2)
                        {
                            Region.AddField(field3);
                            foreach (TekField field4 in field2.CommonInfluencers(field3))
                                if (field4 != field && field4.PossibleValues.Count == 2 && Region.IsTriplet(field4, false))
                                {
                                    foreach (int value in field.TotalPossibleValues(field2, field3))
                                        if (field.ValuePossible(value) && field2.ValuePossible(value) && field3.ValuePossible(value) && !field4.ValuePossible(value))
                                        {
                                            Region.AddField(field4);
                                            AddHeuristicFields(Region.Fields);
                                            AddAffectedField(field);
                                            AddValue(value);
                                            return true;
                                        }
                                }
                            Region.RemoveField(field3);
                        }
                    Region.RemoveField(field2);
                }
            return false;
        }
    } // TripletHeuristic2

    public class BlockingHeuristic : TekHeuristic
    {
        // The field is next to an area influencing all instances of a value in that area. 
        // Entering the value into the triggering field would render the next area invalid
        // because that value would be blocked by the triggering field
        //
        public BlockingHeuristic() : base("Blocking", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            List<TekArea> AdjacentAreas = field.Area.GetAdjacentAreas();
            foreach (TekArea area in AdjacentAreas)
            {
                foreach (int value in field.PossibleValues)
                {
                    bool possible = false;
                    Region.Clear();
                    foreach (TekField f in area.Fields)
                        if (f.ValuePossible(value))
                        {
                            if (field.Influencers.Contains(f))
                                Region.AddField(f);
                            else
                            {
                                possible = true;
                                break;
                            }
                        }
                    if (!possible && Region.Fields.Count > 0)
                    {
                        AddAffectedField(field);
                        AddValue(value);
                        AddHeuristicFields(Region.Fields);
                    }
                }
            }
            return AffectedFields.Count > 0 && HeuristicValues.Count > 0;
        }
    } // BlockingHeuristic

    public class BlockingThreePairsHeuristic : TekHeuristic
        // the field is next to a triplet where at least one of the triplet fields has only two values. 
        // if the triggering field blocks the third value of the triplet, the triplet reverts to 
        // three pairs which would be in an invalid configuration
        //
    {
        public BlockingThreePairsHeuristic() : base("Blocking (three pairs)", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            if (field.PossibleValues.Count == 0)
                return false;
            for (int i = 0; i < field.PossibleValues.Count; i++) // note: foreach can not work since we modify during processing!
            {
                try
                {
                    field.Value = field.PossibleValues[i];// trying this value
                    foreach (TekField field1 in field.Influencers) // field must at least influence two other fields (which could now be pairs)
                    {
                        Region.AddField(field1);
                        foreach (TekField field2 in field.Influencers)
                            if (field1 != field2)
                            {
                                Region.AddField(field2);
                                foreach (TekField field3 in field1.CommonInfluencers(field2))
                                    // and if there is a third field as well we might have the invalid configuration
                                    if (Region.IsInvalidThreePairs(field3))
                                    {
                                        Region.AddField(field3);
                                        AddHeuristicFields(Region.Fields);
                                        AddAffectedField(field);
                                        AddValue(field.Value);
                                        return true;
                                    }
                                Region.RemoveField(field2);
                            }
                    }
                }
                finally 
                {
                    field.Value = 0;
                }
            }
            Region.RemoveField(field);
            return false;
        }
    } // BlockingThreePairsHeuristic

    public class AlternatingChainHeuristic : TekHeuristic
        // a field influences two fields of a chain (a series of coupled pairs). 
        // The chain has an even number of steps between the two chain fields. 
        // The triggering field can not have any of the values in the chain
        //
    {
        TekChains Chains;
        public AlternatingChainHeuristic() : base("Alternating Chain", HeuristicAction.haExcludeValue)
        {
        }

        protected override void BeforeProcessingBoard(TekBoard board)
        {
            Chains = new TekChains(board);
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            if (!Chains.HasChains())
                return false;

            List<List<TekField>> localChains = new List<List<TekField>>();
            List<TekField> localFields = new List<TekField>();
            foreach(TekField f in field.Influencers)
            {
                List<TekField> chain = Chains.FindChain(f);
                if (chain != null && Chains.FindChain(field) != chain)
                {
                    localChains.Add(chain);
                    localFields.Add(f);
                }
            }
            for (int i = 0; i < localChains.Count; i++)
            {
                int j = localChains.IndexOf(localChains[i], i + 1);
                if (j != -1)
                {
                    if (Chains.ComputeDistance(localFields[i], localFields[j]) % 2 == 1)
                    {
                        bool noInfluence = true;
                        foreach (int value in Chains.ChainValues(localChains[i]))
                            if (field.ValuePossible(value))
                                noInfluence = false;
                        if (!noInfluence)
                        {
                            AddHeuristicFields(Chains.ShortestRoute(localFields[i], localFields[j]));
                            AddAffectedField(field);
                            AddValues(Chains.ChainValues(localChains[i]));
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    } // AlternatingChainHeuristic

    public class ConflictingChainsHeuristic : TekHeuristic
    // a variation/generalization of Alternating Chains:
    // a field is part of a chain and influences a field of another chain. 
    // both chains intersect at some other field as well and share one value 
    //  (two values is not possible and zero values means the chains don't interact)
    // if the first chain has an even number of steps between the endpoints (the other fields where the chains meet)
    // and the other chain has an odd number of steps, the common value of the second chain can be eliminated
    // if the first chain is odd and the second chain is even, the common value of the first chain can be eliminated
    // if they're both odd or even there is no interaction
    //
    {

        TekChains Chains;

        public ConflictingChainsHeuristic() : base("Conflicting Chains", HeuristicAction.haExcludeValue)
        {
        }

        protected override void BeforeProcessingBoard(TekBoard board)
        {
            Chains = new TekChains(board);
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            if (field.PossibleValues.Count != 2 || !Chains.HasChains())
                return false;
            List<TekField> chain = Chains.FindChain(field);
            if (chain == null)
                return false;
            foreach (TekField f in field.Influencers)
            {
                List<TekField> chain2 = Chains.FindChain(f);
                if (chain2 == null || chain2 == chain || Chains.CommonValues(chain, chain2).Count != 1)
                    continue;
                List<TekField> touchPoints1 = Chains.Intersection(chain2, chain);
                List<TekField> touchPoints2 = Chains.Intersection(chain, chain2);
                if (touchPoints1.Count == 2 && touchPoints2.Count == 2)
                {
                    List<TekField> chainFields1 = Chains.ShortestRoute(touchPoints1[0], touchPoints1[1]);
                    List<TekField> chainFields2 = Chains.ShortestRoute(touchPoints2[0], touchPoints2[1]);
                    if ((chainFields1.Count % 2 == 0) != (chainFields2.Count % 2 == 0))
                    {
                        AddValue(Chains.CommonValues(chain, chain2)[0]);
                        if (chainFields1.Count % 2 == 0)
                        {
                            AddAffectedFields(touchPoints2);
                            AddHeuristicFields(chainFields1);
                        }
                        else
                        {
                            AddAffectedFields(touchPoints1);
                            AddHeuristicFields(chainFields2);
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    } // ConflictingChainsHeuristic

    public class CompactRegionsHeuristic : TekHeuristic
    // based on "compact regions": if all fields in a region are mutual influencer 
    // (note: a compact region is not necessarily within one area)
    // and you check for each of the border fields if you can enter a value, in some cases
    // there are values that would leave insufficient possible values in the region, 
    // so you can eliminate that value(s) in the border field
    // some of the simpler heuristics are actually special cases of this compact regions heuristic
    //
    {
        public CompactRegionsHeuristic() : base("Compact Regions", HeuristicAction.haExcludeValue)
        {
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            foreach (TekRegion region in TekRegion.GetCompactRegions(field))
            {
                foreach (TekField f in region.GetBorderFields())
                {
                    try
                    {
                        int[] tryValues = f.PossibleValues.ToArray(); // setting value will empty possiblevalues array
                        foreach (int value in tryValues)
                        {
                            f.Value = value;
                            if (region.GetTotalPossibleValues().Count < region.Fields.Count)
                            {
                                AddHeuristicFields(region.Fields);
                                AddAffectedFields(f);
                                AddValue(value);
                            }
                        }
                        if (AffectedFields.Count > 0)
                            return true;
                    }
                    finally
                    {
                        f.Value = 0;
                    }
                }
            }
            return false;
        }
    } // CompactRegionsHeuristic

    public class TrialAndErrorHeuristic : TekHeuristic
    // try setting a value in a field, then see whether it can be solved using the other heuristics
    // in most cases you reach either an impossible situation (which means the tried value must be excluded)
    // or a solution (which means you set the tried value)
    // in the case you've found a solution, the heuristics steps you made are saved so that you can repeat the
    // steps without repeating the heuristics themselves
    //
    {
        public TekHeuristics heuristics;
        public TekMoves temMoves;
        const string STARTSTRING = @"startTrialAndError";
        List<TekHeuristicResult> temStoredResults;

        public TrialAndErrorHeuristic(TekHeuristics Heuristics) : base("Trial-and-Error", HeuristicAction.haSetValue)
        {
            heuristics = Heuristics;
        }

        protected override void BeforeProcessingBoard(TekBoard board)
        {
            temMoves = new TekMoves(board);
            temStoredResults = new List<TekHeuristicResult>();
        }

        private int _ssIndex = 1;
        private string _ssDescription(TekField field)
        {
            return String.Format("{0} ({1}): {2}", STARTSTRING, _ssIndex, field.AsString());
        }

        private HeuristicAction _tryValue(TekField field, int value)
        {
            try
            {
                field.Value = value;
            }
            catch (ETekFieldInvalid)
            {
                return HeuristicAction.haExcludeValue;
            }
            return HeuristicAction.haNone;
        }

        private HeuristicAction _tryHeuristic(TekHeuristic heuristic, TekBoard board)
        {
            if (heuristic != null)
            {
                try
                {
                    temStoredResults.Add(new TekHeuristicResult(heuristic));
                    heuristic.ExecuteAction(temMoves);
                    return HeuristicAction.haNone;
                }
                catch (ETekFieldInvalid)
                {
                    return HeuristicAction.haExcludeValue;
                }
            }
            else if (board.IsSolved())
            {
                foreach (TekHeuristicResult result in temStoredResults)
                    heuristics.PrecomputedResults.Add(new TekHeuristicResult(result));
                return HeuristicAction.haSetValue;
            }
            else
            {
                return HeuristicAction.haImpossible;
            }
        }

        private HeuristicAction TryValue(TekBoard board, TekField field, int value)
        {
            bool prev = board.EatExceptions;
            HeuristicAction result = HeuristicAction.haNone;
            try
            {
                board.EatExceptions = false;
                this.Enabled = false; // make sure FindHeuristic doesnt call this recursively!
                if ((result = _tryValue(field, value)) == HeuristicAction.haNone)
                {
                    temStoredResults.Clear();
                    temMoves.TakeSnapshot(_ssDescription(field));
                    try { 
                        do
                        {
                            try
                            {
                                result = _tryHeuristic(heuristics.FindHeuristic(board), board);
                            }
                            catch (ETekFieldInvalid)
                            {
                                result = HeuristicAction.haImpossible;
                            }

                        } while (result == HeuristicAction.haNone);
                    }
                    finally
                    {
                        temMoves.RestoreSnapshot(_ssDescription(field));
                        _ssIndex++;
                    }
                }
            }
            finally
            {
                board.EatExceptions = prev;
                this.Enabled = true;
                field.Value = 0; // backtracking
            }
            return result;
        }

        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            HeuristicAction action = HeuristicAction.haNone;
            HeuristicAction found = HeuristicAction.haNone;
            HeuristicValues.Clear(); // clear any values already set
            foreach (int value in new List<int>(field.PossibleValues)) // can't use the list directly in foreach
            {
                switch (action = TryValue(board, field, value))
                {
                    case HeuristicAction.haSetValue:
                        HeuristicValues.Clear();
                        AddValue(value);
                        found = action;
                        break;
                    case HeuristicAction.haExcludeValue:
                        AddValue(value);
                        found = action;
                        break;
                }
                if (action == HeuristicAction.haSetValue) // solution found, so we can stop
                    break;
                // if you haven't found a solution, it doesnt hurt to continue maybe you can find a solution
                // or maybe you can add a second value to exclude
            }
            if (found == HeuristicAction.haSetValue || found == HeuristicAction.haExcludeValue)
            {
                SetHeuristicAction(found);
                AddHeuristicField(field);
                AddAffectedField(field);
            }
            return AffectedFields.Count > 0;
        }
    } // TrialAndErrorHeuristic


    public class BruteForceHeuristic : TekHeuristic
    {   // simple brute force solving with backtracking making sure that IF there is a solution it will be found even if no other heuristics do
        // in theory trial-and-error might not find always find either a solution or an impossible situation
        //

        private class TekFieldComparer : IComparer<TekField>
        {
            public int Compare(TekField x, TekField y)
            {
                if (x.PossibleValues.Count == y.PossibleValues.Count)
                    return 0;
                else if (x.PossibleValues.Count == 0)
                    return 1;
                else if (y.PossibleValues.Count == 0 || x.PossibleValues.Count < y.PossibleValues.Count)
                    return -1;
                else
                    return 1;
            }
        }

        private List<TekField> _SortedCandidates;
        private List<TekField> _NonCandidates;
        protected List<TekField> SortedCandidates { get { return _SortedCandidates; } }
        protected List<TekField> NonCandidates { get { return _NonCandidates; } }
        private TekFieldComparer sorter;
        private TekBoard Board;
        public TimeSpan timeElapsed;

        public override string AsString()
        {
            return String.Format("{0} (time: {1:00}:{2:00}.{3:00})", Description, timeElapsed.Minutes, timeElapsed.Seconds, timeElapsed.Milliseconds);
        }
        public BruteForceHeuristic() : base("Brute Force", HeuristicAction.haNone)
        {
            _SortedCandidates = new List<TekField>();
            _NonCandidates = new List<TekField>();
            sorter = new TekFieldComparer();
        }
        public void SortFields()
        {
            SortedCandidates.Sort(sorter);
        }
        private void SetFieldValue(TekField field, int value)
        {
            field.Value = value;
            if (value != 0)
            {
                NonCandidates.Add(field);
                SortedCandidates.Remove(field);
            }
            else
            {
                SortedCandidates.Add(field);
                NonCandidates.Remove(field);
            }
            SortFields();
        }
        private bool BruteForceSolve()
        {
            if (SortedCandidates.Count == 0)
                return Board.IsSolved();
            TekField Field0 = SortedCandidates[0];
            if (Field0.PossibleValues.Count == 0)
                return Board.IsSolved();
            for (int i = 0; i < Field0.PossibleValues.Count; i++)
            {
                SetFieldValue(Field0, Field0.PossibleValues[i]);
                if (BruteForceSolve())
                    return true;
                else // backtrack 
                {
                    SetFieldValue(Field0, 0);
                }
            } // if we get here, this branch has no solution
            return false;
        }
        protected override void BeforeProcessingBoard(TekBoard board)
        {
            Board = new TekBoard(board);
            SortedCandidates.Clear();
            foreach (TekField field in Board.Fields)
                if (field.Value == 0)
                    SortedCandidates.Add(field);
            SortFields();
        }
        public override bool HeuristicApplies(TekBoard board, TekField field)
        {
            Stopwatch s = Stopwatch.StartNew();
            if (BruteForceSolve())
            {
                s.Stop();
                timeElapsed = s.Elapsed;
                board.LoadValues(Board.CopyValues());
                return true;
            }
            return false; // can't be solved            
        }
    } // BruteForceHeuristic

    public class TekHeuristicResult
    {
        public TekHeuristic Heuristic;
        public List<TekField> HeuristicFields;
        public List<TekField> AffectedFields;
        public List<int> HeuristicValues;
        public TekHeuristicResult(TekHeuristic heuristic)
        {
            Heuristic = heuristic;
            HeuristicFields = new List<TekField>(Heuristic.HeuristicFields);
            AffectedFields = new List<TekField>(Heuristic.AffectedFields);
            HeuristicValues = new List<int>(heuristic.HeuristicValues);
        }
        public TekHeuristicResult(TekHeuristicResult result)
        {
            Heuristic = result.Heuristic;
            HeuristicFields = new List<TekField>(result.HeuristicFields);
            AffectedFields = new List<TekField>(result.AffectedFields);
            HeuristicValues = new List<int>(result.HeuristicValues);
        }
        public TekHeuristic AsHeuristic()
        {
            Heuristic.Reset();
            Heuristic.HeuristicFields.AddRange(HeuristicFields);
            Heuristic.AffectedFields.AddRange(AffectedFields);
            Heuristic.HeuristicValues.AddRange(HeuristicValues);
            return Heuristic;
        }
    }

    public delegate void AfterHeuristicFound(TekHeuristic heuristic);
    public delegate bool HeuristicExecution(TekHeuristic heuristic);

    public class TekHeuristics
    {
        List<TekHeuristic> Heuristics;
        List<TekHeuristicResult> StoredResults;
        public List<TekHeuristicResult> PrecomputedResults;
        private List<int> HeuristicIndex;
        public AfterHeuristicFound AfterHeuristicFoundHandler;

        public HeuristicExecution BeforeExecutionHandler;
        public HeuristicExecution AfterExecutionHandler;

        public TekHeuristics()
        {
            StoredResults = new List<TekHeuristicResult>();
            PrecomputedResults = new List<TekHeuristicResult>();
            Heuristics = new List<TekHeuristic>();
            Heuristics.Add(new SingleValueHeuristic());
            Heuristics.Add(new HiddenSingleValueHeuristic());
            Heuristics.Add(new CoupledPairHeuristic());
            Heuristics.Add(new HiddenPairHeuristic());
            Heuristics.Add(new CompactRegionsHeuristic());
            Heuristics.Add(new CoupledTripletsHeuristic());
            Heuristics.Add(new BlockingHeuristic());
            Heuristics.Add(new BlockingThreePairsHeuristic());
            Heuristics.Add(new AlternatingChainHeuristic());
            Heuristics.Add(new CascadingTripletsHeuristic());
            Heuristics.Add(new ConflictingChainsHeuristic());
            Heuristics.Add(new TrialAndErrorHeuristic(this));
            Heuristics.Add(new BruteForceHeuristic());
            HeuristicIndex = new List<int>();
            try
            {
                using (StreamReader cfg = new StreamReader("heuristics.cfg"))
                {
                    LoadConfiguration(cfg);
                }
            }
            catch (Exception)
            {
                for (int i = 0; i < Heuristics.Count; i++)
                {
                    HeuristicIndex.Add(i);
                }
            }
        }

        public List<string> GetHeuristicDescriptions()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < HeuristicIndex.Count; i++)
            {
                result.Add(Heuristics[HeuristicIndex[i]].Description);
            }
            return result;
        }

        public void SetHeuristicDescriptions(List<string> descriptions)
        {
            HeuristicIndex.Clear();
            for (int i = 0; i < descriptions.Count; i++)                
            {
                string description = descriptions[i];
                TekHeuristic heuristic = GetHeuristic(description);
                if (heuristic != null) 
                    HeuristicIndex.Add(Heuristics.IndexOf(heuristic));
            }
        }

        public TekHeuristic GetHeuristic(string description)
        {
            foreach (TekHeuristic heuristic in Heuristics)
                if (heuristic.Description == description)
                    return heuristic;
            return null;
        }

        public bool GetHeuristicEnabled(string description)
        {
            TekHeuristic heuristic = GetHeuristic(description);
            if (heuristic != null)
                return heuristic.Enabled;
            else
                return false;
        }

        public void SetHeuristicEnabled(string description, bool value)
        {
            TekHeuristic heuristic = GetHeuristic(description);
            if (heuristic != null)
                heuristic.Enabled = value;
        }
        public TekHeuristic FindHeuristic(TekBoard board)
        {
            board.AutoNotes = true;
            if (PrecomputedResults.Count > 0)
            {
                TekHeuristic result = PrecomputedResults[0].AsHeuristic();
                PrecomputedResults.RemoveAt(0);
                return result;
            }
            for(int i = 0; i < HeuristicIndex.Count; i++)
            {
                TekHeuristic heuristic = Heuristics[HeuristicIndex[i]];
                if (heuristic.Enabled && heuristic.Applies(board))
                {
                    return heuristic;
                }
            }
            return null;
        }
        public int StoreResult(TekHeuristic heuristic)
        {
            StoredResults.Add(new TekHeuristicResult(heuristic));
            return StoredResults.Count - 1;
        }
        public TekHeuristicResult ReturnResult(int index)
        {
            return StoredResults[index];
        }
        public void Reset()
        {
            StoredResults.Clear();
            PrecomputedResults.Clear();
            foreach (TekHeuristic h in Heuristics)
                h.Reset();
        }
        public void Dump(StreamWriter sw)
        {
            sw.WriteLine("heuristics (in order of application):");
            for(int i = 0; i < HeuristicIndex.Count; i++)
            {
                TekHeuristic heuristic = Heuristics[HeuristicIndex[i]];
                string s = String.Format("{0}: {1}", i + 1, heuristic.Description);
                if (!heuristic.Enabled)
                    s = s + "[disabled]";
                sw.WriteLine(s);
            }
        }

        public bool HeuristicSolve(TekBoard board, TekMoves moves)
        {
            TekHeuristic heuristic = FindHeuristic(board);
            bool Paused = false;
            while (heuristic != null && !Paused)
            {
                AfterHeuristicFoundHandler?.Invoke(heuristic);
                StoreResult(heuristic);
                if (BeforeExecutionHandler != null && !BeforeExecutionHandler(heuristic))
                    return false;
                heuristic.ExecuteAction(moves);
                AfterExecutionHandler?.Invoke(heuristic);
                heuristic = FindHeuristic(board);
            }
            return board.IsSolved();     
        }

        const string HEURISTICS = @"HEURISTICS:";
        const string HEURFORMAT = @"HEUR{0}={1}{2}";
        const string HEURPATTERN = @"HEUR(?<index>\d+)=(((?<description>.*)(?<disabled>\(disabled\)))|(?<description>.*))";
        public void SaveConfiguration(StreamWriter sw)
        {
            sw.WriteLine(HEURISTICS);
            for (int i = 0; i < HeuristicIndex.Count; i++)
            {
                TekHeuristic heuristic = Heuristics[HeuristicIndex[i]];
                sw.WriteLine(HEURFORMAT, i, heuristic.Description, heuristic.Enabled?"":"(disabled)");
            }
        }


        public bool LoadConfiguration(StreamReader sr)
        {
            string s = sr.ReadLine();
            if (s == null)
                return false;
            if (s != HEURISTICS)
                return false;
            HeuristicIndex.Clear();
            Regex pattern = new Regex(HEURPATTERN);
            string description;
            TekHeuristic heuristic;
            int index;
            while ((s = sr.ReadLine()) != null)
            {
                Match match = pattern.Match(s);
                if (match.Success &&
                   Int32.TryParse(match.Groups["index"].Value, out index) &&
                    (description = match.Groups["description"].Value) != null &&
                    (heuristic = GetHeuristic(description)) != null)
                {
                    HeuristicIndex.Add(Heuristics.IndexOf(heuristic));
                    heuristic.Enabled = (match.Groups["disabled"].Value == "");
                }
            }
            return true;
        }
    }    
}
