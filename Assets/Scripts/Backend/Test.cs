using System;

public class Test
{


    public static void Main(String[] args)
    {
        ExistentialGraph c = "((A))".To_Existential_Graph();
        ExistentialGraph d = "A".To_Existential_Graph();
        AlphaChecker e = new AlphaChecker();
        Pair<ExistentialGraph, ExistentialGraph> dif = e.Find_Difference_Between(c, d);
        Console.WriteLine(dif.item2);
        Console.WriteLine(e.Is_Double_Cut(dif.item1, dif.item2));
    }


}