
//Tests if a graph is empty
Node.prototype.empty = function(){
	if(this.subtrees.length !== 0){return false;}
	if(this.leaves.length !== 0){return false;}
	return true;
}

//checks if node is a model
Node.prototype.is_model = function(){
	//checks that all cuts contain only one atomic sentence
	for(var i=0; i<this.subtrees.length; i++){
		if(this.subtrees[i].subtrees.length > 0 || this.subtrees[i].leaves.length !== 1){return false;}
	}
	return true;
}

//checks if node is the disjunction of models
Node.prototype.model_disjunct = function(){
	if(this.leaves.length !== 0){return false;}
	for(var i=0; i<this.subtrees.length; i++){
		if(!this.subtrees[i].is_model()){return false;}
	}
	return true;
}


//adds a literal to the current node.
//literal is the name of the variable
//truth is either true or false and tells whether the literal is the variable or the negation of the variable.
Node.prototype.add_lit = function(literal,truth){
	if(truth){
		this.leaves.push(literal);
	}
	else{
		var new_node = Node.NodeSkeleton(this);
		this.subtrees.push(new_node);
		new_node.leaves.push(literal);
	}
}

//Paste_routine
//literal is the name of the variable
//truth is either true or false and tells whether the literal is the variable or the negation of the variable.
Node.prototype.paste =  function(literal, truth){
	document.write("paste ");
	if(truth){document.write(literal, " ");}
	else{document.write("(", literal, ") ");}
	document.write(" into ");
	this.print_node();
	document.write("|");
	if(this.subtrees.length === 1 && this.subtrees[0].empty() && this.leaves.length === 0){return;} //if empty cut
	if(this.is_model()){this.add_lit(literal, truth);} //if model
	else{ //disjunction of models
		var n = this.subtrees[0];
		if(!n.model_disjunct()){
			n.DN_leaves();
		}
		for(var i=0; i < n.subtrees.length; i++){
			n.subtrees[i].add_lit(literal, truth);
		}
	}
}

//removes all instances of a literal from a graph.
Node.prototype.remove_lit = function(lit, truth){
	//checks all the leaves
	for(var i=0; i<this.leaves.length; i++){
		if(this.leaves[i]==lit){
			this.leaves[i] = this.leaves[this.leaves.length-1];
			this.leaves.pop(); i--;
			if(!truth){ //if removing not A, and an A is found, replaces it with the empty cut
				this.subtrees.push(Node.NodeSkeleton(this));
			}
		}
	}
	//checks subtrees
	for(var i=0; i<this.subtrees.length; i++){
		//checks if subtree is a cut containing only the literal-name
		//if removing a negative literal, removes this subgraph
		if(!truth && this.subtrees[i].subtrees.length===0 &&
			this.leaves[0]===lit && this.leaves.length===1){
			this.subtrees[i] = this.subtrees[this.subtrees.length-1];
			this.subtrees.pop(); i--;
		}
		else{ // recusive call
			this.subtrees[i].remove_lit(lit, truth);
		}
	}
	this.remove_DN(); //removes any double-cuts created at this level.
}

//prints out a node, representing cuts as ()
Node.prototype.print_node = function(){
	for(var i=0; i<this.leaves.length; i++){
		document.write(this.leaves[i], " ");
	}
	for(var i=0; i<this.subtrees.length; i++){
		document.write(" ( ");
		this.subtrees[i].print_node();
		document.write(") ");
	}
}

//duplicates node
Node.prototype.duplicate = function() {
	var dup = Node.NodeSkeleton(this.parent);
	//copies leaves
	for(var i=0; i<this.leaves.length; i++){
		dup.leaves.push(this.leaves[i]);
	}
	//makes recursive calls on all subtrees
	for(var i=0; i<this.subtrees.length; i++){
		var child_dup = this.subtrees[i].duplicate();
		child_dup.parent = dup;
		dup.subtrees.push(child_dup);
	}
	return dup;
}



//Transforms node into Disjunctive Normal Form
//See Description of Process at link below
//http://www.cogsci.rpi.edu/~heuveb/Research/EG/Presentations/EGAPG.ppt
Node.prototype.DNFTransform = function(){
	this.remove_DN();
	document.write("DNF:");
	this.print_node(); document.write(" | "); 
	if(!(this.subtrees.length === 1 && this.subtrees[0].empty()) && //if empty cut
		!(this.subtrees.length === 0) && // if all leaves
		!(this.empty())){ //if empty graph
		//checks for at least one literal at depth 0 and removes it, then pastes it into the rest of the graph
		var lit; var truth; var has_lit = false;
		if(this.leaves.length !== 0){ //checks for true literal in leaves, and removes if fout
			has_lit = true;
			lit = this.leaves[this.leaves.length-1];
			this.leaves.pop();
			truth = true;	
		}
		else{ //looks for false literal at depth 0
			var lit_ind = 0;
			while(lit_ind !== this.subtrees.length){ // finds index of false literal in subgraphs
				if(this.subtrees[lit_ind].subtrees.length ===0 &&
					this.subtrees[lit_ind].leaves.length === 1)
					{break;}
				lit_ind++;
			}
			if(lit_ind !== this.subtrees.length){ // if false literal is found, removes it
				has_lit = true;
				lit = this.subtrees[lit_ind].leaves[0];
				this.subtrees[lit_ind] = this.subtrees[this.subtrees.length-1]; 
				this.subtrees.pop();
				truth = false;
			}		
		} 
		if(has_lit){ //if a literal was found, pastes it into the rest of the graph
			this.remove_lit(lit, truth); 
			this.DNFTransform();
			this.paste(lit, truth);
			this.remove_DN();
			return;
		}
		else if(this.subtrees.length !== 1){//step three in transformation

			//picks subtree, G1, in graph and removes it.
			//G2 is original graph without G1
			var G1 = this.subtrees[this.subtrees.length-1].duplicate();
			this.subtrees.pop(); //sets this to G2
			var G2 = this.duplicate();
			this.subtrees = []; this.leaves = [];
			this.subtrees.push(Node.NodeSkeleton(this));

			//handles each cut as a subgraph, G4
			//Disjuncts not-G4 and G2 for every G4
			for(var i=0; i<G1.subtrees.length; i++){
				var G3 = Node.NodeSkeleton(this.subtrees[0]);
				this.subtrees[0].subtrees.push(G3)
				G3.absorb_graph(G2);
				var G4 = Node.NodeSkeleton(G3);
				G4.subtrees.push(G1.subtrees[i].duplicate());
				G3.subtrees.push(G4);
				G3.DNFTransform();
			}

			//handles conjunction of leaves as a subgraph, LeafG4, like G4s above
			if(G1.leaves.length > 0){
				var LeafG3 = Node.NodeSkeleton(this.subtrees[0]);
				var LeafG4 = Node.NodeSkeleton(LeafG3);
				LeafG3.subtrees.push(LeafG4);
				for(var i=0; i<G1.leaves.length; i++){
					LeafG4.leaves.push(G1.leaves[i]);
				}
				LeafG3.absorb_graph(G2);
				LeafG3.DNFTransform();
				this.subtrees[0].subtrees.push(LeafG3);
			}
			this.subtrees[0].remove_DN();
		}
	}
	document.write("yields: "); this.print_node();
	document.write("removing DNs: ");
	this.remove_DN();
	this.print_node();
	document.write(" | ")
};

//Copies G and adds every leaf and subgraph to the current node
Node.prototype.absorb_graph = function(G){
	for(var i=0; i<G.subtrees.length; i++){
		var new_sub = G.subtrees[i].duplicate();
		new_sub.parent = this;
		this.subtrees.push(new_sub);
	}
	for(var i=0; i<G.leaves.length; i++){
		this.leaves.push(G.leaves[i]);
	}
}


//removes all Double negations from subgraphs one level below current level
Node.prototype.remove_DN = function(){
	for(var i=0; i<this.subtrees.length; i++){
		if(this.subtrees[i].leaves.length === 0 && this.subtrees[i].subtrees.length === 1){
			this.absorb_graph(this.subtrees[i].subtrees[0]);
			this.subtrees[i] = this.subtrees[this.subtrees.length-1];
			this.subtrees.pop();
			i--; //repeats test on new subtrees[i];
		}
	}
}

//Wraps double negation around each leaf
Node.prototype.DN_leaves = function(){
	for(var i=0; i<this.leaves.length; i++){
		this.subtrees.push(Node.NodeSkeleton());
		this.subtrees[this.subtrees.length-1].add_lit(this.leaves[i],false);
	}
	this.leaves = [];
}

//returns a new NodeSkeleton (uses arrays instead of lists)
Node.NodeSkeleton = function(parent){
	var new_node = new Node(parent);
	new_node.subtrees = [];
	new_node.leaves = [];
	return new_node;
}


//converts a level to a node skeleton
Node.node_to_node_skeleton = function(node){
	var node_skeleton = Node.NodeSkeleton();
	//node_skeleton.subtrees = [];
	node.subtrees.iterate(function(s) {
		var node_subtree = Node.node_to_node_skeleton(s);
		node_subtree.parent = node_skeleton;
		node_skeleton.subtrees.push(node_subtree);
	});
	//node_skeleton.leaves = [];
	node.leaves.iterate(function(s){
		node_skeleton.leaves.push(s.text.attr("text"));
	})
	return node_skeleton;
}

//given graphs n1 and n2 detemines if n1 entails n2
//does this by negating n2 and conjuncting it with n1 and searching for a contradiction
Node.ProofExists = function(n1, n2){
	var outer_node = Node.NodeSkeleton();
	n1.parent = outer_node;
	outer_node.absorb_graph(n1);
	outer_node.subtrees.push(Node.NodeSkeleton(outer_node));
	outer_node.subtrees[outer_node.subtrees.length-1].absorb_graph(n2);
	outer_node.print_node();
	outer_node.DNFTransform();
	outer_node.print_node();

	//outer loop should be the conjunction of empty cuts.
	if(outer_node.leaves.length > 0){return false;}
	for(var i=0; i<outer_node.subtrees.length; i++){
		if(outer_node.subtrees[i].subtrees.length != 0 || outer_node.subtrees[i].leaves.length != 0){
			return false;
		}
	}
	return true;
}
