////////////////////////////////////////////////////////////////////////////////
// Inference rules for allowing inserting anything into a even cut creating odd leveled nodes

function touchInsertionAttribute(tree) {
	var ia = tree.getAttribute('insertion');
	if(!ia) {
		tree.addAttribute('insertion', {});
		ia = tree.getAttribute('insertion');
	}
	return ia;
}

function GetInsertionNode(tree) {
	var ia = touchInsertionAttribute(tree);
	return tree.getChildByIdentifier(ia.insertionNodeID);
}

function SetInsertionNode(tree, inode) {
	var ia = touchInsertionAttribute(tree);
	ia.insertionNodeID = inode.getIdentifier();
}

function GetOriginalInsertionSet(tree) {
	var ia = touchInsertionAttribute(tree);
	return ia.originalNodes;
}

function SetOriginalInsertionSet(tree, inode) {
	var ia = touchInsertionAttribute(tree);
	ia.originalNodes = {};
	inode.fmap(function(node) { ia.originalNodes[node.getIdentifier()] = true; });
	delete ia.originalNodes[inode.getIdentifier()];
}

function NodeInOriginalInsertionSet(tree, node) {
	var onodes = GetOriginalInsertionSet(tree);
	if(node.getIdentifier() in onodes) {
		return true;
	}
	return false;
}

function ClearInsertionAttributes(tree) {
	tree.removeAttribute('insertion');
}

function ValidateInsertionStart(tree, nodes) {
	return nodes.length === 1 &&  NodesAllHaveParent(nodes) && NodesAllOddLevel(nodes);
}

function ApplyInsertionStart(tree, nodes) {
	var diff = NewDiff();

	// set initial insertion properties
	var inode = nodes.first(); // insertion node
	SetInsertionNode(tree, inode);
	SetOriginalInsertionSet(tree, inode);

	return {tree: tree, diff: diff};
}

function ApplyVisualInsertionStart(tree, nodes, diff, attrs) {
	var inodeID = GetInsertionNode(tree).getIdentifier();
	attrs[inodeID]['stroke-dasharray'] = '--';
	return {attrs: attrs};
}

function ValidateInsertionEnd(tree, nodes) {
	return true;
}

function ApplyInsertionEnd(tree, nodes) {
	var diff = NewDiff();
	ClearInsertionAttributes(tree);
	return {tree: tree, diff: diff};
}

function ApplyVisualInsertionEnd(tree, nodes, diff, attrs) {
	for(var id in attrs) {
		var attr = attrs[id];
		if('stroke-dasharray' in attr && attr['stroke-dasharray'] === '--') {
			attr['stroke-dasharray'] = '';
		}
	}
	return {attrs: attrs};
}

// make sure no node is outside the insertion node
function NodesInInsertionSubtree(tree,nodes) {
	var inode = GetInsertionNode(tree);
	var outOfInsertion = false;
	nodes.iterate(function(node) {
		if(!outOfInsertion) {
			var inInsertion = false;
			// find insertion ancesto
			var p = node;
			while(p) {
				if(p.getIdentifier() === inode.getIdentifier()) {
					inInsertion = true;
					break;
				}
				p = p.parent;
			}
			if(!p && !inInsertion) {
				outOfInsertion = true;
			}
		}
	});
	return !outOfInsertion;
}

// validator for insertion context
function ValidateInsertion(tree,nodes) {
	var insertionValid = true;
	nodes.iterate(function(node) {
		// make sure no orignal insertion nodes chosen
		insertionValid = insertionValid && !NodeInOriginalInsertionSet(tree, node);
	});
	insertionValid = insertionValid && NodesInInsertionSubtree(tree,nodes);
	return insertionValid;
}

// decorator for standard validators to also validate for insertion
function ValidateInsertionRule(ruleValidator) {
	return function(tree, nodes) {
		return ValidateInsertion(tree,nodes) && ruleValidator(tree, nodes);
	};
}

// validate cuts do not occur on insertion node
function ValidateNotOnInsertionRule(ruleValidator) {
	return function(tree, nodes) {
		var includesInsertionNode = false;
		var inodeID = GetInsertionNode(tree).getIdentifier();
		nodes.iterate(function(node) {
			if(node.getIdentifier() === inodeID) {
				includesInsertionNode = true;
			}
		});
		return !includesInsertionNode && ruleValidator(tree, nodes);
	};
}
