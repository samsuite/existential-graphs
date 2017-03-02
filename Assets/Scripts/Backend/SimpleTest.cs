using System;
using System.Text;
using System.Collections.Generic;


namespace SimpleTest
{

    class Program
    {

        static void Main(string[] args)
        {

            Node a = new Node();
            Node A = new Node();
            Node b = new Node();
            Node B = new Node();
            Node c = new Node();
            Node C = new Node();
            Node d = new Node();
            Node D = new Node();
            Node e = new Node();
            Node E = new Node();

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
