using System;
using System.Collections.Generic;

public class Counter<G> 
{
	private Dictionary<G, int> counts;
	
	public Counter() {
		counts = new Dictionary<G, int>();
	}
	
	public void Add(G element) {
	
		if(!this.counts.ContainsKey(element)) {
			counts.Add(element, 1);
		} else {
			counts[element] = counts[element] + 1;
		}
	}
	
	public void Decrement(G element, int amount) {
		
		if(!this.counts.ContainsKey(element))
			return;
		else
			if(this.counts[element] == 0)
				return;
			this.counts[element] -= amount;
		return;
		
	}
	
	public int Count(G element) {
		if(!this.counts.ContainsKey(element)) {
			return 0;
		} else {
			return counts[element];
		}
	}
	
	public bool ContainsElement(G element) {
		return this.counts.ContainsKey(element);	
	}
	
	public int TotalUniqueElements() {
		return this.counts.Keys.Count;
	}
	
	public List<G> Keys() {
		return new List<G>(this.counts.Keys);	
	}
	
}