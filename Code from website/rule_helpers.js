////////////////////////////////////////////////////////////////////////////////
// Various property helpers for rules

function NodesAllEvenLevel(nodes) {
	for(var itr=nodes.begin(); itr !== nodes.end(); itr = itr.next) {
		var node = itr.val;
		if((node.getLevel())%2 === 1) {
			return false;
		}
	}
	return true;
}

function NodesAllOddLevel(nodes) {
	for(var itr=nodes.begin(); itr !== nodes.end(); itr = itr.next) {
		var node = itr.val;
		if((node.getLevel())%2 === 0) {
			return false;
		}
	}
	return true;
}

function NodesAllSameLevel(nodes) {
	var level = null;
	for(var itr=nodes.begin(); itr !== nodes.end(); itr = itr.next) {
		var node = itr.val;
		if(level === null) {
			level = node.getLevel();
			continue;
		}
		if(node.getLevel() !== level) {
			return false;
		}
	}
	return true;
}

function NodesAllSameParent(nodes) {
	var parent = null;
	for(var itr=nodes.begin(); itr !== nodes.end(); itr = itr.next) {
		var node = itr.val;
		if(!parent) {
			parent = node.parent;
			continue;
		}
		if (node.parent !== parent) {
			return false;
		}
	}
	return true;
}

function NodesAllHaveParent(nodes) {
	for(var itr=nodes.begin(); itr !== nodes.end(); itr = itr.next) {
		var node = itr.val;
		if(!node.parent) {
			return false;
		}
	}
	return true;
}
