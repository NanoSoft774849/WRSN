using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class hex
    {
        // row
        public double r;//row
        // column
        public double q;//column
        public hex(double r, double q)
        {
            this.r = r;
            this.q = q;
        }
        public cube toCube()
        {
            cube cb = new cube(this.q, this.r);
            return cb;
        }
        public hex scale(double r)
        {
            this.q *= r;
            this.r *= r;
            return this;
        }
        public hex scale(double r, double c)
        {
            this.q *= c;
            this.r *= r;
            return this;
        }
        public ns_point toPointy(double size)
        {
            var x = size * (sqrt(3) * this.q + sqrt(3) / 2 * this.r);
            var y = size * (this.r * 3.0 / 2);
            return new ns_point(x, y);
        }
        public ns_point toFlatPoint(double size)
        {
            var x = size * (this.q * 3 / 2);
            var y = size * ((this.q * sqrt(3) / 2) + (sqrt(3) * this.r));
            return new ns_point(x, y);
        }
        public ns_point toPointy(ns_point start, double size)
        {
            var x = size * (sqrt(3) * this.q + sqrt(3) / 2 * this.r);
            var y = size * (this.r * 3.0 / 2);
            return start + new ns_point(x, y);
        }
        public ns_point toFlatPoint(ns_point start , double size)
        {
            var x = size * (this.q * 3 / 2);
            var y = size * ((this.q * sqrt(3) / 2) + (sqrt(3) * this.r));
            return start + new ns_point(x, y);
        }
        private double sqrt(double x)
        {
            return Math.Sqrt(x);
        }
        public List<hex> Circular()
        {
            // Hex(+1, 0), Hex(+1, -1), Hex(0, -1), 
            // Hex(-1, 0), Hex(-1, +1), Hex(0, +1)
            List<hex> hexs = new List<hex>();
           // hexs.Add(new hex(0, 0));

            hexs.Add(new hex(1, 0));
            hexs.Add(new hex(1, -1));

            hexs.Add(new hex(0, -1));
            hexs.Add(new hex(-1, 0));

            hexs.Add(new hex(-1, 1));
            hexs.Add(new hex(0, 1));

            return hexs;
        }
        public List<hex> Circular(double r, double q)
        {
            // Hex(+1, 0), Hex(+1, -1), Hex(0, -1), 
            // Hex(-1, 0), Hex(-1, +1), Hex(0, +1)
            List<hex> hexs = new List<hex>();
            // hexs.Add(new hex(0, 0));

            hexs.Add(new hex(r, 0));
            hexs.Add(new hex(r, -q));

            hexs.Add(new hex(0, -q));
            hexs.Add(new hex(-r, 0));

            hexs.Add(new hex(-r, q));
            hexs.Add(new hex(0, q));

            return hexs;
        }
        public List<hex> toRing()
        {
            List<hex> hexs = new List<hex>();
            // hexs.Add(new hex(0, 0));

            hexs.Add(new hex(this.r, 0));
            hexs.Add(new hex(this.r, -this.q));

            hexs.Add(new hex(0, -this.q));
            hexs.Add(new hex(-this.r, 0));

            hexs.Add(new hex(-this.r, this.q));
            hexs.Add(new hex(0, this.q));

            return hexs;
        }
       public List<cube> Test()
        {
           List<cube> cubes = new List<cube>();
            double x = -2;//-2+2=0
            int i = 0, j = 0;
            for (i = 0; i <=2; i++)
            {
                cube c = new cube(x, i);
                cubes.Add(c);
            }
            x = -1; //i + 2;-1+2=1
            for (i = -1; i <= 2;i++)
            {
                cube c = new cube(x, i);
                cubes.Add(c);
            }
            x = 0;
            for (i = -2; i <= 2; i++)
            {
                cube c = new cube(x, i);
                cubes.Add(c);
            }

                return cubes;
        }
    }
    public class cube
    {
        public double x;
        public double y;
        public double z;
        public cube(double x , double y)
        {
            this.x = x;
            this.y = y;
            this.z = (x + y) * -1;
        }
        public cube(double x, double y , double z)
        {
            this.x = x;
            this.y = y;

            this.z = z;
        }
        public hex toAxial()
        {
            hex he = new hex(this.z, this.x);
            return he;
        }
        public hex ToHex()
        {
            hex he = new hex(this.z, this.x);
            return he;
        }
        public double distance(cube b)
        {
            return 0.5 * (abs(this.x - b.x) + abs(this.y - b.y) + abs(this.z - b.z));
        }
        private double abs(double x)
        {
            return Math.Abs(x);
        }
    }
    public class hexagon
    {
        private ns_point east = new ns_point(1,0);
        private ns_point ne = new ns_point(0,1);
        private ns_point nw = new ns_point(-1,1);
        private ns_point west = new ns_point(-1,0);
        private ns_point sw = new ns_point(0,-1);
        private ns_point ew = new ns_point(1,-1);
        public ns_point start;
        public hexagon(ns_point start)
        {
            this.start = start;
        }
        public ns_point moveEast(double units)
        {
            return start + (units * this.east);
        }
        public ns_point moveNorthEast(double units)
        {
            return this.start + (units * this.ne);
        }
        public ns_point moveWest(double units)
        {
            return this.start + (units * this.west);
        }
        public ns_point moveSouthWest(double units)
        {
            return this.start + (units * this.sw);
        }
        public ns_point moveNorthWest(double units)
        {
            return this.start + (units * this.nw);
        }
        public ns_point moveEastWest(double units)
        {
            return this.start + (units * this.ew);
        }
    }
}
