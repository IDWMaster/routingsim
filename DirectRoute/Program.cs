using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace DirectRoute
{
    class Program
    {
        static void PopulateRandom(Agent[] fb)
        {

            Random shield = new Random(42);
            for (int i = 0; i < fb.Length; i++)
            {
                fb[i] = new Agent(shield);

            }
            for (int i = 0; i < fb.Length; i++)
            {
                for (int c = 0; c < 100; c++ /*This is how C++ was invented*/)
                {
                    fb[i].AddConnection(fb[shield.Next(0, fb.Length)]);
                }
            }
        }

        static void Main(string[] args)
        {
            

            Stopwatch mwatch = new Stopwatch();
            Random mrand = new Random();
            Agent[] fb = new Agent[200];
            
            PopulateRandom(fb);


            int maxhops = 0;
            int reachable = 0;
            int total = 0;
            for(int i = 0;i<fb.Length;i++)
            {
                for (int c = 0; c < fb.Length; c++ /*this is how C++ was invented*/)
                {
                    mwatch.Start();
                    int hops = fb[i].TryRoute(fb[i],null, fb[c], 0);
                    if(hops>maxhops)
                    {
                        maxhops = hops;
                    }
                    mwatch.Stop();
                    if(hops == -1)
                    {
                        if(fb[i].CanRoute(fb[c].ID))
                        {
                            
                            total++;
                        }
                    }else
                    {
                        if(!fb[i].CanRoute(fb[c].ID))
                        {
                            throw new Exception();
                        }
                        reachable++;
                        total++;
                    }
                }
            }
            Console.WriteLine("Success rate: " + ((((double)reachable/total))*100)+"%"+"\nTotal CPU time: "+mwatch.Elapsed+"\nMax hops:" +maxhops);
        }
    }
}
