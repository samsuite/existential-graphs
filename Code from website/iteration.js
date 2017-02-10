////////////////////////////////////////////////////////////////////////////////
// Inference rules for copying nodes from a lower level into an adjacent higher level
// and removing equivalents of a adjacent higher and lower levels from the higher level (opposite of iteration)

function getSourceAndDestination(nodes) {
	var node1 = nodes.first();
	var node2 = nodes.begin().next.val;
	// find source and destination
	var node_source;
	var node_dest;
	if(node1.getLevel() > node2.getLevel()) {
		node_source = node2;
		node_dest = node1;
	}
	else {
		node_source = node1;
		node_dest = node2;
	}
	return {source: node_source, dest: node_dest};
}

// check if destination meets at source's parent as an anscestor
function shareAncestor(node_source, node_dest) {
	var main_parent = node_source.parent;
	var ancestor = node_dest.parent;
	while(ancestor!==null) {
		if(main_parent === ancestor) {
			return true;
		}
		ancestor = ancestor.parent;
	}
	return false;
}

function ValidateIteration(tree, nodes) {
	if( nodes.length === 2 ){
		var sd = getSourceAndDestination(nodes);
		var node_source = sd.source;
		var node_dest = sd.dest;

		if (!node_dest.isLeaf()) {
			// find out if from same ancestor root
			if(shareAncestor(node_source, node_dest)) {
				return true;
			}
		}
	}
	return false;
}

function ValidateInsertionIteration(tree, nodes) {
	if(!ValidateIteration(tree,nodes)) {
		return false;
	}

	// validate destination node is not in orignal insertion set
	// source node can be anywhere in insertion subtree
	var sd = getSourceAndDestination(nodes);
	var node_dest = sd.dest;
	return NodesInInsertionSubtree(tree,nodes) && !NodeInOriginalInsertionSet(tree, node_dest);
}

function AddIteration(tree, nodes) {
	var diff = NewDiff();

	var sd = getSourceAndDestination(nodes);
	var source = sd.source;
	var dest = sd.dest;
	// clone
	var sourceDup = source.duplicate();
	var child;

	// add to destination
	if (!sourceDup.isLeaf()) {
		child = dest.takeNewSubtree(sourceDup);
	} else {
		child = dest.takeNewLeaf(sourceDup);
	}
	diff.additions = child.preOrderFlattenToIDs();
	return {tree: tree, diff: diff};
}

function ApplyVisualIteration(tree,nodes,diff,attrs,params) {
	var sd = getSourceAndDestination(nodes);
	var source = sd.source;

	var point = PullPointFromParams(params);
	if(!point) {
		return null;
	}

	// get all source points in an pre-order
	var sourcePoints = [];
	source.preOrderFMap(function(node) {
		var attr = attrs[node.getIdentifier()];
		sourcePoints.push(NewPoint(attr.x, attr.y));
	});

	// calculate shift to input point
	var deltaPoint = NewPoint(point.x - sourcePoints[0].x,
														point.y - sourcePoints[0].y);

	// map source points across to input destination area
	for(var i in sourcePoints) {
		var id = diff.additions[i];
		var spoint = sourcePoints[i];
		var dpoint = NewPoint(deltaPoint.x + spoint.x,
							  deltaPoint.y + spoint.y);
		AddPointToID(dpoint, id, attrs);
	}

	return {attrs: attrs};
}

function ValidateDeiteration(tree, nodes) {
	if( nodes.length === 2 ) {
		var sd = getSourceAndDestination(nodes);
		var node_source = sd.source;
		var node_dest = sd.dest;

		// find out if from same ancestor root and equivalent
		if(shareAncestor(node_source, node_dest) && node_source.equals(node_dest)) {
			return true;
		}
	}
}

function AddDeiteration(tree, nodes) {
	var diff = NewDiff();

	var sd = getSourceAndDestination(nodes);
	var dest = sd.dest;
	
	// remove destination
	diff.deletions = dest.preOrderFlattenToIDs();
	var parent = dest.parent;
	parent.removeChild(dest);
	return {tree: tree, diff: diff};
}

function ApplyVisualDeiteration(tree,nodes,diff,attrs,params) {
	// deiterated nodes removal does not influence any other attributes
	return {attrs: attrs};
}

