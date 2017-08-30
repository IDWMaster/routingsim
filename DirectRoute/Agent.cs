using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace DirectRoute
{
    class Agent:IComparable<Agent>
    {
        HashSet<uint> knownUnreachables = new HashSet<uint>();
        SortedSet<Agent> connections = new SortedSet<Agent>();
        Agent[] cachedConnections = null;
        //List<Agent> connections = new List<Agent>();
        public uint ID;
        public Agent(Random mrand)
        {

            ID = unchecked((uint)mrand.Next(int.MinValue, int.MaxValue));
        }
        object searchToken = null;
        bool GraphSearch(uint id,object token)
        {
            if(knownUnreachables.Contains(id))
            {
                return false;
            }
            foreach(var iable in connections)
            {
                if(iable.searchToken == token)
                {
                    //Already traversed
                    continue;
                }
                iable.searchToken = token;
                if(iable.ID == id)
                {
                    return true;
                }
                iable.GraphSearch(id, token);
            }
            knownUnreachables.Add(id);
            return false;
        }
        public bool CanRoute(uint id)
        {
            return GraphSearch(id, new object());
        }
        public void AddConnection(Agent other)
        {
            knownUnreachables.Clear();
            cachedConnections = null;
            if(other == this)
            {
                return;
            }
            connections.Add(other);
        }
        public int TryRoute(Agent from,Agent dest,int hops)
        {

            hops++;
            if(dest.ID == ID)
            {
                return hops;
            }
            if(hops == 30)
            {
                return -1;
            }

            if(cachedConnections == null)
            {
                cachedConnections = connections.ToArray();
            }
            if(cachedConnections.Length == 0)
            {
                return -1;
            }
            int found = Array.BinarySearch(cachedConnections, dest);
            if(found<0)
            {
                found = ~found;
                if(found == cachedConnections.Length)
                {
                    found--;
                }
            }

            if(cachedConnections[found] == from)
            {
                return -1; //NOTE: This should never happen
            }
            return cachedConnections[found].TryRoute(this, dest, hops);

            /*uint bestDiff = uint.MaxValue;
            Agent candidate = null;
            foreach(var iable in connections)
            {
                if(iable == from)
                {
                    continue;
                }
                uint diff = iable.ID ^ dest;
                if(diff<bestDiff)
                {
                    candidate = iable;
                    bestDiff = diff;
                }
            }
            if(candidate == null || hops == 30)
            {
                return -1; //unroutable
            }

            return candidate.TryRoute(this, dest, hops);*/
        }

        public int CompareTo(Agent other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}
