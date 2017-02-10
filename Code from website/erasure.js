////////////////////////////////////////////////////////////////////////////////
// Inference rule for removing nodes

function ValidateConstructionErasure(tree, nodes) {
	return NodesAllHaveParent(nodes);
}

function ValidateProofErasure(tree, nodes) {
	// Anything enclosed in an even amount of levels means its on an odd level
	return NodesAllHaveParent(nodes) && NodesAllOddLevel(nodes);
}

function AddErasure(tree, nodes) {
	var diff = NewDiff();
	nodes.iterate(function(node) {
		var parent = node.parent;
		parent.removeChild(node);
		diff.deletions = node.preOrderFlattenToIDs();
	});
	return {tree: tree, diff: diff};
}

function ApplyVisualErasure(tree,nodes,diff,attrs,params) {
	// erasure does not influence any other attributes
	return {attrs: attrs};
}

