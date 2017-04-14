using System;


public static class StringExtension
{
    public static ExistentialGraph To_Existential_Graph(this string s)
    {
        ExistentialGraph root = new ExistentialGraph.Cut();
        ExistentialGraph current = root;


        foreach(char c in s)
        {
            if(c == '(')
            {
                ExistentialGraph next = new ExistentialGraph.Cut();
                current.Add_Subgraph(next);
                current = next;
            }

            if(c == ')')
            {
                current = current.Get_Parent();
            }

            if (Char.IsLetter(c))
            {
                
                ExistentialGraph next = new ExistentialGraph.Var(c.ToString());
                current.Add_Subgraph(next);
            }
        }
        return root;
    }
}
