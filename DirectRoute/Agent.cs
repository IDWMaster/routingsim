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
        Dictionary<Agent, Agent> knownMappings = new Dictionary<Agent, Agent>();

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
            if(id == ID)
            {
                return true;
            }
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
               if(iable.GraphSearch(id, token))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanRoute(uint id)
        {
            bool retval = GraphSearch(id, new object());
            
            return retval;
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
        public int TryRoute(Agent origin,Agent from,Agent dest,int hops)
        {
            if (!knownMappings.ContainsKey(origin) && from != null)
            {
                knownMappings.Add(origin, from);
            }
            
            hops++;
            
            if (dest == this)
            {
                return hops;
            }
            if(hops == 30)
            {
                return -1;
            }
            if (knownMappings.ContainsKey(dest))
            {
                return knownMappings[dest].TryRoute(origin, this, dest, hops);
            }
            if (cachedConnections == null)
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
                uint bestDiff = uint.MaxValue;
                Agent value = null;
                if (found + 1 < cachedConnections.Length)
                {
                    uint diff = cachedConnections[found + 1].ID ^ ID;
                    if (found > 0)
                    {
                        uint odiff = ID ^ cachedConnections[found - 1].ID;
                        if (odiff < diff)
                        {
                            found--;
                        }
                        else
                        {
                            found++;
                        }
                    }
                }
                else
                {
                    found--;
                }
            }
            int retval = cachedConnections[found].TryRoute(origin,this, dest, hops);
            return retval;
           
        }

        public int CompareTo(Agent other)
        {
            return unchecked((int)(ID-other.ID));
        }
    }
}
