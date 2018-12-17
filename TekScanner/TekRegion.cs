using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Tek1
{
    public class TekRegionComparer : IComparer<TekRegion>
    {
        public int Compare(TekRegion x, TekRegion y)
        {
            if (x.Fields.Count > y.Fields.Count)
                return -1;
            else if (x.Fields.Count < y.Fields.Count)
                return 1;
            else
                return 0;
        }
    }
    public class TekRegion : TekFields
    {
        public TekRegion() : base()
        {
        }

        public TekRegion(params TekField[] fields)
        {
            AddFields(fields);
        }

        public TekRegion(List<TekField> fields)
        {
            AddFields(fields);
        }

        public void AddFields(params TekField[] fields)
        {
            foreach (TekField field in fields)
                AddField(field);
        }

        public void AddFields(List<TekField> fields)
        {
            foreach (TekField field in fields)
                AddField(field);
        }

        public void RemoveField(TekField field)
        {
            Fields.Remove(field);
        }

        public void Clear()
        {
            Fields.Clear();
        }

        protected bool IsPair(TekField field1, TekField field2)
        // hidden pairs are ignored
        {
            if (!field1.Influencers.Contains(field2))
                return false;
            if (field1.PossibleValues.Count != 2 || field2.PossibleValues.Count != 2)
                return false;
            foreach (int value in field1.PossibleValues)
                if (!field2.ValuePossible(value))
                    return false;
            return true;
        }

        public bool IsPair(TekField field)
        // hidden pairs are ignored
        {
            if (Fields.Count != 1)
                return false;
            else
                return IsPair(Fields[0], field);
        }

        public bool IsPair()
        // hidden pairs are ignored
        {
            if (Fields.Count != 2)
                return false;
            return IsPair(Fields[0], Fields[1]);
        }


        protected bool IsTriplet(TekField field1, TekField field2, TekField field3, bool inSameArea = true)
        // hidden triplets are ignored
        {
            if (inSameArea && (field1.Area != field2.Area || field1.Area != field3.Area || field2.Area != field3.Area))
                return false;
            if (field1.TotalPossibleValues(field2, field3).Count != 3)
                return false;
            // 2 or 3 values per field
            if (field1.PossibleValues.Count < 2 || field1.PossibleValues.Count > 3)
                return false;
            if (field2.PossibleValues.Count < 2 || field2.PossibleValues.Count > 3)
                return false;
            if (field3.PossibleValues.Count < 2 || field3.PossibleValues.Count > 3)
                return false;

            return true;
        }

        public bool IsTriplet(TekField field3, bool inSameArea = true)
        {
            if (Fields.Count != 2)
                return false;
            else
                return IsTriplet(Fields[0], Fields[1], field3, inSameArea);
        }

        public bool IsTriplet(bool inSameArea = true)
        // hidden triplets are ignored
        {
            if (Fields.Count != 3)
                return false;
            return IsTriplet(Fields[0], Fields[1], Fields[2]);
        }

        protected bool IsInvalidThreePairs(TekField field1, TekField field2, TekField field3)
        {
            if (field1.CommonPossibleValues(field2, field3).Count != 2 || !IsPair(field1, field2) || !IsPair(field1, field3) || !IsPair(field2, field3))
                return false;
            return (field1.Influencers.Contains(field2) && field1.Influencers.Contains(field3) && field2.Influencers.Contains(field3));
        }

        public bool IsInvalidThreePairs(TekField field3)
        {
            if (Fields.Count != 2)
                return false;
            return IsInvalidThreePairs(Fields[0], Fields[1], field3);
        }

        public List<TekField> CommonInfluencers()
        {
            List<TekField> result = new List<TekField>();
            if (Fields.Count == 0)
                return result;
            return Fields[0].CommonInfluencers(Fields.GetRange(1, Fields.Count - 1));
        }

        public bool IsInvalidThreePairs()
        {
            if (Fields.Count != 3)
                return false;
            TekField field1 = Fields[0];
            TekField field2 = Fields[1];
            TekField field3 = Fields[1];

            if (field1.CommonPossibleValues(field2, field3).Count != 2 || !IsPair(field1, field2) || !IsPair(field1, field3) || !IsPair(field2, field3))
                return false;
            return (field1.Influencers.Contains(field2) && field1.Influencers.Contains(field3) && field2.Influencers.Contains(field3));
        }

        public bool IsCompact(TekField addingField)
        {
            if (!IsCompact() || addingField.Value != 0 || Fields.Contains(addingField))
                return false;
            int i = 0;
            while (i < Fields.Count)
            {
                if (!addingField.Influencers.Contains(Fields[i]))
                    return false;
                i++;
            }
            return true;
        }

        public bool IsCompact()
        {
            if (Fields.Count == 0)
                return false;
            int i = 0;
            while (i < Fields.Count)
            {
                TekField field1 = Fields[i++];
                if (field1.Value != 0)
                    continue;
                int j = i;
                while (j < Fields.Count)
                {
                    if (Fields[j].Value == 0 && !Fields[j].Influencers.Contains(field1))
                        return false;
                    j++;
                }
            }
            return true;
        }

        public List <TekField> GetBorderFields()
        {
            List <TekField > result = new List<TekField>();
            foreach (TekField field in Fields)
                foreach (TekField f in field.Influencers)
                    if (f.Value == 0 && !Fields.Contains(f) && !result.Contains(f))
                        result.Add(f);
            return result;
        }
        static bool ListContains(List<TekRegion> list, TekRegion region)
        {
            foreach (TekRegion listRegion in list)
                if (region.IsEqual(listRegion))
                    return true;
            return false;
        }
        static private List <TekRegion> GetSize2CompactRegions(TekField field)
        {
            List<TekRegion> result = new List<TekRegion>();
            if (field.Value != 0)
                return result;
            foreach(TekField f in field.Influencers)
            {
                if (f.Value == 0)
                {
                    TekRegion newRegion = new TekRegion(field, f);
                    if (!ListContains(result, newRegion))
                        result.Add(newRegion);
                }
            }
            result.Sort(new TekRegionComparer());
            return result;
        }

        static private List<TekRegion> GetCompactRegions(List<TekRegion> list)
        {
            List<TekRegion> result = new List<TekRegion>();
            foreach(TekRegion region in list)
            {
                foreach (TekField f in region.CommonInfluencers())
                {
                    if (region.IsCompact(f))
                    {
                        TekRegion region2 = new TekRegion(region.Fields);
                        region2.AddField(f);
                        if (!ListContains(result, region2))
                            result.Add(region2);
                    }
                }
            }
            result.Sort(new TekRegionComparer());
            return result;
        }

        static public List <TekRegion> GetCompactRegions(TekField field)
        {
            List<TekRegion> result = GetSize2CompactRegions(field);
            for (int i = 3; i <= Const.MAXTEK; i++)
            {
                List<TekRegion> newList = GetCompactRegions(result);
                foreach (TekRegion region in newList)
                    if (!ListContains(result, region))
                        result.Add(region);
            }                
            result.Sort(new TekRegionComparer());
            return result;
        }

        static public void DumpList(List<TekRegion> list, StreamWriter sw)
        {
            foreach(TekRegion region in list)
            {
                sw.WriteLine("--- region: ");
                region.Dump(sw);
                if (!region.IsCompact())
                    sw.WriteLine("*** ERROR: not compact");
            }
        }
    }
}
