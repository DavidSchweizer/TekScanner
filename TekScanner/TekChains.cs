using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tek1
{
    class TekChains
    {
        List<List<TekField>> Chains;
        List<int[,]> Distances;

        private List<TekField> ChainBackTracking = new List<TekField>(); 
        // prevent circular routing when computing distances

        public TekChains(TekBoard board)
        {
            Chains = new List<List<TekField>>();
            Distances = new List<int[,]>();
            InitializeChains(board);
            NormalizeChains();
            SortChains();
            ComputeDistances();
        }

        public bool HasChains()
        {
            return Chains.Count > 0;
        }

        private void SortChains()
        {// give a more logical order to the fields (debugging purposes)
            TekFieldComparer sorter = new TekFieldComparer();
            for (int i = 0; i < Chains.Count; i++)
                Chains[i].Sort(sorter);
        }

        private void InitializeChains(TekBoard board)
        {
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Cols; c++)
                {
                    if (board.Fields[r, c].Value == 0 && 
                        board.Fields[r, c].PossibleValues.Count == 2)
                        AddField(board, board.Fields[r, c]);
                }
        }

        private void AddField(TekBoard board, TekField field)
        {
            List<TekField> chain = FindChain(field);
            if (chain == null)
            {
                chain = new List<TekField>();
                Chains.Add(chain);
            }
            chain.Add(field);
        }

        private bool HasSameValues(List<TekField> chain1, List<TekField> chain2)
        {
            if (chain1 == null || chain1.Count == 0 || chain2 == null || chain2.Count == 0)
                return false;
            else
            {
                return chain1[0].ValuesPossible(chain2[0].PossibleValues[0], chain2[0].PossibleValues[1]);
            }
        }

        private void NormalizeChains()
        // combine chains if possible, also remove chains that are too small
        {
            int i = 0;
            while (i < Chains.Count)
            {
                int j = i+1;
                while (j < Chains.Count)
                {
                    bool canCombine = false;
                    if (HasSameValues(Chains[i], Chains[j]))
                    { 
                        foreach (TekField field in Chains[j])
                        {
                            if (IsConnected(Chains[i], field))
                            {
                                canCombine = true;
                                break;
                            }
                        }
                    }
                    if (canCombine)
                    {
                        foreach (TekField field in Chains[j])
                            Chains[i].Add(field);
                        Chains.RemoveAt(j);
                    }
                    else
                        j++;
                }
                if (Chains[i].Count < 2) // ?? <= is perhaps better, a chain of two fields is also a pair
                    Chains.RemoveAt(i);
                else
                    i++;
            }
        }

        public int ComputeDistance(TekField field1, TekField field2)
        {
            List<TekField> chain = FindChain(field1); 
            if (chain == null || !chain.Contains(field2))
                return -1;
            int[,] table = Distances[Chains.IndexOf(chain)];
            return table[chain.IndexOf(field1), chain.IndexOf(field2)];
        }

        private int ComputeDistance(TekField field1, TekField field2, List<TekField> chain)
        {
            if (field1.Influencers.Contains(field2))
                return 1;
            else
            {
                int value = ComputeDistance(field1, field2);
                if (value >= 0)
                    return value;
                else
                {
                    int minDistance = Int32.MaxValue-1;
                    foreach (TekField f in field1.Influencers)
                        if (chain.Contains(f) && !ChainBackTracking.Contains(f))
                        {
                            ChainBackTracking.Add(f);
                            value = ComputeDistance(f, field2, chain);
                            if (value < minDistance)
                                minDistance = value;
                            ChainBackTracking.Remove(f);
                        }
                    return 1 + minDistance;
                }
            }
        }

        private void ComputeChainDistances(List<TekField> chain)
        {
            int[,] table = new int[chain.Count, chain.Count];
            Distances.Add(table);
            for (int i = 0; i < chain.Count; i++)
                for (int j = i + 1; j < chain.Count; j++)
                {
                    table[i, j] = -1;
                    table[j, i] = -1;
                }
            for (int i = 0; i < chain.Count; i++)
            {                                
                TekField field = chain[i];
                ChainBackTracking.Clear();
                ChainBackTracking.Add(field);
                for (int j = i + 1; j < chain.Count; j++)
                {
                    table[i, j] = ComputeDistance(field, chain[j], chain);
                    table[j, i] = table[i, j];
                }
            }
        }

        private void ComputeDistances()
        {
            Distances.Clear();
            for (int i = 0; i < Chains.Count; i++)
            {
                ComputeChainDistances(Chains[i]);
            }
        }

        public List<TekField> FindChain(TekField field)
        {
            for (int i = 0; i < Chains.Count; i++)
            {
                List<TekField> chain = Chains[i];
                if (chain.Count > 0 && field.PossibleValues.Count == 2 && 
                    chain[0].ValuesPossible(field.PossibleValues[0], field.PossibleValues[1]) 
                    && IsConnected(chain, field))
                    return chain;
            }                
            return null;
        }

        private bool IsConnected(List<TekField> chain, TekField field)
        {
            if (chain.Contains(field))
                return true;
            else
                foreach (TekField f in chain)
                   if (f.Influencers.Contains(field))
                        return true;
            return false;
        }

        public List<int> ChainValues(List<TekField> chain)
        {
            return chain[0].PossibleValues;
        }

        public List<int> CommonValues(List<TekField> chain1, List<TekField> chain2)
        {
            List<int> result = new List<int>();
            foreach (int value in ChainValues(chain1))
            {
                if (chain2[0].ValuePossible(value))
                    result.Add(value);
            }
            return result;
        }

        public List<TekField> Intersection(List<TekField> chain1, List<TekField> chain2)
        // returns all fields in chain2 that touch chain1
        {
            List<TekField> result = new List<TekField>();
            foreach (TekField field in chain1)
                foreach (TekField field2 in field.Influencers)
                    if (chain2.Contains(field2))
                        result.Add(field2);
            return result;
        }

        public List<TekField> ShortestRoute(TekField field1, TekField field2)
        {
            List<TekField> chain = FindChain(field1);
            if (!chain.Contains(field2))
                return null;
            List<TekField> result = new List<TekField>();
            result.Add(field1);
            int distance = ComputeDistance(field1, field2);
            if (distance == 1)
            {
                result.Add(field2);
                return result;
            }
            List<List<TekField>> tempList = new List<List<TekField>>();
            foreach (TekField f in field1.Influencers)
                if (chain.Contains(f) && ComputeDistance(f, field2) < distance)
                    tempList.Add(ShortestRoute(f, field2));

            foreach(List<TekField> list in tempList)
                if (list != null) 
                {
                    result.InsertRange(1, list);
                    return result;
                }
            return null;
        }

        public void Dump(StreamWriter sw)
        {
            for (int i = 0; i < Chains.Count; i++)
            {
                sw.Write("chain {0} [{1},{2}]: ", i, Chains[i][0].PossibleValues[0], Chains[i][0].PossibleValues[1]);
                foreach (TekField field in Chains[i])
                    sw.Write(field.AsString());
                    //field.Dump(sw, TekField.FLD_DMP_INFLUENCERS | TekField.FLD_DMP_POSSIBLES);
                sw.WriteLine("...end chain {0}", i);
            }

            sw.WriteLine("distances:");
            for (int i = 0; i < Distances.Count; i++)
            {
                sw.WriteLine("Chain {0}", i);
                int[,] table = Distances[i];
                for (int j = 0; j < table.GetLength(0); j++)
                {
                    string s = "";
                    for (int k = 0; k < table.GetLength(1); k++)
                        s = s + String.Format("{0,2} ", table[j, k]);
                    sw.WriteLine(s);
                }
            }
        }
    } // TekChains
}
