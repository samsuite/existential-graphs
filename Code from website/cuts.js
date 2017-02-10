////////////////////////////////////////////////////////////////////////////////
// Inference rule for adding cuts around items

function ValidateCut(tree, nodes) {
	return validateNCut(1, tree, nodes);
}

function AddCut(tree, nodes) {
	return addNCut(1, tree, nodes);
}

function ApplyVisualCut(tree,nodes,diff,attrs,params) {
	return applyVisualNCut(1, tree, nodes, diff, attrs, params);
}

function ValidateDoubleCut(tree, nodes) {
	return validateNCut(2, tree, nodes);
}

function AddDoubleCut(tree, nodes) {
	return addNCut(2, tree, nodes);
}

function ApplyVisualDoubleCut(tree,nodes,diff,attrs,params) {
	return applyVisualNCut(2, tree, nodes, diff, attrs, params);
}

function validateNCut(n, tree, nodes) {
	if(NodesAllHaveParent(nodes) && NodesAllSameParent(nodes)) {
		return true;
	}
	return false;
}

function addNCut(n, tree, nodes) {
	var diff = NewDiff();

	//add n-cut
	var p = nodes.first().parent.addSubtree();
	diff.additions.push(p);
	n--;
	while(n>0) {
		p = p.addSubtree();
		diff.additions.push(p);
		n--;
	}

	//move children into n-cut
	nodes.iterate(function(node) {
		var oldIDs = node.preOrderFlattenToIDs();
		var newNode;
		if(!node.isLeaf()) {
			newNode = p.takeNewSubtree(node);
		}
		else {
			newNode = p.takeNewLeaf(node);
		}
		var newNodes = [];
		newNode.fmap(function(node) { newNodes.push(node); });
		for(var i in oldIDs) {
			diff.changes.push([oldIDs[i],newNodes[i]]);
		}
	});

	return {tree: tree, diff: NodeDiffToIdDiff(diff)};
}

function applyVisualNCut(n, tree, nodes, diff, attrs, params) {
	// pull top most cut by deepest level
	var parentNode;
	var topLevel = -1;
	var i;
	for(i in diff.additions) {
		var node = tree.getChildByIdentifier(diff.additions[i]);
		if(node.getLevel() > topLevel) {
			topLevel = node.getLevel();
			parentNode = node;
		}
	}

	// setup added cuts
	var point = ChildrenCentroidPoint(tree,parentNode,attrs);
	if(!point) {
		return null;
	}

	for(i in diff.additions) {
		AddPointToID(point, diff.additions[i], attrs);
	}

	return {attrs: attrs};
}

////////////////////////////////////////////////////////////////////////////////
// Inference rule for adding empty nested cuts into a cut

function ValidateEmptyCut(tree, nodes) {
	return validateEmptyNCut(1, tree, nodes);
}

function AddEmptyCut(tree, nodes) {
	return addEmptyNCut(1, tree, nodes);
}

function ApplyVisualEmptyCut(tree,nodes,diff,attrs,params) {
	return applyVisualEmptyNCut(1,tree,nodes,diff,attrs,params);
}

function ValidateEmptyDoubleCut(tree, nodes) {
	return validateEmptyNCut(2, tree, nodes);
}

function AddEmptyDoubleCut(tree, nodes) {
	return addEmptyNCut(2, tree, nodes);
}

function ApplyVisualEmptyDoubleCut(tree,nodes,diff,attrs,params) {
	return applyVisualEmptyNCut(2,tree,nodes,diff,attrs,params);
}

function validateEmptyNCut(n, tree, nodes) {
	if(nodes.length === 1) {
		var node = nodes.first();
		if(!node.isLeaf()) {
			return true;
		}
	}
	return false;
}


function addEmptyNCut(n, tree, nodes) {
	var diff = NewDiff();

	// add cuts
	var p = nodes.first().addSubtree();
	diff.additions.push(p);
	n--;
	while(n>0) {
		p = p.addSubtree();
		diff.additions.push(p);
		n--;
	}

	return {tree: tree, diff: NodeDiffToIdDiff(diff)};
}

function applyVisualEmptyNCut(n, tree, nodes, diff, attrs, params) {
	var point = PullPointFromParams(params);
	if(!point) {
		return null;
	}

	for(var i in diff.additions) {
		AddPointToID(point, diff.additions[i], attrs);
	}

	return {attrs: attrs};
}

////////////////////////////////////////////////////////////////////////////////
// Inference rule for removing cuts around items

function ValidateReverseCut(tree, nodes) {
	return validateReverseNCut(1, tree, nodes);
}

function ValidateInsertionReverseCut(tree, nodes) {
	return validateInsertionReverseNCut(1, tree, nodes);
}

function AddReverseCut(tree, nodes) {
	return addReverseNCut(1, tree, nodes);
}

function ApplyVisualReverseCut(tree,nodes,diff,attrs,params) {
	return applyVisualReverseNCut(1,tree,nodes,diff,attrs,params);
}

function ValidateReverseDoubleCut(tree, nodes) {
	return validateReverseNCut(2, tree, nodes);
}

function ValidateInsertionReverseDoubleCut(tree, nodes) {
	return validateInsertionReverseNCut(2, tree, nodes);
}

function AddReverseDoubleCut(tree, nodes) {
	return addReverseNCut(2, tree, nodes);
}

function ApplyVisualReverseDoubleCut(tree,nodes,diff,attrs,params) {
	return applyVisualReverseNCut(2,tree,nodes,diff,attrs,params);
}

function isEmptyReverseCut(n, nodes) {
	var node = nodes.first();
	// First try to move one cut deeper to allow for easier double cut removal
	if (node.leaves.length === 0 && node.subtrees.length === 1 && n === 2) {
		node = node.subtrees.head.val;
	}

	if (nodes.length === 1 && !node.isLeaf() && node.subtrees.length === 0 && node.leaves.length === 0 && node.parent) {
		return true;
	}
	return false;
}

function validateReverseNCut(n, tree, nodes) {
	if(nodes.length === 0 || !((NodesAllHaveParent(nodes) && NodesAllSameParent(nodes)))) {
		return false;
	}
	// validate nested n cuts
	var node = nodes.first();
	var p;

	// First try to move one cut deeper to allow for easier double cut removal
	if (node.leaves.length === 0 && node.subtrees.length === 1 && n === 2 && isEmptyReverseCut(n, nodes)) {
		node = node.subtrees.head.val;
	}

	// if node is empty cut
	var emptyCut = isEmptyReverseCut(n, nodes);
	if (emptyCut) {
		p = node;
	} else {
		// reverse cut from parent
		p = node.parent;
	}
	n--;
	// parent cannot be root
	if(!p || (!p.parent && n<=0)) {
		return false;
	}
	// go up cuts
	while(n>0) {
		p = p.parent;
		n--;
		// check if just a cut with one cut inside it
		if(!p || !(p.parent && !p.leaves.length && p.subtrees.length === 1)) {
			return false;
		}
	}

	// validate all children are chosen for inside reverse cut
	p = nodes.first().parent;
	if(emptyCut ||  p.subtrees.length + p.leaves.length === nodes.length) {
		return true;
	}
	return false;
}

// validate reverse cut does not reach out of insertion node
function validateInsertionReverseNCut(n, tree, nodes) {
	if(!(ValidateInsertion(tree,nodes) && validateReverseNCut(n,tree,nodes))) {
		return false;
	}

	// collect all removed nodes
	var node = nodes.first();
	var reverseCuts = new List();
	// collect a cut
	var reverseCutCollect = function(reversableCut) {
		reverseCuts.push_back(reversableCut);
		n--;
	};
	// collect reverse cuts down
	// if node is empty cut
	var emptyCut = isEmptyReverseCut(n, nodes);
	if (emptyCut) {
		tparent = node;
	} else {// reverse cut from parent
		tparent = node.parent;
	}
	reverseCutCollect(tparent);
	while(n>0) {
		tparent = tparent.parent;
		reverseCutCollect(tparent);
	}

	// validate reversed cuts are still in insertion and not effect original nodes
	return ValidateInsertion(tree, reverseCuts);
}

function addReverseNCut(n, tree, nodes) {
	var diff = NewDiff();

	var node = nodes.first();
	var tparent;

	// First try to move one cut deeper to allow for easier double cut removal
	if (node.leaves.length === 0 && node.subtrees.length === 1 && n === 2 && isEmptyReverseCut(n, nodes)) {
		node = node.subtrees.head.val;
	}

	// delete a cut
	var reverseCut = function(reversableCut) {
		diff.deletions.push(reversableCut.getIdentifier());
		var itr = reversableCut.parent.subtrees.contains(reversableCut);
		reversableCut.parent.subtrees.erase(itr);
		n--;
	};

	// reverse cuts down
	// if node is empty cut
	var emptyCut = isEmptyReverseCut(n, nodes);
	if (emptyCut) {
		tparent = node;
	} else {
		// reverse cut from parent
		tparent = node.parent;
	}
	reverseCut(tparent);
	while(n>0) {
		tparent = tparent.parent;
		reverseCut(tparent);
	}
	// beginning of reversing cuts
	var tgrandparent = tparent.parent;

	// change nodes' parent
	if(!emptyCut) {
		nodes.iterate(function(node) {
			var oldIDs = node.preOrderFlattenToIDs();
			var newNode;
			if(!node.isLeaf()) {
				newNode = tgrandparent.takeNewSubtree(node);
			} else {
				newNode = tgrandparent.takeNewLeaf(node);
			}

			var newNodes = [];
			newNode.fmap(function(node) { newNodes.push(node); });
			for(var i in oldIDs) {
				diff.changes.push([oldIDs[i],newNodes[i]]);
			}
		});
	}

	return {tree: tree, diff: NodeDiffToIdDiff(diff)};
}

function applyVisualReverseNCut(n, tree, nodes, diff, attrs, params) {
	// removed cuts don't displace children
	return {attrs: attrs};
}
