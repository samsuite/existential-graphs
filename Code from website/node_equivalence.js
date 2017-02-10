//Tests if a graph is empty
Node.prototype.empty = function(){
	if(this.subtrees.length !== 0){return false;}
	if(this.leaves.length !== 0){return false;}
	return true;
}

//Check if two levels, trees, are the same structure
Node.prototype.equivalence = function(other) {
	//checks if one of the trees is empty (no graph)
	if ((this.empty() && !other.empty()) || (other.empty() && !this.empty()))
		return false;
	// get leaves of both trees
	var L1 = tree_leaves(this);
	var L2 = tree_leaves(other);
	// check of same leaf count
	if (L1.length !== L2.length)
		return false;
	// simple case of two empty cuts
	if (L1.length === L2.length && L1.length===0)
		return true;
	// run treeisomorphism
	return tree_isomorphic(L1,L2,0);
};

// verstion of tree isomorphsim using a modified AHU algorithm
function tree_isomorphic(L1,L2,level) {
	// recursivly check equality on deeper levels first
	if(level+1<L1.length) {
		// if deeper level not equal return false
		if (!tree_isomorphic(L1,L2,level+1)) {
			return false;
		}
	}
	// get canonical names of nodes on level
	// also get name:parent mapping
	var N1 = name_subtrees(L1[level]);
	var N2 = name_subtrees(L2[level]);
	// sort names
	N1[0].sort();
	N2[0].sort();
	// if level cancanonical names equal
	if (arrays_equal(N1[0],N2[0])) {
		if (level!==0) {
			// add aliased canonical names to upper level
			// these are the names of the subtrees of the above level
			L1[level-1].concat(leaf_name_alias(N1[0],N1[1]));
			L2[level-1].concat(leaf_name_alias(N2[0],N2[1]));
		}
		// if this is the root level then trees are isomorphic
		// else the levels are equvilant on current level
		return true;
	}
	// else level of trees not isomorphic
	return false;
}

// rename canonical names of leaves into simplified names
// and add to names list with appropriate parent
function leaf_name_alias (names,leaves_parents) {
	var alias_leaves = [];
	var alias_map = {};
	var alias_counter = 0;
	for(var i in names) {
		// go through all names and add the name with its
		// integer alias if its in the alias map
		if (names[i] in alias_map) {
			alias_leaves.push( alias_map[names[i]] );
		}
		// else create a new integer alias for the new name
		else {
			alias_leaves.push( ++alias_counter );
			alias_map[ names[i] ] = [leaves_parents[ names[i] ], alias_counter ];
		}
	}
	return alias_leaves;
}

// generate 2-tuple of a list of canonical names of leaves and
// a map of name:parent tuples for a level of a tree given the subtrees list
function name_subtrees (subtrees_list) {
	// get map of parent:names
	var degree_spectrum = new List();
	for (var n in subtrees_list) {
		// add name to parent list or make new parent name list
		var sitr = degree_spectrum.skipUntil( function(t) {
			return (t[0]===subtrees_list[n][0]); //find parent
		});
		if (sitr !== degree_spectrum.end()) {
			sitr.val[1].push(subtrees_list[n][1]);
		}
		else {
			degree_spectrum.push_back([subtrees_list[n][0],[subtrees_list[n][1]]]);
		}
	}
	// canonical names list
	var names = [];
	// name:parent map
	var subtrees_spectrum = {};
	degree_spectrum.iterate( function(d) {
		// sort canonical name before adding it to names list
		var sorted_spectrum = d[1].sort();
		names.push(sorted_spectrum);
		// setup name:parent item if it doesn't exist
		if (!(sorted_spectrum in subtrees_spectrum)) {
			subtrees_spectrum[sorted_spectrum] = d[0];
		}
	});
	return [names,subtrees_spectrum];
}

// get all leaves in a tree
function tree_leaves(root) {
	// list of each level in tree
	var levels = [];
	// frontier of tree exploration
	var frontier = new List();
	// starting search level
	var level = 0;

	// frontier structure is:
	//   [...nodes on level i...,i,...nodes on level i+1...,i+1,...]
	// start of frontier
	frontier.push_back(root);
	frontier.push_back(level);
	// explore as long as the frontier has items and the item is not a level number
	while(frontier.length && !(typeof(frontier.first())==='number' && frontier.length===1)) {
		var leaves = []; //leaves on level

		// go through all nodes on level before reaching level number
		while(typeof(frontier.first())!=='number') {
			var node = frontier.pop_front();

			// add all empty cuts into leaves list
			// add all subtrees into frontier for exploration
			node.subtrees.iterate(function(c) {
				if(!(c.subtrees.length || c.leaves.length)) {
					leaves.push([c.parent,0]);
				}
				else {
					frontier.push_back(c);
				}
			});

			// add all variables into leaves list with canonical name of variable name
			node.leaves.iterate(function(v) {
				leaves.push([v.parent,v.context]);
			});
		}
		// if there are leaves then add to current level
		levels.push(leaves);

		// pop off remaning level number and add next level number
		frontier.pop_front();
		frontier.push_back(++level);
	}
	return levels;
}

// Checks to see if two nodes have identical names and structures
Node.prototype.hardNodeEquivalence = function(other) {
	// run recursive treeisomorphism check
	return node_tree_hard_match(this.leaves,other.leaves,this.subtrees,other.subtrees);
};

function node_tree_hard_match(L1leaves, L2leaves, L1subtrees, L2subtrees) {
	// Check leaves is defined
	if((L1leaves && !L2leaves) || (!L1leaves && L2leaves))
		return false;

	// check # leaves is the same
	if(L1leaves !== undefined) {
		if(L1leaves.length != L2leaves.length)
			return false;

		// Check actual values of leaves
		if(L1leaves.length > 0) {
			var L1itr = L1leaves.head;
			var L2itr = L2leaves.head;
			for(var index = 0; index < L1leaves.length; index++) {
				if(L1itr.val.attributes.label !== L2itr.val.attributes.label)
					return false;
				L1itr = L1itr.next;
				L2itr = L2itr.next;
			}
		}
	}

	// check subtrees are equivalently defined
	if((L1subtrees && !L2subtrees) || (!L1subtrees && L2subtrees))
		return false;


	// check # subtrees is the same
	if(L1subtrees) {
		if(L1subtrees.length != L2subtrees.length)
			return false;

		// Check actual values of leaves with recursion
		if(L1subtrees.length > 0) {
			var L1itr = L1subtrees.head;
			var L2itr = L2subtrees.head;
			for(var index = 0; index < L1subtrees.length; index++) {
				if(!node_tree_hard_match(L1itr.val.leaves,L2itr.val.leaves,
						L1itr.val.subtrees,L2itr.val.subtrees))
					return false;
				L1itr = L1itr.next;
				L2itr = L2itr.next;
			}
		}
	}
	return true;
}
