using System;

public class Pair<T1, T2>
{
    public T1 item1;
    public T2 item2;
    public Pair(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}

public static class Pair
{
    public static Pair<T1, T2> Make<T1, T2>(T1 item1, T2 item2)
    {
        return new Pair<T1, T2>(item1, item2);
    }
}