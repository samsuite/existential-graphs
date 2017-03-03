using System;
using System.Text;
using System.Collections.Generic;


namespace SimpleTest
{

    class Program
    {

        static void Main(string[] args)
        {

            IsoNode a = new IsoNode();
            IsoNode A = new IsoNode();
            IsoNode b = new IsoNode();
            IsoNode B = new IsoNode();
            IsoNode c = new IsoNode();
            IsoNode C = new IsoNode();
            IsoNode d = new IsoNode();
            IsoNode D = new IsoNode();
            IsoNode e = new IsoNode();
            IsoNode E = new IsoNode();

            A.Add_Child(B);
            A.Add_Child(C);
            C.Add_Child(D);
            C.Add_Child(E);

            a.Add_Child(b);
            a.Add_Child(c);
            b.Add_Child(d);
            b.Add_Child(e);

            Tree t = new Tree(A);
            Tree u = new Tree(a);
            Console.WriteLine(t.Is_Isomorphic_With(u));

        }


    }


}
